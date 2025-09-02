datablock ItemData(batItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/WoodenBat.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Wooden Bat";
	iconName = "./icons/icon_WoodenBat";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";

	image = batImage;
	canDrop = true;
};

datablock ShapeBaseImageData(batImage)
{
    shapeFile = "./models/WoodenBat.dts";
    emap = true;

    mountPoint = 0;
    offset = "0 0 0";
    correctMuzzleVector = false;
    eyeOffset = "0 0 0";
    className = "WeaponImage";

    item = batItem;
    ammo = " ";
    projectile = "";
    projectileType = Projectile;

    melee = true;
	isSpecial = true;
    doRetraction = false;
    armReady = false;
    doColorShift = batItem.doColorShift;
    colorShiftColor = batItem.colorShiftColor;

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.1;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]                    = "WeaponSwitchsound";

	stateName[1]                     = "Ready";
	stateScript[1]                  = "onReady";
	stateTransitionOnTriggerDown[1]  = "PreFire";
	stateAllowImageChange[1]         = true;

	stateName[2]					= "PreFire";
	stateScript[2]                  = "onPreFire";
	stateAllowImageChange[2]        = false;
	stateTimeoutValue[2]            = 0.15;
	stateTransitionOnTimeout[2]     = "Fire";

	stateName[3]                    = "Fire";
	stateTransitionOnTimeout[3]     = "CheckFire";
	stateFire[3]                    = false;
	stateScript[3]                  = "onFire";
	stateTimeoutValue[3]            = 0.1;
	stateEmitter[3]					= "";
	stateEmitterNode[3]             = "muzzlePoint";
	stateEmitterTime[3]             = "0.225";

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "StopFire";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Break";
	stateTimeoutValue[5]            = 0.1925; //0.1925
	stateScript[5]                  = "onStopFire";
	stateEmitter[5]					= "";
	stateEmitterNode[5]             = "muzzlePoint";
	stateEmitterTime[5]             = "0.1";

	stateName[6]                    = "Break";
	stateTransitionOnTimeout[6]     = "Ready";
	stateTimeoutValue[6]            = 1;
	
	cooldown = 36000;
};

function batImage::onReady(%this, %obj, %slot)
{
	%obj.playthread(1, "root");
}

function batImage::onMount(%this, %obj, %slot)
{
	if((%obj.lastBatTime+%this.cooldown) > getSimTime())
		{
		%time = (%this.cooldown) - (getSimTime() - %obj.lastBatTime);
		centerprint(%obj.client,"<font:arial:13><color:ff7744>Can't use this yet!<br>" @ mFloatLength(%time / 1000, 1) @ "s remaining..." ,1);
		serverCmdUnUseTool(%obj.client);
		%obj.playThread(2, undo);
		return;
		}
}

function batImage::onFire(%this, %obj, %slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;	
	%obj.lastBatTime = getSimTime();
	%obj.playthread(1, "shiftTo");
	%startpos = %obj.getEyePoint();
	%endpos = %obj.getEyeVector();
	%typemasks = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	serverPlay3D(melee_batswing_sound,%obj.getPosition());
	
	%hit = containerRayCast(%startpos,vectorAdd(%startpos,VectorScale(%endpos,4)),%typemasks,%obj);
	if(isObject(%hit))
	{
		%hitpos = posFromRaycast(%hit);
		%p = new Projectile()
		{
			dataBlock = "daggerProjectile";
			initialPosition = %hitpos;
			sourceObject = %obj;
			client = %obj.client;
		};
		%p.explode();					

		// Hit player? Push them back and do damage, play a sound too
		if(%hit.getType() & $TypeMasks::PlayerObjectType && minigameCanDamage(%obj,%hit))
		{						
			%hit.applyImpulse(%hit.getposition(),vectorAdd(vectorScale(%obj.getEyeVector(),1000),"0 0 500"));
			%hit.Damage(%obj, %hit.getPosition(), 50, $DamageType::Default);
			%hit.mountimage("sm_stunImage",3);
			serverPlay3D("melee_bathit" @ getRandom(1,2) @ "_sound",%hitpos);
		}
		else
		{
			serverPlay3D("melee_bathit" @ getRandom(1,2) @ "_sound",%hitpos);
		}
	}
	%obj.schedule(%this.cooldown,notifyBatReady);
	serverCmdUnUseTool(%obj.client);
}

function Player::notifyBatReady(%obj)
{
	centerprint(%obj.client,"<font:arial:13><color:44ff44>Bat ability is ready!" ,1);
	serverPlay3D(medi_gauzeGunReloadSound,%obj.getPosition());
}

function batImage::onPreFire(%this, %obj, %slot)
{	
	%obj.playthread(1, "shiftAway");
	%obj.schedule(75,spawnKillerTrail,PlayerRenowned.meleetrailskin,"0.4 1.2 0.375","0 -90 0","3 2.5 1");
}