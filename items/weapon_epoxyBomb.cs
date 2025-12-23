if(!$pref::epoxyBomb::EpoxySearchRadius)
	$pref::epoxyBomb::EpoxySearchRadius = 5;


datablock ParticleData(epoxySmokeParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 525;
	lifetimeVarianceMS   = 55;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.9 0.9 0.9 0.5";
	colors[1]     = "1.0 1.0 0.9 0.0";
	sizes[0]      = 0.15;
	sizes[1]      = 0.1;

	useInvAlpha = false;
};
datablock ParticleEmitterData(epoxySmokeEmitter)
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
   particles = "epoxySmokeParticle";
};

datablock ParticleData(epoxyExplosionParticle)
{
	dragCoefficient      = 0.2;
	gravityCoefficient   = -0.4;
	inheritedVelFactor   = 0.6;
	constantAcceleration = 0.0;
	lifetimeMS           = 1500;
	lifetimeVarianceMS   = 400;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 12;
	spinRandomMin		= -25;
	spinRandomMax		= 25;
	colors[0]     = "0.8 0.8 0.7 0.3";
	colors[1]     = "1.0 1.0 0.8 0.55";
	colors[2]     = "1.0 1.0 0.9 0.0";
	sizes[0]      = 2.0;
	sizes[1]      = 6.0;
	sizes[2]      = 8.0;
	times[0]	= 0;
	times[1]	= 0.2;
	times[2]	= 1;

	useInvAlpha = true;
};
datablock ParticleEmitterData(epoxyExplosionEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 0.4;
   velocityVariance = 0.2;
   ejectionOffset   = 0.3;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "epoxyExplosionParticle";

   uiName = "Epoxy Explosion Smoke";
};

datablock ParticleData(epoxyExplosionFlashParticle)
{
	dragCoefficient      = 8;
	gravityCoefficient   = -0.4;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 60;
	lifetimeVarianceMS   = 45;
	textureName          = "base/data/particles/star1";
	spinSpeed		= 500.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "1 1 0.0 0.6";
	colors[1]     = "0.9 0.6 0.0 0.0";
	sizes[0]      = 7;
	sizes[1]      = 0;

	useInvAlpha = false;
};
datablock ParticleEmitterData(epoxyExplosionFlashEmitter)
{
	lifeTimeMS = 95;

   ejectionPeriodMS = 2;
   periodVarianceMS = 0;
   ejectionVelocity = 0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 89;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "epoxyExplosionFlashParticle";

   useEmitterColors = true;
   uiName = "Epoxy Explosion Flash";
};

datablock ParticleData(epoxyDebrisTrailParticle)
{
	dragCoefficient      = 0.4;
	gravityCoefficient   = 0.3;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	lifetimeMS           = 1500;
	lifetimeVarianceMS   = 70;
	textureName          = "base/data/particles/cloud";
	spinSpeed		= 12;
	spinRandomMin		= -25;
	spinRandomMax		= 25;
	colors[0]     = "0.9 0.9 0.4 1.0";
	colors[1]     = "0.9 0.9 0.4 1.0";
	colors[2]     = "0.9 0.9 0.4 0.9";
	colors[3]     = "1.0 1.0 0.4 0.0";
	sizes[0]      = 2.0;
	sizes[1]      = 1.8;
	sizes[2]      = 1.0;
	sizes[3]      = 0.2;
	times[0]	= 0;
	times[1]	= 0.2;
	times[2]	= 0.8;
	times[3]	= 1;

	useInvAlpha = true;
};
datablock ParticleEmitterData(epoxyDebrisTrailEmitter)
{
   ejectionPeriodMS = 100;
   periodVarianceMS = 0;
   ejectionVelocity = 0.0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.3;
   thetaMin         = 89;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "epoxyDebrisTrailParticle";
};

datablock DebrisData(epoxyDebris)
{
   emitters = "epoxyDebrisTrailEmitter";

	shapeFile = "base/data/shapes/empty.dts";
	lifetime = 20;
	minSpinSpeed = 0;
	maxSpinSpeed = 0;
	elasticity = 0.7;
	friction = 0.4;
	numBounces = 0;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = false;

	gravModifier = 8;
};

