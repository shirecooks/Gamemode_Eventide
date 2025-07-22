datablock ItemData(blueSodaItem)
{
	className = "Weapon";

	shapeFile = "./models/blueSoda.dts";
	emap = 1;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	canDrop = 1;
	
	uiName = "Blue Soda";
	iconName = "./icons/icon_blueSoda";

	doColorShift = false;
	
	image = blueSodaImage;
};

$c = -1;
datablock shapeBaseImageData(blueSodaImage)
{
	className = "WeaponImage";

	shapeFile = "./models/blueSoda.dts";
	emap = true;

	isSpecial = 1;
	mountPoint = 0;
	offset = "-0.01 0.1 0";

	armReady = true;
	doColorShift = FlareGunItem.doColorShift;
	colorShiftColor = FlareGunItem.colorShiftColor;

	item = blueSodaItem;

	stateName[$c++] = "ready";
	stateTransitionOnTriggerDown[$c] = "fire";

	stateName[$c++] = "fire";
	stateFire[$c] = true;
	stateTimeOutValue[$c] = 0.01;
	stateTransitionOnTriggerUp[$c] = "open";

	stateName[$c++] = "open";
	stateScript[$c] = "onOpen";
	stateTimeOutValue[$c] = 1;
	stateTransitionOnTimeout[$c] = "drink";

	statename[$c++] = "drink";
	stateScript[$c] = "onDrink";
	stateTimeOutValue[$c] = 1;
	
	cooldown = 32000;
};

function blueSodaImage::OnOpen(%data,%obj,%slot)
{
	serverPlay3D("soda_can_open_sound",%obj.getPosition());
	%obj.playThread(2,"shiftleft");
}

function blueSodaImage::onMount(%this, %obj, %slot)
{
	if((%obj.lastDrinkTime+%this.cooldown) > getSimTime())
		{
		centerprint(%obj.client,"<font:arial:13><color:ff7744>Can't use this yet!" ,1);
		serverCmdUnUseTool(%obj.client);
		%obj.playThread(2, undo);
		return;
		}
}

function SpeedEffect::Spawn(%e,%obj)
{
	%obj.setTempSpeed(1.25);
}

function SpeedEffect::Despawn(%e,%obj)
{
	%obj.setTempSpeed();
}

function blueSodaImage::OnDrink(%data,%obj,%slot)
{
	serverPlay3D("soda_gulp" @ getRandom(1,3) @ "_sound",%obj.getPosition());
	%obj.playThread(2,"jump");
	%obj.lastDrinkTime = getSimTime();

	if(%e = %obj.StatusEffect_FindName("SodaBoost")) %e.duration(%e.getDuration() + 6000);	
	else %obj.StatusEffect("SpeedEffect","SodaBoost","mod=0.25;").duration(6000);
	
	%obj.schedule(%data.cooldown,notifyDrinkReady);
	serverCmdUnUseTool(%obj.client);
	new projectile()
	{
		dataBlock = "blueSodaProjectile";
		initialPosition = %obj.getMuzzlePoint(%slot);
	}.explode();
}

function Player::notifyDrinkReady(%obj)
{
	centerprint(%obj.client,"<font:arial:13><color:44ff44>Soda ability is ready!" ,1);
	serverPlay3D(medi_gauzeGunReloadSound,%obj.getPosition());
}

datablock DebrisData(blueSodaDebris)
{
	elasticity = 0.5;
	gravModifier = 0.9;
	lifeTime = 5;
	maxSpinSpeed = 1000;
	numBounces = 2;
	fade = true;
	snapOnMaxBounce = false;
	staticOnMaxBounce = true;
	shapeFile = "./models/blueSoda.dts";
	velocity = 0;
};

datablock ExplosionData(blueSodaExplosion)
{
	debris = "blueSodaDebris";
	debrisNum = 1;
	debrisPhiMax = 360;
	debrisPhiMin = 180;
	debrisThetaMax = 180;
	debrisThetaMin = 0;
	debrisVelocity = 2;
};

datablock ProjectileData(blueSodaProjectile)
{
	explosion = "blueSodaExplosion";
};