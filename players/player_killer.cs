datablock PlayerData(PlayerKiller : PlayerEventide) 
{
    class = "PlayerKiller";
    superClass = "PlayerEventide";

	uiName = "";
    isKiller = true;
    firstPersonOnly = true;
	
	meleeWeaponImage = KillerMeleeImage;

	facePack = "";
	voicePack = "";

	rechargeRate = 0.26;
	maxDamage = 9999;
	maxTools = 0;
	maxWeapons = 0;

	killerlight = "NoFlareRLight";	
	killerLoopTick = 200;
};
//Inherit functions from `PlayerEventide`.
PlayerKiller.inheritFunctionsFromSuperClass();

function PlayerKiller::onNewDatablock(%this, %obj)
{
	%this.super("onNewDatablock", %this, %obj);

	//Hide the light if the killer spawns invisible, or set it to the appropriate datablock if not.
	%light = %obj.light;
	if(%obj.isInvisible && isObject(%light))
	{
		%light.delete();
		%obj.light = "";
	}
	else
	{
		if(isObject(%light))
		{
			%light.setDatablock(%this.killerlight);
		}
		else
		{
			%light = new fxLight()
			{
				dataBlock = %this.killerlight;
				source = %obj;
			};
			%obj.light = %light;
			%light.attachToObject(%obj);
		}
		
		//Hide the light from everyone except the killer.
		%light.setNetFlag(6, true);
		%light.adjustObjectScopeToAll(false, %obj.client);
	}

	//Mount the melee weapon.
	if(%this.meleeWeaponImage !$= "")
	{
		%obj.mountImage(%this.meleeWeaponImage, 0);
	}

	//Face and voice config setup, if specified.
	if(%this.facePack !$= "")
	{
		%obj.createFaceConfig($Eventide_FacePacks[%this.facePack]);
	}
	if(%this.voicePack !$= "")
	{
		%obj.createVoiceConfig($Eventide_VoicePacks[%this.voicePack]);
		//Set some default cooldowns.
		%voiceConfig = %obj.voiceConfig;
		%voiceConfig.setLineCooldown("Idle", 10000);
	}

	//Store some information used for voice-lines and chase management.
	if(isObject(%obj.chasingVictims))
	{
		%obj.chasingVictims.delete();
	}
	%obj.chasingVictims = new SimSet();

	if(isObject(%obj.incapVictims))
	{
		%obj.incapVictims.delete();
	}
	%obj.incapVictims = new SimSet();

	if(isObject(%obj.nearVictims))
	{
		%obj.nearVictims.delete();
	}
	%obj.nearVictims = new SimSet();

	if(isObject(%obj.threatenedBy))
	{
		%obj.threatenedBy.delete();
	}
	%obj.threatenedBy = new SimSet();

	//Start the killer tick loop, used for voice lines, chase music and other functionality.
	%this.killerLoop(%obj);
}

//
// Killer list management.
//

$Eventide_Killers = new SimSet(Eventide_Killers);

function PlayerKiller::onAdd(%this, %obj)
{
	Parent::onAdd(%this, %obj);
	$Eventide_Killers.add(%obj);
}

function PlayerKiller::onRemove(%this, %obj)
{
	%obj.chasingVictims.delete();
	%obj.incapVictims.delete();
	%obj.nearVictims.delete();
	%obj.threatenedBy.delete();

	Parent::onRemove(%this, %obj);
	if($Eventide_Killers.isMember(%obj))
	{
		$Eventide_Killers.remove(%obj);
	}
}

function PlayerKiller::onDisabled(%this, %obj, %state)
{
	Parent::onDisabled(%this, %obj, %state);
	if($Eventide_Killers.isMember(%obj))
	{
		$Eventide_Killers.remove(%obj);
	}
}

function PlayerKiller::onIncapacitateVictim(%this, %obj, %victim, %killed)
{
	%victimClient = %victim.client;
	if(isObject(%victimClient))
	{
		%obj.incapVictims.add(%victimClient);
	}

	//Play a voice line celebrating, if available.
	%obj.playVoiceLine("Kill");
}

//
// Custom behaviors.
//

// function PlayerKiller::killerGUI(%this, %obj, %client)
// {	
// 	%energyLevel = %obj.getEnergyLevel();
//     %iconPath = filePath($Con::File) @ "/icons/";

// 	//Determine if we have enough energy to perform and left or right click action. Then, choose the appropriate icons.
// 	%leftClickStatus = (%energyLevel >= 25) ? "hi" : "lo";
// 	%rightClickStatus = (%energyLevel == %this.maxEnergy && %obj.gazingPlayer) ? "hi" : "lo";
// 	%leftClickText = (%this.leftclickicon !$= "") ? "<just:left>\c6Left click" : "";
// 	%rightClickText = (%this.rightclickicon !$= "") ? "<just:right>\c6Right click" : "";		

