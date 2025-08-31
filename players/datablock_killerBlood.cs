//
// Generic blood effects.
//

datablock ParticleData(KillerBloodRingParticle)
{
	//------------//
	// Rendering: //
	//------------//
	
	textureName = "./particles/hitRing";

	useInvAlpha = true;

	colors[0]	= "0.65 0 0 1";
	colors[1]	= "0.40 0 0 1";

	sizes[0]	= 1;
	sizes[1]	= 3.35;

	times[0]	= 0;
	times[1]	= 1;

	//-------------//
	// Properties: //
	//-------------//
	
	dragCoefficient = 1;
	gravityCoefficient = 0;

	inheritedVelFactor = 0;
	constantAcceleration = 0;

	lifetimeMS = 200;
	lifetimeVarianceMS = 0;

	spinSpeed = 1000;
	spinRandomMin = -250;
	spinRandomMax = 250;
};
datablock ParticleEmitterData(KillerBloodRingEmitter)
{
	//------------//
	// Rendering: //
	//------------//
	
	particles = "KillerBloodRingParticle";
	
	overrideAdvance = false;
	useEmitterColors = false;

	//-------------//
	// Properties: //
	//-------------//
	
	ejectionPeriodMS = 150;
	periodVarianceMS = 0;
	
	ejectionVelocity = 0;
	velocityVariance = 0;
	
	ejectionOffset = 0;
	
	thetaMin = 89;
	thetaMax = 90;
	
	phiReferenceVel = 0;
	phiVariance = 0;
	
	uiName = "Killer's Blood Ring";
};

datablock ParticleData(KillerBloodSprayParticle)
{
	//------------//
	// Rendering: //
	//------------//
	
	textureName = "./particles/hitSpark";
	
	useInvAlpha = true;
	
	colors[0]	= "0.65 0 0 1";
	colors[1]	= "0.40 0 0 1";

	sizes[0]	= 0.75;
	sizes[1]	= 1.3125;
	
	times[0]	= 0;
	times[1]	= 1;

	//-------------//
	// Properties: //
	//-------------//
	
	dragCoefficient = 1;
	gravityCoefficient = 0;

	inheritedVelFactor = 0;
	constantAcceleration = 0;

	lifetimeMS = 120;
	lifetimeVarianceMS = 25;

	spinSpeed = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
};
datablock ParticleEmitterData(KillerBloodSprayEmitter)
{
	//------------//
	// Rendering: //
	//------------//
	
	particles = "KillerBloodSprayParticle";
	
	overrideAdvance = false;
	useEmitterColors = false;
	
	orientParticles = true;
	orientOnVelocity = true;

	//-------------//
	// Properties: //
	//-------------//
	
	ejectionPeriodMS = 18;
	periodVarianceMS = 0;
	
	ejectionVelocity = 22;
	velocityVariance = 3;
	
	ejectionOffset = 0.3;
	
	thetaMin = 6;
	thetaMax = 35;
	
	phiReferenceVel = 0;
	phiVariance = 360;
	
	uiName = "Killer's Blood Spray";
};

datablock ParticleData(KillerBloodSplatterParticle : KillerBloodSprayParticle)
{
	//------------//
	// Rendering: //
	//------------//
	
	textureName = "./particles/hitWave";
	
	sizes[0]	= 1.75;
	sizes[1]	= 3;

	//-------------//
	// Properties: //
	//-------------//
	
	dragCoefficient = 2;

	lifetimeMS = 90;
	lifetimeVarianceMS = 20;
};
datablock ParticleEmitterData(KillerBloodSplatterEmitter : KillerBloodSprayEmitter)
{
	//------------//
	// Rendering: //
	//------------//
	
	particles = "KillerBloodSplatterParticle";

	//-------------//
	// Properties: //
	//-------------//
	
	ejectionPeriodMS = 60;
	
	ejectionVelocity = 18;
	velocityVariance = 4;
	
	ejectionOffset = 0.5;
	
	thetaMin = 12;
	thetaMax = 60;
	
	uiName = "Killer's Blood Splatter";
};

