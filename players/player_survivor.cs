datablock PlayerData(PlayerSurvivor : PlayerEventide)
{
    class = "PlayerSurvivor";
    superClass = "PlayerEventide";

    uiName = "Eventide Player";
    maxTools = 3;
	maxWeapons = 3;

	shoveForce = 1;
};
//Inherits functions from `PlayerEventide`.
PlayerSurvivor.inheritFunctionsFromSuperClass();

//
// Voice-line, expression handlers.
//

function PlayerSurvivor::onNewDatablock(%this, %obj)
{
	%this.super("onNewDatablock", %this, %obj);

	%client = %obj.client;
	%clientExists = isObject(%client);

	//Face pack initialization, very basic for now. 
	//This can't use the usual datablock field method, since it needs to be dynamic.
	%facePack = (%clientExists && %client.chest) ? $Eventide_FacePacks["female"] : $Eventide_FacePacks["male"];
	%obj.createFaceConfig(%facePack);

	//Voice Pack initialization.
	%voicePack = (%clientExists && %client.chest) ? $Eventide_VoicePacks["female"] : $Eventide_VoicePacks["male"];
	%obj.createVoiceConfig(%voicePack);

	//Store some information used for voice-lines and chase management.
	%obj.chasingKillers = new SimSet();
	%obj.nearbyKillers = new SimSet();
}

function PlayerSurvivor::onRemove(%this, %obj)
{
	%obj.chasingKillers.delete();
	%obj.nearbyKillers.delete();

	Parent::onRemove(%this, %obj);
}

function PlayerSurvivor::onKillerEnterRange(%this, %obj, %target)
{
	%obj.nearbyKillers.add(%target);
	//If the survivor has a killer near them, change the survivor's face to scared.
	if(%obj.nearbyKillers.getCount() == 1)
	{
		%obj.createSubfaceConfig("Scared");
	}

	//Play the killer's nearby music.
	%obj.playAmbiantMusic(%target.killerNearMusic, 1, 1.0);
}

function PlayerSurvivor::onKillerExitRange(%this, %obj, %target)
{
	%obj.nearbyKillers.remove(%target);
	//The survivor no longer has any killers near them, reset their face back to normal.
	if(%obj.nearbyKillers.getCount() == 0)
	{
		%obj.revertSubfaceConfig();
	}

	//No killers are chasing or nearby, resume the ambiant music track.
	if(%obj.nearbyKillers.getCount() == 0 && %obj.chasingKillers.getCount() == 0)
	{
		%obj.playRandomAmbiantTrack(true);
	}
}

function PlayerSurvivor::onKillerChaseStart(%this, %obj, %target)
{
	%obj.chasingKillers.add(%target);
	//If the survivor has started being chased, play a talking animation that emulates shaking in fear.
	if(%obj.chasingKillers.getCount() == 1)
	{
		%obj.playThread(2, "talk");
	}

	//Play the killer's chase music.
	%obj.playAmbiantMusic(%target.killerChaseMusic, 2, 1.0);
}

function PlayerSurvivor::onKillerChaseEnd(%this, %obj, %target)
{
	%obj.chasingKillers.remove(%target);
	//The survivor is no longer being chased, stop the fearful shaking.
	if(%obj.chasingKillers.getCount() == 0)
	{
		%obj.playThread(2, "root");
	}

	//Depending on the circumstances, play a killer's nearby music, or just some ambiant tracks.
	%amountChasingKillers = %obj.chasingKillers.getCount();
	%amountNearbyKillers = %obj.nearbyKillers.getCount();
	if(%amountChasingKillers > 0)
	{
		%chosenKiller = %obj.chasingKillers.getObject(getRandom(0, %amountChasingKillers));
		%obj.playAmbiantMusic(%chosenKiller.killerChaseMusic, 2, 1.0, true);
	}
	else if(%amountNearbyKillers > 0)
	{
		%chosenKiller = %obj.nearbyKillers.getObject(getRandom(0, %amountNearbyKillers));
		%obj.playAmbiantMusic(%chosenKiller.killerNearMusic, 1, 1.0, true);
	}
	else
	{
		%obj.playRandomAmbiantTrack(true);
	}
}

//
// Custom behaviors.
//

