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

	function Slayer_MiniGameSO::onRoundStart(%this)
	{
		%minigame = %this.minigame;
		%minigame.assignSurvivorClasses();
	}

    function MiniGameSO::Reset(%minigame,%client)
	{
		//Need to clear this before everyone spawns when the parent is called.
		clearCurrentKillers();
		$Eventide_currentAltAmbiance = "musicData_altAmbiance" @ getRandom(1, 6);
		
		Parent::Reset(%minigame, %client);

		if (isObject(Eventide_MinigameGroup)) Eventide_MinigameGroup.delete();
		
		if (isObject($EventideRitualBrick)) 
		{
			$EventideRitualBrick.ritualsPlaced = 0;
			$EventideRitualBrick.gemcount = 0;
			$EventideRitualBrick.candlecount = 0;
			$EventideRitualBrick.resetEffects = false;
			$EventideRitualBrick.setEmitter();
			$EventideRitualBrick.getdatablock().ritualCheck($EventideRitualBrick);
		}

		%minigame.escapedCount = 0;
    	%minigame.livingCount = 0; 
		
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
	if (!isObject(%minigame) || !isObject(%datablock))
	{
		return;
	}
	
	for (%i = 0; %i < %minigame.numMembers; %i++)
	{
		if (isObject(%member = %minigame.member[%i])) 
		{
			%member.play2D(%datablock);	
		}
	}
}

function MinigameSO::playMusic(%minigame, %datablock)
{
	for(%i = 0; %i < %minigame.numMembers; %i++)
	{
		%client = %minigame.member[%i];
		if(isObject(%client))
		{
			%client.SetChaseMusic(%datablock, true);			
		}									
	}
}