// 	//Format the icons into a status message.
// 	%leftClickIcon = (%this.leftclickicon !$= "") ? "<just:left><bitmap:" @ %iconPath @ %leftClickStatus @ %this.leftclickicon @ ">" : "";
// 	%rightClickIcon = (%this.rightclickicon !$= "") ? "<just:right><bitmap:" @ %iconPath @ %rightClickStatus @ %this.rightclickicon @ ">" : "";

// 	//Display the status message in the bottom print bar.
//     //Setting the time to 0 causes the bottom print to never automatically clear under normal circumstances.
// 	%client.bottomPrint(%leftClickText @ %rightClickText @ "<br>" @ %leftClickIcon @ %rightClickIcon, 0, false);
// }

function PlayerKiller::killerLoop(%this, %obj)
{
    if(!isObject(%obj) || !%obj.getDataBlock().isKiller || %obj.getState() $= "Dead")
	{
		return;
	}

	%this.preKillerLoop(%obj);

	//Update the UI.
    // if(isObject(%obj.client)) 
	// {
	// 	%this.killerGUI(%obj, %obj.client);
	// }

	%searchDistance = 25; //80 studs, should be a decent balance for both small and large maps.
	%gracePeriod = 4000;
	%currentTime = getSimTime();

	//Cycle through the current list of people near the killer, and determine if they are now too far away.
	for(%i = 0; %i < %obj.nearVictims.getCount(); %i++)
	{
		%victim = %obj.nearVictims.getObject(%i);
		%victimDistance = VectorDist(%victim.getPosition(), %obj.getPosition());

		if(%victimDistance > %searchDistance && (%currentTime - %victim.timeSinceKillerNear) > %gracePeriod)
		{
			//The victim is too far away, unmark them as being near the killer.
			%victim.isKillerNear = false;
			%victim.getDatablock().onKillerExitRange(%victim, %obj);

			%obj.nearVictims.remove(%victim);
			%this.onKillerExitRange(%obj, %victim);
		}
		else
		{
			//The victim is still near, renew the grace period.
			%victim.timeSinceKillerNear = %currentTime;
		}
	}

	//Cycle through the current list of people being chased, determine if they've been out of sight for too long.
	for(%i = 0; %i < %obj.chasingVictims.getCount(); %i++)
	{
		%victim = %obj.chasingVictims.getObject(%i);
		%victimClient = %victim.client;

		if((%currentTime - %victim.timeSinceChased) > %gracePeriod)
		{
			//It's been too long since the victim was last seen, end the chase.
			%obj.chasingVictims.remove(%victim);

			%victim.isBeingChased = false;
			%victim.getDatablock().onKillerChaseEnd(%victim);

			%victimDistance = VectorDist(%victim.getPosition(), %obj.getPosition());
			if(%victimDistance > %searchDistance)
			{
				//The victim is completely out-of-range, unmark them as being near.
				%victim.isKillerNear = false;
				%victim.getDatablock().onKillerExitRange(%obj, %victim);
				
				%obj.nearVictims.remove(%victim);
				%this.onKillerExitRange(%obj, %victim);
			}
			else
			{
				//The killer does not see the victim, but is still nearby.
				%victim.getDatablock().onKillerEnterRange(%victim);
			}
		}
		else
		{
			//The grace period has not expired, run the chase tick.
			%victim.getDatablock().onKillerChase(%victim);

			//Check if the victim has equipped a weapon, and if so, play a voice line on the killer. Don't do this more than once per chase.
			%victimItemEquipped = %victim.getMountedImage($RightHandSlot);
			if(!%obj.threatenedBy.isMember(%victim) && %victimItemEquipped != 0 && %victimItemEquipped.isWeapon)
			{
				%obj.playVoiceLine("Threatened");
				%obj.threatenedBy.add(%victim);
			}
		}
	}

	//Pre-emptively set this, it may be overwritten during the container search.
	%closeRangeFlag = false;
	%obj.gazingPlayer = false;

	//Figure out who is near the killer or in view of the killer.
    initContainerRadiusSearch(%obj.getEyePoint(), %searchDistance, $TypeMasks::PlayerObjectType);
    while(%victim = containerSearchNext())
    {
        //Skip invalid conditions
        %victimDatablock = %victim.getDataBlock();
        if(!%victimDatablock.isEventideClass || %victimDatablock.isKiller || %victim.isInvisible || %victim.getState() $= "Dead" || !MinigameCanDamage(%obj, %victim))
        {
            continue;
        }

		%victimPosition = %victim.getEyePoint();
		%killerPosition = %obj.getEyePoint();
        
        %typemasks = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | %TypeMasks::TerrainObjectType;
        %dot = VectorDot(%obj.getLookVector(), VectorNormalize(VectorSub(%victimPosition, %killerPosition)));
        %victimObstructed = containerRayCast(%killerPosition, %victimPosition, %typemasks, %obj);
        %victimDistance = containerSearchCurrDist();

		//Before anything else, mark the victim as near the killer if they aren't already.
		if(!%victim.isKillerNear)
		{
			%obj.nearVictims.add(%victim);
			%this.onKillerEnterRange(%obj, %victim);

			%victim.isKillerNear = true;
			%victim.timeSinceKillerNear = %currentTime;
			%victim.getDatablock().onKillerEnterRange(%victim, %obj);
		}

		//If the victim is in view of the killer, start some music, do some facial expressions, etc.
        if(%dot > 0.45 && !%victimObstructed)
        {
			//If the victim is close enough to melee, mark as such for the UI.
			if(%victimDistance <= %this.meleeRange && !%closeRangeFlag)
			{
				%closeRangeFlag = true;
				%obj.gazingPlayer = true;
			}

			%victim.timeSinceChased = %currentTime;

			//If the victim wasn't being chased previously, do some actions.
			if(!%obj.isBeingChased)
			{
				%obj.chasingVictims.add(%victim);
				%this.onKillerChaseStart(%obj, %victim);

				%victim.isBeingChased = true;
				%victim.getDatablock().onKillerChaseStart(%victim, %obj);
			}
		}
    }

	%this.postKillerLoop(%obj);

	//Schedule next loop, prevent double scheduling by cancelling the previous loop schedule.
    cancel(%obj.killerLoopSchedule);
    %obj.killerLoopSchedule = %this.schedule(%this.killerLoopTick, "killerLoop", %obj);
}

