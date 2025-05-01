//findClientByName("Robbin").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("Staller"));

//
// Resources that must go in this file.
//

//
/// Class hats.

datablock ShapeBaseImageData(menderMaskImage) 
{
	shapeFile = "Add-Ons/Gamemode_Eventide/modules/players/models/SurgicalMask.dts";
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 -1000";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
	emap = 0;
};

datablock ShapeBaseImageData(stallerHoodImage)
{
	shapeFile = "Add-Ons/Gamemode_Eventide/modules/players/models/grimhood.dts";
	mountPoint = $HeadSlot;

	eyeOffset = "0 0 -1000";
	emap = 0;
	
	doColorShift = true;
	colorShiftColor = "0.1 0.1 0.1 1";
};

//
/// Class playertypes.

function EventidePlayer::StallerCallback(%this, %obj)
{
	%obj.noFootsteps = true;
	%obj.playerClass = %obj.client.playerClass;
}

function EventidePlayer::onTrigger(%this, %obj, %trig, %press)
{
	Parent::onTrigger(%this, %obj, %trig, %press);

	if(%obj.playerClass !$= "" && %obj.playerClass.title $= "Staller")
	{
		if(%trig == 3 && %press)
		{
			if(%obj.getEnergyLevel() >= 100 && (getSimTime() - %obj.lastFadeTime) > 5000 && !%obj.getDataBlock().isDowned)
			{
				%this.StallerFadeOut(%obj, 1);
			}
		}
		else if(%trig == 3 && !%press)
		{
			if(%obj.isInvisible)
			{
				%this.StallerFadeIn(%obj, 0);
			}
		}
	}
}

function EventidePlayer::StallerFadeOut(%this, %obj)
{
	//Delete the player's flashlight, if they have it enabled.
	if(isObject(%obj.light))
	{
		%obj.light.delete();
		%obj.deleteFlashlightBeam();
	}
	%obj.flashlightDisabled = true;

	%obj.playaudio(1, "staller_cloak_sound");

	//Turn the player invisible by hiding all their nodes. Including the custom hat.
	%obj.hideNode("ALL");
	%obj.unmountImage(3);

	//Set some internal flags so the hidden nodes do not get revealed. Also ensure the killer loop cannot see the invisible person.
	%obj.dontChangeAppearance = true;
	%obj.isInvisible = true;
	%obj.lastFadeTime = getSimTime();

	//Unequip the player's items, don't let them use them while invisible.
	if(isObject(%obj.client))
	{
		serverCmdUnuseTool(%obj.client);
	}

	//Start a loop that detects when the player runs out of energy, and disabled their invisibility when it does.
	%this.invisibilityTick(%obj);
}

function EventidePlayer::invisibilityTick(%this, %obj)
{
	if(!isObject(%obj))
	{
		return;
	}

	//Not needed in theory, but just in case...
	cancel(%obj.invisibilitySched);

	//The player ran out of energy, reveal them.
	if(%obj.getEnergyLevel() <= 1)
	{
		%this.StallerFadeIn(%obj);
		return;
	}
	else
	{
		//The player still has energy left, decrease it.
		%obj.setEnergyLevel(%obj.getEnergyLevel() - (0.3125 + %this.rechargeRate));
	}

	//Plan to check and decrease energy again in a little bit.
	%obj.invisibilitySched = %this.schedule(31, invisibilityTick, %obj);
}

function EventidePlayer::StallerFadeIn(%this, %obj)
{
	if(!isObject(%obj))
	{
		return;
	}

	//Not needed in theory, but just in case...
	cancel(%obj.invisibilitySched);

	%obj.playaudio(1, "staller_uncloak_sound");

	//Reset the flags dedicated to invisibility.
	%obj.isInvisible = false;
	%obj.dontChangeAppearance = false;

	//Re-enable the player's flashlight.
	%obj.flashlightDisabled = false;

	//Restore the Staller appearance.
	%obj.mountImage(stallerHoodImage, 3);
	%this.EventideAppearance(%obj, %obj.client);	
}

//
/// Class packages/overrides.

