package Eventide_Killers
{
	function getBrickGroupFromObject(%obj)
	{	
		// Workaround for AIPlayers
		if(isObject(%obj) && %obj.getClassName() $= "AIPlayer")
		{
			switch$(%obj.getDataBlock().getName())			
			{
				case "ShireZombieBot": return %obj.ghostclient.brickgroup;
				case "PuppetMasterPuppet": return %obj.client.brickgroup;
			}
		}

		Parent::getBrickGroupFromObject(%obj);
	}

	function serverCmdUseTool(%client, %tool)
	{	
		// If the player is not a victim, then continue
		if(!%client.player.victim)
		{
			return parent::serverCmdUseTool(%client, %tool);
		}
	}	

	function MiniGameSO::Reset(%obj, %client)
	{
		parent::Reset(%obj, %client);
	}
	
	function Observer::onTrigger (%this, %obj, %trigger, %state)
	{		
		if (%obj.getControllingClient().player.stunned)
		{
			return;
		}
		
		Parent::onTrigger (%this, %obj, %trigger, %state);
	}

	function Armor::onNewDatablock(%this,%obj)
	{		
		Parent::onNewDatablock(%this,%obj);
		
		if(%this.isEventideModel)
		{
			%this.schedule(33,killerCheck,%obj);
		}

		%client = %obj.client;
		if(%this.isKiller && isObject(%client))
		{
			addCurrentKiller(%client);
		}
	}

	function Armor::onRemove(%this, %obj)
	{
		Parent::onRemove(%this, %obj);

		%client = %obj.client;
		if(%this.isKiller && isObject(%client))
		{
			removeCurrentKiller(%client);
		}
	}

	function Armor::onDisabled(%this, %obj, %state)
	{
        Parent::onDisabled(%this, %obj, %state);
		
		if (!isObject(%killer = %obj.killer))
		{
			return;
		}
		
		%killer.ChokeAmount = 0;
		%killer.victim = 0;
		%killer.playthread(3,"activate2");
		%obj.dismount();
		%obj.setVelocity(vectorscale(vectorAdd(%killer.getForwardVector(),"0 0 0.25"),15));		
    }
};

// In case the package is already activated, deactivate it first before reactivating it
if(isPackage(Eventide_Killers)) deactivatePackage(Eventide_Killers);
activatePackage(Eventide_Killers);

$Eventide_killers = new SimSet();

function addCurrentKiller(%client)
{
	$Eventide_killers.add(%client);
}
function getCurrentKillers()
{
	return $Eventide_killers;
}
function clearCurrentKillers()
{
	$Eventide_killers.delete();
	$Eventide_killers = new SimSet();
}
function removeCurrentKiller(%client)
{
	%temporarySimset = new SimSet();
	
	for(%i = 0; %i < $Eventide_killers.getCount(); %i++)
	{
		%killer = $Eventide_killers.getObject(%i);
		if(%killer.getId() != %client.getId())
		{
			%temporarySimset.add(%killer);
		}
	}

	$Eventide_killers.delete();
	$Eventide_killers = %temporarySimset;
}

/// This function is called every tick on the killer player
/// It will check if the player is valid, dead or not a killer
/// If the player is valid, it will update the appearance and send a message to all players
/// It will also handle the killer's light
function Armor::killerCheck(%this,%obj)
{
	// Return early if the object is invalid, dead, or not a killer
	if(!isObject(%obj) || %obj.getState() $= "Dead" || !%this.isKiller) 
	{
		return;
	}

	// Update the appearance, only if the client exists
	%clientExists = isObject(%obj.client);
	if(%clientExists) 
	{
		%this.EventideAppearance(%obj,%obj.client);
	}

	// Handle the killer's light
	// The killer's light is only visible to the killer itself
	// If the killer is invisible, the light is not created
	if(!%obj.isInvisible && %clientExists)
	{
		// If the light does not exist, create it
		if(!isObject(%obj.light))
		{
			%obj.light = new fxLight()
			{
				dataBlock = %this.killerlight;
				source = %obj;
			};
		}

		// Attach the light to the player and set a net flag
		%obj.light.attachToObject(%obj);
		%obj.light.setNetFlag(6,true);
		adjustObjectScopeToAll(%obj.light, false, %obj.client);	//Object, visible?, exceptions.
		
		// If the Eventide Minigame Group does not exist, create it
		if(!isObject(Eventide_MinigameGroup)) 
		{
			missionCleanUp.add(new SimGroup(Eventide_MinigameGroup));
		}

		// Add the light to the Eventide Minigame Group
		Eventide_MinigameGroup.add(%obj.light);
	}
	else if(isObject(%obj.light)) 
	{
		// Delete the light if the killer is invisible
		%obj.light.delete();
	}

	%this.onKillerLoop(%obj);
}

