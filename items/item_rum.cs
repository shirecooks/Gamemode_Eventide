//
// Explosions/particles/emitters.
//

//
// Bottle cork debris.
datablock DebrisData(rumBottleCorkDebris)
{
	shapeFile = "./models/rum/cork.dts";
	lifetime = 5.0;
	minSpinSpeed = -400.0;
	maxSpinSpeed = 200.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 3;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;
	gravModifier = 2;
};

datablock ExplosionData(rumBottleCorkDebrisExplosion)
{
	debris = rumBottleCorkDebris;
	debrisNum = 1;
	debrisPhiMax = 360;
	debrisPhiMin = 180;
	debrisThetaMax = 180;
	debrisThetaMin = 160;
	debrisVelocity = 2;
};

datablock ProjectileData(rumBottleCorkProjectile)
{
	explosion = rumBottleCorkDebrisExplosion;
};

//
// Bottle debris.
datablock DebrisData(rumBottleDebris)
{
	elasticity = 0.5;
	gravModifier = 0.9;
	lifeTime = 5;
	maxSpinSpeed = 1000;
	numBounces = 1;
    bounceVariance = 1;
	fade = true;
	snapOnMaxBounce = false;
	staticOnMaxBounce = true;
	shapeFile = "./models/rum/bottle.dts";
	velocity = 0;
};

datablock ExplosionData(rumBottleDebrisExplosion : rumBottleCorkDebrisExplosion)
{
	debris = rumBottleDebris;
};

datablock ProjectileData(rumBottleProjectile)
{
	explosion = rumBottleDebrisExplosion;
};

//
// Particles and emitters.
//

datablock ParticleData(drunkParticleA)
{
	textureName = "base/data/particles/thinRing";
	dragCoefficient = 1;
	gravityCoefficient = -0.1;
	inheritedVelFactor = 1.0;
	lifetimeMS = 250;
	lifetimeVarianceMS = 10;

	colors[0] = "1.0 1.0 1.0 1.0";
	colors[1] = "1.0 1.0 1.0 1.0";

	sizes[0] = 0.3;
    sizes[1] = 0.0;

	times[0] = 0.0;
	times[1] = 1.0;
};

datablock ParticleEmitterData(drunkEmitter)
{
	particles = drunkParticleA;

	thetaMin = 0;
	thetaMax = 180;

	ejectionOffset = 1.0;
	ejectionVelocity = 0.0;
	velocityVariance = 0.0;

	ejectionPeriodMS = 4;
	periodVarianceMS = 2;

	phiReferenceVel = 0;
	phiVariance = 360;

	useInvAlpha = 0;
	orientParticles = 0;
	overrideAdvance = 0;
};

//
// The item and image data.
//

datablock ItemData(rumBottleItem)
{
	category = "Weapon";
	className = "Weapon";
	
	shapeFile = "./models/rum/bottle.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	
	uiName = "Pirate's Rum";
	iconName = "./icons/icon_rum";
	doColorShift = true;
	colorShiftColor = "0.392 0.196 0 1";
	
	image = rumBottleImage;
	canDrop = true;
};

datablock ShapeBaseImageData(rumBottleImage : cooldownImage)
{
	shapeFile = "./models/rum/bottle.dts";
	emap = true;
	mountPoint = $RightHandSlot;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix("0 0 0");
	
	className = "WeaponImage";
	item = RumBottleItem;
	
	armReady = true;
	
	doColorShift = RumBottleItem.doColorShift;
	colorShiftColor = RumBottleItem.colorShiftColor;
	
	casing = RumBottleCorkDebris;
	shellExitDir		= "1.0 1.0 1.0";
	shellExitOffset		= "0 0 0";
	shellExitVariance	= 5;
	shellVelocity		= 10;

	//The rum has been equipped.
    stateName[0] = "Activate";
	stateAllowImageChange[0] = false;
    stateTimeoutValue[0] = 0.01;

	//The rum is inactive, simply being held.
    stateName[1] = "Ready";
    stateScript[1] = "onReady";
	stateAllowImageChange[1] = true;
    stateTransitionOnTriggerDown[1]	= "Open";

	//The rum has been opened, prepare to drink.
	stateName[2] = "Open";
	stateScript[2] = "onOpen";
	stateAllowImageChange[2] = false;
	stateWaitForTimeout[2] = true;
	stateTimeOutValue[2] = 1;
	stateTransitionOnTimeout[2] = "Drink";

	//Activate the status effect, put the rum on cooldown.
	stateName[3] = "Drink";
	stateScript[3] = "onDrink";
	stateAllowImageChange[3] = false;
	stateWaitForTimeout[3] = true;
	stateTimeOutValue[3] = 0.5;
	stateTransitionOnTimeout[3] = "Discard";

    //Remove the rum bottle from the player's inventory, spawn debris.
	stateName[4] = "Discard";
	stateScript[4] = "onDiscard";
	stateAllowImageChange[4] = false;
	stateWaitForTimeout[4] = false;

	cooldown = 60000;
};
rumBottleImage.implementCooldownCallbacks();

