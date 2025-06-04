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

// Abilitiy FX

datablock ParticleData(GlitchAmbientAParticle)
{
	dragCoefficient = 0;
	windCoefficient = 0;
	gravityCoefficient = 0.35;
	inheritedVelFactor = 0.25;
	constantAcceleration = 0;
	lifetimeMS = 1200;
	lifetimeVarianceMS = 500;
	spinSpeed = 0;
	spinRandomMin = -900;
	spinRandomMax = 900;
	useInvAlpha = false;
	framesPerSec = 1;
	textureName = "./particles/binary0red";

	colors[0] = "0.6 0.0 0.0 1.0";
	colors[1] = "1   0   0 1.0";
	colors[2] = "0.6 0.0 0.0 0.0";
	sizes[0] = 0.25;
	sizes[1] = 0.33;
	sizes[2] = 0.1;
	times[0] = 0;
	times[1] = 0.5;
	times[2] = 1;
};
datablock ParticleData(GlitchAmbientBParticle)
{
	dragCoefficient = 0;
	windCoefficient = 0;
	gravityCoefficient = 0.35;
	inheritedVelFactor = 0.25;
	constantAcceleration = 0;
	lifetimeMS = 1200;
	lifetimeVarianceMS = 500;
	spinSpeed = 0;
	spinRandomMin = -900;
	spinRandomMax = 900;
	useInvAlpha = false;
	framesPerSec = 1;
	textureName = "./particles/binary1red";

	colors[0] = "0.6 0.0 0.0 1.0";
	colors[1] = "1   0   0 1.0";
	colors[2] = "0.6 0.0 0.0 0.0";
	sizes[0] = 0.25;
	sizes[1] = 0.33;
	sizes[2] = 0.1;
	times[0] = 0;
	times[1] = 0.5;
	times[2] = 1;
};
datablock ParticleEmitterData(GlitchAmbientEmitter)
{
	ejectionPeriodMS = 30;
	periodVarianceMS = 5;
	ejectionVelocity = 1;
	velocityVariance = 0;
	ejectionOffset = 0;
	thetaMin = 0;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	particles = "GlitchAmbientAParticle GlitchAmbientBParticle";
	uiName = "Glitch - Ambient";
};
datablock ShapeBaseImageData(GlitchAmbientImage)
{
shapeFile = "base/data/shapes/empty.dts";
	emap = 0;
	mountPoint = $LeftHandSlot;
	stateName[0]               = "Wait";
	stateTimeoutValue[0]       = 1;
	stateEmitter[0]            = GlitchAmbientEmitter;
	stateEmitterTime[0]        = 5000;
	stateEmitterTime[0]        = 5;
	stateSound[0]			   = "eventide_spike_ambience_loop_sound";
	stateTransitionOnTimeout[0]= "Wait";
};


datablock DebrisData(glitchShardDebris)
{
   emitters = glitchAmbientEmitter;

	shapeFile = "./models/GlitchShard.dts";
	lifetime = 3;
	minSpinSpeed = -200;
	maxSpinSpeed = 200;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 1;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 1.5;
};

datablock ParticleData(GlitchExplosionAParticle)
{
	dragCoefficient = 1;
	windCoefficient = 0;
	gravityCoefficient = 1;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 1100;
	lifetimeVarianceMS = 300;
	spinSpeed = 0;
	spinRandomMin = -900;
	spinRandomMax = 900;
	useInvAlpha = false;
	textureName = "./particles/binary0red";
	colors[0] = "0.6 0.0 0.0 1.0";
	colors[1] = "1   0   0 1.0";
	colors[2] = "0.6 0.0 0.0 0.0";
	sizes[0] = 0.8;
	sizes[1] = 0.7;
	sizes[2] = 0.6;
	times[0] = 0;
	times[1] = 0.5;
	times[2] = 1;
};
datablock ParticleData(GlitchExplosionBParticle)
{
	dragCoefficient = 1;
	windCoefficient = 0;
	gravityCoefficient = 1;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 1100;
	lifetimeVarianceMS = 300;
	spinSpeed = 0;
	spinRandomMin = -900;
	spinRandomMax = 900;
	useInvAlpha = false;
	textureName = "./particles/binary1red";
	colors[0] = "0.6 0.0 0.0 1.0";
	colors[1] = "1   0   0 1.0";
	colors[2] = "0.6 0.0 0.0 0.0";
	sizes[0] = 0.8;
	sizes[1] = 0.7;
	sizes[2] = 0.6;
	times[0] = 0;
	times[1] = 0.5;
	times[2] = 1;
};
datablock ParticleEmitterData(GlitchExplosionEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 25;
	velocityVariance = 0;
	ejectionOffset = 1;
	thetaMin = 0;
	thetaMax = 75;
	phiReferenceVel = 0;
	phiVariance = 360;
	particles = "GlitchExplosionAParticle GlitchExplosionBParticle";
	lifetimeMS = 300;
};

