datablock ParticleData (Blockhead666Particle0)
{
	dragCoefficient = 0.1;
	gravityCoefficient = 0;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 1000;
	lifetimeVarianceMS = 500;
	useInvAlpha = 0;
	textureName = "./particles/binary0red";
	colors[0] = "0.6 0.0 0.0 0.0";
	colors[1] = "1   0   0 1.0";
	colors[2] = "0.6 0.0 0.0 0.0";
	sizes[0] = 0.4;
	sizes[1] = 0.2;
	sizes[2] = 0;
	times[0] = 0;
	times[1] = 0.2;
	times[2] = 1;
};
datablock ParticleData (Blockhead666Particle1)
{
	dragCoefficient = 0.1;
	gravityCoefficient = 0;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 1000;
	lifetimeVarianceMS = 500;
	useInvAlpha = 0;
	textureName = "./particles/binary1red";
	colors[0] = "0.6 0 0 0.0";
	colors[1] = "1 0 0 1.0";
	colors[2] = "0.6 0 0 0.0";
	sizes[0] = 0.4;
	sizes[1] = 0.2;
	sizes[2] = 0;
	times[0] = 0;
	times[1] = 0.2;
	times[2] = 1;
};
datablock ParticleEmitterData (Blockhead666Emitter)
{
	ejectionPeriodMS = 35;
	periodVarianceMS = 0;
	ejectionVelocity = 1;
	ejectionOffset = 0.75;
	velocityVariance = 0.24;
	thetaMin = 0;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	overrideAdvance = 0;
	particles = "Blockhead666Particle0 Blockhead666Particle1";
	uiName = "Blockhead 666 Emitter";
};
datablock ShapeBaseImageData (Blockhead666Image)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = 0;
	mountPoint = $BackSlot;
	eyeOffset = "0 0 -1000";
	stateName[0]               = "Wait";
	stateTimeoutValue[0]       = 1;
	stateEmitter[0]            = Blockhead666Emitter;
	stateEmitterTime[0]        = 5000;
	stateEmitterTime[0]        = 5;
	stateTransitionOnTimeout[0]= "Wait";
};