function Armor::onKillerChaseStart(%this, %obj)
{
	//Hello, World!
}

function Armor::onKillerChase(%this, %obj, %chasing)
{
	//Hello world
}

function Armor::onKillerChaseEnd(%this, %obj)
{
	//Hello, World!
}

function Armor::onIncapacitateVictim(%this, %obj, %victim, %killed)
{
	//Hello, World!
}

function Armor::onEnterStun(%this, %obj)
{
	//Hello, World!
}

function Armor::onExitStun(%this, %obj)
{
	//Hello, World!
}

function Armor::onAllRitualsPlaced(%this, %obj)
{
	//Hello, World!
}

function Armor::onRoundEnd(%this, %obj, %won)
{
	//Hello, World!
}

function GameConnection::SetChaseMusic(%client, %songname, %ischasing)
{
    // Do not continue if there is no client, invalid song, or if the ritual is complete
	if(!isObject(%client) || !isObject(%songname))
	{		
		return;    
	}

	// If the ritual is complete and the song is not the hurry song, return
	if((isObject($EventideRitualBrick) && $EventideRitualBrick.ritualsPlaced >= 10) && strlwr(%songname) !$= "musicData_eventide_hurry")
	{
		return;
	}
    
	if(isObject(%currentMusicEmitter = %client.EventideMusicEmitter)) 
	{
		if(%currentMusicEmitter.profile $= %songname)
		{
			return;
		}
		else
		{
			%client.EventideMusicEmitter.delete();
		}
	}

    %client.EventideMusicEmitter = new AudioEmitter()
    {
        position = "9e9 9e9 9e9";
        profile = %songname;
        volume = 1;
        type = 10;
        useProfileDescription = false;
        is3D = false;
    };

    MissionCleanup.add(%client.EventideMusicEmitter);
	adjustObjectScopeToAll(%client.EventideMusicEmitter, false, %client);
}

function GameConnection::PlaySkullFrames(%client,%frame)
{
    if(!isObject(%client) || %frame > 12)
	{
		return;
	}

	if(!%frame)
	{
		%frame = 1;
	}

	%client.centerprint("<br><br><bitmap:Add-ons/Gamemode_Eventide/modules/misc/icons/skullFrames/SkullFrame" @ %frame @ ">",0.2);

	cancel(%client.SkullFrameSched);
	%client.SkullFrameSched = %client.schedule(60, PlaySkullFrames, %frame++);
}

function GameConnection::playAmbiance(%client)
{
	%ambientMusicDatablock = "musicData_ambiance" @ getRandom(1, 4);
	%client.SetChaseMusic(%ambientMusicDatablock, false);
}

function GameConnection::StopChase(%client)
{
    if(!isObject(%client))
	{
		return;
	}

    if(isObject(%client.EventideMusicEmitter))
	{
		%client.EventideMusicEmitter.delete();
	}

	//Play ambiant music track.
	%client.playAmbiance();

	// Handle survivor conditions
	if(isObject(%client.player) && %client.player.getdataBlock().getName() $= "EventidePlayer")
	{
		//Face system functionality. Make the victim return to calm facial expressions when they are no longer being chased.
		if(isObject(%client.player.faceConfig) && %client.player.faceConfig.subCategory $= "Scared")
		{
			if(%client.player.getDamagePercent() > 0.33 && $Eventide_FacePacks[%client.player.faceConfig.category, "Hurt"] !$= "")
			{
				%client.player.createFaceConfig($Eventide_FacePacks[%client.player.faceConfig.category, "Hurt"]);
			}
			else
			{
				%client.player.createFaceConfig($Eventide_FacePacks[%client.player.faceConfig.category]);
			}		
		}
	}

    %client.player.chaseLevel = 0;
    %client.musicChaseLevel = 0;
}