datablock ExplosionData(GlitchExplosion)
{
	soundProfile = "GlitchSpikeExplosion_sound";

	lifeTimeMS = 200;

	particleEmitter = GlitchExplosionEmitter;
	particleDensity = 100;
	particleRadius = 5;

   debris = glitchShardDebris;
   debrisNum = 5;
   debrisNumVariance = 1;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 15;
   debrisThetaMax = 60;
   debrisVelocity = 22;
   debrisVelocityVariance = 2;

	emitter[0] = GlitchExplosionEmitter;

	faceViewer = true;
	explosionScale = "1 1 1";

	shakeCamera = false;
	camShakeFreq = "10.0 11.0 10.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 10.0;

	lightStartRadius = 10;
	lightEndRadius = 1;
	lightStartColor = "1 0.5 0";
	lightEndColor = "1 0.5 0";
};

// Projectile Data

datablock ProjectileData(GlitchProjectile)
{
	shapeFile = "base/data/shapes/empty.dts";
   directDamage = 0;
  // directDamageType = $DamageType::StoneSpike;
  // radiusDamageType = $DamageType::StoneSpike;
   collideWithPlayers = true;
   brickExplosionRadius = 5;
   brickExplosionImpact = true;
   brickExplosionForce = 50;
   brickExplosionMaxVolume = 15;
   brickExplosionMaxVolumeFloating = 25;

   impactImpulse = 0;
   verticalImpulse = 0;
   explosion = GlitchExplosion;
   particleEmitter = GlitchAmbientEmitter;

   muzzleVelocity = 60;
   velInheritFactor = 1;

   armingDelay = 0;
   lifetime = 4000;
   fadeDelay = 3500;
   bounceElasticity = 0.5;
   bounceFriction = 0.20;
   isBallistic = true;
   gravityMod = 1;

   hasLight = true;
   lightRadius = 2;
   lightColor = "1 0.5 0";
};

datablock StaticShapeData(GlitchSpikeData)
{
	category = "Statics";
	shapeFile = "./models/GlitchSpike.dts";
	skinName = 'null';
};

function GlitchProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	if(isObject(%col) && (%col.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj,%col))
	{
		%col.setTempSpeed(0.5);
		%col.schedule(2000,setTempSpeed,1);
		%col.addhealth(-15);
		%col.markedforGlitchDeath = true;
		%spike.schedule(100, delete);
		%start = %col.getPosition();
		//%end = vectorAdd(%start, "0 0 " @ -2 * getWord(%col.getScale(), 2));
		%ray = containerRayCast(%start, %end, $TypeMasks::FxBrickObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType);
		if(!isObject(%col = firstWord(%ray)))
		{
			return;
		}
		%pos = posFromRaycast(%ray);
		%normal = normalFromRaycast(%ray);
	}
	if(%col.getType() & $TypeMasks::InteriorObjectType || %col.getType() & $TypeMasks::FxBrickObjectType || %col.getType() & $TypeMasks::TerrainObjectType)
	{
		%scale = "1.5 1.5 1.5";
		%spike = new TSstatic()
		{
			datablock = GlitchSpikeData;
			shapeName = "./models/GlitchSpike.dts";
			position = %pos;
			rotation = "0 0 0 0";
			scale = %scale;
		};
		MissionCleanup.add(%spike);

		for(%i = 2; %i <= 24; %i++)
		{
			%spike.schedule(%i * 100 + 13000, setScale, vectorScale(%scale, 1 / (%i / 2)));
		}
		GlitchSpikeData.schedule(10, doDamage, %spike, %obj.sourceObject, %pos, %scale);
		%spike.schedule(15000, delete);
	}
}

function GlitchSpikeData::doDamage(%data, %spike, %obj, %pos, %scale)
{
	%scale = getWord(%scale, 2);
	%typemasks = $Typemasks::PlayerObjectType | $Typemasks::VehicleObjectType;
	InitContainerRadiusSearch(%pos, 2 * %scale, %typemasks);
	while(isObject(%hit = ContainerSearchNext()))
	{
		if(minigameCanDamage(%obj, %hit) && getMinigameFromObject(%hit).weaponDamage)
		{
			%hit.damage(%obj, %pos, 3 * %scale, $DamageType::GlitchSpike);
			%add = vectorScale(vectorAdd("0 0 1", getRandom(-2, 2) SPC getRandom(-2, 2) SPC getRandom(0, 5)), %scale);
			%hit.setVelocity(vectorAdd(%hit.getVelocity(), %add));
			%hit.lastPusher = %obj.sourceObject;
			%hit.lastPushTime = getSimTime();
		}
	}
	%boxpos = vectorAdd(%pos, "0 0 " @ %scale * 1);
	%boxsize = vectorScale("3 3 7", %scale);
	InitContainerBoxSearch(%boxpos, %boxsize, %typemasks);
	while(isObject(%hit = ContainerSearchNext()))
	{
		if(minigameCanDamage(%obj, %hit) && getMinigameFromObject(%hit).weaponDamage)
		{
			%hit.damage(%obj, %pos, 7 * %scale, $DamageType::GlitchSpike);
			%hit.lastPusher = %obj.sourceObject;
			%hit.lastPushTime = getSimTime();
			%add = vectorScale(vectorAdd("0 0 4", getRandom(-2, 2) SPC getRandom(-2, 2) SPC getRandom(0, 2)), %scale);
			%hit.setVelocity(vectorAdd(%hit.getVelocity(), %add));
		}
	}
}