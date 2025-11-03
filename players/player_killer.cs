datablock PlayerData(PlayerKiller : PlayerEventide) 
{
    class = "PlayerKiller";
    superClass = "PlayerEventide";

	uiName = "";
    isKiller = true;
    firstPersonOnly = true;
	
	killerWeaponImage = eventideMeleeImage;

	facePack = "";
	voicePack = "";

	rechargeRate = 0.26;
	maxDamage = 9999;
	maxTools = 1;
	maxWeapons = 1;

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
	%killerWeaponImage = (%obj.killerWeaponImage $= "") ? %this.killerWeaponImage : %obj.killerWeaponImage;
	if(%killerWeaponImage !$= "")
	{
		%obj.mountImage(%killerWeaponImage, 0);
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

	//Special functionality: killers having more than one player datablock. 
	//Don't mess up the killer loop if that is the case. Stop here.
	if(%this.superClass !$= "PlayerKiller")
	{
		return;
	}
	else if(%obj.shallowDatablockChanges !$= "" && %obj.shallowDatablockChanges > 0)
	{
		%obj.shallowDatablockChanges--;
		return;
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

function PlayerKiller::killerLoop(%this, %obj)
{
    if(!isObject(%obj) || !%obj.getDataBlock().isKiller || %obj.getState() $= "Dead")
	{
		return;
	}

	%this.preKillerLoop(%obj);

	%searchDistance = 25; //80 studs, should be a decent balance for both small and large maps.
	%gracePeriod = 4000;
	%currentTime = getSimTime();

	//Cycle through the current list of people near the killer, and determine if they are now too far away.
	for(%i = 0; %i < %obj.nearVictims.getCount(); %i++)
	{
		%victim = %obj.nearVictims.getObject(%i);

		//Can't do anything if the victim no longer exists.
		if(!isObject(%victim) || %victim.getState() $= "Dead")
		{
			%obj.nearVictims.remove(%victim);
			continue;
		}

		%victimDistance = VectorDist(%victim.getPosition(), %obj.getPosition());
		if(%victimDistance > %searchDistance)
		{
			//If the grace period hasn't expired, don't do this yet.
			if((%currentTime - %victim.timeSinceKillerNear) < %gracePeriod)
			{
				continue;
			}

			//The victim is too far away, unmark them as being near the killer.
			%victim.getDatablock().onKillerExitRange(%victim, %obj);
			%victim.isKillerNear = false;

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

		//Can't do anything if the victim no longer exists.
		if(!isObject(%victim) || %victim.getState() $= "Dead")
		{
			%obj.chasingVictims.remove(%victim);
			continue;
		}

		%victimClient = %victim.client;
		if((%currentTime - %victim.timeSinceChased) > %gracePeriod)
		{
			//It's been too long since the victim was last seen, end the chase.
			%obj.chasingVictims.remove(%victim);
			%obj.getDatablock().onKillerChaseEnd(%obj, %victim);

			%victim.getDatablock().onKillerChaseEnd(%victim, %obj);
			%victim.isBeingChased = false;

			%victimDistance = VectorDist(%victim.getPosition(), %obj.getPosition());
			if(%victimDistance > %searchDistance)
			{
				//The victim is completely out-of-range, unmark them as being near.
				%victim.getDatablock().onKillerExitRange(%victim, %obj);
				%victim.isKillerNear = false;
				
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
	%currentVictimDistance = 2147483647; //Maximum 32-bit signed integer.

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

		//Necessary for the head-turn mechanic presented further down.
		if(%victimDistance < %currentVictimDistance)
		{
			%currentVictimDistance = %victimDistance;
			%obj.closestChasingVictim = %victim;
		}

		//Before anything else, mark the victim as near the killer if they aren't already.
		if(!%victim.isKillerNear)
		{
			%obj.nearVictims.add(%victim);
			%this.onKillerEnterRange(%obj, %victim);

			%victim.getDatablock().onKillerEnterRange(%victim, %obj);
			%victim.isKillerNear = true;
			%victim.timeSinceKillerNear = %currentTime;
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
			if(!%victim.isBeingChased)
			{
				%obj.chasingVictims.add(%victim);
				%this.onKillerChaseStart(%obj, %victim);

				%victim.getDatablock().onKillerChaseStart(%victim, %obj);
				%victim.isBeingChased = true;
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
		%obj.playAmbiantMusic(%this.killerChaseMusic, 1.0, "Chase");
	}
	else if(%obj.nearVictims.getCount() > 0)
	{
		//The killer isn't in a chase, but people are nearby.
		%obj.playVoiceLine("Near");
		%obj.playAmbiantMusic(%this.killerNearMusic, 1.0, "Near");
	}
	else
	{
		//The killer has no action at all.
		%obj.playVoiceLine("Idle");
		%obj.playRandomAmbiantTrack();
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
// Package to manage the killer's melee.
//

package Player_Killer
{
	function ServerCmdUnUseTool(%client)
	{
		Parent::ServerCmdUnUseTool(%client);
		%player = %client.Player;
		if(!%player)
		{
			return;
		}

		%playerDatablock = %player.getDataBlock();
		if(%playerDatablock.isKiller)
		{
			%killerMeleeImage = (%player.killerWeaponImage $= "") ? %playerDatablock.killerWeaponImage : %player.killerWeaponImage;
			%player.mountImage(%killerMeleeImage, 0);
		}
	}
};
if(isPackage(Player_Killer))
{
	deactivatePackage(Player_Killer);
}
activatePackage(Player_Killer);