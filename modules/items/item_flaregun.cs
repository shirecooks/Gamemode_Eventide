datablock ParticleData(flareGunTrailParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = 0;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 4600;
	lifetimeVarianceMS   = 700;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]			= "1 1 1 0.3";
	colors[1]			= "1 1 0.5 0.5";
	colors[2]			= "1 0.5 0.1 0";
	sizes[0]			= 0.25;
	sizes[1]			= 0.25;
	sizes[2]			= 0.1;
	times[0]			= 0;
	times[1]			= 0.1;
	times[2]			= 1;
	useInvAlpha 		= true;
};

datablock ParticleEmitterData(flareGunTrailEmitter)
{
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 0.2;
	velocityVariance	= 0;
	ejectionOffset  	= 0.0;
	thetaMin			= 0;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= flareGunTrailParticle;
};
datablock ParticleData(flareGunExplosionDustParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = 0;
	inheritedVelFactor   = 0;
	constantAcceleration = 0.0;
	lifetimeMS           = 5000;
	lifetimeVarianceMS   = 700;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]			= "1 1 1 0.3";
	colors[1]			= "1 1 0.5 0.5";
	colors[2]			= "1 0.2 0.1 0";
	sizes[0]			= 0.25;
	sizes[1]			= 0.5;
	sizes[2]			= 0.25;
	times[0]			= 0;
	times[1]			= 0.05;
	times[2]			= 1;
	useInvAlpha 		= true;
};

datablock ParticleEmitterData(flareGunExplosionDustEmitter)
{
	ejectionPeriodMS	= 20;
	periodVarianceMS	= 0;
	ejectionVelocity	= 1;
	velocityVariance	= 1;
	ejectionOffset  	= 0.0;
	thetaMin			= 0;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= flareGunExplosionDustParticle;
};
datablock DebrisData(flareGunDebrisA)
{
	shapeFile = "base/data/shapes/empty.dts";
	lifetime = 0.5;
	minSpinSpeed = -400.0;
	maxSpinSpeed = 200.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 3;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;
	emitters[0] = flareGunExplosionDustEmitter;

	gravModifier = 1;
};
datablock explosionData(flareGunExplosionMain)
{
	debris 					= flareGunDebrisA;
	debrisNum 				= 10;
	debrisNumVariance 		= 0;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 5;
	debrisVelocityVariance 	= 2;
	particleEmitter 		= "flareGunExplosionDustEmitter";
	particleDensity 		= 20;
	particleRadius 			= 1;

	damageRadius = 0;
	radiusDamage = 0;
	impulseRadius = 0;
	impulseForce = 0;
	impulseVertical = 0;
};

datablock ProjectileData(flareGunProjectile : gunProjectile)
{
	projectileShapeName 	= "base/data/shapes/empty.dts";
	directDamage        	= 25;
	explodeOnDeath = 1;
	explosion 				= "flareGunExplosionMain";
	directDamageType    	= $DamageType::FlareGun;
	radiusDamageType    	= $DamageType::FlareGun;
	particleEmitter     	= flareGunTrailEmitter;
	uiName 					= "";
	muzzleVelocity 			= 200;
	verticalImpulse 		= 20;
	impactImpulse			= 20;
	lifeTime				= 20000;
	sound = flaregun_loop_sound;
	sProjectile = 0;
	gravityMod = 1;
	isBallistic = 1;
};

function flareGunProjectile::onExplode(%data, %proj, %pos, %fade)
{
	serverPlay3d("flaregun_explosion" @ getRandom(1,2) @ "_sound",%pos);
	Parent::onExplode(%data, %proj, %pos, %fade);
}

function flareGunProjectile::onCollision(%data, %proj, %col, %fade, %pos, %normal)
{
	Parent::onCollision(%data, %proj, %col, %fade, %pos, %normal);

	if(%col.getClassName() $= "Player") {	
		%col.setWhiteOut(10);
	}	
}
datablock ItemData(FlareGunItem)
{
	className = "Weapon";

	shapeFile = "./models/flaregun.dts";
	emap = 1;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	canDrop = 1;
	
	uiName = "Flare Gun";
	iconName = "./icons/icon_flaregun";

	doColorShift = true;
	colorShiftColor = "0.8 0.3 0.1 1";

	ItemAmmo_ammoTypes = "Flare";
	ItemAmmo_maxStorageUnits = 1;
	
	image = FlareGunImage;
};

