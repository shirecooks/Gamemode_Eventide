datablock DebrisData(slugFragDebris) //this is all ripped from sweps
{
	shapeFile 			= "./models/frag.dts";
	lifetime 			= 2.8;
	spinSpeed			= 1200.0;
	minSpinSpeed 		= -3600.0;
	maxSpinSpeed 		= 3600.0;
	elasticity 			= 0.5;
	friction 			= 0.2;
	numBounces 			= 3;
	staticOnMaxBounce 	= true;
	snapOnMaxBounce 	= false;
	fade 				= true;
	gravModifier 		= 4;
};

datablock ParticleData(SlugExplosionParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = 0.4;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 800;
	lifetimeVarianceMS   = 100;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]			= "0.6 0.6 0.6 0.3";
	colors[1]			= "0.5 0.5 0.5 0.0";
	sizes[0]			= 0.75;
	sizes[1]			= 1.5;
	useInvAlpha 		= true;
};

datablock ParticleEmitterData(SlugExplosionEmitter)
{
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 2;
	velocityVariance	= 1.0;
	ejectionOffset  	= 0.0;
	thetaMin			= 89;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= SlugExplosionParticle;
};

datablock ExplosionData(SlugSubExplosion)
{
	debris 					= slugFragDebris;
	debrisNum 				= 6;
	debrisNumVariance 		= 2;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
	explosionShape 			= "";
	particleEmitter 		= SlugExplosionEmitter;
	particleDensity 		= 20;
	particleRadius 			= 0.4;
	lifeTimeMS 				= 150;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= false;
	camShakeFreq 			= "";
	camShakeAmp 			= "";
	camShakeDuration 		= 0;
	camShakeRadius 			= 0.0;
};

datablock explosionData(SlugExplosion : gunExplosion)
{
	soundProfile = "";
	particleEmitter = "";
	subExplosion[0] = SlugSubExplosion;
};

