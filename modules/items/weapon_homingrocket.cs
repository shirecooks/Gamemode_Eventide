//
// Support functions.
//

function getClosestIntersectionPoint(%viewingPoint, %lineStartPoint, %lineEndPoint)
{
	%line = VectorSub(%lineEndPoint, %lineStartPoint);
	%numberOfSteps = mCeil(VectorLen(%line)); //The longer the line, the more checks for intersections.
	%typemask = ($TypeMasks::FxBrickAlwaysObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::StaticShapeObjectType | $TypeMasks::TerrainObjectType);

	%segmentSize = VectorScale(%line, 1 / %numberOfSteps);
	for(%i = 0; %i < %numberOfSteps; %i++)
	{
		%possibleIntersection = VectorAdd(%lineStartPoint, VectorScale(%segmentSize, %i));
		
		%raycast = containerRayCast(%viewingPoint, %possibleIntersection, %typemask);
		%viewerToIntersectionCollision = getWord(%raycast, 0);

		if(!isObject(%viewerToIntersectionCollision))
		{
			//We didn't hit anything, the intersection is valid.
			return %possibleIntersection;
		}
	}

	return ""; //No viable intersection point.
}

//
// Projectile data.
//

datablock ExplosionData(homingRocketLauncherExplosion : rocketExplosion)
{
	impulseRadius = 6;
	impulseForce = 1000;
};

datablock ProjectileData(homingRocketLauncherProjectile)
{
	projectileShapeName = "Add-Ons/Projectile_GravityRocket/RocketGravityProjectile.dts";
	directDamage        = 100;
	directDamageType 	= $DamageType::RocketDirect;
	radiusDamageType 	= $DamageType::RocketRadius;
	impactImpulse	   	= 1000;
	verticalImpulse	   	= 1000;
	explosion           = homingRocketLauncherExplosion;
	particleEmitter     = rocketTrailEmitter;

	brickExplosionRadius = 3;
	brickExplosionImpact = false;          //destroy a brick if we hit it directly?
	brickExplosionForce  = 30;             
	brickExplosionMaxVolume = 30;          //max volume of bricks that we can destroy
	brickExplosionMaxVolumeFloating = 60;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

	sound = "rocket_fly_sound";

	muzzleVelocity      = 30; //Was originally 50.
	velInheritFactor    = 1.0;

	armingDelay         = 00;
	lifetime            = 4000;
	fadeDelay           = 3500;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.20;
	isBallistic         = false;
	gravityMod = 0.0;

	hasLight    = true;
	lightRadius = 5.0;
	lightColor  = "1.0 0.1 0.1";

	doColorShift = true;
	colorShiftColor = "0.8 0 0";

	bounceLimit = 5;
};

//
// Fizzle-out explosion data, dud rocket.
//

datablock ExplosionData(homingRocketFizzleOutExplosion)
{
	//explosionShape = "";
	sound = "";
	lifeTimeMS = 800;
	particleEmitter = rocketLauncherSmokeEmitter;
	particleDensity = 40;
	particleRadius = 0.5;
	shakeCamera = false;
};

datablock ProjectileData(homingRocketLauncherDudProjectile)
{
	projectileShapeName = "./models/DudRocketProjectile.dts";
	explosion           = homingRocketFizzleOutExplosion;
	bounceExplosion     = "";
	particleEmitter     = gunSmokeEmitter;
	explodeOnDeath = false;

	brickExplosionRadius = 0;
	brickExplosionImpact = 0;             //destroy a brick if we hit it directly?
	brickExplosionForce  = 0;             
	brickExplosionMaxVolume = 0;          //max volume of bricks that we can destroy
	brickExplosionMaxVolumeFloating = 0;  //max volume of bricks that we can destroy if they aren't connected to the ground (should always be >= brickExplosionMaxVolume)

	sound = "rocket_fizzle_sound";

	muzzleVelocity      = 65;
	velInheritFactor    = 1.0;

	armingDelay         = 15000;
	lifetime            = 15000;
	fadeDelay           = 9500;
	bounceElasticity    = 0.5;
	bounceFriction      = 0.5;
	isBallistic         = true;
	gravityMod          = 1.0;

	hasLight    = false;

	uiName = "Dud Homing Rocket"; 
};

//
// Scripted behavior.
//

