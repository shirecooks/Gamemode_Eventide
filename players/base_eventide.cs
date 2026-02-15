datablock TSShapeConstructor(PlayerEventideDTS) 
{
	baseShape = "./models/playerEventide/playerEventide.dts";
	sequence0 = "./models/playerEventide/default.dsq";
	sequence1 = "./models/playerEventide/default_melee.dsq";
};

datablock PlayerData(PlayerEventide : PlayerStandardArmor)
{
	shapeFile = PlayerEventideDTS.baseShape;
	uiName = "";

	enablePeggFootsteps = true;
	uniformCompatible = true;
	isEventideClass = true;
    renderFirstPerson = false;
	canJet = false;

	showEnergyBar = true;
	rechargeRate = 0.375;

	maxWeapons = 3;
	maxTools = 3;

	useCustomPainEffects = true;
	jumpSound = "";
	PainSound = "";
	DeathSound = "";

	jumpForce = 0;	
	cameramaxdist = 2.25;
    cameratilt = 0.1;
	maxfreelookangle = 2.5;

	minimpactspeed = 15;
	groundImpactMinSpeed = 5;
	groundImpactShakeFreq = "4.0 4.0 4.0";
	groundImpactShakeAmp = "1.0 1.0 1.0";
	groundImpactShakeDuration = 0.8;
	groundImpactShakeFalloff = 15;
};

//
// `PlayerEventide` functions.
//

function PlayerEventide::onNewDatablock(%this, %obj)
{
	Parent::onNewDatablock(%this, %obj);

	//I doubt this is needed, but we'll leave it in anyway.
	%obj.setActionThread("root");
	%obj.setScale("1 1 1");

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
}

