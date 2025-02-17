function GameConnection::getRemainingTeamMembers(%client)
{
	//Iterate through each member of the survivor's team, checking how many people are still alive and unescaped.
	%clientTeam = %client.getTeam();
	if(!isObject(%clientTeam))
	{
		return 0;
	}

	%teamPlayerAmount = %clientTeam.numMembers;
	%playersLeftCount = 0;
	for(%i = 0; %i < %teamPlayerAmount; %i++)
	{
		%currentMember = %clientTeam.member[%i];
		if(isObject(%currentMember))
		{
			if(!%currentMember.escaped && !%currentMember.Dead() && isObject(%currentMember.player))
			{
				%playersLeftCount++;
			}
		}
	}

	return %playersLeftCount;
}

package Eventide_Player
{

	function serverCmdSuicide(%client)
	{
		if(isObject(%client.player) && %client.player.isSkinwalker)
		{			
			%client.player.getDatablock().onTrigger(%client.player,4,1);
			return;
		}

		Parent::serverCmdSuicide(%client);		
	}

	function ServerCmdDropTool (%client, %position)
	{
		if(isObject(%client.player) && %client.player.tool[%position].canDrop)
		{		
			%client.player.playthread(3,"activate");
			$oldTimescale = getTimescale();
			%soundpitch = getRandom(100,200);
			setTimescale((%soundpitch*0.01) * $oldTimescale);
			serverPlay3D("melee_swing" @ getRandom(1,2) @ "_sound",%client.player.getHackPosition());
			setTimescale($oldTimescale);
		}
		
		Parent::ServerCmdDropTool (%client, %position);
	}
	
	function Player::Unmount(%obj)
	{
		if(!%obj.stunned) 
		{
			Parent::Unmount(%obj);
		}
	}
	
	function GameConnection::setControlObject(%client,%obj)
	{		
		Parent::setControlObject(%client,%obj);
		
		if (%obj == %client.player && %obj.getDatablock().maxTools != %client.lastMaxTools)
		{
			%client.lastMaxTools = %obj.getDatablock().maxTools;
			commandToClient(%client,'PlayGui_CreateToolHud',%obj.getDatablock().maxTools);
		}

		if(isObject(%client.getControlObject()) && %client.getType() & $TypeMasks::PlayerObjectType)
        {
			if(%client.getControlObject().getState() !$= "Dead" && %client.getControlObject().getDataBlock().getName() $= "ShireZombieBot")
			{
				%client.player = %client.getControlObject();
			}            
        }
	}	
	
	function gameConnection::applyBodyColors(%client) 
	{
		parent::applyBodyColors(%client);
		
		// Call the EventideAppearance function if the player is an Eventide player
		if (isObject(%player = %client.player) && %player.getDataBlock().isEventideModel) 
		%player.getDataBlock().EventideAppearance(%player,%client);
	}
	function gameConnection::applyBodyParts(%client) 
	{
		parent::applyBodyParts(%client);

		// Call the EventideAppearance function if the player is an Eventide player
		if (isObject(%player = %client.player) && %player.getDataBlock().isEventideModel) 
		%player.getDataBlock().EventideAppearance(%player,%client);
	}

	function Armor::onImpact(%this, %obj, %col, %vec, %force)
	{
		Parent::onImpact(%this, %obj, %col, %vec, %force);

		if (%obj.isInvisible || !isObject(%obj))
		{
			return;
		}		

		%oScale = getWord(%obj.getScale(),2);
		%forcescale = %force/25 * %oscale;
		%obj.spawnExplosion(pushBroomProjectile,%forcescale SPC %forcescale SPC %forcescale);
		
		if(%obj.isCrouched() || %force < %this.minImpactSpeed) return;

		serverPlay3D("impact_" @ (%force < 40 ? "medium" : "hard") @ getRandom(1,3) @ "_sound",%obj.getPosition());

		// Play a sound if the player is falling
		if (%obj.getState() !$= "Dead" && getWord(%vec,2) > %obj.getdataBlock().minImpactSpeed * 2)
		{
			serverPlay3D("impact_fall_sound",%obj.getPosition());		
		}        
	}

	function Armor::onNewDatablock(%this,%obj)
	{		
		Parent::onNewDatablock(%this,%obj);

		%obj.schedule(33,setActionThread,"root");

		// Initiate the gaze loop if the player is in a minigame
		if (isObject(getMinigamefromObject(%obj))) 
		{
			%this.GazeLoop(%obj);
		}

		// Update the tool HUD
		if (%this != %obj.getDatablock() && %this.maxTools != %obj.client.lastMaxTools && isObject(%obj.client))
		{
			%obj.client.lastMaxTools = %this.maxTools;
			commandToClient(%obj.client,'PlayGui_CreateToolHud',%this.maxTools);
			
			for (%i=0;%i<%this.maxTools;%i++) 
			{			
				messageClient(%obj.client,'MsgItemPickup',"",%i,isObject(%obj.tool[%i]) ? %obj.tool[%i].getID() : 0,1);
			}
		}
	}

	function Armor::onDisabled(%this, %obj, %state)
	{
        Parent::onDisabled(%this, %obj, %state);

		for (%i = 0; %i < %obj.getMountedObjectCount(); %i++)
		{
			if (isObject(%obj.getMountedObject(%i)) && (%obj.getMountedObject(%i).getDataBlock().className $= "PlayerData")) 
			{
				%obj.getMountedObject(%i).delete();	
			}
		}

        if (isObject(%obj.client))
		{
			// Remove the Eventide music emitter if it exists and reset the music level
			%obj.client.StopChase();
		}
    }

	function Armor::onRemove(%this,%obj)
	{
		Parent::onRemove(%this, %obj);

		for (%i = 0; %i < %obj.getMountedObjectCount(); %i++)
		{
			if (isObject(%obj.getMountedObject(%i)) && (%obj.getMountedObject(%i).getDataBlock().className $= "PlayerData")) 
			{
				%obj.getMountedObject(%i).delete();
			}			
		}
	}

	function Player::ActivateStuff (%player)
	{
		Parent::ActivateStuff(%player);
		
		if (%player.getState() !$= "Dead" && isFunction(%player.getDataBlock().getName(),onActivate)) 
		{
			%player.getDataBlock().onActivate(%player);
		}		
	}
};