package Gamemode_Eventide_Homing_Rocket
{
	function Projectile::onAdd(%obj)
	{
		Parent::onAdd(%obj);
		if(%obj.getDataBlock().getID() != homingRocketLauncherProjectile.getID())
		{
			//Not a homing rocket.
			return;
		}

		//Play the iconic error sound effect.
		serverPlay3D(errorSound, %obj.getPosition());

		if(%obj.completedTrackingDelay)
		{
			//The homing rocket is already tracking, don't waste our time with this logic.
			if(%obj.isBouncing)
			{
				%killerDataBlock = %obj.sourceObject.getDataBlock();
				%trackingDelay = (%killerDataBlock.homingRocketBounceTrackingDelay !$= "") ? %killerDataBlock.homingRocketBounceTrackingDelay : 250;
				%obj.schedule(%trackingDelay, homingRocketTick);
			}
			else
			{
				%obj.schedule(%obj.trackingTickRate, homingRocketTick);
			}
			return;
		}

		%sourceObject = %obj.sourceObject;
		if(!%sourceObject.getDataBlock().isKiller)
		{
			return;
		}
		else if(!%sourceObject.trackingCandidate && !%obj.trackingTarget)
		{
			return;
		}

		%killerDataBlock = %obj.sourceObject.getDatablock();

		//Get homing rocket preferences from the killer's datablock.
		%trackingDelay = (%killerDataBlock.homingRocketTrackingDelay !$= "") ? %killerDataBlock.homingRocketTrackingDelay : 250;
		%trackingTickRate = (%killerDataBlock.homingRocketTickRate !$= "") ? %killerDataBlock.homingRocketTickRate : 75;
		%bounceLimit = (%killerDataBlock.homingRocketBounceLimit !$= "") ? %killerDataBlock.homingRocketBounceLimit : 5;

		//Assign the rocket preferences to the rocket itself, so we don't have to run this logic every tick.
		%obj.trackingDelay = %trackingDelay;
		%obj.trackingTickRate = %trackingTickRate;
		%obj.bounceLimit = %bounceLimit;
		%obj.trackingTarget = %sourceObject.trackingCandidate;

		//Begin rocket tracking.
		%obj.schedule(%trackingDelay, homingRocketTick);
	}
};
activatePackage(Gamemode_Eventide_Homing_Rocket);

