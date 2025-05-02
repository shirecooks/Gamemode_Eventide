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
		%player = %client.player;
		%item = %player.tool[%position];
		if(isObject(%player) && %item.canDrop)
		{		
			%player.playthread(3,"activate");
			%oldTimescale = getTimescale();
			%soundpitch = getRandom(100,200);
			setTimescale((%soundpitch*0.01) * %oldTimescale);
			serverPlay3D("melee_swing" @ getRandom(1,2) @ "_sound",%client.player.getHackPosition());
			setTimescale(%oldTimescale);
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
	
	function GameConnection::applyBodyColors(%client) 
	{
		%player = %client.player;
		if(!isObject(%player))
		{
			return;
		}

		%playerDatablock = %player.getDataBlock();
		if(isObject(%player) && %playerDatablock.isEventideModel)
		{
			//This already gets called in applyBodyParts, don't need to do it twice.
			//%playerDatablock.EventideAppearance(%player, %client);
		}
		else
		{
			parent::applyBodyColors(%client);
		}
	}
	function GameConnection::applyBodyParts(%client) 
	{
		%player = %client.player;
		if(!isObject(%player))
		{
			return;
		}

		%playerDatablock = %player.getDataBlock();
		if(isObject(%player) && %playerDatablock.isEventideModel)
		{
			// Call the EventideAppearance function if the player is an Eventide player
			%playerDatablock.EventideAppearance(%player, %client);
		}
		else
		{
			parent::applyBodyParts(%client);
		}
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

		%datablock = %obj.getDataBlock();
		if(%datablock.isKiller && %obj.getMountedImage($LeftHandSlot) == nameToID("FlashlightImage"))
		{
			%obj.deleteFlashlightBeam();
			%obj.unmountImage($LeftHandSlot);
			if(isObject(%obj.Light))
			{
				%obj.Light.setDatablock(%this.killerLight);
			}
		}

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
	%obj.unHideNode((%tempclient.chest 	? 	"femChest" : "chest"));	
	%obj.unHideNode((%tempclient.rhand 	? 	"rhook" : "rhand"));
	%obj.unHideNode((%tempclient.lhand 	? 	"lhook" : "lhand"));
	%obj.unHideNode((%tempclient.rarm 	? 	"rarmSlim" : "rarm"));
	%obj.unHideNode((%tempclient.larm 	? 	"larmSlim" : "larm"));
	%obj.unHideNode("headskin");

	//Packs
	if ($pack[%tempclient.pack] !$= "none")
	{
		%obj.unHideNode($pack[%tempclient.pack]);
		%obj.setNodeColor($pack[%tempclient.pack],%tempclient.packColor);
	}
	if ($secondPack[%tempclient.secondPack] !$= "none")
	{
		%obj.unHideNode($secondPack[%tempclient.secondPack]);
		%obj.setNodeColor($secondPack[%tempclient.secondPack],%tempclient.secondPackColor);
	}

	//Hats
	%hat = $HatMod::save::wornHat[%tempclient.bl_id];
	if(isFunction(isHat) && isHat(%hat))
	{
		%obj.mountHat(%hat);
	}
	else if (%tempclient.hat)
	{	
		%hatName = $hat[%tempclient.hat];
		%tempclient.hatString = %hatName;
		
		// Only check if it's the first hat
		if (%tempclient.hat == 1)
		{
			%newhat = (%tempclient.accent ? "helmet" : "hoodie1");
			%obj.unHideNode(%newhat);
			%obj.setNodeColor(%newhat,%tempclient.hatColor);
		}
		else
		{
			%obj.unHideNode(%hatName);
			%obj.setNodeColor(%hatName,%tempclient.hatColor);
		}			
	}
	
	//Legs
	if (%tempclient.hip) %obj.unHideNode("skirt");
	else
	{
		%obj.unHideNode("pants");
		%obj.unHideNode((%tempclient.rleg ? "rpeg" : "rshoe"));
		%obj.unHideNode((%tempclient.lleg ? "lpeg" : "lshoe"));
	}

	%obj.setHeadUp((%tempclient.pack+%tempclient.secondPack));

	%obj.setDecalName(%tempclient.decalName);

	// Set node colors
	%obj.setNodeColor("headskin",%tempclient.headColor);	
	%obj.setNodeColor("chest",%tempclient.chestColor);
	%obj.setNodeColor("femChest",%tempclient.chestColor);
	%obj.setNodeColor("pants",%tempclient.hipColor);
	%obj.setNodeColor("skirt",%tempclient.hipColor);	
	%obj.setNodeColor("rarm",%tempclient.rarmColor);
	%obj.setNodeColor("larm",%tempclient.larmColor);
	%obj.setNodeColor("rarmSlim",%tempclient.rarmColor);
	%obj.setNodeColor("larmSlim",%tempclient.larmColor);
	%obj.setNodeColor("rhand",%tempclient.rhandColor);
	%obj.setNodeColor("lhand",%tempclient.lhandColor);
	%obj.setNodeColor("rhook",%tempclient.rhandColor);
	%obj.setNodeColor("lhook",%tempclient.lhandColor);	
	%obj.setNodeColor("rshoe",%tempclient.rlegColor);
	%obj.setNodeColor("lshoe",%tempclient.llegColor);
	%obj.setNodeColor("rpeg",%tempclient.rlegColor);
	%obj.setNodeColor("lpeg",%tempclient.llegColor);

	if (%obj.bloody["lshoe"]) %obj.unHideNode("lshoe_blood");
	if (%obj.bloody["rshoe"]) %obj.unHideNode("rshoe_blood");
	if (%obj.bloody["lhand"]) %obj.unHideNode("lhand_blood");
	if (%obj.bloody["rhand"]) %obj.unHideNode("rhand_blood");
	if (%obj.bloody["chest_front"]) %obj.unHideNode((%tempclient.chest ? "fem" : "") @ "chest_blood_front");
	if (%obj.bloody["chest_back"]) %obj.unHideNode((%tempclient.chest ? "fem" : "") @ "chest_blood_back");

	// Set blood colors
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