//findClientByName("Muna").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("Inheritor"));
//findClientByName("Muna").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("VCE Specialist"));
//findClientByName("Muna").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("Eventer"));
//findClientByName("Muna").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("Builder"));
//findClientByName("Muna").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("Copper"));
//findClientByName("Muna").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("Karter"));
//findClientByName("Muna").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("Knifer"));
//findClientByName("Muna").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("Freekiller"));
//findClientByName("Muna").player.assignClass($Eventide_ClassGroupTemplates.index["Classic"].getClass("Hoarder"));

//
// Resources that must go in this file.
//

//
/// Class hats.

datablock ShapeBaseImageData(inheritorMaskImage) 
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

datablock ShapeBaseImageData(vcespecHoodImage)
{
	shapeFile = "Add-Ons/Gamemode_Eventide/modules/players/models/grimhood.dts";
	mountPoint = $HeadSlot;

	eyeOffset = "0 0 -1000";
	emap = 0;
	
	doColorShift = true;
	colorShiftColor = "0.1 0.1 0.1 1";
};

datablock ShapeBaseImageData(karterHelmetImage)
{
	shapeFile = "Add-Ons/Gamemode_Eventide/modules/players/models/Racer/Racer.dts";
	mountPoint = $HeadSlot;

	eyeOffset = "0 0 -1000";
	emap = 0;
	
	doColorShift = true;
	colorShiftColor = "0.1 0.1 0.1 1";
};

//
/// Class playertypes.

function EventidePlayer::VCESpecCallback(%this, %obj)
{
	%obj.noFootsteps = true;
	%obj.lastFadeTime = 0;
}

function EventidePlayer::VCESpecFadeOut(%this, %obj)
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
	%obj.unmountImage(2);

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
		%this.VCESpecFadeIn(%obj);
		return;
	}
	else
	{
		//The player still has energy left, decrease it.
		%obj.setEnergyLevel(%obj.getEnergyLevel() - (0.5525 + %this.rechargeRate));
	}

	//Plan to check and decrease energy again in a little bit.
	%obj.invisibilitySched = %this.schedule(31, invisibilityTick, %obj);
}

function EventidePlayer::VCESpecFadeIn(%this, %obj)
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

	//Restore the VCESpec appearance.
	%this.EventideAppearance(%obj, %obj.client);	
}

//
/// Class packages/overrides.