datablock ParticleData(KillerBloodDropletParticle)
{
	//------------//
	// Rendering: //
	//------------//
	
	textureName = "base/data/particles/dot";
	
	useInvAlpha = true;

	colors[0]	= "0.65 0 0 1";
	colors[1]	= "0.40 0 0 1";

	sizes[0]	= 0.25;
	sizes[1]	= 0.1;
	
	times[0]	= 0;
	times[1]	= 1;

	//-------------//
	// Properties: //
	//-------------//
	
	dragCoefficient = 2.7;
	gravityCoefficient = 0.9;

	inheritedVelFactor = 0;
	constantAcceleration = 0;

	lifetimeMS = 660;
	lifetimeVarianceMS = 170;

	spinSpeed = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
};
datablock ParticleEmitterData(KillerBloodDropletEmitter)
{
	//------------//
	// Rendering: //
	//------------//
	
	particles = "KillerBloodDropletParticle";
	
	overrideAdvance = false;
	useEmitterColors = false;

	//-------------//
	// Properties: //
	//-------------//
	
	ejectionPeriodMS = 20;
	periodVarianceMS = 0;
	
	ejectionVelocity = 16;
	velocityVariance = 4;
	
	ejectionOffset = 0.4;
	
	thetaMin = 2;
	thetaMax = 55;
	
	phiReferenceVel = 0;
	phiVariance = 360;
	
	uiName = "Killer's Blood Droplet";
};

datablock ParticleData(KillerSlashParticle)
{
	//------------//
	// Rendering: //
	//------------//
	
	textureName = "./particles/thinSlash";
	
	useInvAlpha = false;

	colors[0]	= "1 0.9 0.8 1";
	colors[1]	= "1 0.7 0.6 1";
	colors[2]	= "1 0.3 0.2 0";

	sizes[0]	= 8;
	sizes[1]	= 8;
	sizes[2]	= 8;
	
	times[0]	= 0;
	times[1]	= 0.3;
	times[2]	= 1;

	//-------------//
	// Properties: //
	//-------------//
	
	dragCoefficient = 3.2;
	gravityCoefficient = 0;

	inheritedVelFactor = 0;
	constantAcceleration = 0;

	lifetimeMS = 1000;
	lifetimeVarianceMS = 0;

	spinSpeed = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
};
datablock ParticleEmitterData(KillerSlashEmitter)
{
	//------------//
	// Rendering: //
	//------------//
	
	particles = "KillerSlashParticle";
	
	overrideAdvance = false;
	useEmitterColors = false;
	
	orientParticles = true;
	orientOnVelocity = true;

	//-------------//
	// Properties: //
	//-------------//
	
	ejectionPeriodMS = 76;
	periodVarianceMS = 0;
	
	ejectionVelocity = -14;
	velocityVariance = 0;
	
	ejectionOffset = 3;
	
	thetaMin = 160;
	thetaMax = 180;
	
	phiReferenceVel = 0;
	phiVariance = 360;
	
	uiName = "Killer's Slash";
};

//
// Generic weapon hit effects, dependent on the blood effects.
//

datablock ExplosionData(KillerSharpHitExplosion)
{
	//------------//
	// Rendering: //
	//------------//
	
	particleEmitter = KillerBloodRingEmitter;
	particleDensity = 1;
	particleRadius = 0;

	emitter[0] = KillerBloodSprayEmitter;
	emitter[1] = KillerBloodDropletEmitter;
	emitter[2] = KillerSlashEmitter;
	
	//-------------//
	// Properties: //
	//-------------//

	lifeTimeMS = 150;
	
	shakeCamera = true;
	camShakeFreq = "2 3 2";
	camShakeAmp = "1 1 1";
	camShakeDuration = 0.75;
	camShakeRadius = 15;
};
datablock ProjectileData(KillerSharpHitProjectile)
{
	//------------//
	// Explosion: //
	//------------//
	
	explosion = KillerSharpHitExplosion;
	explodeOnDeath = true;
	
	//-------------//
	// Properties: //
	//-------------//
	
	uiName = "Killer's Sharp Hit";

	hitSound = "generic_playerSharpHit";
	hitSoundAmount = 3;
};

datablock ExplosionData(KillerRoughHitExplosion : KillerSharpHitExplosion)
{
	//------------//
	// Rendering: //
	//------------//

	emitter[0] = KillerBloodSplatterEmitter;
	emitter[1] = KillerBloodDropletEmitter;
};
datablock ProjectileData(KillerRoughHitProjectile : KillerSharpHitProjectile)
{
	//------------//
	// Explosion: //
	//------------//
	
	explosion = KillerRoughHitExplosion;
	
	//-------------//
	// Properties: //
	//-------------//
	
	uiName = "Killer's Rough Hit";
};