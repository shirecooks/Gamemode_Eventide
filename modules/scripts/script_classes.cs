//
// Resources that must go in this file.
//

datablock ShapeBaseImageData(menderMaskImage) 
{
	shapeFile = "Add-Ons/Gamemode_Eventide/modules/players/models/SurgicalMask.dts";
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 -1000";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
};

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
	};

	//Can't store datablocks directly in a ScriptGroup. How inconvenient.
	%menderClass = %this.index["Classic"].getClass("Mender");
	%menderClass.appearance.facePack["female"] = $Eventide_FacePacks["menderF"];
	%menderClass.appearance.facePack["male"] = $Eventide_FacePacks["menderM"];
	%menderClass.appearance.add(new ScriptObject()
	{
		class = "EventideClassCustomNode";
		targetNode = $HeadSlot;
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
}

//
/// Containers for class node modifications.

function EventideClassCustomNode::onAdd(%this)
{
	if(%this.targetNode $= "")
	{
		%this.targetNode = $BackSlot;
	}

	if(%this.mountableObject $= "") 
	{
		%this.mountableObject = 0;
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

	%player.playerClass = %eventidePlayerClass;

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

    //Make the player's face match the class they were assigned.
	%selectedFacePack = %client.chest ? %eventidePlayerClass.appearance.facePack["female"] : %eventidePlayerClass.appearance.facePack["male"];
	if(isObject(%selectedFacePack))
	{
		%client.customFacePack = %selectedFacePack;
    	%player.createFaceConfig(%selectedFacePack);
	}
	else
	{
		//TODO: Find a fix for this issue.
		talk("Class face pack bugged out, not applying...");
	}

	//If the class has any custom nodes, apply them.
	%customAppearance = %eventidePlayerClass.appearance;
	for(%i = 0; %i < %customAppearance.getCount(); %i++)
	{
		%customNode = %customAppearance.getObject(%i);
		if(isObject(%customNode.mountableObject))
		{
			%player.mountImage(%customNode.mountableObject, $HeadSlot);
		}
	}

    //Inform the player what class they got selected for.
    %client.centerprint("<font:impact:40><color:FFFF00>Class: " @ %eventidePlayerClass.title @ "<br>" @ %eventidePlayerClass.spawnMessage, 4);
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