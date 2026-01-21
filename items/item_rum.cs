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
	numBounces = 2;
    bounceVariance = 2;
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

datablock ShapeBaseImageData(rumBottleImage)
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
    stateSequence[0] = "ready";
	stateAllowImageChange[0] = false;
    stateTimeoutValue[0] = 0.01;
    stateTransitionOnTimeout[0]	= "CooldownCheck";

	//Check if the rum is on cooldown. If not, proceed to "Ready".
    stateName[1] = "CooldownCheck";
    stateScript[1] = "onCooldownCheck";
    stateAllowImageChange[1] = false;
    stateWaitForTimeout[1] = true;
    stateTimeOutValue[1] = 0.01;
    stateTransitionOnTimeout[1] = "CooldownRedirect";

	//Redirect to another state based on what was set in the previous "CooldownCheck" state.
    stateName[2] = "CooldownRedirect";
    stateAllowImageChange[2] = false;
    stateTransitionOnNoAmmo[2] = "Ready";
	stateTransitionOnAmmo[2] = "Cooldown";

	//The rum is on cooldown and cannot be used.
    stateName[3] = "Cooldown";
    stateScript[3] = "onCooldown";
    stateAllowImageChange[3] = false;
    ////The cooldown ended while the rum was equipped, transition to the "Ready" state.
    stateTransitionOnNoAmmo[3] = "CooldownRevert";

	//The rum is transitioning from "Cooldown" to "Ready", we need to raise the arm back up.
    stateName[4] = "CooldownRevert";
    stateScript[4] = "onCooldownRevert";
    stateAllowImageChange[4] = false;
    stateWaitForTimeout[4] = true;
    stateTimeOutValue[4] = 0.01;
    stateTransitionOnTimeout[4] = "Ready";

	//The rum is inactive, simply being held.
    stateName[5] = "Ready";
    stateScript[5] = "onReady";
	stateAllowImageChange[5] = true;
    stateTransitionOnTriggerDown[5]	= "Open";

	//The rum has been opened, prepare to drink.
	stateName[6] = "Open";
	stateScript[6] = "onOpen";
	stateAllowImageChange[6] = false;
	stateWaitForTimeout[6] = true;
	stateTimeOutValue[6] = 1;
	stateTransitionOnTimeout[6] = "Drink";

	//Activate the status effect, put the rum on cooldown.
	stateName[7] = "Drink";
	stateScript[7] = "onDrink";
	stateAllowImageChange[7] = false;
	stateWaitForTimeout[7] = true;
	stateTimeOutValue[7] = 0.5;
	stateTransitionOnTimeout[7] = "Discard";

    //Remove the rum bottle from the player's inventory, spawn debris.
	stateName[8] = "Discard";
	stateScript[8] = "onDiscard";
	stateAllowImageChange[8] = false;
	stateWaitForTimeout[8] = false;

	cooldown = 45000;
};

//
// Sequence callbacks.

function rumBottleImage::onCooldownCheck(%this, %obj)
{
    //The rum's animation always needs to be reset at this point.
    %obj.playThread(2, root);

    //If the rum has not passed it's cooldown time limit, transition to the "Cooldown" state.
    //Otherwise, transition to the "Ready" state.
	%cooldownEndTime = (%obj.lastRumTime + %this.cooldown);
	%currentTime = getSimTime();
    if(%cooldownEndTime > %currentTime)
    {
        %obj.setImageAmmo(%obj.currTool, true);
    }
    else
    {
        %obj.setImageAmmo(%obj.currTool, false);
    }
}

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

function rumBottleImage::onCooldownRevert(%this, %obj)
{
    //Raise the arm back up after being lowered.
    fixArmReady(%obj);
}

function rumBottleImage::onReady(%this, %obj)
{

}

function rumBottleImage::onOpen(%this, %obj)
{
	//Play the sound of the rum opening.
	serverPlay3D("rum_open_sound", %obj.getMuzzlePoint(rumBottleImage.mountPoint));

	//Raise the bottle so it is ready to drink.
	%obj.playThread(1, armReadyRight);

	//Play an animation of cracking open the tab.
	%obj.playThread(2, shiftleft);

	//Spawn a cork.
	new Projectile()
	{
		dataBlock = rumBottleCorkProjectile;
		initialPosition = %obj.getMuzzlePoint(rumBottleImage.mountPoint);
	}.explode();
}

function rumBottleImage::onDrink(%this, %obj)
{
	//Play the rum drinking sound and animation.
	serverPlay3D("soda_gulp" @ getRandom(1, 3) @ "_sound", %obj.getEyePoint());
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

    //Remove the leftover image from the player's hand and communicate to the client.
    %obj.unmountImage(%obj.currTool);
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
		%originalHeadColor = %client.headColor;
		%r = getWord(%originalHeadColor, 0);
		%g = getWord(%originalHeadColor, 1);
		%b = getWord(%originalHeadColor, 2);
		%a = getWord(%originalHeadColor, 3);
		if((%r + 0.2) <= 1.0)
		{
			%obj.setNodeColor("headSkin", setWord(%originalHeadColor, 0, %r + 0.2));
		}
		else if((%g - 0.2) >= 0.0 && (%b - 0.2) >= 1.0)
		{
			%obj.setNodeColor("headSkin", %r SPC (%g - 0.2) SPC (%b - 0.2) SPC %a);
		}
	}

	//Give the player a drunken-looking face, use the "Mender" face pack for now until a proper one is made.
	%facePack = (%isClient && %client.chest) ? $Eventide_FacePacks["menderF"] : $Eventide_FacePacks["menderM"];
	%obj.faceConfig.setFacePack(%facePack);

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
	%obj.shakeCamera(0.12);

	//"Blur" vision a little.
	%obj.setWhiteOut(0.1);

	//Cycle until we're done.
	%obj.drunkSchedule = %this.schedule(1000, "tick", %obj);
}

// function PlayerRumEffect::damage(%this, %data, %obj, %sourceObject, %pos, %damage, %damageType)
// {
// 	//Halve all damage taken.
// 	%damage = %damage / 2.0;

// 	//This function needs to return something to override the damage.
// 	%parentCall = %data.damage(%obj, %sourceObject, %position, %damage, %damageType);
// 	return (%parentCall $= "" ? %damage : %parentCall);
// }

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
	%faceConfig.setFacePack(%faceConfig.previousFacePack);
	%obj.faceConfigShowFaceTimed("Blink", 300);
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
//	minRunOverSpeed = 100;  	// how fast you need to be going to run someone over (do damage)
	minRunOverSpeed = 1;  	// how fast you need to be going to run someone over (do damage)
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
			%impactNormal = VectorNormalize(VectorSub(%obj.getPosition(), %sourceObject.getPosition()));
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
	}
};
if(isPackage(Item_Rum))
{
	deactivatePackage(Item_Rum);
}
activatePackage(Item_Rum);