datablock ExplosionData(epoxyExplosion)
{
	soundProfile = VehicleExplosionSound;

   lifeTimeMS = 150;

   particleEmitter = epoxyExplosionEmitter;
   particleDensity = 6;
   particleRadius = 0.2;

   emitter[0] = epoxyExplosionFlashEmitter;

   faceViewer     = true;
   explosionScale = "1 1 1";

   debris = epoxyDebris;
   debrisNum = 20;
   debrisNumVariance = 3;
   debrisPhiMin = 0;
   debrisPhiMax = 360;
   debrisThetaMin = 0;
   debrisThetaMax = 180;
   debrisVelocity = 25;
   debrisVelocityVariance = 15;

   shakeCamera = false;
   camShakeFreq = "5.0 6.0 5.0";
   camShakeAmp = "2.0 7.0 2.0";
   camShakeDuration = 0.5;
   camShakeRadius = 20.0;

   // Dynamic light
   lightStartRadius = 0;
   lightEndRadius = 0;
   lightStartColor = "1 1 0 1";
   lightEndColor = "0 0 0 0";

   damageRadius = 10;
   radiusDamage = 10;

   impulseRadius = 0;
   impulseForce = 0;

};

AddDamageType("Epoxy",   '<bitmap:base/client/ui/ci/bomb> %1',    '%2 <bitmap:base/client/ui/ci/bomb> %1',0.5,1);
datablock ProjectileData(epoxyProjectile)
{
   projectileShapeName = "base/data/shapes/empty.dts";
   directDamage        = 0;
   directDamageType    = $DamageType::epoxy;
   radiusDamageType    = $DamageType::epoxy;

   brickExplosionRadius = 0.0;
   brickExplosionImpact = false;          //destroy a brick if we hit it directly?
   brickExplosionForce  = 0;
   brickExplosionMaxVolume = 0;          //max volume of bricks that we can destroy
   brickExplosionMaxVolumeFloating = 0;  //max volume of bricks that we can destroy if they aren't connected to the ground

   impactImpulse	     = 0;
   verticalImpulse     = 0;
   explosion           = epoxyExplosion;

   muzzleVelocity      = 60;
   velInheritFactor    = 1;

   armingDelay         = 0;
   lifetime            = 100;
   fadeDelay           = 900;
   bounceElasticity    = 0.5;
   bounceFriction      = 0.45;
   isBallistic         = false;
   gravityMod = 0.0;
   explodeondeath = true;

   hasLight    = false;
};

datablock ItemData(epoxyItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/epoxyBomb/HoldEpoxy.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	doColorShift = false;
	colorShiftColor = "51 51 51 1";
	image = epoxyImage;
	candrop = true;
	canPickup = true;
	uiName = "Epoxy Bomb";
	iconName = "./icon_epoxy";
};

datablock ShapeBaseImageData(epoxyImage)
{
	shapeFile = "./models/epoxyBomb/HoldEpoxy.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0 0 0";
	item = epoxyItem;
	doColorShift = false;
	colorShiftColor = "";
	armReady = true;
};

function epoxyImage::onMount(%this,%obj,%slot)
{
	%client = %obj.client;
	%obj.playThread(0, armreadyboth);

	if(isObject(%obj.client.epoxy))
	{
		// %obj.unMountImage(0);
	}
}
function epoxyImage::onUnMount(%this,%obj,%slot)
{
	%obj.playthread(0,Root);
	if(!isObject(%obj.client.epoxy))
		commandtoclient(%obj.client,'clearbottomprint');
}

datablock ShapeBaseImageData(epoxyPlayerTrailImagedata)
{
   shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = 2;

	stateName[0]				= "Ready";
	stateTransitionOnTimeout[0]		= "FireA";
	stateTimeoutValue[0]			= 0.01;

	stateName[1]				= "FireA";
	stateTransitionOnTimeout[1]		= "Ready";
	stateWaitForTimeout[1]			= True;
	stateTimeoutValue[1]			= 0.350;
	stateEmitter[1]				= "epoxyDebrisTrailEmitter";
	stateEmitterTime[1]			= 0.350;
};

datablock StaticShapeData(epoxyShape)
{
	shapeFile = "./models/epoxyBomb/Epoxy.dts";
};