package Gamemode_Eventide_Player_Staller
{
    function ServerCmdStartTalking(%client)
	{
		if(%client.playerClass !$= "" && %client.playerClass.title $= "Staller")
		{
			return;
		}
		Parent::ServerCmdStartTalking(%client);
	}

    function serverCmdMessageSent(%client, %message)
	{
        if(%client.playerClass !$= "" && %client.playerClass.title $= "Staller")
		{
			%client.centerPrint("<color:ffffff>...", 3);
			return;
		}
		Parent::serverCmdMessageSent(%client, %message);
	}

	function ServerCmdTeamMessageSent(%client, %message)
	{
		if(isObject(%client.playerClass) && %client.playerClass.title $= "Staller")
		{
			%client.centerPrint("<color:ffffff>...", 3);
			return;
		}
		Parent::ServerCmdTeamMessageSent(%client, %message);
	}

	function Player::emote(%player, %data, %skipSpam)
	{
		%client = %player.client;
		if(isObject(%player.playerClass) && %player.playerClass.title $= "Staller")
		{
			%client.centerPrint("<color:ffffff>...", 3);
			return;
		}
		Parent::emote(%player, %data, %skipSpam);
	}

	function ServerCmdUseTool(%client, %slot)
    {
        if(%client.playerClass !$= "" && %client.playerClass.title $= "Staller" && isObject(%client.player) && %client.player.isInvisible)
		{
			return;
		}
		parent::ServerCmdUseTool(%client, %slot);
    }
};
if(isPackage("Gamemode_Eventide_Player_Staller"))
{
	deactivatePackage("Gamemode_Eventide_Player_Staller");
}
activatePackage("Gamemode_Eventide_Player_Staller");

package Gamemode_Eventide_Server_Hatmod
{
	function serverCmdHat(%client, %na, %nb, %nc, %nd, %ne)
	{
		if(%client.playerClass !$= "")
		{
			return;
		}
		parent::serverCmdHat(%client, %na, %nb, %nc, %nd, %ne);
	}
};
if(isPackage("Gamemode_Eventide_Server_Hatmod"))
{
	deactivatePackage("Gamemode_Eventide_Server_Hatmod");
}
activatePackage("Gamemode_Eventide_Server_Hatmod");

//
// Miscellaneous functions.
//

function cloneScriptGroup(%targetObject)
{
    %targetObjectName = %targetObject.getName();
    %targetObject.setName("targetScriptGroup");

    %cloneObject = new ScriptGroup(cloneScriptGroup : targetScriptGroup);

    %targetObject.setName(%targetObjectName);
    %cloneObject.setName("");

    return %cloneObject;
}

// Templates, containers for pre-made groups of classes.
///