//
// Some universal voice line handlers.
//

function PlayerKiller::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);
	if(%obj.getState() !$= "Dead")
	{
		%obj.playVoiceLine("Pain");
		%obj.faceConfigShowFace("Pain");
	}
}

function PlayerKiller::preKillerLoop(%this, %obj)
{
	
}

function PlayerKiller::postKillerLoop(%this, %obj)
{
	//Play some killer voice lines.
	if(%obj.chasingVictims.getCount() > 0)
	{
		//The killer is actively chasing someone.
		%obj.playVoiceLine("Chase");
		%obj.playAmbiantMusic(%this.killerChaseMusic, 2, 1.0, true);
	}
	else if(%obj.nearVictims.getCount() > 0)
	{
		//The killer isn't in a chase, but people are nearby.
		%obj.playVoiceLine("Near");
		%obj.playAmbiantMusic(%this.killerNearMusic, 1, 1.0, true);
	}
	else
	{
		//The killer has no action at all.
		%obj.playVoiceLine("Idle");
		%obj.playRandomAmbiantTrack(true);
	}
}

function PlayerKiller::onKillerChaseStart(%this, %obj, %target)
{
	//The chase is initially starting.
	if(%obj.chasingVictims.getCount() == 1)
	{
		//Cache the amount of people killed before the chase started.
		%obj.cachedIncapVictimAmount = %obj.incapVictims.getCount();

		//Play a voice line of glee.
		%obj.playVoiceLine("FoundVictim");
	}
}

function PlayerKiller::onKillerChaseEnd(%this, %obj, %target)
{
	//Remove them from the threat list, so they can threaten the killer again next chase.
	%obj.threatenedBy.remove(%target);

	if(%obj.chasingVictims.getCount() == 0 && (%obj.incapVictims.getCount() - %obj.cachedIncapVictimAmount) == 0)
	{
		//The killer did not kill anyone during the chase. Play a dismayed voice line.
		%obj.playVoiceLine("LostVictim");
	}
}

//
// Package for ensuring killer-related loops and functions do not continue after a datablock change.
//

package Player_Killer
{
	function Armor::onNewDatablock(%this, %obj)
	{
		cancel(%obj.killerLoopSchedule);

		%client = %obj.client;
		if(isObject(%client) && %this.isKiller)
		{
			%client.bottomPrint("", 1, true); //Clear the bottom print. This does not happen normally.
		}
		
		Parent::onNewDatablock(%this, %obj);
	}
};
if(isPackage(Player_Killer))
{
	deactivatePackage(Player_Killer);
}
activatePackage(Player_Killer);