function PlayerSurvivor::Damage(%this, %obj, %sourceObject, %position, %damage, %damageType)
{
	%minigame = getMinigamefromObject(%obj);

	//Some killers have projectile weapons, and this allows you to call `onIncapacitateVictim` properly.
	if(isObject(%sourceObject))
	{	
		if(%sourceObject.getDataBlock().getClassName() $= "ProjectileData")
		{
			%sourceDatablock = %sourceObject.sourceObject.getDatablock();
			%killerSourceObject = %sourceObject.sourceObject;
		}
		else
		{
			%killerSourceObject = %sourceObject;
		}
		%killerDatablock = %killerSourceObject.getDataBlock();
	}	

	//The player is about to die, let's do some things beforehand.
	%fatalDamage = (%damage + %obj.getDamageLevel() >= %this.maxDamage);
	if(%fatalDamage)
    {   
		//The player has already been downed once. Execute the same function, but with the %killed parameter set to true.
		if(%killerDatablock.isKiller)
		{
			%killerDatablock.onIncapacitateVictim(%killerSourceObject, %obj, true);
		}
    }

	//Let the damage actually occur.
    Parent::Damage(%this, %obj, %sourceObject, %position, %damage, %damageType);
		
	if(%damage > 5 && !%fatalDamage) 
	{
		//Face system functionality: play a pained facial expression when the player is hurt, and switch to hurt facial expression afterward 
		//if enough damage has been received.
		if(isObject(%obj.faceConfig))
		{
			if(%obj.getDamagePercent() > 0.33 && $Eventide_FacePacks[%obj.faceConfig.category, "Hurt"] !$= "") 
			{
				%obj.createSubfaceConfig("Hurt");
			}

			if(%obj.faceConfig.isFace("Pain")) 
			{
				%obj.schedule(1, "faceConfigShowFace", "Pain"); //This needs to be delayed for whatever reason. Blinking doesn't start otherwise.
			}		
		}

		//Play a pained grunt from the player.
		%obj.playVoiceLine("Pain", true);
	}
}

function PlayerSurvivor::onTrigger(%this, %obj, %trigger, %state)
{
	%returnValue = Parent::onTrigger(%this, %obj, %trigger, %state);
	if(%trigger == 4 && %state)
	{
		%this.shove(%obj);
	}
	return %returnValue;
}

function PlayerSurvivor::shove(%this, %obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") 
	{
		return;
	}

	%triggerTime = getSimTime();
	%minEnergy = (%this.maxEnergy / 4);
	%energyCost = 20;

	if((%triggerTime - %obj.shoveCooldown) > 3500)//Reset the delay if the player waits long enough, 3.5 seconds
	{
		%obj.shoveFatigue = 0;
		%obj.shoveCooldown = 0;
	}

	//If we have enough energy, we can shove.
	if(%obj.shoveCooldown < %triggerTime && %obj.getEnergyLevel() >= %minEnergy)
	{
		%obj.setEnergyLevel(%obj.getEnergyLevel() - %energyCost);
		%obj.shoveFatigue++;
		%obj.shoveCooldown = (%triggerTime + 400) + (40 * %obj.shoveFatigue);
		%soundpitch = getRandom(50, 125);

		//Shoving five times consecutively causing exhaustion.
		if(%obj.shoveFatigue >= 5)
		{
			cancel(%obj.resetStamina);
			%soundpitch = getRandom(50,80);
			%obj.resetStamina = %this.schedule(4000, resetStamina, %obj);

			if(%obj.shoveFatigue == 5)
			{							
				%obj.resetStamina = %this.schedule(4000, resetStamina, %obj);
			}								
		}
		
		//Play the shoving animation.
		%obj.playThread(2, "activate2");

		//Play the shoving sound, with a randomized pitch.
		%oldTimescale = getTimescale();
		setTimescale((%soundpitch * 0.01) * %oldTimescale);
		serverPlay3D("melee_swing" @ getRandom(1, 2) @ "_sound", %obj.getHackPosition());
		setTimescale(%oldTimescale);
		
		%pos = %obj.getEyePoint();
		%eyeVector = %obj.getLookVector();
		%radius = 0.25;
		%mask = $TypeMasks::PlayerObjectType;

		initContainerRadiusSearch(%pos, %radius, %mask);
		while(%hit = containerSearchNext())
		{
			%victimPosition = %hit.getHackPosition();

			%playerObstructed = containerRayCast(%pos, %victimPosition, $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType, %obj);
			%dot = VectorDot(%eyeVector, VectorNormalize(VectorSub(%victimPosition, %obj.getHackPosition())));

			//If the container search hit ourselves, a dead person or a wall, do nothing. Also a small sanity check in case we aren't looking at the person we hit.
			if(%hit == %obj || %hit.getState() $= "Dead" || isObject(%playerObstructed) || %dot < 0.5)
			{
				continue;
			}

			//Determine the shove force based on player class and exhaustion.
			%shoveForce = %this.shoveForce;
			%reductionDivider = (%obj.shoveFatigue >= 5) ? 1.25 : 1;
			%forwardImpulse = %shoveForce * ((8 + %shoveForce) / %reductionDivider);
			%upwardImpulse = %shoveForce * (4 / %reductionDivider);

			//Finally, apply the shove force to the victim.
			%finalVelocity = VectorAdd(VectorScale(%eyeVector, %forwardImpulse), "0 0 " @ %upwardImpulse);
			%hit.getDataBlock().onShoved(%hit, %finalVelocity);
		}			
	}
}

function PlayerEventide::onShoved(%this, %obj, %velocity)
{
	//Play a blunt hit sound on the shove victim.
	serverPlay3D("melee_shove_sound", %obj.getHackPosition());

	//Play an animation on the shove victim.
	%obj.playThread(2, "jump");

	//Make the victim get pushed by the shove.
	%obj.setVelocity(%velocity);
}