function player::epoxify(%this, %duration)
{
	%this.setMaxForwardSpeed(%this.getMaxCrouchForwardSpeed());
	%this.setMaxSideSpeed(%this.getMaxCrouchSideSpeed());
	%this.setMaxBackwardSpeed(%this.getMaxCrouchBackwardSpeed());

	%this.mountImage(epoxyPlayerTrailImagedata, 3);

	%this.schedule(%duration, depoxify);
}

function player::depoxify(%this)
{
	%datablock = %this.getDatablock();

	%this.unmountImage(3);

	%this.setMaxForwardSpeed(%datablock.maxForwardSpeed);
	%this.setMaxSideSpeed(%datablock.maxSideSpeed);
	%this.setMaxBackwardSpeed(%datablock.maxBackwardSpeed);
}

function epoxySplosion(%this)
{
	%proj = new projectile()
	{
		initialPosition = %this.getPosition();
		datablock = epoxyProjectile;
		sourceObject = %this.client.player;
		client = %this.client;
	};

	%this.delete();
}

function epoxycheckForEnemies(%this)
{
	if(!isObject(%this))
		return;

	if(!isObject(%this.client) || !isObject(%this.client.player))
	{
		%this.delete();
		return;
	}

	//Search for an enemy.
	initContainerRadiusSearch(%this.getPosition(), $pref::epoxyBomb::EpoxySearchRadius, $TypeMasks::playerObjectType);


	while(isObject(%search = containerSearchNext()))
	{
		%iteration++;

		if(%search == %this.client.player)
		{
			continue;
		}

		else if(%search.getDamageState() $= "disabled")
		{
			continue;
		}

		else if(minigameCanDamage(%search, %this.client.player))
		{
			epoxySplosion(%this);
			return;
		}
	}

	schedule(500, %this, epoxyCheckForEnemies, %this);
}

package epoxyPackage
{

	function GameConnection::onClientLeaveGame(%client)
	{
		if(isObject(%client.epoxy))
			%client.epoxy.delete();
		Parent::onClientLeaveGame(%client);
	}

	function epoxyProjectile::radiusDamage(%this, %obj, %col, %distanceFactor, %pos, %damageAmt)
	{
		%return = parent::radiusDamage(%this, %obj, %col, %distanceFactor, %pos, %damageAmt);

		%col.epoxify(3000);

		return %return;
	}

	function gameConnection::spawnPlayer(%this)
	{
		if(isObject(%this.epoxy))
			%this.epoxy.delete();

		parent::spawnPlayer(%this);
	}

	function armor::onTrigger(%this, %obj, %triggerNum, %val)
	{
		%client = %obj.client;
		if(%triggerNum == 0 && %obj.getMountedImage(0) $= nameToID("epoxyimage"))
		{
			if(!%val)
			{
				%aimVec = %obj.getEyeVector();
				%start = %obj.getEyePoint();
				%end = vectorAdd(%start, vectorScale(%aimVec, 4.5));
				%targets = ($typeMasks::playerObjectType | $TypeMasks::FxBrickObjectType | $typeMasks::terrainObjectType);

				%ray = ContainerRayCast(%start, %end, %targets, %obj);
				%wall = firstWord(%ray);
				%position = getWords(%ray, 1, 3);

				if(isObject(%wall))
				{
					if(%wall.getType() & $TypeMasks::PlayerObjectType)
					{
						return;
					}

					if(isObject(%client.epoxy))
						%client.epoxy.delete();

					%currSlot = %obj.currtool;
					%obj.tool[%currSlot] = 0;
					%obj.weaponCount--;
					messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
					serverCmdUnUseTool(%obj.client);

					%client.epoxy = new StaticShape()
					{
						datablock = epoxyShape;
						client = %client;
						timer = %client.epoxytimer;
						mode = %client.epoxymode;
						position = vectorAdd(%position, "0 0 0.1");
					};

					MissionCleanup.Add(%client.epoxy);
					%client.epoxy.client = %client;
					%client.epoxy.checkSchedule = schedule(5000, %client.epoxy, epoxyCheckForEnemies, %client.epoxy);

					return;
				}
				else
				{
					%client.centerPrint("Epoxy must be mounted to something.", 2);
				}
			}
		}
		Parent::onTrigger(%this, %obj, %triggerNum, %val);
	}
};

activatePackage(epoxypackage);