function PlayerEventide::onImpact(%this, %obj, %col, %vec, %force)
{	
	%zVelocity = getWord(%vec, 2);	

	//Simple force multiplier that may be too forgiving.
	if(%zVelocity > %this.minImpactSpeed)
	{
		%force = (%force * (%this.minImpactSpeed / %zVelocity));
	}

	//Do anything else that may be needed.
	Parent::onImpact(%this, %obj, %col, %vec, mCeil(%force));

	//Make the player "thud" if the impact had sufficient velocity.
	if(%obj.getState() !$= "Dead" && %zVelocity > %this.minImpactSpeed)
	{
		%obj.playthread(3, "plant");
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

function PlayerEventide::clearHatmodHat(%this, %obj)
{
	//Get rid of the Hatmod hat if the player isn't supposed to have it.
	%hatModHat = %obj.getMountedImage(2);
	if(isObject(%hatModHat) && isFunction(isHat) && isHat(%hatModHat))
	{
		%obj.unmountImage(2);
		return 1;
	}
	return -1;
}

function PlayerEventide::eventideBodyParts(%this, %obj)
{
	//We need a client to do anything here.
	%client = %obj.client;
	if(!isObject(%client))
	{
		//No client, so substitute with the default Blockhead apparance.
		%obj.hideNode("ALL");
		
		%obj.unHideNode("headskin");
		%obj.unHideNode("chest");
		%obj.unHideNode("larm");
		%obj.unHideNode("rarm");
		%obj.unHideNode("lhand");
		%obj.unHideNode("rhand");
		%obj.unHideNode("pants");
		%obj.unHideNode("lshoe");
		%obj.unHideNode("rshoe");

		%obj.setHeadUp(0);

		%obj.setDecalName("AAA-None");
		%obj.setFaceName("smiley");
		return;
	}

	//Start by first cleaning everything up.
    %obj.hideNode("ALL");

	//Get rid of the Hatmod hat if the player isn't supposed to have it.
	%hatModHat = %obj.getMountedImage(2);
	if(%this.noHatmod && isObject(%hatModHat) && isFunction(isHat) && isHat(%hatModHat))
	{
		%this.clearHatmodHat(%obj);
	}
	
	//Core body parts.
	%obj.unHideNode((%client.chest ? "femChest" : "chest"));	
	%obj.unHideNode((%client.rhand ? "rhook" : "rhand"));
	%obj.unHideNode((%client.lhand ? "lhook" : "lhand"));
	%obj.unHideNode((%client.rarm ? "rarmSlim" : "rarm"));
	%obj.unHideNode((%client.larm ? "larmSlim" : "larm"));
	%obj.unHideNode("headskin");

	//Packs.
	%pack = $pack[%client.pack];
	%secondPack = $secondPack[%client.secondPack];
	if(%pack !$= "none")
	{
		%obj.unHideNode($pack[%client.pack]);
	}
	if(%secondPack !$= "none")
	{
		%obj.unHideNode($secondPack[%client.secondPack]);
	}
	//Extend the neck if the player has a pack on.
	if(%pack $= "none" && %secondPack $= "none")
	{
		%obj.setHeadUp(0);
	}
	else
	{
		%obj.setHeadUp(1);
	}

	//Mount the HatMod hat of the player if it is available, otherwise give them their default hat.
	%hat = $HatMod::save::wornHat[%client.bl_id];
	if(!%this.noHatmod && isFunction(isHat) && isHat(%hat))
	{
		%obj.mountHat(%hat);
	}
	else if(%client.hat)
	{	
		%hatName = $hat[%client.hat];
		%client.hatString = %hatName;
		
		//Only check if it's the first hat.
		if(%client.hat == 1)
		{
			%newhat = (%client.accent ? "helmet" : "hoodie1");
			%obj.unHideNode(%newhat);
		}
		else
		{
			%obj.unHideNode(%hatName);
		}			
	}
	
	//Legs.
	if(%client.hip) 
	{
		%obj.unHideNode("skirt");
	}
	else
	{
		%obj.unHideNode("pants");
		%obj.unHideNode(%client.rleg ? "rpeg" : "rshoe");
		%obj.unHideNode(%client.lleg ? "lpeg" : "lshoe");
	}

    //Give their torso a decal.
	%obj.setDecalName(%client.decalName);

	//Unhide blood nodes, where appropriate.
	if(%obj.bloody["lshoe"]) 
	{
		%obj.unHideNode("lshoe_blood");
	}
	if(%obj.bloody["rshoe"]) 
	{
		%obj.unHideNode("rshoe_blood");
	}
	if(%obj.bloody["lhand"]) 
	{
		%obj.unHideNode("lhand_blood");
	}
	if(%obj.bloody["rhand"]) 
	{
		%obj.unHideNode("rhand_blood");
	}
	if(%obj.bloody["chest_front"]) 
	{
		%obj.unHideNode((%client.chest ? "fem" : "") @ "chest_blood_front");
	}
	if(%obj.bloody["chest_back"])
	{
		%obj.unHideNode((%client.chest ? "fem" : "") @ "chest_blood_back");
	}

	//Optional face pack initialization.
	if(!isObject(%obj.faceConfig) && %this.facePack !$= "")
	{
		%obj.createFaceConfig($Eventide_FacePacks[%this.facePack]);
	}
}

function PlayerEventide::eventideBodyColors(%this, %obj)
{
    //We need a client to do anything here.
	%client = %obj.client;
	if(!isObject(%client))
	{
		%skinColor = "1 0.878 0.611 1";
		%sleeveColor = "0.9 0 0 1";
		%pantsColor = "0.2 0 0.8 1";

		%obj.setNodeColor("headskin", %skinColor);
		%obj.setNodeColor("chest", "1 1 1 1");
		%obj.setNodeColor("larm", %sleeveColor);
		%obj.setNodeColor("rarm", %sleeveColor);
		%obj.setNodeColor("lhand", %skinColor);
		%obj.setNodeColor("rhand", %skinColor);
		%obj.setNodeColor("pants", %pantsColor);
		%obj.setNodeColor("lshoe", %pantsColor);
		%obj.setNodeColor("rshoe", %pantsColor);
		return;
	}

    //Set the color of the player's hat.
    if(%client.hat !$= "none")
    {
        %obj.setNodeColor($hat[%client.hat], %client.hatColor);
    }

    //Set the color of the player's packs.
    %pack = $pack[%client.pack];
    if(%pack !$= "none")
	{
		%obj.setNodeColor(%pack, %client.packColor);
	}
    %secondPack = $secondPack[%client.secondPack];
	if(%secondPack !$= "none")
	{
		%obj.setNodeColor(%secondPack, %client.secondPackColor);
	}

    //Set the color of code body parts.
	%obj.setNodeColor("headskin", %client.headColor);	
	%obj.setNodeColor("chest", %client.chestColor);
	%obj.setNodeColor("femChest", %client.chestColor);
	%obj.setNodeColor("pants", %client.hipColor);
	%obj.setNodeColor("skirt", %client.hipColor);	
	%obj.setNodeColor("rarm", %client.rarmColor);
	%obj.setNodeColor("larm", %client.larmColor);
	%obj.setNodeColor("rarmSlim", %client.rarmColor);
	%obj.setNodeColor("larmSlim", %client.larmColor);
	%obj.setNodeColor("rhand", %client.rhandColor);
	%obj.setNodeColor("lhand", %client.lhandColor);
	%obj.setNodeColor("rhook", %client.rhandColor);
	%obj.setNodeColor("lhook", %client.lhandColor);	
	%obj.setNodeColor("rshoe", %client.rlegColor);
	%obj.setNodeColor("lshoe", %client.llegColor);
	%obj.setNodeColor("rpeg", %client.rlegColor);
	%obj.setNodeColor("lpeg", %client.llegColor);

	//Set the color of blood nodes.
	%obj.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("lhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("rhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_back", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_back", "0.7 0 0 1");
}

function PlayerEventide::tunnelVision(%this, %obj, %bool)
{
	%client = %obj.client;
	if(!isObject(%obj.client) || %obj.getState() $= "Dead") 
	{
		return;
	}

	//No tunnel vision set, so it must be 0.
	if(%obj.tunnelVision $= "")
	{
		%obj.tunnelVision = 0;
	}

	//Fade in the tunnel vision vignette.
	if(%bool) 
	{
		//The player already has complete tunnel vision, do no more.
		if(%obj.tunnelVision >= 1) 
		{
			return;
		}

		%obj.tunnelVision = mClampF(%obj.tunnelVision + 0.1, 0, 1);
		commandToClient(%obj.client, 'SetVignette', true, "0 0 0" SPC %obj.tunnelVision);
	}
	//Reverse the tunnel vision vignette.
	else
	{
		if(%obj.tunnelVision > 0)
		{
			%obj.tunnelVision = mClampF(%obj.tunnelVision - 0.1, 0, 1);
		    commandToClient(%client, 'SetVignette', true, "0 0 0" SPC %obj.tunnelVision);
		}
		//Once fully removed, restore the environment vignette settings.
		else
		{
			commandToClient(%client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor);		
			return;
		}
	}

	%obj.tunnelVisionSchedule = %this.schedule(50, "tunnelVision", %obj, %bool);	
}

function PlayerEventide::dropAllTools(%this, %obj)
{
	%minigame = getMinigamefromObject(%obj);
	if(!isObject(%obj) || !isObject(%minigame))
	{
		return;
	}

	//Make the player visually drop whatever they're holding.
	%obj.unmountImage(0);
	
	%client = isObject(%obj.client) ? %obj.client : "";
	for(%i = 0; %i < %this.maxTools; %i++)
	{
		%itemDatablock = %obj.tool[%i];
		if(!isObject(%itemDatablock))
		{
			continue;
		}

		//Spawn the item at the player's position.
		%item = new Item()
		{
			dataBlock = %itemDatablock;
			position = %obj.getHackPosition();	
			BL_ID = %client.BL_ID;
			minigame = %minigame;
		};

		if(!isObject(Eventide_MinigameGroup)) 
		{
			MissionCleanup.add(new SimGroup(Eventide_MinigameGroup));
		}
		Eventide_MinigameGroup.add(%item); //Add the item to the minigame group for cleanup when the minigame ends or restarts.

		//Toss the item in a random direction, relative to the player's current velocity.
		%item.setVelocity(VectorAdd(%obj.getVelocity(), getRandom(-4,4) SPC getRandom(-4,4) SPC getRandom(4,8)));
		
		//Clear the item from the player's inventory. Usually redundant, as the player is dying anyway.
		%obj.tool[%i] = 0;
		if(%client !$= "")
		{			
			messageClient(%client, 'MsgItemPickup', '', %i, 0);
		}
	}
}

function PlayerEventide::SetTempSpeed(%this, %obj, %speedMultiplier)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	if(%speedMultiplier $= "")
	{
		%speedMultiplier = (%obj.defaultSpeed $= "") ? 1.0 : %obj.defaultSpeed;
	}
	
	//Standing speed.
	%obj.setMaxForwardSpeed(%this.MaxForwardSpeed * %speedMultiplier);
	%obj.setMaxSideSpeed(%this.MaxSideSpeed * %speedMultiplier);
	%obj.setMaxBackwardSpeed(%this.maxBackwardSpeed * %speedMultiplier);

	//Crouching speed.
	%obj.setMaxCrouchForwardSpeed(%this.maxForwardCrouchSpeed * %speedMultiplier);
  	%obj.setMaxCrouchBackwardSpeed(%this.maxSideCrouchSpeed * %speedMultiplier);
  	%obj.setMaxCrouchSideSpeed(%this.maxSideCrouchSpeed * %speedMultiplier);

  	//Swimming speed.
 	%obj.setMaxUnderwaterBackwardSpeed(%this.MaxUnderwaterBackwardSpeed * %speedMultiplier);
  	%obj.setMaxUnderwaterForwardSpeed(%this.MaxUnderwaterForwardSpeed * %speedMultiplier);
  	%obj.setMaxUnderwaterSideSpeed(%this.MaxUnderwaterForwardSpeed * %speedMultiplier);	
}

//
// Some stub functions that both survivors and killers use, but don't have shared functionality.
//

function PlayerEventide::onKillerEnterRange(%this, %obj, %target)
{

}

function PlayerEventide::onKillerExitRange(%this, %obj, %target)
{
	
}

function PlayerEventide::onKillerChaseStart(%this, %obj, %target)
{

}

function PlayerEventide::onKillerChaseEnd(%this, %obj, %target)
{
	
}

function PlayerEventide::onKillerChase(%this, %obj)
{

}

function PlayerEventide::onRoundEnd(%this, %obj, %won)
{

}

//
// Package to make the `eventideBodyParts` and `eventideBodyColors` functions get called correctly.
//

package Player_Eventide
{
    function GameConnection::applyBodyParts(%client)
    {
        %player = %client.player;
        if(isObject(%player))
        {
            %playerDatablock = %player.getDatablock();
            if(%playerDatablock.isEventideClass)
            {
                %playerDatablock.eventideBodyParts(%player);
                return;
            }
        }

        return Parent::applyBodyParts(%client);
    }

    function GameConnection::applyBodyColors(%client)
    {
        %player = %client.player;
        if(isObject(%player))
        {
            %playerDatablock = %player.getDatablock();
            if(%playerDatablock.isEventideClass)
            {
                %playerDatablock.eventideBodyColors(%player);
                return;
            }
        }

        return Parent::applyBodyColors(%client);
    }

	//Correction for corpses sometimes standing up after dying. Clear all animation slots.
	function Player::playDeathAnimation(%this)
	{
		%this.setArmThread("root");
		%this.playThread(3, "Death1");
		%this.playThread(2, "Death1");
		%this.playThread(1, "Death1");
		%this.playThread(0, "Death1");
	}
};
if(isPackage(Player_Eventide))
{
    deactivatePackage(Player_Eventide);
}
activatePackage(Player_Eventide);