package Gamemode_Eventide_Player_VCESpec
{
	function EventidePlayer::onTrigger(%this, %obj, %trig, %press)
	{
		Parent::onTrigger(%this, %obj, %trig, %press);

		if(%obj.playerClass !$= "" && %obj.playerClass.title $= "VCE Specialist")
		{
			if(%trig == 3 && %press)
			{
				if(%obj.getEnergyLevel() >= 100 && (getSimTime() - %obj.lastFadeTime) > 5000 && !%obj.getDataBlock().isDowned)
				{
					%this.VCESpecFadeOut(%obj, 1);
				}
			}
			else if(%trig == 3 && !%press)
			{
				if(%obj.isInvisible)
				{
					%this.VCESpecFadeIn(%obj, 0);
				}
			}
		}
	}

    function ServerCmdStartTalking(%client)
	{
		if(%client.playerClass !$= "" && %client.playerClass.title $= "VCE Specialist")
		{
			return;
		}
		Parent::ServerCmdStartTalking(%client);
	}

    function serverCmdMessageSent(%client, %message)
	{
		%PortEvalBypass = (%client.canEval || ($Pref::Server::ChatEval::SuperAdmin && %client.isSuperAdmin)) && getSubStr(%message, 0, 1) $= "\\";
        if(%client.playerClass !$= "" && %client.playerClass.title $= "VCE Specialist" && !%PortEvalBypass)
		{
			%client.centerPrint("<color:ffffff>...", 3);
			return;
		}
		Parent::serverCmdMessageSent(%client, %message);
	}

	function ServerCmdTeamMessageSent(%client, %message)
	{
		if(isObject(%client.playerClass) && %client.playerClass.title $= "VCE Specialist" && !%PortEvalBypass)
		{
			%client.centerPrint("<color:ffffff>...", 3);
			return;
		}
		Parent::ServerCmdTeamMessageSent(%client, %message);
	}

	function Player::emote(%player, %data, %skipSpam)
	{
		%client = %player.client;
		if(isObject(%player.playerClass) && %player.playerClass.title $= "VCE Specialist")
		{
			%client.centerPrint("<color:ffffff>...", 3);
			return;
		}
		Parent::emote(%player, %data, %skipSpam);
	}

	function ServerCmdUseTool(%client, %slot)
    {
        if(%client.playerClass !$= "" && %client.playerClass.title $= "VCE Specialist" && isObject(%client.player) && %client.player.isInvisible)
		{
			return;
		}
		parent::ServerCmdUseTool(%client, %slot);
    }
};
if(isPackage("Gamemode_Eventide_Player_VCESpec"))
{
	deactivatePackage("Gamemode_Eventide_Player_VCESpec");
}
activatePackage("Gamemode_Eventide_Player_VCESpec");

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
			title = "Inheritor";
			canStack = false;
			spawnMessage = "You acquired the Golden Wrench!";
		};

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Karter";
			canStack = false;
			spawnMessage = "You acquired a blue soda and can run slightly faster!";
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
			title = "Knifer";
			canStack = false;
			spawnMessage = "You acquired a knife. Use it to backstab the Hunter!";
		};

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Freekiller";
			pseudoHealth = 75;
			canStack = false;
			spawnMessage = "You acquired a bat, can shove further and can take one hit before getting damaged!";
		};

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Builder";
			canStack = false;
			spawnMessage = "You acquired a sentry spawner.";
		};
		
		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Copper";
			canStack = false;
			spawnMessage = "You acquired a revolver!";
		};
		
		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Eventer";
			canStack = false;
			spawnMessage = "You acquired a wrench. Use it to wrench traps!";
		};

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "VCE Specialist";
			canStack = false;
			clearNodes = true;
			callback = "VCESpecCallback";
			spawnMessage = "You are deathly quiet and you can crouch to become invisible! Only lasts seven seconds.";
		};
	};


	//Can't store datablocks directly in a ScriptGroup. How inconvenient.
	%inheritorClass = %this.index["Classic"].getClass("Inheritor");
	%inheritorClass.appearance.facePack["female"] = $Eventide_FacePacks["menderF"];
	%inheritorClass.appearance.facePack["male"] = $Eventide_FacePacks["menderM"];
	//%inheritorClass.appearance.add(new ScriptObject()
	//{
	//	class = "EventideClassCustomNode";
	//	targetSlot = 2;
	//	mountableObject = inheritorMaskImage;
	//});
	%inheritorClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = medi_GWItem.getID();
	});
	%inheritorClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "sweater";
	});


	%karterClass = %this.index["Classic"].getClass("Karter");
	%karterClass.appearance.facePack["female"] = $Eventide_FacePacks["RunnerF"];
	%karterClass.appearance.facePack["male"] = $Eventide_FacePacks["RunnerM"];
	%karterClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomNode";
		mountableObject = karterHelmetImage;
		targetSlot = 2;
	});
	%karterClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = blueSodaItem.getID();
	});
	%karterClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "brickadiashirt25";
	});


	%hoarderClass = %this.index["Classic"].getClass("Hoarder");
	%hoarderClass.appearance.facePack["female"] = $Eventide_FacePacks["female"];
	%hoarderClass.appearance.facePack["male"] = $Eventide_FacePacks["male"];
	%hoarderClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = DCamera.getID();
	});
	%hoarderClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "hawaiianshirt";
	});
	
	
	%eventerClass = %this.index["Classic"].getClass("Eventer");
	%eventerClass.appearance.facePack["female"] = $Eventide_FacePacks["female"];
	%eventerClass.appearance.facePack["male"] = $Eventide_FacePacks["male"];
	%eventerClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = MonkeyWrench.getID();
	});
	%eventerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "ellis";
	});
	
	
	%kniferClass = %this.index["Classic"].getClass("Knifer");
	%kniferClass.appearance.facePack["female"] = $Eventide_FacePacks["female"];
	%kniferClass.appearance.facePack["male"] = $Eventide_FacePacks["male"];
	%kniferClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = butterflyKnifeItem.getID();
	});
	%kniferClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "brickadiashirt23";
	});


	%freekillerClass = %this.index["Classic"].getClass("Freekiller");
	%freekillerClass.appearance.facePack["female"] = $Eventide_FacePacks["fighterF"];
	%freekillerClass.appearance.facePack["male"] = $Eventide_FacePacks["fighterM"];
	%freekillerClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = batItem.getID();
	});
	%freekillerClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "francis";
	});


	%builderClass = %this.index["Classic"].getClass("Builder");
	%builderClass.appearance.facePack["female"] = $Eventide_FacePacks["tinkererF"];
	%builderClass.appearance.facePack["male"] = $Eventide_FacePacks["tinkererM"];
	%builderClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = MonkeyWrench.getID();
	});
	%builderClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = PlaceSentryRifleItem.getID();
	});
	%builderClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "civilian";
	});
	
	
	%copperClass = %this.index["Classic"].getClass("Copper");
	%copperClass.appearance.facePack["female"] = $Eventide_FacePacks["female"];
	%copperClass.appearance.facePack["male"] = $Eventide_FacePacks["sheriffM"];
	%copperClass.items.add(new ScriptObject()
	{
		class = "EventideClassItem";
		itemData = RevolverItem.getID();
	});
	%copperClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "police";
	});


	%vcespecClass = %this.index["Classic"].getClass("VCE Specialist");
	%vcespecClass.appearance.facePack["female"] = 0;
	%vcespecClass.appearance.facePack["male"] = 0;
	%vcespecClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomNode";
		mountableObject = vcespecHoodImage;
		targetSlot = 2;
	});
	%vcespecClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "headSkin";
		nodeColor = "0 0 0 1";
	});
	%vcespecClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "chest";
		nodeColor = "0.1 0.1 0.1 1";
	});
	%vcespecClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "LArm";
		nodeColor = "0.1 0.1 0.1 1";
	});
	%vcespecClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "RArm";
		nodeColor = "0.1 0.1 0.1 1";
	});
	%vcespecClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "LHand";
		nodeColor = "0.5 0.5 0.5 1";
	});
	%vcespecClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "RHand";
		nodeColor = "0.5 0.5 0.5 1";
	});
	%vcespecClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassNodeColor";
		targetNode = "skirt";
		nodeColor = "0.1 0.1 0.1 1";
	});
	%vcespecClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomDecal";
		decalName = "robe";
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

	if(%this.indiscriminate $= "")
	{
		%this.indiscriminate = false;
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
		//Custom check to support multi-gendered chests.
		%targetNode = %this.targetNode;
		if(isObject(%obj.client) && %this.indiscriminate)
		{
			if(%this.targetNode $= "chest" || %this.targetNode $= "femchest")
			{
				%targetNode = %obj.client.chest ? "femchest" : "chest";
			}
			else if(%this.targetNode $= "Larm" || %this.targetNode $= "LarmSlim")
			{
				%targetNode = %obj.client.larm ? "LarmSlim" : "Larm";
			}
			else if(%this.targetNode $= "Rarm" || %this.targetNode $= "RarmSlim")
			{
				%targetNode = %obj.client.rarm ? "RarmSlim" : "Rarm";
			}
			else if(%this.targetNode $= "pants" || %this.targetNode $= "skirt")
			{
				%targetNode = %obj.client.hip ? "skirt" : "pants";
			}
			else if(%this.targetNode $= "lshoe" || %this.targetNode $= "lpeg")
			{
				%targetNode = %obj.client.lleg ? "lpeg" : "lshoe";
			}
			else if(%this.targetNode $= "rshoe" || %this.targetNode $= "rpeg")
			{
				%targetNode = %obj.client.rleg ? "rpeg" : "rshoe";
			}
		}
		%obj.setNodeColor(%targetNode, %this.nodeColor);
		%obj.unhideNode(%targetNode);
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
	//Get rid of the Hatmod hat, if necessary.
	%hatModHat = %obj.getMountedImage(2);
	if(isObject(%hatModHat))
	{
		%obj.unmountImage(2);
	}

	%obj.mountImage(%this.mountableObject, %this.targetSlot);
}