datablock ParticleData(SlugProjectileParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 120;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;

	textureName		= "base/data/particles/dot";

	colors[0]	= "1 0.6 0.4 0.99";
	colors[1]	= "1 0.3 0 0.99";
	colors[2]	= "1 0.1 0 0.99";
	sizes[0]	= 0.2;
	sizes[1]	= 0.15;
	sizes[2]	= 0.0;
	times[0]	= 0.0;
	times[1]	= 0.5;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(SlugProjectileEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0;
	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = SlugProjectileParticle;
	useEmitterColors = true;
};

AddDamageType("BreakActionShotgun",'<bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_hunting> %1','%2 <bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_hunting> %1',1,1);
datablock ProjectileData(BreakActionShotgunProjectile : gunProjectile)
{
	projectileShapeName 	= "base/data/shapes/empty.dts";
	directDamage        	= 35;
	explosion 				= SlugExplosion;
	directDamageType    	= $DamageType::BreakActionShotgun;
	radiusDamageType    	= $DamageType::BreakActionShotgun;
	particleEmitter     	= SlugProjectileEmitter;
	uiName 					= "";
	muzzleVelocity 			= 200;
	verticalImpulse 		= 20;
	impactImpulse			= 20;
	sProjectile = 1;
};

datablock DebrisData(shotgunShellDebris)
{
	shapeFile = "./models/buckshot.dts";
	lifetime = 6.0;
	lifetimeVariance = 1.0;
	minSpinSpeed = -400.0;
	maxSpinSpeed = 200.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 3;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 4;
};

datablock ItemData(BreakActionShotgunItem)
{
	category = "Weapon";
	className = "Weapon";
	
	weaponClass = "primary";

	shapeFile = "./models/hunting.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Old Shotgun";
	iconName = "./icons/OldShotGunIcon";

	ItemAmmo_ammoTypes = "Shell";
	ItemAmmo_maxStorageUnits = 1;

	image = BreakActionShotgunImage;
	canDrop = true;
};

$c = -1;
datablock shapeBaseImageData(BreakActionShotgunImage)
{
	className = "ItemAmmo_WeaponImage";

	shapeFile = "./models/hunting.dts";
	emap = true;

	correctMuzzleVector = true;
	mountPoint = 0;
	offset = "-0.014 0 -0.08";

	armReady = true;

	item = BreakActionShotgunItem;

	casing = shotgunShellDebris;
	shellExitDir        = "0.2 -1.3 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 12.0;	
	shellVelocity       = 7.0;

	stateName[$c++] = "cooldown";
	stateSequence[$c] = "root";
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
	stateTransitionOnTimeout[$c] = "unload";

	stateName[$c++] = "unload";
	stateScript[$c] = "onUnload";
	stateSequence[$c] = "unload";
	stateTimeOutValue[$c] = 0.5;
	stateEjectShell[$c]	= true;
	stateTransitionOnTimeout[$c] = "reloadFinish";
	
	statename[$c++] = "reloadFinish";
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

function BreakActionShotgunProjectile::onCollision(%data, %proj, %col, %fade, %pos, %normal)
{
	if(%col.getClassName() $= "Player")
	{
		%col.mountimage("sm_stunImage",3);
	}
	parent::onCollision(%data, %proj, %col, %fade, %pos, %normal);
}

function BreakActionShotgunImage::onFire(%data,%p,%slot)
{
	%p.playThread(2,"plant");
	%p.spawnExplosion("impulseProjectile",%p.getScale()); 
	serverPlay3D("shotgun_fire_sound",%p.getHackPosition());
	parent::onFire(%data,%p,%slot);
}

function BreakActionShotgunImage::onDryFire(%data,%p,%slot)
{
	serverPlay3D("flaregun_dryfire_sound",%p.getHackPosition());
	parent::onDryFire(%data,%p,%slot);
}

function BreakActionShotgunImage::onBreak(%data,%p,%slot)
{
	%p.playThread(2,"plant");
	serverPlay3D("shotgun_break_sound",%p.getHackPosition());
}

function BreakActionShotgunImage::onUnload(%data,%p,%slot)
{
	schedule(400,0,"serverPlay3D","shotgun_shellDrop" @ getRandom(1,2) @ "_sound",MatrixMulPoint(%p.getPosition(),"-1 -2 0"));
}

function BreakActionShotgunImage::OnReloadFinish(%data,%p,%slot)
{
	%p.playThread(2,"plant");
	serverPlay3D("shotgun_load_sound",%p.getHackPosition());
	parent::OnReloadFinish(%data,%p,%slot);
}

function BreakActionShotgunImage::onUnbreak(%data,%p,%slot)
{
	%p.playThread(2,"plant");
	serverPlay3D("shotgun_unbreak_sound",%p.getHackPosition());
}

function BreakActionShotgunImage::OnReloadFail(%data,%p,%slot)
{
	parent::OnReloadFail(%data,%p,%slot);
}

datablock ItemData(ShotgunSlugItem)
{
	className = "Weapon";

	shapeFile = "./models/buckshot.dts";
	iconName = "./icons/SlugIcon";
	emap = 1;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	canDrop = 1;
	
	uiName = "Slug";
	
	ItemAmmo_ammo = "Shell";
	ItemAmmo_storageUnits = 1;
	ItemAmmo_projectile = "BreakActionShotgunProjectile";
	
	image = ShotgunSlugImage;
};

$c = -1;
datablock shapeBaseImageData(ShotgunSlugImage)
{
	className = "ItemAmmo_ContainerImage";

	shapeFile = "./models/buckshot.dts";
	emap = 1;

	correctMuzzleVector = 0;
	mountPoint = 0;
	armReady = 1;

	item = ShotgunSlugItem;

	stateName[$c++] = "ready";
	stateTransitionOnTriggerDown[$c] = "select";

	stateName[$c++] = "select";
	stateScript[$c] = "onSelect";
	stateTimeOutValue[$c] = 0.1;
	stateTransitionOnTriggerUp[$c] = "ready";
};