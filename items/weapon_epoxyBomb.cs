//
// Preferences.
//

$Pref::Eventide::EpoxyBomb::Range = 5;
$Pref::Eventide::EpoxyBomb::TickRate = 250;
$Pref::Eventide::EpoxyBomb::Damage = 30;
$Pref::Eventide::EpoxyBomb::EffectDuration = 6000;

//
// Explosion/particles/player emitters.
//

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
   particles = epoxySmokeParticle;
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
   particles = epoxyExplosionParticle;

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
	particles = epoxyExplosionFlashParticle;

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
   particles = epoxyDebrisTrailParticle;
};

datablock DebrisData(epoxyDebris)
{
	emitters = epoxyDebrisTrailEmitter;

	shapeFile = "base/data/shapes/empty.dts";
	lifetime = 20;
	minSpinSpeed = 0;
	maxSpinSpeed = 0;
	elasticity = 0.7;
	friction = 0.4;
	numBounces = 3;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = false;

	gravModifier = 1.0;
};

//
//The explosion. It's important that this has at least some damage to it, otherwise the `radiusImpulse` function will not fire.
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

	damageRadius = $Pref::Eventide::EpoxyBomb::Range;
	radiusDamage = 30;

	impulseRadius = $Pref::Eventide::EpoxyBomb::Range;
	impulseForce = $Pref::Eventide::EpoxyBomb::Range;
};

//
// Projectile data.
//