function rumBottleImage::getHintMessage(%this, %obj)
{
	return "Hunter giving you a hard time? Drown your sorrows.";
}

//
// Sequence callbacks.

function rumBottleImage::onCooldown(%this, %obj)
{
	//The rum bottle was raised during the drinking animation, lower it again.
	%obj.playThread(1, root);

	%client = %obj.client;
	if(isObject(%client)) 
	{
		%client.printFormatString("hint", "You've had enough to drink.");
	}
}

function rumBottleImage::onReady(%this, %obj)
{

}

function rumBottleImage::onOpen(%this, %obj)
{
	//Play the sound of the rum opening.
	serverPlay3D("rum_open_sound", %obj.getMuzzlePoint(%this.mountPoint));

	//Raise the bottle so it is ready to drink.
	%obj.playThread(1, armReadyRight);

	//Play an animation of cracking open the tab.
	%obj.playThread(2, shiftleft);

	//Spawn a cork.
	new Projectile()
	{
		dataBlock = rumBottleCorkProjectile;
		initialPosition = %obj.getMuzzlePoint(%this.mountPoint);
	}.explode();
}

function rumBottleImage::onDrink(%this, %obj)
{
	//Play the rum drinking sound and animation.
	serverPlay3D("drink_gulp" @ getRandom(1, 3) @ "_sound", %obj.getEyePoint());
	%obj.playThread(2, shiftUp);

	//Apply the speed boost status effect.
	%obj.applyStatusEffect("PlayerRumEffect", "Powerup", 45000);

    //Remove the tool from the player's object, so they can't cancel out and re-equip for infinite intoxication.
    %obj.removeItemFromInventory("", true);

	//Prevent the rum from being used again until the cooldown expires.
	%obj.lastRumTime = getSimTime();
}

function rumBottleImage::onDiscard(%this, %obj)
{
    //Play a bottle dropping animation.
    %obj.playThread(0, shiftTo);
    %obj.playThread(2, plant);

    //Lower the arm again.
	%obj.playThread(1, root);

	//Spawn a debris rum bottle for the cool effect.
    %rightVector = VectorCross(%obj.getForwardVector(), "0 0 1");
    %pelvisPosition = VectorAdd(%obj.getHackPosition(), "0 0 -0.5");
    %rightHandLocation = VectorAdd(%pelvisPosition, VectorScale(%rightVector, 1));
    %rumDebris = new Projectile()
	{
		dataBlock = rumBottleProjectile;
		initialPosition = %rightHandLocation;
	};
    %rumDebris.explode();

	//After 200 milliseconds, play a shell-drop sound effect by the player's side, relative to where they are facing.
	schedule(200, 0, "serverPlay3D", "rum_break" @ getRandom(1, 3) @ "_sound", MatrixMulPoint(%obj.getTransform(), "1 0 0"));

	//If the player happens to be holding another rum bottle by the time they're sober, let them know they can drink again.
	%obj.weaponCooldown(%this.mountPoint, "I'm feeling a little woozy...", "Ugh, that's better. I could sure use another drink.");

    //Remove the leftover image from the player's hand and communicate to the client.
    %obj.unmountImage(%this.mountPoint);
}

//
// The status effect.
//

datablock ShapeBaseImageData(playerDrunkImage)
{
	shapeFile = "base/data/shapes/empty.dts";

	mountPoint = $HeadSlot;
	mountSlot = 2;

	offset = "0 0 0";
	eyeOffset = "0 0 -1000";

	projectileType = Projectile;

	stateName[0] = "Drunk";
	stateTimeoutValue[0] = 1.0;
	stateEmitter[0] = drunkEmitter;
	stateEmitterTime[0] = 1.0;
	stateTransitionOnTimeout[0] = "Loop";

	stateName[1] = "Loop";
	stateTransitionOnTimeout[1] = "Drunk";
	stateTimeoutValue[1] = 0.01;
};