$c = -1;
datablock shapeBaseImageData(FlareGunImage)
{
	className = "ItemAmmo_WeaponImage";

	shapeFile = "./models/flaregun.dts";
	emap = true;

	correctMuzzleVector = true;
	mountPoint = 0;
	offset = "-0.01 0.1 0";

	armReady = true;
	doColorShift = FlareGunItem.doColorShift;
	colorShiftColor = FlareGunItem.colorShiftColor;

	item = FlareGunItem;

	stateName[$c++] = "cooldown";
	stateSequence[$c] = "cock";
	stateTimeOutValue[$c] = 0.1;
	stateTransitionOnTimeout[$c] = "ready";

	stateName[$c++] = "ready";
	stateTransitionOnAmmo[$c] = "getReload";
	stateTransitionOnTriggerDown[$c] = "checkLoaded";

	stateName[$c++] = "checkLoaded";
	stateTransitionOnLoaded[$c] = "fire";
	stateTransitionOnNotLoaded[$c] = "dryfire";

	stateName[$c++] = "fire";
	stateScript[$c] = "onFire";
	stateEmitter[$c] = gunFlashEmitter;
	stateEmitterTime[$c] = 0.05;
	stateFire[$c] = true;
	stateSequence[$c] = "trig";
	stateTimeOutValue[$c] = 0.01;
	stateTransitionOnTriggerUp[$c] = "cooldown";

	stateName[$c++] = "dryfire";
	stateScript[$c] = "onDryFire";
	stateSequence[$c] = "trig";
	stateTimeOutValue[$c] = 0.01;
	stateTransitionOnTriggerUp[$c] = "cooldown";

	stateName[$c++] = "getReload";
	stateScript[$c] = "getReload";
	stateTimeOutValue[$c] = 0.01;
	stateTransitionOnTimeout[$c] = "checkReload";

	stateName[$c++] = "checkReload";
	stateTransitionOnAmmo[$c] = "break";
	stateTransitionOnNoAmmo[$c] = "reloadfail";

	stateName[$c++] = "break";
	stateScript[$c] = "onBreak";
	stateSequence[$c] = "break";
	stateTimeOutValue[$c] = 0.5;
	stateTransitionOnTimeout[$c] = "reloadFinish";

	stateName[$c++] = "reloadFinish";
	stateScript[$c] = "onReloadFinish";
	stateSequence[$c] = "load";
	stateTimeOutValue[$c] = 0.01;
	stateTransitionOnTimeout[$c] = "unbreak";

	statename[$c++] = "unbreak";
	stateScript[$c] = "onUnbreak";
	stateSequence[$c] = "unbreak";
	stateTimeOutValue[$c] = 0.5;
	stateTransitionOnTimeout[$c] = "ready";

	stateName[$c++] = "reloadfail";
	stateScript[$c] = "onReloadFail";
	stateTimeOutValue[$c] = 0.01;
	stateTransitionOnTimeout[$c] = "ready";
};

function FlareGunImage::onFire(%data,%p,%slot)
{
	%p.playThread(2,"plant");
	serverPlay3D("flaregun_fire_sound",%p.getPosition());
	parent::onFire(%data,%p,%slot);
}

function FlareGunImage::onDryFire(%data,%p,%slot)
{
	serverPlay3D("flaregun_dryfire_sound",%p.getPosition());
	parent::onDryFire(%data,%p,%slot);
}

function FlareGunImage::onBreak(%data,%p,%slot)
{
	%p.playThread(2,"plant");
	serverPlay3D("flaregun_break_sound",%p.getPosition());
}

function FlareGunImage::OnReloadFinish(%data,%p,%slot)
{
	%p.playThread(2,"plant");
	serverPlay3D("flaregun_load_sound",%p.getPosition());
	parent::OnReloadFinish(%data,%p,%slot);
}

function FlareGunImage::onUnbreak(%data,%p,%slot)
{
	%p.playThread(2,"plant");
	serverPlay3D("flaregun_unbreak_sound",%p.getPosition());
}

function FlareGunImage::OnReloadFail(%data,%p,%slot)
{
	parent::OnReloadFail(%data,%p,%slot);
}

datablock ItemData(FlareItem)
{
	className = "Weapon";

	shapeFile = "./models/ammo_flaregun.dts";
	emap = 1;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	canDrop = 1;
	
	uiName = "Flare";
	iconName = "./icons/icon_ammoflares";

	doColorShift = 1;
	colorShiftColor = "0.8 0.3 0.1 1";
	
	ItemAmmo_ammo = "Flare";
	ItemAmmo_storageUnits = 1;
	ItemAmmo_projectile = "flareGunProjectile";
	
	image = FlareImage;
};

$c = -1;
datablock shapeBaseImageData(FlareImage)
{
	className = "ItemAmmo_ContainerImage";

	shapeFile = "./models/ammo_flaregun.dts";
	emap = 1;

	correctMuzzleVector = 0;
	mountPoint = 0;
	armReady = 1;
	doColorShift = FlareItem.doColorShift;
	colorShiftColor = FlareItem.colorShiftColor;

	item = FlareItem;

	stateName[$c++] = "ready";
	stateTransitionOnTriggerDown[$c] = "select";

	stateName[$c++] = "select";
	stateScript[$c] = "onSelect";
	stateTimeOutValue[$c] = 0.1;
	stateTransitionOnTriggerUp[$c] = "ready";
};

function FlareImage::OnSelect(%data,%p,%slot)
{
	parent::OnSelect(%data,%p,%slot);
}