function Projectile::homingRocketTick(%this)
{
	%sourceObject = %this.sourceObject;
	%target = %this.trackingTarget;

	//No shooter or no target, let the rocket drift off.
	if(!isObject(%this) || !isObject(%target) || %target.isDisabled() || !isObject(%sourceObject))
	{
		return;
	}

	//Get the direction of the rocket.
	%currentVelocity = %this.getVelocity();

	if(VectorLen(%currentVelocity) == 0)
	{
		//The rocket isn't moving, it probably bugged out. Just make it explode.
		%this.explode();
		return;
	}

	%currentPosition = %this.getPosition();
	if(VectorDist(%currentPosition, %sourceObject.getPosition()) > 64) //1 Torque Units = 2 studs.
	{
		//To help prevent across-the-map sniping with homing rockets, cause the rocket to "fizzle-out" if it ventures too far from it's spawn point.
		%finalVelocity = VectorScale(%this.getVelocity(), 0.1);
		%this.delete();

		//Spawn a cloud of smoke.
		%fizzleOutExplosion = new Projectile()
		{
			dataBlock = homingRocketLauncherDudProjectile;
			initialPosition = %currentPosition;
			initialVelocity = "0 0 0";
			sourceObject = %sourceObject;
		};
		%fizzleOutExplosion.explode();

		//Spawn a dud rocket.
		%dudRocket = new Projectile()
		{
			dataBlock = homingRocketLauncherDudProjectile;
			initialPosition = %currentPosition;
			initialVelocity = %finalVelocity;
			sourceObject = %sourceObject;
		};
		MissionCleanup.add(%dudRocket);
		
		return;
	}

	%muzzleVelocity = %this.getDataBlock().muzzleVelocity; //Movement speed in Torque Units per second.

	%finalVelocity = "";

	if(%this.pathPoint && VectorDist(%currentPosition, %this.pathPoint) > 1)
	{
		//If the rocket has a previously defined path point and has not yet reached it, go there.
		%vectorTowardsPathPoint = VectorSub(%this.pathPoint, %currentPosition);
		%finalVelocity = VectorScale(VectorNormalize(%vectorTowardsPathPoint), %muzzleVelocity);
	}
	else
	{
		//We either don't have a pathing point or already reached it, let's see if we can make it to the target.
		%this.pathPoint = ""; 

		%targetPosition = %target.getHackPosition();

		//Is the path to the target blocked?
		%typemask = ($TypeMasks::FxBrickAlwaysObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::StaticShapeObjectType | $TypeMasks::TerrainObjectType);
		%raycast = containerRayCast(%currentPosition, %targetPosition, %typemask);
		%targetPathObjectHit = getWord(%raycast, 0);

		//The path directly to the target is blocked, we need to find a way around.
		if(isObject(%targetPathObjectHit))
		{
			%northVector = "0 1 0";
			%southVector = "0 -1 0";
			%eastVector = "1 0 0";
			%westVector = "-1 0 0";
			%upVector = "0 0 1";
			%downVector = "0 0 -1";

			%maximumLineDistance = 64; //128 studs.

			//
			// On each axis (the vectors above,) check if there is a clear path of intersection between it and the rocket. If so, lead the rocket there.
			//
			//Programming this made me hate my own guts.
			
			//Up?
			if(!%finalVelocity)
			{
				%targetUpPoint = VectorAdd(%targetPosition, VectorScale(%upVector, %maximumLineDistance));
				%upCollision = containerRaycast(%targetPosition, %targetUpPoint, %typemask);
				%targetUpPoint = (%upCollision !$= "0") ? (posFromRaycast(%upCollision)) : (%targetUpPoint);

				%upAttempt = getClosestIntersectionPoint(%currentPosition, %targetPosition, %targetUpPoint);
				if(%upAttempt !$= "")
				{
					%this.pathPoint = %upAttempt;
					%finalVelocity = VectorScale(VectorNormalize(VectorSub(%upAttempt, %currentPosition)), %muzzleVelocity);
				}
			}

			//North?
			if(!%finalVelocity)
			{
				%targetNorthPoint = VectorAdd(%targetPosition, VectorScale(%northVector, %maximumLineDistance));
				%northCollision = containerRaycast(%targetPosition, %targetNorthPoint, %typemask);
				%targetNorthPoint = (%northCollision !$= "0") ? (posFromRaycast(%northCollision)) : (%targetNorthPoint);

				%northAttempt = getClosestIntersectionPoint(%currentPosition, %targetPosition, %targetNorthPoint);
				if(%northAttempt)
				{
					%this.pathPoint = %northAttempt;
					%finalVelocity = VectorScale(VectorNormalize(VectorSub(%northAttempt, %currentPosition)), %muzzleVelocity);
				}
			}
			
			//South?
			if(!%finalVelocity)
			{
				%targetSouthPoint = VectorAdd(%targetPosition, VectorScale(%southVector, %maximumLineDistance));
				%southCollision = containerRaycast(%targetPosition, %targetSouthPoint, %typemask);
				%targetSouthPoint = (%southCollision !$= "0") ? (posFromRaycast(%southCollision)) : (%targetSouthPoint);

				%southAttempt = getClosestIntersectionPoint(%currentPosition, %targetPosition, %targetSouthPoint);
				if(%southAttempt)
				{
					%this.pathPoint = %southAttempt;
					%finalVelocity = VectorScale(VectorNormalize(VectorSub(%southAttempt, %currentPosition)), %muzzleVelocity);
				}
			}

			//East?
			if(!%finalVelocity)
			{
				%targetEastPoint = VectorAdd(%targetPosition, VectorScale(%eastVector, %maximumLineDistance));
				%eastCollision = containerRaycast(%targetPosition, %targetEastPoint, %typemask);
				%targetEastPoint = (%eastCollision !$= "0") ? (posFromRaycast(%eastCollision)) : (%targetEastPoint);

				%eastAttempt = getClosestIntersectionPoint(%currentPosition, %targetPosition, %targetEastPoint);
				if(%eastAttempt)
				{
					%this.pathPoint = %eastAttempt;
					%finalVelocity = VectorScale(VectorNormalize(VectorSub(%eastAttempt, %currentPosition)), %muzzleVelocity);
				}
			}

			//West?
			if(!%finalVelocity)
			{
				%targetWestPoint = VectorAdd(%targetPosition, VectorScale(%westVector, %maximumLineDistance));
				%westCollision = containerRaycast(%targetPosition, %targetWestPoint, %typemask);
				%targetWestPoint = (%westCollision !$= "0") ? (posFromRaycast(%westCollision)) : (%targetWestPoint);

				%westAttempt = getClosestIntersectionPoint(%currentPosition, %targetPosition, %targetWestPoint);
				if(%westAttempt)
				{
					%this.pathPoint = %westAttempt;
					%finalVelocity = VectorScale(VectorNormalize(VectorSub(%westAttempt, %currentPosition)), %muzzleVelocity);
				}
			}

			//Down? (This would likely never happen. If push comes to shove, we could remove this check for performance.)
			if(!%finalVelocity)
			{
				%targetDownPoint = VectorAdd(%targetPosition, VectorScale(%downVector, %maximumLineDistance));
				%downCollision = containerRaycast(%targetPosition, %targetDownPoint, %typemask);
				%targetDownPoint = (%downCollision !$= "0") ? (posFromRaycast(%downCollision)) : (%targetDownPoint);

				%downAttempt = getClosestIntersectionPoint(%currentPosition, %targetPosition, %targetDownPoint);
				if(%downAttempt)
				{
					%this.pathPoint = %downAttempt;
					%finalVelocity = VectorScale(VectorNormalize(VectorSub(%downAttempt, %currentPosition)), %muzzleVelocity);
				}
			}

			//None of the cardinal directions returned a clear path, just keep going forward.
			//This is done instead of just yoloing it towards the target to assist with doing trickshots.
			if(!%finalVelocity)
			{
				%finalVelocity = %currentVelocity;
			}
		}
		else
		{
			//No obstructions found, just go for the target.
			%vectorTowardsTarget = VectorNormalize(VectorSub(%targetPosition, %currentPosition));
			%initialVelocity = VectorScale(%vectorTowardsTarget, %muzzleVelocity);
			%velocityAdjustment = VectorSub(%initialVelocity, %currentVelocity);
			%finalVelocity = VectorAdd(%currentVelocity, %velocityAdjustment);
		}
	}

	//Setting the velocity of a rocket isn't possible, so we can do this instead.
	%newRocket = new Projectile()
	{
		dataBlock = %this.dataBlock;
		initialPosition = %currentPosition;
		initialVelocity = %finalVelocity;
		sourceObject = %this.sourceObject;
		client = %this.client;
		sourceSlot = %this.sourceSlot;
		originPoint = %this.originPoint;
		reflectTime = %this.reflectTime;

		//Custom data for rocket tracking, inherited from the original rocket.
		completedTrackingDelay = true;
		isBouncing = false;
		trackingDelay = %this.trackingDelay;
		trackingTickRate = %this.trackingTickRate;
		trackingTarget = %this.trackingTarget;
		pathPoint = %this.pathPoint;
		bounceCount = %this.bounceCount;
		bounceLimit = %this.bounceLimit;
	};
	
	//Remove the old rocket to give the illusion of a single rocket.
	if(isObject(%newRocket))
	{
		MissionCleanup.add(%newRocket);
		%newRocket.setScale(%this.getScale());
		%this.delete();
	}
}