datablock ExplosionData(drunkCamShakeExplosion : camShakeExplosion)
{
    camShakeFreq = "1.0 1.0 1.0";
    camShakeAmp = "1.0 2.0 1.0";
    camShakeDuration = 35.0;
};

datablock ProjectileData(drunkCamShakeProjectile : camShakeProjectile)
{
    explosion = drunkCamShakeExplosion;
};

function PlayerRumEffect::beginStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	%client = %obj.client;
	%isClient = isObject(%client);

	if(%isClient)
	{
		//Add color correction (vignette multiply) to simulate drunken euphoria.
		%this.client = %client;
		commandToClient(%client, 'SetVignette', 1, "0.750 0.5 0.0 1.0");

		//Flush the player's face.
		%tintAmount = 0.3;
		%originalHeadColor = %client.headColor;
		%r = getWord(%originalHeadColor, 0);
		%g = getWord(%originalHeadColor, 1);
		%b = getWord(%originalHeadColor, 2);
		%a = getWord(%originalHeadColor, 3);
		if((%r + %tintAmount) <= 1.0)
		{
			%obj.setNodeColor("headSkin", setWord(%originalHeadColor, 0, %r + %tintAmount));
		}
		else if((%g - %tintAmount) >= 0.0 && (%b - %tintAmount) >= 0.0)
		{
			%obj.setNodeColor("headSkin", %r SPC (%g - %tintAmount) SPC (%b - %tintAmount) SPC %a);
		}
	}

	//Give the player a drunken-looking face, use the "Mender" face pack for now until a proper one is made.
	%facePack = (%isClient && %client.chest) ? $Eventide_FacePacks["menderF"] : $Eventide_FacePacks["menderM"];
	if(isObject(%obj.faceConfig))
	{
		%obj.faceConfig.setFacePack(%facePack);
	}

	//Add a cartoony visual effect.
	%obj.mountImage(playerDrunkImage, playerDrunkImage.mountSlot);

	//Start the player on a camera effect loop.
	%this.tick(%obj);
}

function PlayerRumEffect::tick(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	//I'm feeling a little woozy...
	%playerVelocity = VectorLen(%obj.getVelocity());
	if(%playerVelocity < 1.0)
	{
		%obj.shakeCamera(0.1, drunkCamShakeProjectile);
	}
	else
	{
		//Add some sideways velocity to the player to mimic staggering as well.
		%playerRelativeVelocity = mAbs(%obj.getRelativeVelocity());
		if(%playerRelativeVelocity >= 1.0 && mAbs(getWord(%playerVelocity, 2)) < 1.0) //If the player is walking forward AND not falling...
		{
			//Make the player randomly step left or right.
			%playerForwardVector = %obj.getForwardVector();
			%viewDriftChance = getRandom(-1, 1);
			%staggerChance = getRandom(0, 1);

			//Calculate the player's current yaw, then offset it randomly.
			if(%viewDriftChance != 0)
			{
				%yaw = mAtan(getWord(%playerForwardVector, 0), getWord(%playerForwardVector, 1));
				%staggerYaw = %yaw + (mDegToRad(getRandom(15, 30)) * %viewDriftChance); //Drift view some degrees to the left or right.
				%obj.setTransform(%obj.position SPC "0 0 1" SPC %staggerYaw);
			}

			//Stagger left, or stagger right?
			if(%staggerChance)
			{
				%playerUpVector = %obj.getUpVector();
				%staggerVector = (getRandom(0, 1) ? VectorCross(%playerForwardVector, %playerUpVector) : VectorCross(%playerUpVector, %playerForwardVector));
				%staggerVelocity = setWord(VectorScale(%staggerVector, 5), 2, 2); //Add two TU up, 5 to the left or right of the player.
				%obj.AddVelocity(%staggerVelocity);
			}

			//If we drifted or stagged, shake the camera to simulate disorientation.
			if(%viewDriftChance != 0 || %staggerChance)
			{
				//Vary the intensity of the camera-shaking when moving, to simulate the player staggering organically.
				%obj.shakeCamera(getRandom(25, 40) / 100, drunkCamShakeProjectile);
			}
		}
	}

	//Cycle until we're done.
	%obj.drunkSchedule = %this.schedule(500, "tick", %obj);
}