function EventideClassGroupTemplates::onAdd(%this)
{
	%this.index["Classic"] = new ScriptGroup()
	{
		class = "EventideClassGroup";
		template = "Classic";
		doNotAutofill = true;

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Mender";
			canStack = false;
			spawnMessage = "You acquired a medical item and can revive survivors faster!";
		};

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Runner";
			canStack = false;
			spawnMessage = "You acquired a soda and can run slightly faster!";
		};

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Hoarder";
			maxItems = 5;
			canStack = false;
			spawnMessage = "You acquired a camera and have five item slots!";
		};

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Fighter";
			pseudoHealth = 75;
			canStack = false;
			spawnMessage = "You acquired a pool cue, can shove further and can take 1 hit before getting damaged!";
		};

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Tinkerer";
			canStack = false;
			spawnMessage = "You acquired a monkey wrench and stungun. Use the wrench to repair generators faster!";
		};

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Staller";
			canStack = false;
			clearNodes = true;
			callback = "StallerCallback";
			spawnMessage = "You are deathly quiet and you can crouch to become invisible! Only lasts 10 seconds.";
		};
	};

	//Can't store datablocks directly in a ScriptGroup. How inconvenient.
	%menderClass = %this.index["Classic"].getClass("Mender");
	%menderClass.appearance.facePack["female"] = $Eventide_FacePacks["menderF"];
	%menderClass.appearance.facePack["male"] = $Eventide_FacePacks["menderM"];
	%menderClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomNode";
		targetSlot = $HeadSlot;
		mountableObject = menderMaskImage;
	});
	%menderClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = (getRandom(0, 1) ? GauzeItem.getID() : ZombieMedpackItem.getID());
	});

	%runnerClass = %this.index["Classic"].getClass("Runner");
	%runnerClass.appearance.facePack["female"] = $Eventide_FacePacks["RunnerF"];
	%runnerClass.appearance.facePack["male"] = $Eventide_FacePacks["RunnerM"];
	%runnerClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = SodaItem.getID();
	});

	%hoarderClass = %this.index["Classic"].getClass("Hoarder");
	%hoarderClass.appearance.facePack["female"] = $Eventide_FacePacks["female"];
	%hoarderClass.appearance.facePack["male"] = $Eventide_FacePacks["male"];
	%hoarderClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = DCamera.getID();
	});

	%fighterClass = %this.index["Classic"].getClass("Fighter");
	%fighterClass.appearance.facePack["female"] = $Eventide_FacePacks["fighterF"];
	%fighterClass.appearance.facePack["male"] = $Eventide_FacePacks["fighterM"];
	%fighterClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = sm_poolCueItem.getID();
	});

	%tinkererClass = %this.index["Classic"].getClass("Tinkerer");
	%tinkererClass.appearance.facePack["female"] = $Eventide_FacePacks["tinkererF"];
	%tinkererClass.appearance.facePack["male"] = $Eventide_FacePacks["tinkererM"];
	%tinkererClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = StunGun.getID();
	});
	%tinkererClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = MonkeyWrench.getID();
	});

	%stallerClass = %this.index["Classic"].getClass("Staller");
	%stallerClass.appearance.facePack["female"] = 0;
	%stallerClass.appearance.facePack["male"] = 0;
	%stallerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomNode";
		mountableObject = stallerHoodImage;
		targetSlot = 3;
	});
	%stallerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "headSkin";
		nodeColor = "0 0 0 1";
	});
	%stallerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "chest";
		nodeColor = "0.1 0.1 0.1 1";
	});
	%stallerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "LArm";
		nodeColor = "0.1 0.1 0.1 1";
	});
	%stallerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "RArm";
		nodeColor = "0.1 0.1 0.1 1";
	});
	%stallerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "LHand";
		nodeColor = "0.5 0.5 0.5 1";
	});
	%stallerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "RHand";
		nodeColor = "0.5 0.5 0.5 1";
	});
	%stallerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "skirt";
		nodeColor = "0.1 0.1 0.1 1";
	});
	%stallerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "AAA-None";
	});
}

//Using an internal array to fetch templates is faster than iteration via a for or while loop.
function EventideClassGroupTemplates::addTemplate(%this, %classGroup)
{
	%this.index[%classGroup.name] = %classGroup;
}

function EventideClassGroupTemplates::getTemplate(%this, %classGroupName)
{
	return %this.index[%classGroupName];
}

function EventideClassGroupTemplates::cloneTemplate(%this, %classGroupName)
{
    return cloneScriptGroup(%this.getTemplate(%classGroupName));
}

// Container for classes.
///

function EventideClassGroup::onAdd(%this)
{
	//Nothing but a template was given, auto-fill the data from an existing template if possible.
	if(%this.template && !%this.doNotAutofill)
	{
		%existingTemplate = $Eventide_ClassGroupTemplates.getTemplate(%this.template);
		if(%existingTemplate)
		{
			%this = $Eventide_ClassGroupTemplates.cloneTemplate(%this.template);
		}
	}
}

function EventideClassGroup::getClass(%this, %className)
{
	for(%i = 0; %i < %this.getCount(); %i++)
	{
		%classObject = %this.getObject(%i);
		if(strlwr(%className) $= strlwr(%classObject.title))
		{
			return %classObject;
		}
	}

	return 0;
}

// Containers for class information.
///