// In case the package is already activated, deactivate it first before reactivating it
if (isPackage(Eventide_Player)) deactivatePackage(Eventide_Player);
activatePackage(Eventide_Player);

function Player::SetTempSpeed(%obj,%speedMultiplier)
{
	if (!isObject(%obj) || %obj.getstate() $= "Dead")
	{
		return;
	}

	// If the speed multiplier is not set, reset it to 1, or 1.1 if the player is a runner class
	if (%speedMultiplier $= "") 
	{
		%speedMultiplier = (%obj.survivorclass $= "runner") ? 1.1 : 1;
	}

	%this = %obj.getDataBlock();
	
	// Normal speed
	%obj.setMaxForwardSpeed(%this.MaxForwardSpeed * %speedMultiplier);
	%obj.setMaxSideSpeed(%this.MaxSideSpeed * %speedMultiplier);
	%obj.setMaxBackwardSpeed(%this.maxBackwardSpeed * %speedMultiplier);

	// Crouch speed
	%obj.setMaxCrouchForwardSpeed(%this.maxForwardCrouchSpeed * %speedMultiplier);
  	%obj.setMaxCrouchBackwardSpeed(%this.maxSideCrouchSpeed * %speedMultiplier);
  	%obj.setMaxCrouchSideSpeed(%this.maxSideCrouchSpeed * %speedMultiplier);

  	// Underwater speed
 	%obj.setMaxUnderwaterBackwardSpeed(%this.MaxUnderwaterBackwardSpeed * %speedMultiplier);
  	%obj.setMaxUnderwaterForwardSpeed(%this.MaxUnderwaterForwardSpeed * %speedMultiplier);
  	%obj.setMaxUnderwaterSideSpeed(%this.MaxUnderwaterForwardSpeed * %speedMultiplier);	
}

registerInputEvent("fxDtsBrick", "onGaze", "Self fxDtsBrick\tPlayer Player\tClient GameConnection\tMinigame Minigame");
function Armor::GazeLoop(%this,%obj)
{		
	// Some conditions to return early if one is met
	if (!$Pref::Server::GazeEnabled || !isObject(%obj) || %obj.getState() $= "Dead") 
	{
		return;
	}

	%eye = %obj.getEyePoint();
	%end = vectorAdd(%obj.getEyePoint(),vectorScale(%obj.getEyeVector(),$Pref::Server::GazeRange));
	%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::PlayerObjectType;
	%hit = containerRayCast (%eye, %end, %mask, %obj);
	%obj.gazingPlayer = false;
	%obj.gazing = 0;
	
	if (isObject(%hit))
	{
		if (%hit.getType() & $TypeMasks::FxBrickObjectType)
		{
			$InputTarget_Self = %hit;
			$InputTarget_Player = %obj;
			$InputTarget_Client = (isObject(%client = %obj.client) ? %client : 0);
			$InputTarget_Minigame = %minigame;
			%hit.processInputEvent("onGaze", %gazer);
		}

		if (%hit.getType() & $TypeMasks::PlayerObjectType) 
		{
			%obj.gazingPlayer = true;
		}
		
		%obj.gazing = %hit;
	}	

	// Cancel and reschedule the loop to avoid any overlapping schedules
	cancel(%obj.GazeLoop);
	%obj.GazeLoop = %this.schedule(33,GazeLoop,%obj);	
}