function PlayerRumEffect::finalizeStatusEffect(%this, %obj)
{
	//Reset the color correction.
	%client = %this.client;
	%isClient = isObject(%client);

	if(%isClient)
	{
		commandToClient(%client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor);
	}

	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	//Revert the drunken face.
	%faceConfig = %obj.faceConfig;
	if(isObject(%faceConfig))
	{
		%faceConfig.setFacePack(%faceConfig.previousFacePack);
		%obj.faceConfigShowFaceTimed("Blink", 300);
	}
	if(%isClient)
	{
		//Revert the flushed face.
		%obj.setNodeColor("headSkin", %client.headColor);
	}

	//Have the player shake themselves awake.
	%obj.playThread(3, undo);

	//Remove the visual effect.
	if(%obj.getMountedImage(playerDrunkImage.mountSlot) == playerDrunkImage.getID())
	{
		%obj.unmountImage(playerDrunkImage.mountSlot);
	}

	//Cancel the camera effect loop.
	cancel(%obj.drunkSchedule);
}

function Player::drunkify(%obj, %time)
{
    //Have the freeze time default to 1 second.
    if(%time $= "")
    {
        %time = 6000;
    }

    //Get the player drunk.
    %obj.applyStatusEffect("PlayerRumEffect", "Powerup", %time);
}

//
// Tumble mechanic: instead of taking damage, get launched.
//

datablock WheeledVehicleData(betterTumbleVehicle)
{
	//Basic Settings
	category = "Vehicles";
	shapeFile = "Add-Ons/Item_Skis/deathVehicle.dts";
	emap = true;

	//Vehicle Settings
	numMountPoints = 1;
	maxDamage = 999999;
	destroyedLevel = 200;

	//Rigid Body
	mass = 200;
	massCenter = "0 0.1 0.7";	// Center of mass for rigid body
	massBox = "0.6 0.8 1";	// Size of box used for moment of inertia

	//Physics
	drag = 0.2;
	density = 1;
	integration = 4; 			// Physics integration: Tick Sec/Rate
	bodyFriction = 1;
	bodyRestitution = 0.0;

	//Collision
	minImpactSpeed = 0.1;		// Impacts over this invoke the script callback
	minRunOverSpeed = 100;  	// how fast you need to be going to run someone over (do damage)
	runOverDamageScale = 100; //how much damage running over does
	collisionTol = 0.4;			// Collision distance tolerance (was 0.25)

	isSled = true;				//if its a sled, the wing surfaces dont work unless its on the ground
};

function betterTumbleVehicle::onObjectCollision(%this, %obj, %col)
{
	%speed = VectorLen(%obj.getVelocity());

	//If we're stopped, end the tumble.
	if(%speed < 0.01)
	{
		%player = %obj.player;
		%player.isTumbling = false;

		//End the cutscene.
		%player.lockInputs = false;
		%player.restoreCameraFromOrbit();

		//Exit the player from the tumble.
		%player.canDismount = true;
		%obj.delete();

		return true;
	}
	else if(%col.getType() & $TypeMasks::PlayerObjectType)
	{
		%col.applyDamage(%speed * 0.25);
		if(%col.getState() !$= "Dead")
		{
			return false;
		}
		
		%impactReverseNormal = VectorNormalize(VectorSub(%col.getPosition(), %obj.getPosition()));
		%inheritedVelocity = VectorScale(%impactReverseNormal, %speed * 0.75);
		%col.betterTumble(%inheritedVelocity);
		return false;
	}

	return true;
}

function Player::betterTumble(%obj, %velocity)
{
	if(%obj.isTumbling)
	{
		return;
	}
	%obj.isTumbling = true;

	//Reset any animations that might be playing.
	%obj.setActionThread(root); //Movement thread.
	%obj.playThread(3, root); //Default death animation thread.
	%obj.playThread(2, root);
	%obj.playThread(1, root);
	%obj.playThread(0, root);

	%playerInitialTransform = %obj.getTransform();
	%playerInitialPosition = VectorAdd(posFromTransform(%playerInitialTransform), "0 0 0.1");
	%playerInitialRotation = rotFromTransform(%playerInitialTransform);

	%playerInitialVelocity = %obj.getVelocity();
	%tumbleVelocity = VectorAdd(%playerInitialVelocity, %velocity);

	//Create the tumble vehicle, match the invitial position/rotation/velocity of the player.
	%obj.canDismount = false; //Prevent dismounting while tumbling.
	%tumbleVehicle = new WheeledVehicle()
	{
		dataBlock = betterTumbleVehicle;
		player = %obj;
		position = %playerInitialPosition;
	};
	%obj.tumbleVehicle = %tumbleVehicle;
	%tumbleVehicle.mountObject(%obj, 0);
	%tumbleVehicle.setTransform(%playerInitialTransform);
	%tumbleVehicle.setVelocity(%tumbleVelocity);

	//Special calculations for the angular velocity, to make the tumble look better.
    %angularAxis = VectorNormalize(%velocity);
    %angularSpeed = VectorLen(%velocity) * 0.5;
    %angularVelocity = VectorScale(%angularAxis, %angularSpeed);
	%tumbleVehicle.setAngularVelocity(%angularVelocity);
	
	//Cutscene.
	%obj.lockInputs = true;
	%obj.createCameraOrbit();
}