function EventidePlayerClass::onAdd(%this)
{
	if(!%this.items)
	{
		%this.items = new ScriptGroup();
	}
	else
	{
		//Cycle through the item set to make sure they're all valid. Delete any items that are invalid.
		for(%i = 0; %i < %this.items.getCount(); %i++)
		{
			%itemContainer = %this.getObject(%i);
			if(!%itemContainer.itemData || %itemContainer.itemData.getClassName() !$= "ItemData")
			{
				%itemContainer.delete();
			}
		}
	}

    if(%this.maxItems $= "")
    {
        %this.maxItems = 3;
    }
	
	if(%this.canStack $= "")
	{
		%this.canStack = false;
	}

    if(%this.pseudoHealth $= "")
    {
        %this.pseudoHealth = 0;
    }
	
	if(%this.title $= "")
	{
		%this.title = "Specialist";
	}

    if(!%this.appearance)
    {
        %this.appearance = new ScriptGroup()
        {
            class = "EventideClassAppearance";
        };
    }

	if(%this.clearNodes $= "")
    {
		%this.clearNodes = false;
    }
}

//
/// Containers for class node modifications.

function EventideClassNodeColor::onAdd(%this)
{
	if(%this.targetNode $= "")
	{
		%this.targetNode = "chest";
	}

	if(%this.nodeColor $= "")
	{
		%this.nodeColor = "0 0 0 1";
	}

	if(%this.nodeVisible $= "")
	{
		%this.nodeVisible = true;
	}
}
function EventideClassNodeColor::apply(%this, %obj)
{
	if(!%this.nodeVisible)
	{
		%obj.hideNode(%this.targetNode);
	}
	else
	{
		%obj.setNodeColor(%this.targetNode, %this.nodeColor);
		%obj.unhideNode(%this.targetNode);
	}
}

function EventideClassCustomNode::onAdd(%this)
{
	if(%this.targetSlot $= "") 
	{
		%this.targetSlot = 2;
	}

	if(%this.mountableObject $= "") 
	{
		%this.mountableObject = 0;
	}
}
function EventideClassCustomNode::apply(%this, %obj)
{
	%obj.mountImage(%this.mountableObject, %this.targetSlot);
}

function EventideClassCustomDecal::onAdd(%this)
{
	if(%this.decalName $= "")
	{
		%this.decalName = "AAA-None";
	}
}
function EventideClassCustomDecal::apply(%this, %obj)
{
	if(%this.decalName !$= "")
	{
		%obj.setDecalName($this.decalName);
	}
}

//
// Assigning a class to player.
//

function Player::assignClass(%player, %eventidePlayerClass)
{
    //We can't give the player the class items otherwise.
    %client = %player.client;
    if(!%client)
    {
        return;
    }

    //If the class doesn't exist, or isn't a class, we can stop right here.
    if(!isObject(%eventidePlayerClass) || %eventidePlayerClass.class !$= "EventidePlayerClass")
    {
        return;
    }

	if(%eventidePlayerClass.customDatablock !$= "" && %eventidePlayerClass.customDatablock != %player.getDataBlock())
	{
		%player.setDatablock(%eventidePlayerClass.customDatablock);
	}

	%player.playerClass = %eventidePlayerClass;

	//Apply the custom appearance of the class.
	%client.playerClass = %eventidePlayerClass;
	%playerDatablock = %player.getDataBlock();
	%playerDatablock.EventideAppearance(%player, %client);

    //Set the player's psuedohealth, if applicable.
    %player.psuedohealth = %eventidePlayerClass.psuedoHealth;

    //Give the player the class items.
    %items = %eventidePlayerClass.items;
    for(%i = 0; %i < %items.getCount(); %i++)
    {
        %itemDatablock = %items.getObject(%i).itemData;

        %player.tool[%i] = %itemDatablock;
        messageClient(%client, 'MsgItemPickup', '', %i, %itemDatablock);
    }

    //Fix for the hoarder class: communicate to the client if the innventory size is bigger than 3 items.
    %maxItems = %eventidePlayerClass.maxItems;
    if(%maxItems > 3)
    {
        commandToClient(%client, 'PlayGui_CreateToolHud', %maxItems);
		%player.hoarderToolCount = %maxItems;
    }

    //Inform the player what class they got selected for.
    %client.centerprint("<font:impact:40><color:FFFF00>Class: " @ %eventidePlayerClass.title @ "<br>" @ %eventidePlayerClass.spawnMessage, 4);

	//Perform any custom actions the class may have.
	if(%eventidePlayerClass.callback !$= "")
	{
		%playerDatablock.call(%eventidePlayerClass.callback, %player);
	}
}