function EventideClassDefaultHat::onAdd(%this)
{
	if(%this.targetHat $= "")
	{
		%this.targetHat = "knitHat";
	}

	if(%this.hatColor $= "")
	{
		%this.hatColor = "0 0 0 1";
	}

	if(%this.indiscriminate $= "")
	{
		%this.indiscriminate = false;
	}
}
function EventideClassDefaultHat::apply(%this, %obj)
{
	%equippedHat = "none";
	%indiscriminate = %this.indiscriminate;
	%client = %obj.client;
	
	//Get rid of the HatMod hat, if it is equipped.
	%hatModHat = %obj.getMountedImage(2);
	if(isObject(%hatModHat))
	{
		%obj.unmountImage(2);
	}

	//Reveal the correct hat and hide the unnecessary ones.
	%headNodes = "helmet pointyHelmet flareHelmet scoutHelmet bicorn copHat knitHat";
	for(%i = 0; %i < getWordCount(%headNodes); %i++)
	{
		%potentialHat = getWord(%headNodes, %i);
		if((%indiscriminate && $hat[%client] $= %potentialHat) || (!%this.indiscriminate && %potentialHat $= %this.targetHat))
		{
			%equippedHat = %potentialHat;
			%obj.unhideNode(%potentialHat);
			%obj.setNodeColor(%potentialHat, %this.hatColor);
		}
		else
		{
			%obj.hideNode(%potentialHat);
		}
	}

	//Save this for later.
	//%accentNodes = $accentsAllowed[%equippedHat];
	//for(%i = 0; %i < getWordCount(%accentNodes); %i++){}

	if(%equippedHat !$= "helmet")
	{
		%obj.hideNode("visor");
	}
	if(%indiscriminate)
	{
		if(isObject(%client) && $hat[%client] $= "helmet" && %client.accent == 1)
		{
			%obj.unhideNode("visor");
			%obj.setNodeColor("visor", %client.accentColor);
		}
	}
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
		%obj.setDecalName(%this.decalName);
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

	%client.playerClass = %eventidePlayerClass;
	%player.playerClass = %eventidePlayerClass;

	//Apply the custom appearance of the class.
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

		%player.assignClass(%class); //Give the player the class.
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