AddDamageType("Epoxy", '<bitmap:base/client/ui/ci/bomb> %1', '%2 <bitmap:base/client/ui/ci/bomb> %1', 0.5, 1);
datablock ProjectileData(epoxyProjectile)
{
	projectileShapeName = "base/data/shapes/empty.dts";
	directDamage        = $Pref::Eventide::EpoxyBomb::Damage;
	directDamageType    = $DamageType::Epoxy;
	radiusDamageType    = $DamageType::Epoxy;

	brickExplosionRadius = 0.0;
	brickExplosionImpact = false;          //destroy a brick if we hit it directly?
	brickExplosionForce  = 0;
	brickExplosionMaxVolume = 0;          //max volume of bricks that we can destroy
	brickExplosionMaxVolumeFloating = 0;  //max volume of bricks that we can destroy if they aren't connected to the ground

	impactImpulse	     = 5;
	verticalImpulse     = 5;
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

//Activates the effects of the epoxy bomb on any players within range.
function epoxyProjectile::radiusImpulse(%this, %obj, %col, %distanceFactor, %pos, %impulseAmt, %verticalAmt)
{
	if(%obj.sourceObject == %col || !minigameCanDamage(%obj.sourceObject, %col))
	{
		return parent::radiusImpulse(%this, %obj, %col, %distanceFactor, %pos, %impulseAmt, %verticalAmt);
	}

	//Apply the epoxy status effect to the victim, if unobstructed.
	%typemask = ($TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticShapeObjectType);
	%obstruction = ContainerRayCast(%pos, %col.getPosition(), %typemask);
	if(%obstruction $= "0")
	{
		%col.epoxify($Pref::Eventide::EpoxyBomb::EffectDuration);
	}

	//Do push the victim as any normal explosion would.
	return parent::radiusImpulse(%this, %obj, %col, %distanceFactor, %pos, %impulseAmt, %verticalAmt);
}

//
// Item and image.
//

datablock ItemData(epoxyItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/epoxyBomb/flatEpoxyBomb.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	doColorShift = false;
	colorShiftColor = "51 51 51 1";
	image = epoxyImage;
	canDrop = true;
	canPickup = true;
	uiName = "Epoxy Bomb";
	iconName = "./icon_epoxy";
};

datablock ShapeBaseImageData(epoxyImage)
{
	item = epoxyItem;

	shapeFile = "./models/epoxyBomb/uprightEpoxyBomb.dts";

	emap = true;
	mountPoint = 0;
	offset = "0 0.1 0";
	eyeOffset = "0 0 0";
	rotation = eulerToMatrix("-30 0 90");

	doColorShift = false;
	colorShiftColor = "";

	armReady = true;

	//The bomb has been equipped.
    stateName[0] = "Activate";
    stateTimeoutValue[0] = 0.01;
	stateTransitionOnTimeout[0] = "Ready";

	//The bomb is ready to place.
	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "AttemptPlace";
	stateAllowImageChange[1] = true;

	//Attempt to place the bomb. If it doesn't work, we just go back to the "Ready" state.
	stateName[2] = "AttemptPlace";
	stateScript[2] = "attemptPlace";
	stateAllowImageChange[2] = false;
	stateTransitionOnTriggerUp[2] = "Ready";
};

function epoxyImage::attemptPlace(%this, %obj, %slot)
{
	%start = %obj.getEyePoint();
	%aimVector = VectorNormalize(%obj.getLookVector());
	%end = VectorAdd(%start, VectorScale(%aimVector, 4));

	%typemask = ($TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticShapeObjectType);
	%obstruction = ContainerRayCast(%start, %end, %typemask);
	
	//The raycast didn't find something to place the epoxy bomb on, abort.
	if(%obstruction $= "0")
	{
		%client = %obj.client;
		if(isObject(%client))
		{
			%client.printFormatString("hint", "You must place the epoxy bomb on a surface.");
		}
		return;
	}

	//We found something, let's put down the epoxy bomb.
	%foundObject = firstWord(%obstruction);
	%normal = normalFromRaycast(%obstruction);

	//Move the position out slightly, so it doesn't clip into the surface.
	%adjustment = VectorScale(%normal, 0.1);
	%position = VectorAdd(posFromRaycast(%obstruction), %adjustment); 

	//Orient the bomb to face the surface normal.
	%referenceVector = "0 0 1";
	%directionVector = VectorNormalize(%normal);
	%rotationAxis = VectorNormalize(VectorCross(%directionVector, %referenceVector));

	%dot = VectorDot(%referenceVector, %directionVector);
	%rotationAngle = mACos(%dot);
	if(VectorLen(%rotationAxis) < 0.00001)
	{
		if(%dot > 0)
		{
			%rotationAxis = "0 0 1";
			%rotationAngle = 0;
		}
		else
		{
			%rotationAxis = "1 0 0";
			%rotationAngle = $pi;
		}
	}
	%rotation = %rotationAxis SPC %rotationAngle;

	//Rotate the static shape based on the direction of the player,
	//Also rotating it 225 degrees to match the orientation of the item in the player's hand.
	//TODO
	
	//Create the epoxy bomb static shape, and by extension the trigger zones and whatnot.
	%epoxyBomb = new StaticShape()
	{
		datablock = flatEpoxyShape;
		sourceObject = %obj;
		client = %obj.client;
	};
	%epoxyBomb.setTransform(%position SPC %rotation);
	%epoxyBomb.Datablock.updateTrigger(%epoxyBomb);

	//Assign the expoxy bomb to some object that can be tracked.
	//If it's a player, assign to their client. If it's a bot, assign it to their AIPlayer object and clear when it dies.
	%currentEpoxy = %obj.epoxy;
	%currentEpoxyData = %currentEpoxy.Datablock;

	if(isObject(%currentEpoxy))
	{
		//If the player already has an epoxy bomb placed, detonate it.
		%currentEpoxyData.detonate(%currentEpoxy);
	}
	%obj.epoxy = %epoxyBomb;
	
	%client = %obj.client;
	if(isObject(%client))
	{
		%currentEpoxy = %client.epoxy;
		if(isObject(%currentEpoxy))
		{
			%currentEpoxyData.detonate(%currentEpoxy);
		}

		%client.epoxy = %epoxyBomb;
	}

	//Play a sound effect.
	serverPlay3D("epoxyBomb_place1_sound", %epoxyBomb.getPosition());

	//Remove the epoxy bomb from the player's inventory, since it's been placed.
	%obj.removeItemFromInventory();
}

//
// Epoxy trap object, placed by the image.
//

//
// Trigger data for sensing players around the epoxy bomb.
datablock TriggerData(flatEpoxyTrigger)
{
    tickPeriodMS = $Pref::Eventide::EpoxyBombTickRate;
    polyhedron = -($Pref::Eventide::EpoxyBomb::Range / 2) SPC
				 -($Pref::Eventide::EpoxyBomb::Range / 2) SPC
				 -($Pref::Eventide::EpoxyBomb::Range / 2) SPC
             	 $Pref::Eventide::EpoxyBomb::Range @ " 0 0" SPC
             	 "0 " @ $Pref::Eventide::EpoxyBomb::Range @ " 0" SPC
             	 "0 0 " @ $Pref::Eventide::EpoxyBomb::Range;
};

function flatEpoxyTrigger::onTickTrigger(%this, %trigger)
{
	for(%i = 0; %i < %trigger.getNumObjects(); %i++)
	{
		%target = %trigger.getObject(%i);

		//If it isn't a player or we can't damage them, ignore.
		if(!(%target.getType() & $TypeMasks::PlayerObjectType) || %trigger.player == %target || !minigameCanDamage(%trigger.client, %target))
		{
			continue;
		}

		%epoxyBomb = %trigger.epoxy;

		//Approximate check to determine if the player isn't covered from the bomb.
		%foundVictim = containerRaycast(%epoxyBomb.getPosition(), %target.getHackPosition(), $TypeMasks::PlayerObjectType, %epoxyBomb);
		if(%foundVictim !$= "0" && %foundVictim == %target.getID())
		{
			//The victim is in view, explode the bomb.
			%epoxyBomb.Datablock.detonate(%epoxyBomb);
			return;
		}
		%epoxyBomb.Datablock.detonate(%epoxyBomb);
	}
}

//
// The static shape itself.
datablock StaticShapeData(flatEpoxyShape)
{
	shapeFile = "./models/epoxyBomb/flatEpoxyBomb.dts";
};

function flatEpoxyShape::onAdd(%this, %obj)
{
	//Create the trigger so the epoxy bomb can sense players and explode.
	%trigger = new Trigger()
	{
		datablock = flatEpoxyTrigger;
		polyhedron = flatEpoxyTrigger.polyhedron;
		position = %obj.getPosition();
		
		player = %obj;
		client = %obj.client; 
		epoxy = %obj;
	};
	%obj.trigger = %trigger;
	%this.updateTrigger(%obj);
}

function flatEpoxyShape::updateTrigger(%this, %obj)
{
	%obj.trigger.setTransform(%obj.getTransform());
}

function flatEpoxyShape::detonate(%this, %obj)
{
	//Create the visual effect and radius status effect.
	new Projectile()
	{
		datablock = epoxyProjectile;
		initialPosition = %obj.getPosition();
		initialVelocity = "0 0 0";
		sourceObject = %obj.client.player;
		client = %obj.client;
	}.explode();

	//We're done, get rid of the epoxy bomb.
	%obj.delete();
}

function flatEpoxyShape::onRemove(%this, %obj)
{
	//Get rid of the trigger so we stop detecting players.
	%trigger = %obj.trigger;
	if(isObject(%trigger))
	{
		%trigger.delete();
	}
}

//
// Status effect.
//

datablock ParticleEmitterData(playerEpoxifiedEmitter : epoxyDebrisTrailEmitter)
{
   ejectionPeriodMS = 20;
};


datablock ShapeBaseImageData(playerEpoxifiedImage)
{
	shapeFile = "base/data/shapes/empty.dts";

	mountPoint = $BackSlot;
	mountSlot = 3;

	offset = "0 0.5 0";
	eyeOffset = "0 0 -0.1";

	projectileType = Projectile;

	stateName[0] = "Epoxy";
	stateTimeoutValue[0] = 1.0;
	stateEmitter[0] = playerEpoxifiedEmitter;
	stateEmitterTime[0] = 1.0;
	stateTransitionOnTimeout[0] = "Loop";

	stateName[1] = "Loop";
	stateTransitionOnTimeout[1] = "Epoxy";
	stateTimeoutValue[1] = 0.01;
};

function PlayerEpoxyEffect::beginStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Slow down the player.
    %playerDatablock = %obj.getDataBlock();
    %playerDatablock.setTempSpeed(%obj, 0.5);

	//Attach a sticky emitter effect to the player.
	%obj.mountImage(playerEpoxifiedImage, playerEpoxifiedImage.mountSlot);
}

