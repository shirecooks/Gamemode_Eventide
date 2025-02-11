function serverCmdResetMinigame(%client)
{
	if (!%client.isAdmin && !isObject(%client.minigame))
	{
		return;
	}
	%client.minigame.reset();	
}

function serverCmdResetMG(%client)
{
	serverCmdResetMinigame(%client);
}

package Eventide_Minigame
{
	function Slayer_MiniGameSO::endRound(%minigame, %winner, %resetTime)
	{
		Parent::endRound(%minigame, %winner, %resetTime);

		for(%i = 0; %i < %minigame.numMembers; %i++)
		{
			// Delete the music
			if(isObject(%client = %minigame.member[%i]) && isObject(%client.EventideMusicEmitter))
			{
				%client.EventideMusicEmitter.delete();
			}
		}

		%minigame.playSound("round_end_sound");

		// Disable local chat at the end of the round, let everyone banter at the end
		if ($MinigameLocalChat)
		{
			$MinigameLocalChat = false;
			%minigame.bottomprintall("<font:impact:20>\c3Local chat disabled",4);
		}
		
		%killers = getCurrentKillers();
		for(%i = 0; %i < %killers.getCount(); %i++)
		{
			%killer = %killers.getObject(%i);
			if(isObject(%killer) && isObject(%killer.player))
			{
				%killerTeam = %killer.getTeam();
				%won = (%winner.getClassName() $= "Slayer_TeamSO" && %winner.getId() == %killerTeam.getId()) || (%winner.getClassName() $= "GameConnection" && %winner.getId() == %killer.getId());
				
				%killerDataBlock = %killer.getDataBlock();
				%killerDatablock.onRoundEnd(%killer, %won);
			}
		}
	}

    function MiniGameSO::Reset(%minigame,%client)
	{
		//Need to clear this before everyone spawns when the parent is called.
		clearCurrentKillers();
		Parent::Reset(%minigame, %client);

		if (isObject(Eventide_MinigameGroup)) Eventide_MinigameGroup.delete();
		
		if (isObject($EventideRitualBrick)) 
		{
			$EventideRitualBrick.ritualsPlaced = 0;
			$EventideRitualBrick.gemcount = 0;
			$EventideRitualBrick.candlecount = 0;
			$EventideRitualBrick.resetAmbienceSound = false;
			$EventideRitualBrick.setEmitter();
			$EventideRitualBrick.getdatablock().ritualCheck($EventideRitualBrick);
		}

		%minigame.escapedCount = 0;
    	%minigame.livingCount = 0; 

		for (%i = 0; %i < getWordCount($Eventide_SurvivorClasses); %i++)
		%minigame.survivorClass[getWord($Eventide_SurvivorClasses,%i)] = 0;
		
		// Loop through all minigame members to perform some actions
		for (%i=0;%i<%minigame.numMembers;%i++) if (isObject(%client = %minigame.member[%i])) 
		{
			// Reset the escape flag
			%client.escaped = false;
			
			// Remove the Eventide music emitter if it exists and reset the music level
			%client.StopChase();
		}

		// Play the round start sound and announce the minigame
		if (strlwr(%minigame.title) $= "eventide") 
		{
			%minigame.schedule(33,assignSurvivorClasses);
			%minigame.randomizeEventideItems(true);						
			%minigame.playSound("round_start_sound");						
			$MinigameLocalChat = $Pref::Server::ChatMod::lchatEnabled;

			if ($MinigameLocalChat)
			{
				%minigame.bottomprintall("<font:impact:25>\c3Local chat is enabled, find a radio to broadcast to other survivors!",4);
			}
		}
    }

    function MinigameSO::endGame(%minigame,%client)
    {
        Parent::endGame(%minigame,%client);

		//Disable local chat
		if ($MinigameLocalChat)
		{
			$MinigameLocalChat = false;
			%minigame.bottomprintall("<font:impact:30>\c3Local chat disabled",4);
		}

        for (%i=0;%i<%minigame.numMembers;%i++)
        if (isObject(%client = %minigame.member[%i]) && isObject(%client.EventidemusicEmitter)) 
		{
			%client.EventidemusicEmitter.delete();
			%client.escaped = false;
		}

		if (isObject(Eventide_MinigameGroup)) 
		Eventide_MinigameGroup.delete();

		%minigame.randomizeEventideItems(false);
    }
};

