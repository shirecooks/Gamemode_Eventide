//
// Particle and debris data.
//

datablock ParticleData(throwingSpearTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 600;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/ring";

	colors[0]	= "0.75 0.75 0.75 0.3";
	colors[1]	= "0.75 0.75 0.75 0.2";
	colors[2]	= "1 1 1 0.0";
	sizes[0]	= 0.15;
	sizes[1]	= 0.35;
	sizes[2]	= 0.05;
	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(throwingSpearTrailEmitter)
{
   ejectionPeriodMS = 5;
   periodVarianceMS = 0;

   ejectionVelocity = 0; //0.25;
   velocityVariance = 0; //0.10;

   ejectionOffset = 0;

   thetaMin         = 0.0;
   thetaMax         = 90.0;  

   particles = throwingSpearTrailParticle;

   useEmitterColors = true;
   uiName = "Spear Trail";
};

datablock ParticleData(throwingSpearExplosionParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.5;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 900;
	lifetimeVarianceMS	= 300;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/cloud";

	colors[0]	= "0.3 0.3 0.2 0.9";
	colors[1]	= "0.2 0.2 0.2 0.0";
	sizes[0]	= 4.0;
	sizes[1]	= 7.0;
	times[0]	= 0.0;
	times[1]	= 1.0;
};

datablock ParticleEmitterData(throwingSpearExplosionEmitter)
{
   ejectionPeriodMS = 7;
   periodVarianceMS = 0;
   lifeTimeMS	   = 21;
   ejectionVelocity = 8;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "throwingSpearExplosionParticle";

   uiName = "Spear Smoke";
   emitterNode = TenthEmitterNode;
};

datablock ParticleData(throwingSpearExplosionParticle2)
{
	dragCoefficient		= 0.1;
	windCoefficient		= 0.0;
	gravityCoefficient	= 2.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 1000;
	lifetimeVarianceMS	= 500;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;

	textureName		= "base/data/particles/chunk";

	colors[0]	= "0.0 0.0 0.0 1.0";
	colors[1]	= "0.0 0.0 0.0 0.0";
	sizes[0]	= 0.5;
	sizes[1]	= 0.5;
	times[0]	= 0.0;
	times[1]	= 1.0;
};

datablock ParticleEmitterData(throwingSpearExplosionEmitter2)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   lifetimeMS       = 7;
   ejectionVelocity = 15;
   velocityVariance = 5.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "throwingSpearExplosionParticle2";

   useEmitterColors = true;
   uiName = "Throwing Spear Chunk";
   emitterNode = HalfEmitterNode;
};

datablock ExplosionData(throwingSpearExplosion)
{
   lifeTimeMS = 150;

   emitter[0] = throwingSpearExplosionEmitter;
   emitter[1] = throwingSpearExplosionEmitter2;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "7.0 8.0 7.0";
   camShakeAmp = "1.0 1.0 1.0";
   camShakeDuration = 0.5;
   camShakeRadius = 15.0;

   lightStartRadius = 4;
   lightEndRadius = 3;
   lightStartColor = "0.45 0.3 0.1";
   lightEndColor = "0 0 0";

   impulseRadius = 3.5;
   impulseForce = 2000;

   radiusDamage        = 0;
   damageRadius        = 0;
};

//
// Projectile data.
//

datablock ProjectileData(throwingSpearProjectile)
{
    projectileShapeName = "./models/spear/spearProjectile.dts";
    directDamage        = 50;
    directDamageType  = $DamageType::ThrowingSpear;
    radiusDamageType  = "";
    impactImpulse	   = 1000;
    verticalImpulse	   = 1000;
    explosion           = throwingSpearExplosion;
    particleEmitter     = throwingSpearTrailEmitter;

    brickExplosionRadius = 0;
    brickExplosionImpact = true; //destroy a brick if we hit it directly?
    brickExplosionForce  = 20;
    brickExplosionMaxVolume = 200;
    brickExplosionMaxVolumeFloating = 200;

    muzzleVelocity      = 50;
    velInheritFactor    = 1;

    armingDelay         = 0;
    lifetime            = 30000;
    fadeDelay           = 19500;
    bounceElasticity    = 0;
    bounceFriction      = 0;
    isBallistic         = true;
    gravityMod = 0.50;

    hasLight    = false;
    lightRadius = 3.0;
    lightColor  = "0 0 0.5";

    uiName = "Throwing Spear";
};

function throwingSpearProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
    parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);

    //If the spear hit a killer, stun them.
    if((%col.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj.sourceObject, %col))
    {
        %col.stun(2500);
    }

    //Play a wood-breaking sound.
    serverPlay3D("poolcue_smash" @ getRandom(1, 2) @ "_sound", %pos);
}

//
// Item and image data.
//

AddDamageType("ThrowingSpear", '<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_throwingSpear> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_throwingSpear> %1', 1, 1);

datablock ItemData(throwingSpearItem)
{
    category = "Weapon";
    className = "Weapon";

    shapeFile = "./models/spear/spear.dts";
    mass = 1;
    density = 0.2;
    elasticity = 0.2;
    friction = 0.6;
    emap = true;

    uiName = "Throwing Spear";
    iconName = "./icons/icon_throwingSpear";
    doColorShift = true;
    colorShiftColor = "0.400 0.196 0 1.000";

    image = throwingSpearImage;
    canDrop = true;
};

datablock ShapeBaseImageData(throwingSpearImage)
{
    className = "WeaponImage";

    shapeFile = throwingSpearItem.shapeFile;
    emap = false;

    mountPoint = 0;
    offset = "0 0 0";
    correctMuzzleVector = true;
    melee = false;

    item = throwingSpearItem;
    ammo = false;
    projectile = throwingSpearProjectile;
    projectileType = Projectile;

    armReady = true;

    doColorShift = true;
    colorShiftColor = "0.400 0.196 0 1.000";

    stateName[0]			= "Activate";
    stateTimeoutValue[0]		= 0.1;
    stateTransitionOnTimeout[0]	= "Ready";
    stateSequence[0]		= "ready";
    stateSound[0]					= weaponSwitchSound;

    stateName[1]			= "Ready";
    stateTransitionOnTriggerDown[1]	= "Charge";
    stateAllowImageChange[1]	= true;

    stateName[2]                    = "Charge";
    stateTransitionOnTimeout[2]	= "Armed";
    stateTimeoutValue[2]            = 0.7;
    stateWaitForTimeout[2]		= false;
    stateTransitionOnTriggerUp[2]	= "AbortCharge";
    stateScript[2]                  = "onCharge";
    stateAllowImageChange[2]        = false;

    stateName[3]			= "AbortCharge";
    stateTransitionOnTimeout[3]	= "Ready";
    stateTimeoutValue[3]		= 0.3;
    stateWaitForTimeout[3]		= true;
    stateScript[3]			= "onAbortCharge";
    stateAllowImageChange[3]	= false;

    stateName[4]			= "Armed";
    stateTransitionOnTriggerUp[4]	= "Fire";
    stateAllowImageChange[4]	= false;

    stateName[5]			= "Fire";
    stateTransitionOnTimeout[5]	= "Ready";
    stateTimeoutValue[5]		= 0.5;
    stateFire[5]			= true;
    stateSequence[5]		= "fire";
    stateScript[5]			= "onFire";
    stateWaitForTimeout[5]		= true;
    stateAllowImageChange[5]	= false;
};

//
// Sequence callbacks.
//

function throwingSpearImage::onCharge(%this, %obj, %slot)
{
	%obj.playThread(2, spearReady);
}

function throwingSpearImage::onAbortCharge(%this, %obj, %slot)
{
	%obj.playThread(2, root);
}

function throwingSpearImage::onFire(%this, %obj, %slot)
{
	%obj.playThread(2, spearThrow);
	Parent::onFire(%this, %obj, %slot);

    //Play a spear-throwing sound. Can't be a stateSound, as we un-equip it too fast and the sound gets cut off.
    serverPlay3D("throwingSpear_throw_sound", %obj.getMuzzlePoint($RightHandSlot));

    //Remove the spear from the player's inventory, as it is single use.
    %obj.removeItemFromInventory(%obj.currTool);
}