function PlayerEpoxyEffect::finalizeStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Allow the player to move at normal speed again. 
	//The `setTempSpeed` function does this automatically when no multiplier is supplied.
	%playerDatablock = %obj.getDataBlock();
    %playerDatablock.setTempSpeed(%obj);

	//Clear the sticky emitter.
	if(%obj.getMountedImage(playerEpoxifiedImage.mountSlot) == playerEpoxifiedImage.getID())
	{
		%obj.unmountImage(playerEpoxifiedImage.mountSlot);
	}
}

function Player::epoxify(%obj, %time)
{
    //Have the freeze time default to 1 second.
    if(%time $= "")
    {
        %time = 6000;
    }

    //Have the player enter the stun.
    %obj.applyStatusEffect("PlayerEpoxyEffect", "Debuff", %time);
}

//
// Package for managing epoxy bombs.
//

package Gamemode_Eventide_EpoxyBomb
{

	function GameConnection::onClientLeaveGame(%client)
	{
		%epoxyBomb = %client.epoxy;
		if(isObject(%epoxyBomb))
		{
			%epoxyBomb.delete();
		}

		Parent::onClientLeaveGame(%client);
	}

	function GameConnection::spawnPlayer(%this)
	{
		%epoxyBomb = %this.epoxy;
		if(isObject(%epoxyBomb))
		{
			%epoxyBomb.delete();
		}

		Parent::spawnPlayer(%this);
	}

	function Armor::onRemove(%this, %obj)
	{
		%epoxyBomb = %obj.epoxy;
		if(isObject(%epoxyBomb))
		{
			%epoxyBomb.delete();
		}

		Parent::onRemove(%this, %obj);
	}
};
if(isPackage(Gamemode_Eventide_EpoxyBomb))
{
	deactivatePackage(Gamemode_Eventide_EpoxyBomb);
}
activatePackage(Gamemode_Eventide_EpoxyBomb);