//Make the rocket bounce, to compensate for bad/blocked tracking.
function homingRocketLauncherProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	if(%col.getClassName() $= "Player" || %col.getClassName() $= "AIPlayer" || %obj.bounceCount > %obj.bounceLimit)
	{
		%obj.explode();
	}
	else
	{
		%obj.bounceCount++;

		%vel = %obj.getLastImpactVelocity();
		%factor = 1.0;

		%bounceVel = VectorSub(%vel, VectorScale(%normal, VectorDot(%vel, %normal) * 2));
		%bounceVel = VectorScale(%bounceVel, %factor);
		if (VectorLen(%bounceVel) > 200)
		{
			%bounceVel = VectorScale(VectorNormalize(%bounceVel), 200);
		}

		%newRocket = new Projectile()
		{
			dataBlock = %this;
			initialPosition = %pos;
			initialVelocity = %bounceVel;
			sourceObject = %obj.sourceObject;
			client = %obj.client;
			sourceSlot = %obj.sourceSlot;
			originPoint = %obj.originPoint;
			reflectTime = %obj.reflectTime;

			//Custom data for rocket tracking, inherited from the original rocket.
			completedTrackingDelay = true;
			isBouncing = true;
			trackingDelay = %obj.trackingDelay;
			trackingTickRate = %obj.trackingTickRate;
			trackingTarget = %obj.trackingTarget;
			pathPoint = %obj.pathPoint;
			bounceCount = %obj.bounceCount;
			bounceLimit = %obj.bounceLimit;
		};
		MissionCleanup.add(%newRocket);

		//Play a metal clunk sound.
		serverPlay3D("rocket_impact" @ getRandom(1, 4) @ "_sound", %pos);
	}
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
}