//
// Assigning all survivors in a Slayer minigame a class.
//

function MiniGameSO::assignSurvivorClasses(%minigame)
{	
	// Return if there are no teams
	if(!%minigame.isSlayerMinigame || !%minigame.teams.getCount())
	{
		return;
	}

	// Loop through each team and add the team members to the temporary simset
	%survivorTeam = %minigame.teams.getTeamFromName("Survivors");
	if(!isObject(%survivorTeam))
	{
		return;
	}

	//Creates a clone of the default class list.
	if(isObject($Eventide_CurrentClassGroup))
	{
		$Eventide_CurrentClassGroup.delete();
	}

	%currentMode = $Eventide_ClassGroupTemplates.index["Classic"];

	%unpickedClasses = new SimSet();
	for(%i = 0; %i < %currentMode.getCount(); %i++)
	{
		%unpickedClasses.add(%currentMode.getObject(%i));
	}

	//Assign a class to each player.
	for(%i = 0; %i < %survivorTeam.numMembers; %i ++)
	{
		%client = %survivorTeam.member[%i];
		%player = %client.player;

		if(!isObject(%player))
		{
			continue;
		}

		//No more classes are left to be picked, so just make sure the player has an unassuming face pack.
		if(%unpickedClasses.getCount() == 0)
		{
			%client.customFacePack = "";
			%player.createFaceConfig((%client.chest ? $Eventide_FacePacks["female"] : $Eventide_FacePacks["male"]));
			continue;
		}

		%classSelectionIndex = getRandom(0, mClamp(%unpickedClasses.getCount()-1, 0, %unpickedClasses.getCount()));
		%class = %unpickedClasses.getObject(%classSelectionIndex); //Choose a class.

		%player.getDataBlock().assignClass(%player, %class); //Give the player the class.
		//TODO: Legacy class system support. Need to clean all that up eventually.
		%player.survivorclass = strlwr(%class.title);

		//Delete the class if it cannot be stacked.
		if(!%class.canStack)
		{
			%unpickedClasses.remove(%class);
		}
	}

	//Clean up the SimSet we no longer need.
	%unpickedClasses.delete();
}

//
// Instantiate everything here, we get class errors otherwise.
//

if(isObject($Eventide_ClassGroupTemplates))
{
	$Eventide_ClassGroupTemplates.delete();
}
$Eventide_ClassGroupTemplates = new ScriptObject(Eventide_ClassGroupTemplates) {class="EventideClassGroupTemplates";};

function destroySaturn()
{
	%foundObjects = 0;
	for(%i = 0; %i < MissionCleanup.getCount(); %i++)
	{
		%object = MissionCleanup.getObject(%i);
		if(%object.class $= "EventideClassGroupTemplates")
		{
			%object.delete();
			%foundObjects += 1;
		}
		else if(%object.class $= "EventidePlayerClass")
		{
			%object.delete();
			%foundObjects += 1;
		}
		else if(%object.class $= "EventideClassAppearance")
		{
			%object.delete();
			%foundObjects += 1;
		}
		else if(%object.class $= "EventideClassItem")
		{
			%object.delete();
			%foundObjects += 1;
		}
	}
	talk("Deleted" SPC %foundObjects SPC "unneeded objects.");
}

function communicateLaunchCodes()
{
	%foundObjects = 0;
	for(%i = 0; %i < MissionCleanup.getCount(); %i++)
	{
		%object = MissionCleanup.getObject(%i);
		if(%object.class $= "EventideClassGroupTemplates")
		{
			%foundObjects += 1;
		}
		else if(%object.class $= "EventidePlayerClass")
		{
			%foundObjects += 1;
		}
		else if(%object.class $= "EventideClassAppearance")
		{
			%foundObjects += 1;
		}
		else if(%object.class $= "EventideClassItem")
		{
			%foundObjects += 1;
		}
	}
	talk(%foundObjects SPC "found objects.");
}