//
// Package to make drunk players take less damage.
//

package Item_Rum
{
	function getRandomLetter()
	{
		return getSubStr("abcdefghijklmnopqrstuvwxyz", getRandom(0, 25), 1);
	}

	function adjustDrunkMessage(%message)
	{
		%returnString = "";

		for(%i = 0; %i < strLen(%message); %i++)
		{
			%currentCharacter = getSubStr(%message, %i, 1);

			%adjustCharacter = (getRandom(1, 8) == 1);
			if(!%adjustCharacter)
			{
				%returnString = %returnString @ %currentCharacter;
				continue;
			}

			%impairment = getRandom(1, 3);
			switch(%impairment)
			{
				case 1:
					//Double letter.
					%returnString = %returnString @ %currentCharacter @ %currentCharacter;
				case 2:
					//Skip letter.
				case 3:
					//Random letter.
					%returnString = %returnString @ getRandomLetter();
			}
		}

		return %returnString;
	}

	function serverCmdMessageSent(%client, %message)
	{
		%player = %client.player;
		if(isObject(%player) && %player.hasStatusEffect("PlayerRumEffect", "Powerup"))
		{
			%message = adjustDrunkMessage(%message);
		}

		return parent::serverCmdMessageSent(%client, %message);
	}

	function serverCmdTeamMessageSent(%client, %message)
	{
		%player = %client.player;
		if(isObject(%player) && %player.hasStatusEffect("PlayerRumEffect", "Powerup"))
		{
			%message = adjustDrunkMessage(%message);
		}

		return parent::serverCmdTeamMessageSent(%client, %message);
	}

	function PlayerSurvivor::Damage(%this, %obj, %sourceObject, %position, %damage, %damageType)
	{
		//If the player is drunk, halve all damage taken.
		if(!%obj.hasStatusEffect("PlayerRumEffect", "Powerup"))
		{
			return Parent::Damage(%this, %obj, %sourceObject, %position, %damage, %damageType);
		}

		%tumbleChance = getRandom(1, 4);
		if(%tumbleChance == 1 || %obj.damagedSinceTumble == 4)
		{
			//Guarantee the player will tumble at least every 4th hit.
			%obj.damagedSinceTumble = 0;

			//Launch the player instead of damaging them.
			%impactNormal = VectorNormalize(VectorSub(%obj.getHackPosition(), %position));
			%inheritedVelocity = VectorScale(%impactNormal, %damage * 0.75);
			%obj.betterTumble(%inheritedVelocity);

			//Play a voice line.
			//TODO: Need this for individual voice packs.
			%obj.playAudio(3, "drunk_tumble" @ getRandom(1, 3) @ "_sound");

			//No damage taken.
			return 0;
		}
		%obj.damagedSinceTumble++;

		//If the damage is enough to kill the player, remove the drunk effect first.
		if((%obj.getDamageLevel() + %damage) >= %obj.Datablock.maxDamage)
		{
			%obj.clearStatusEffect("PlayerRumEffect", "Powerup");
		}

		//Halve all damage taken.
		%damage /= 2.0;

		//Let the damage occur, but reduced.
		return Parent::Damage(%this, %obj, %sourceObject, %position, %damage, %damageType);
	}

	function Player::onRemove(%this, %obj)
	{
		%tumbleVehicle = %obj.tumbleVehicle;
		if(isObject(%tumbleVehicle))
		{
			%tumbleVehicle.delete();
		}

		%client = %obj.client;
		if(%obj.hasStatusEffect("PlayerRumEffect", "Powerup") && isObject(%client))
		{
			commandToClient(%client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor);
		}

		return parent::onRemove(%this, %obj);
	}
};
if(isPackage(Item_Rum))
{
	deactivatePackage(Item_Rum);
}
activatePackage(Item_Rum);