//
// Warning light strobe before firing.
// 

function Player::SkyCaptainWarningLightOn(%obj, %hideLight)
{
	if(!isObject(%obj.skyCaptainWarningLight))
	{
		%obj.skyCaptainWarningLight = new fxLight() 
		{ 
			dataBlock = "RedLight"; 
		};
		MissionCleanup.add(%obj.skyCaptainWarningLight);

		//Only make the red light visible to other players if the captain is in the middle of a chase.
		if(%hideLight && isObject(%obj.client))
		{
			adjustObjectScopeToAll(%obj.skyCaptainWarningLight, true, %obj.client);
		}

		%obj.skyCaptainWarningLight.setTransform(%obj.getTransform());
		%obj.skyCaptainWarningLight.attachToObject(%obj);
		%obj.skyCaptainWarningLight.Player = %obj;
	}
}

function Player::SkyCaptainWarningLightOff(%obj)
{
	if(isObject(%obj.skyCaptainWarningLight))
	{
		%obj.skyCaptainWarningLight.delete();
	}
}

function Player::SkyCaptainWarningLightShow(%obj, %hideLight)
{
	%interval = 225;
	%numberOfBeeps = 4;

	//Sync the red light on/off pulses with the sound effect.
	for(%i = 0; %i < (%numberOfBeeps * 2); %i++)
	{
		%delay = (%interval * %i);
		if(%i % 2 == 0)
		{
			//Even tick, turn light on.
			%obj.schedule(%delay, "SkyCaptainWarningLightOn", %hideLight);
		}
		else
		{
			//Odd tick, turn light off.
			%obj.schedule(%delay, "SkyCaptainWarningLightOff");
		}
	}
}

//
// Item data.
//

datablock ItemData(homingRocketLauncherItem : rocketLauncherItem)
{
	shapeFile = "./models/homerocket.dts";
	uiName = "SC Rocket L.";
	image = homingRocketLauncherImage;
};

datablock ShapeBaseImageData(homingRocketLauncherImage)
{
	// Basic Item properties
	shapeFile = "./models/homerocket.dts";
	emap = true;

	// Specify mount point & offset for 3rd person, and eye offset
	// for first person rendering.
	mountPoint = $RightHandSlot;
	offset = "0 0 0";
	eyeOffset = 0; //"0.7 1.2 -0.5";
	rotation = eulerToMatrix( "0 0 0" );

	// When firing from a point offset from the eye, muzzle correction
	// will adjust the muzzle vector to point to the eye LOS point.
	// Since this weapon doesn't actually fire from the muzzle point,
	// we need to turn this off.  
	correctMuzzleVector = true;

	// Add the WeaponImage namespace as a parent, WeaponImage namespace
	// provides some hooks into the inventory system.
	className = "WeaponImage";

	// Projectile && Ammo.
	item = BowItem;
	ammo = " ";
	projectile = homingRocketLauncherProjectile;
	projectileType = Projectile;

	//casing = rocketLauncherShellDebris;
	shellExitDir        = "1.0 -1.3 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 15.0;	
	shellVelocity       = 7.0;

	//melee particles shoot from eye node for consistancy
	melee = false;
	//raise your arm up or not
	armReady = true;
	minShotTime = 0;  //Minimum time allowed between shots.

	doColorShift = true;
   	colorShiftColor = rocketLauncherItem.colorShiftColor;//"0.400 0.196 0 1.000";
	
	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.5;
	stateTransitionOnTimeout[0]       = "Ready";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]                     = "Ready";
	stateScript[1]                  = "onReady";
	stateTransitionOnTriggerDown[1]  = "attemptFire";
	stateAllowImageChange[1]         = true;
	stateTransitionOnNoAmmo[1]       = "NoAmmo";
	stateSequence[1]				= "Ready";

	//If Sky Captain has ammo, charge up a rocket and fire. If no, do nothing.
	stateName[2]					= "attemptFire";
	stateScript[2]                  = "onAttemptFire";
	stateFire[2]                    = true;
	stateTransitionOnAmmo[2]		= "Charge";
	stateTransitionOnNoAmmo[2]		= "NoAmmo";
	stateSequence[2]				= "Ready";

	stateName[3]                    = "Charge";
	stateScript[3]                  = "onCharge";
	stateTransitionOnTimeout[3]     = "Fire";
	//Wait for the warning sound effect to complete before firing. `getWavLength` is a BLPython function.
	stateTimeoutValue[3]            = 3.492;//(getWavLength(captain_weaponChargingLoud_sound.fileName) / 1000);
	stateWaitForTimeout[3]			= true;
	stateAllowImageChange[3]        = false;
	stateSequence[3]                = "Ready";

	stateName[4]                    = "Fire";
	stateTransitionOnTimeout[4]     = "Smoke";
	stateTimeoutValue[4]            = 0.1;
	stateFire[4]                    = false;
	stateAllowImageChange[4]        = false;
	stateSequence[4]                = "Fire";
	stateScript[4]                  = "onFire";
	stateWaitForTimeout[4]			= true;
	stateEmitter[4]					= rocketLauncherFlashEmitter;
	stateEmitterTime[4]				= 0.05;
	stateEmitterNode[4]				= tailNode;
	stateSound[4]					= rocketFireSound;

	stateName[5] = "Smoke";
	stateEmitter[5]					= rocketLauncherSmokeEmitter;
	stateEmitterTime[5]				= 0.05;
	stateEmitterNode[5]				= "muzzleNode";
	stateTimeoutValue[5]            = 0.1;
	stateSequence[5]                = "TrigDown";
	stateTransitionOnTimeout[5]     = "CoolDown";

	stateName[6] = "CoolDown";
	stateTimeoutValue[6]            = 0.5;
	stateTransitionOnTimeout[6]     = "Reload";
	stateSequence[6]                = "TrigDown";


	stateName[7]			= "Reload";
	stateTransitionOnTriggerUp[7]     = "Ready";
	stateSequence[7]	= "TrigDown";

	stateName[8]   = "NoAmmo";
	stateTransitionOnAmmo[8] = "Ready";
};