// In case the package is already activated, deactivate it first before reactivating it
if (isPackage(Eventide_Minigame)) deactivatePackage(Eventide_Minigame);
activatePackage(Eventide_Minigame);

function MiniGameSO::checkDownedSurvivors(%minigame)
{
	// This will only work team based Slayer minigames, this will check if all survivors are incapacitated and end the round if they are
	if (!isObject(%teams = %minigame.teams))
	{
		return;
	}
					
	for (%i = 0; %i < %teams.getCount(); %i++) 
	{
		%team = %teams.getObject(%i);
		if (!isObject(%team)) 
		{
			continue;
		}
		
		%teamNameLower = strlwr(%team.name);
		if (strstr(%teamNameLower, "hunter") != -1) 
		{ 
			%hunterteam = %team;
		}

		if (strstr(%teamNameLower, "survivor") != -1) 
		{
			for (%j = 0; %j < %team.numMembers; %j++) 
			{
				%member = %team.member[%j].player;
				if (isObject(%member) && !%member.getdataBlock().isDowned) 
				{
					%livingcount++;
				}
			}
		}
	}

	if (!%livingcount)
	{
		%minigame.endRound(%hunterteam);
		return;
	}
}

function MiniGameSO::playSound(%minigame,%datablock)
{
	if (!isObject(%minigame) || !isObject(%datablock)) return;
	
	for (%i = 0; %i < %minigame.numMembers; %i++)
	if (isObject(%member = %minigame.member[%i])) %member.play2D(%datablock);	
}

$Eventide_SurvivorClasses = "mender runner hoarder fighter tinkerer";
function MiniGameSO::assignSurvivorClasses(%minigame)
{	
	// Return if there are no teams
	if (!isObject(%teams = %minigame.teams) || !%teams.getCount())
	return;
	
	// Create a temporary simset to hold the team members
	%memberSet = new SimSet();

	// Loop through each team and add the team members to the temporary simset
	for (%i = 0; %i < %teams.getCount(); %i++)
	if (isObject(%team = %teams.getObject(%i)) && strlwr(%team.name $= "survivors"))
	{
		for (%j = 0; %j < %team.numMembers; %j++) if (isObject(%team.member[%j].player))
		%memberSet.add(%team.member[%j]);	
	}

	// Return if there are no team members
	if (!%memberset.getCount())
	{
		%memberset.delete();
		return;
	}

	// Shuffle the survivor class list
	%newClassList = shuffleWords($Eventide_SurvivorClasses);

	// Assign the survivor classes to a random team member, preventing duplicates
	for (%i = 0; %i < getWordCount(%newClassList); %i++)
	{
		%randomMember = %memberSet.getObject(getRandom(0,%memberSet.getCount()-1));
		if (%minigame.survivorClass[getWord(%newClassList,%i)] || %randommember.player.survivorClass !$= "") continue;

		%randomMember.player.survivorClass = getWord(%newClassList,%i);
		%minigame.survivorClass[getWord(%newClassList,%i)] = %randomMember;

		if (%randomMember.player.getdataBlock().getName() $= "EventidePlayer") 
		%randomMember.player.getDatablock().assignClass(%randomMember.player,getWord(%newClassList,%i));
	}

	// Delete the temporary simset
	%memberSet.delete();
}

//
// `onLastSurvivor` event.
//

//Call the `onLastSurvivor` event on any brick that has it.
function MinigameSO::onLastSurvivor(%minigame)
{
	for(%i = 0; %i < mainBrickGroup.getCount(); %i++)
	{
		%brickGroup = mainBrickGroup.getObject(%i);
		%brickCount = %brickGroup.getCount();
		for(%j = 0; %j < %brickCount; %j++)
		{
			%checkObj = %brickGroup.getObject(%j);
			if(%checkObj.numEvents > 0)
			{
				%checkObj.onLastSurvivor();
			}
		}
	}
}

function fxDTSBrick::onLastSurvivor(%obj)
{
	$InputTarget_["Self"] = %obj;
	$InputTarget_["Player"] = 0;
	$InputTarget_["Client"] = 0;
	$InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
	%obj.processInputEvent("onLastSurvivor");
}
registerInputEvent("fxDTSBrick", "onLastSurvivor", "Self fxDTSBrick" TAB "MiniGame MiniGame", 1);