//
// Particle and debris data.
//

//
// Player hit projectile.

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

datablock ProjectileData(daggerHitProjectile)
{
	explosion = daggerExplosion;

	hitSound = "dagger_hitPlayer";
	hitSoundAmount = 2;
};

//
// Environment hit projectile.

datablock ProjectileData(daggerObscureProjectile : KillerKatanaClankProjectile)
{
	hitSound = "dagger_hitEnv";
	hitSoundAmount = 2;
};

//
// Item and image data.
//

datablock ItemData(daggerItem : ritualItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/ritualDagger/ritualDaggerDrop.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Ceremonial Dagger";
	iconName = "./icons/icon_ritualDagger";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";

	image = daggerImage;
	canDrop = true;

	ritualType = "Dagger";
	maxRitualsOnCircle = 1;
	possibleOffset1 = "0 -0.65 0.1 90 0 0";
};
daggerItem.inheritFunctionsFromSuperClass("ritualItem");

datablock ShapeBaseImageData(daggerImage : eventideMeleeImage)
{
    shapeFile = "./models/ritualDagger/ritualDagger.dts";
    item = daggerItem;
    staticShape = "brickDaggerStaticShape";
    isRitual = true;	

    doColorShift = daggerItem.doColorShift;
    colorShiftColor = daggerItem.colorShiftColor;

	hitProjectile = daggerHitProjectile;
	hitObscureProjectile = daggerObscureProjectile;

	meleeTrail = $Eventide_MeleeTrails["base.trail"];
	swingSound = "generic_lightSwing";
	swingSoundAmount = 5;
};
daggerImage.inheritFunctionsFromSuperClass("eventideMeleeImage");