function homingRocketLauncherImage::onReady(%this, %obj, %slot)
{
	//Inherit ammo from Player object. Happens after a rocket is fired and when the item is equipped.
    %obj.setImageAmmo(%this.mountPoint, mClamp(%obj.SCMissleCount, 0, 999));
}

function homingRocketLauncherImage::onCharge(%this, %obj, %slot)
{
	//Play a sound que to warn the victim of Sky Captain's attack. Only do it loudly if Sky Captain isn't being stealthy.
	if(%obj.isChasing)
	{
		%obj.playAudio(1, "captain_weaponchargingloud_sound");
	}
	else
	{
		%obj.playAudio(1, "captain_weaponchargingquiet_sound");
	}

	//Red warning light, only visible to others when chasing a player.
	%obj.SkyCaptainWarningLightShow(!%obj.isChasing);
}

function homingRocketLauncherImage::onFire(%this, %obj, %slot)
{
	//Play a cute little animation.
	%obj.playThread(3, plant);
		
	//Update ammo.
	%obj.SCMissleCount--;
	%obj.lastFireTime = getSimTime();
	
	//Update the killer's ammo counter.
	%playerDatablock = %obj.getDataBlock();
	if(%playerDatablock.getName() $= "PlayerCaptain")
	{
		%playerDatablock.killerGUI(%obj, %obj.client);
	}

	//Determine the starting velocity of the rocket, factoring in the current velocity of the shooter.
	%projectile = %this.projectile;
	%shooterVelocity = %obj.getVelocity();
	%muzzleVector = %obj.getMuzzleVector(%slot);
	%initialRocketPath = VectorScale(%muzzleVector, %projectile.muzzleVelocity);
	%velocityInheritenceDelta = VectorScale(%shooterVelocity, %projectile.velInheritFactor);
	%finalVelocity = VectorAdd(%initialRocketPath, %velocityInheritenceDelta);

	%rocket = new (%this.projectileType)()
	{
		dataBlock = %this.projectile;
		initialVelocity = %finalVelocity;
		initialPosition = %obj.getMuzzlePoint(%slot);
		sourceObject = %obj;
		client = %obj.client;
		sourceSlot = %slot;

		bounceCount = 0;
		bounceLimit = %this.projectile.bounceLimit;

		//Signals to the "Projectile::onAdd" package that target tracking needs to be initialized.
		completedTrackingDelay = false;
	};
	MissionCleanup.add(%rocket);

	//Get the vertical size of the player, and scale the rocket accordingly.
	%scaleFactor = getWord(%obj.getScale(), 2);
	%rocket.setScale(%scaleFactor SPC %scaleFactor SPC %scaleFactor);

	return %rocket;
}