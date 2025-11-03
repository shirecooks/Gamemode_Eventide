datablock ParticleData(KillerGenericSharpClankDustParticle)
{
	//------------//
	// Rendering: //
	//------------//
	
	textureName = "base/data/particles/cloud";
	
	useInvAlpha = true;

	colors[0]	= "1 1 1 0.1";
	colors[1]	= "1 1 1 0.07";
	colors[2]	= "1 1 1 0";

	sizes[0]	= 0.35;
	sizes[1]	= 0.8;
	sizes[2]	= 1.2;
	
	times[0]	= 0;
	times[1]	= 0.2;
	times[2]	= 1;

	//-------------//
	// Properties: //
	//-------------//
	
	dragCoefficient = 3.7;
	gravityCoefficient = -0.35;

	inheritedVelFactor = 0;
	constantAcceleration = 0;

	lifetimeMS = 700;
	lifetimeVarianceMS = 200;

	spinSpeed = 20;
	spinRandomMin = -20;
	spinRandomMax = 20;
};
datablock ParticleEmitterData(KillerGenericSharpClankDustEmitter)
{
	//------------//
	// Rendering: //
	//------------//
	
	particles = "KillerGenericSharpClankDustParticle";
	
	overrideAdvance = false;
	useEmitterColors = false;

	//-------------//
	// Properties: //
	//-------------//
	
	ejectionPeriodMS = 2;
	periodVarianceMS = 0;
	
	ejectionVelocity = 8;
	velocityVariance = 2;
	
	ejectionOffset = 0.2;
	
	thetaMin = 80;
	thetaMax = 90;
	
	phiReferenceVel = 0;
	phiVariance = 360;
	
	uiName = "Sharp Clank Dust";
};

datablock ParticleData(KillerGenericSharpClankSprayParticle)
{
	//------------//
	// Rendering: //
	//------------//
	
	textureName = "./particles/hitSpark";
	
	useInvAlpha = false;

	colors[0]	= "1 1 1 1";
	colors[1]	= "1 1 1 1";

	sizes[0]	= 0.5;
	sizes[1]	= 1.25;
	
	times[0]	= 0;
	times[1]	= 1;

	//-------------//
	// Properties: //
	//-------------//
	
	dragCoefficient = 1;
	gravityCoefficient = 0;

	inheritedVelFactor = 0;
	constantAcceleration = 0;

	lifetimeMS = 130;
	lifetimeVarianceMS = 20;

	spinSpeed = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
};
datablock ParticleEmitterData(KillerGenericSharpClankSprayEmitter)
{
	//------------//
	// Rendering: //
	//------------//
	
	particles = "KillerGenericSharpClankSprayParticle";
	
	overrideAdvance = false;
	useEmitterColors = false;
	
	orientParticles = true;
	orientOnVelocity = true;

	//-------------//
	// Properties: //
	//-------------//
	
	ejectionPeriodMS = 16;
	periodVarianceMS = 2;

	ejectionVelocity = 14;
	velocityVariance = 0;
	
	ejectionOffset = 0;
	
	thetaMin = 70;
	thetaMax = 80;
	
	phiReferenceVel = 0;
	phiVariance = 360;
	
	uiName = "Sharp Clank Spray";
};

datablock ParticleData(KillerGenericSharpClankChunkParticle)
{
	//------------//
	// Rendering: //
	//------------//
	
	textureName = "base/data/particles/chunk";
	
	useInvAlpha = true;

	colors[0]	= "0.2 0.2 0.2 1";
	colors[1]	= "0.1 0.1 0.1 1";

	sizes[0]	= 1;
	sizes[1]	= 0;
	
	times[0]	= 0;
	times[1]	= 1;

	//-------------//
	// Properties: //
	//-------------//
	
	dragCoefficient = 0.2;
	gravityCoefficient = 1.7;

	inheritedVelFactor = 0;
	constantAcceleration = 0;

	lifetimeMS = 1000;
	lifetimeVarianceMS = 300;

	spinSpeed = 1000;
	spinRandomMin = -125;
	spinRandomMax = 125;
};
datablock ParticleEmitterData(KillerGenericSharpClankChunkEmitter)
{
	//------------//
	// Rendering: //
	//------------//
	
	particles = "KillerGenericSharpClankChunkParticle";
	
	overrideAdvance = false;
	useEmitterColors = false;

	//-------------//
	// Properties: //
	//-------------//
	
	ejectionPeriodMS = 18;
	periodVarianceMS = 3;
	
	ejectionVelocity = 8;
	velocityVariance = 4;
	
	ejectionOffset = 0.3;
	
	thetaMin = 5;
	thetaMax = 30;
	
	phiReferenceVel = 0;
	phiVariance = 360;
	
	uiName = "Sharp Clank Chunk";
};

datablock ExplosionData(KillerGenericSharpClankExplosion)
{
	//------------//
	// Rendering: //
	//------------//
	
	emitter[0] = KillerGenericSharpClankDustEmitter;
	emitter[1] = KillerGenericSharpClankSprayEmitter;
	emitter[2] = KillerGenericSharpClankChunkEmitter;
	
	//-------------//
	// Properties: //
	//-------------//

	lifeTimeMS = 150;
	
	shakeCamera = true;
	camShakeFreq = "10 11 10";
	camShakeAmp = "0.75 0.75 0.75";
	camShakeDuration = 0.75;
	camShakeRadius = 15;
};
datablock ProjectileData(KillerGenericSharpClankProjectile)
{
	//------------//
	// Explosion: //
	//------------//
	
	explosion = KillerGenericSharpClankExplosion;
	explodeOnDeath = true;
	
	//-------------//
	// Properties: //
	//-------------//
	
	uiName = "Sharp Clank";
};