function Armor::EventideAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");
	%obj.unHideNode((%client.chest ? "femChest" : "chest"));	
	%obj.unHideNode((%client.rhand ? "rhook" : "rhand"));
	%obj.unHideNode((%client.lhand ? "lhook" : "lhand"));
	%obj.unHideNode((%client.rarm ? "rarmSlim" : "rarm"));
	%obj.unHideNode((%client.larm ? "larmSlim" : "larm"));
	%obj.unHideNode("headskin");

	if ($pack[%client.pack] !$= "none")
	{
		%obj.unHideNode($pack[%client.pack]);
		%obj.setNodeColor($pack[%client.pack],%client.packColor);
	}

	if ($secondPack[%client.secondPack] !$= "none")
	{
		%obj.unHideNode($secondPack[%client.secondPack]);
		%obj.setNodeColor($secondPack[%client.secondPack],%client.secondPackColor);
	}
	if ($hat[%client.hat] !$= "none")
	{
		%obj.unHideNode($hat[%client.hat]);
		%obj.setNodeColor($hat[%client.hat],%client.hatColor);
	}
	if (%client.hip)
	{
		%obj.unHideNode("skirthip");
		%obj.unHideNode("skirttrimleft");
		%obj.unHideNode("skirttrimright");
	}
	else
	{
		%obj.unHideNode("pants");
		%obj.unHideNode((%client.rleg ? "rpeg" : "rshoe"));
		%obj.unHideNode((%client.lleg ? "lpeg" : "lshoe"));
	}

	%obj.setHeadUp(0);
	if (%client.pack+%client.secondPack > 0) 
	{
		%obj.setHeadUp(1);
	}
	
	if ($hat[%client.hat] $= "Helmet")
	{
		if (%client.accent == 1 && $accent[4] !$= "none")
		{
			%obj.unHideNode($accent[4]);
			%obj.setNodeColor($accent[4],%client.accentColor);
		}
	}
	else if ($accent[%client.accent] !$= "none" && strpos($accentsAllowed[$hat[%client.hat]],strlwr($accent[%client.accent])) != -1)
	{
		%obj.unHideNode($accent[%client.accent]);
		%obj.setNodeColor($accent[%client.accent],%client.accentColor);
	}

	%bloodyParts = "lshoe rshoe lhand rhand";
	for (%i = 0; %i < getWordCount(%bloodyParts); %i++) 
	{
	    %part = getWord(%bloodyParts, %i);
	    if (%obj.bloody[%part]) 
		{
			%obj.unHideNode(%part @ "_blood");
		}
	}

	if (%obj.bloody["chest_front"]) 
	{
		%obj.unHideNode((%client.chest ? "fem" : "") @ "chest_blood_front");
	}

	if (%obj.bloody["chest_back"])
	{
		%obj.unHideNode((%client.chest ? "fem" : "") @ "chest_blood_back");
	}

	%obj.setFaceName(%client.faceName);
	%obj.setDecalName(%client.decalName);

	%obj.setNodeColor("headskin",%client.headColor);	
	%obj.setNodeColor("chest",%client.chestColor);
	%obj.setNodeColor("femChest",%client.chestColor);
	%obj.setNodeColor("pants",%client.hipColor);
	%obj.setNodeColor("skirthip",%client.hipColor);	
	%obj.setNodeColor("rarm",%client.rarmColor);
	%obj.setNodeColor("larm",%client.larmColor);
	%obj.setNodeColor("rarmSlim",%client.rarmColor);
	%obj.setNodeColor("larmSlim",%client.larmColor);
	%obj.setNodeColor("rhand",%client.rhandColor);
	%obj.setNodeColor("lhand",%client.lhandColor);
	%obj.setNodeColor("rhook",%client.rhandColor);
	%obj.setNodeColor("lhook",%client.lhandColor);	
	%obj.setNodeColor("rshoe",%client.rlegColor);
	%obj.setNodeColor("lshoe",%client.llegColor);
	%obj.setNodeColor("rpeg",%client.rlegColor);
	%obj.setNodeColor("lpeg",%client.llegColor);
	%obj.setNodeColor("skirttrimright",%client.rlegColor);
	%obj.setNodeColor("skirttrimleft",%client.llegColor);

	//Set blood colors.
	%obj.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("lhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("rhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_back", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_back", "0.7 0 0 1");
}

registerOutputEvent("GameConnection", "Escape", "", false);
function GameConnection::Escape(%client)
{
	%clientTeam = %client.getTeam();
	if(!isObject(%minigame = getMinigameFromObject(%client))) 
	{
		return %client.centerprint("This only works in minigames!",1);
	}
	else if(strlwr(%clientTeam.name) !$= "survivors")
	{
		return %client.centerprint("Only survivors can escape the map!",1);
	}
	
	%client.escaped = true;
	
	//Delete the escaped player.
	%client.player.delete();
	%client.camera.setMode("Spectator", %client);
	%client.setcontrolobject(%client.camera);	
	%minigame.chatmsgall("<font:Impact:30>\c3" @ %client.name SPC "\c3has escaped!");
	%client.lives = 0;
	%client.setdead(1);

	//End the round, if everyone else is dead or escaped.
	if(%client.getRemainingTeamMembers() <= 0)
	{
		%minigame.endRound(%clientTeam);
		return;
	}	
}
