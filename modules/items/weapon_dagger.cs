datablock ParticleData(daggerFlashParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 80;
	lifetimeVarianceMS   = 15;
	textureName          = "base/data/particles/star1";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.6 0.6 0.1 0.9";
	colors[1]     = "0.6 0.6 0.6 0.0";
	sizes[0]      = 1.0;
	sizes[1]      = 2.0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(daggerFlashEmitter)
{
	ejectionPeriodMS = 3;
	periodVarianceMS = 0;
	ejectionVelocity = 1.0;
	velocityVariance = 1.0;
	ejectionOffset   = 0.0;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = daggerFlashParticle;
};

datablock ExplosionData(daggerExplosion)
{
	soundProfile = "";
	lifeTimeMS = 150;

	particleDensity = 5;
	particleRadius = 0.2;

	faceViewer     = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeDuration = 1;
	camShakeRadius = 10.0;

	camShakeFreq = "3 3 3";
	camShakeAmp = "0.6 0.6 0.6";
	particleEmitter = daggerFlashEmitter;

	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0 0 0";
	lightEndColor = "0 0 0";
};
datablock ProjectileData(daggerProjectile)
{
	explosion = daggerExplosion;
};

datablock ItemData(daggerItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/dagger.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Ceremonial Dagger";
	iconName = "./icons/icon_dagger";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";

	image = daggerImage;
	canDrop = true;
};

datablock ShapeBaseImageData(daggerImage)
{
    shapeFile = "./models/dagger.dts";
    emap = true;

    mountPoint = 0;
    offset = "0 0 0";
    correctMuzzleVector = false;
    eyeOffset = "0 0 0";
    className = "WeaponImage";

    item = daggerItem;
    ammo = " ";
    projectile = "";
    projectileType = Projectile;

    staticShape = "brickdaggerStaticShape";
    isRitual = true;	

    melee = true;
    doRetraction = false;
    armReady = false;
    doColorShift = daggerItem.doColorShift;
    colorShiftColor = daggerItem.colorShiftColor;

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0;
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
	stateSound[3]					= "sworddaggerswing_sound";
	stateTimeoutValue[3]            = 0.1;
	stateEmitter[3]					= "";
	stateEmitterNode[3]             = "muzzlePoint";
	stateEmitterTime[3]             = "0.225";

	stateName[4]			= "CheckFire";
	stateTransitionOnTriggerUp[4]	= "StopFire";
	stateTransitionOnTriggerDown[4]	= "StopFire";

	stateName[5]                    = "StopFire";
	stateTransitionOnTimeout[5]     = "Break";
	stateTimeoutValue[5]            = 0.1925;
	stateScript[5]                  = "onStopFire";
	stateEmitter[5]					= "";
	stateEmitterNode[5]             = "muzzlePoint";
	stateEmitterTime[5]             = "0.1";

	stateName[6]                    = "Break";
	stateTransitionOnTimeout[6]     = "Ready";
	stateTimeoutValue[6]            = 0.1;
};

datablock StaticShapeData(brickdaggerStaticShape)
{
	isInvincible = true;
	isRitual = true;
	shapeFile = "./models/daggerstatic.dts";
	placementSound = "sworddagger_place_sound";
};

function brickdaggerStaticShape::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	%obj.schedule(33,playaudio,3,%this.placementSound);
}

function daggerImage::onReady(%this, %obj, %slot)
{
	%obj.playthread(1, "root");
}

function daggerImage::onFire(%this, %obj, %slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;	

	%obj.playthread(1, "shiftTo");
	%startpos = %obj.getEyePoint();
	%endpos = %obj.getEyeVector();
	%typemasks = $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	
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
			%hit.applyImpulse(%hit.getposition(),vectorAdd(vectorScale(%obj.getEyeVector(),500),"0 0 500"));
			%hit.Damage(%obj, %hit.getPosition(), 40, $DamageType::Default);
			serverPlay3D("sworddaggerhitPL" @ getRandom(1,2) @ "_sound",%hitpos);
		}
		else
		{
			serverPlay3D("sworddaggerhitEnv" @ getRandom(1,2) @ "_sound",%hitpos);
		}
	}
}

function daggerImage::onPreFire(%this, %obj, %slot)
{	
	%obj.playthread(1, "shiftAway");
	%obj.schedule(75,spawnKillerTrail,PlayerRenowned.meleetrailskin,"0.4 1.2 0.375","0 -90 0","3 2.5 1");
}