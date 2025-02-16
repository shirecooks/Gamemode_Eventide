//
// Miscellaneous functions.
//

function cloneScriptGroup(%targetObject)
{
    %targetObjectName = %targetObject.getName();
    %targetObject.setName("targetScriptObject");

    %cloneObject = new ScriptGroup(cloneScriptObject : targetScriptObject);

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

		new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Mender";
			canStack = false;
			spawnMessage = "You acquired a medical item and can revive survivors faster!";
            appearance = new ScriptObject()
            {
                class = "EventideClassAppearance";
                facePack["female"] = "menderF";
                facePack["male"] = "menderM";
            };
			items = new ScriptGroup()
			{
				//Can't store datablocks directly in a ScriptGroup. How inconvenient.
				new ScriptObject()
				{
                    class = "EventideClassItem";
					itemData = (getRandom(0, 1) ? GauzeItem.getID() : ZombieMedpackItem.getID());
				};
			};
		};

        new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Runner";
			canStack = false;
			spawnMessage = "You acquired a soda and can run slightly faster!";
            appearance = new ScriptObject()
            {
                class = "EventideClassAppearance";
                facePack["female"] = "RunnerF";
                facePack["male"] = "RunnerM";
            };
			items = new ScriptGroup()
			{
				new ScriptObject()
				{
                    class = "EventideClassItem";
					itemData = SodaItem.getID();
				};
			};
		};

        new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Hoarder";
			maxItems = 5;
			canStack = false;
			spawnMessage = "You acquired a camera and have five item slots!";
			items = new ScriptGroup()
			{
				new ScriptObject()
				{
                    class = "EventideClassItem";
					itemData = DCamera.getID();
				};
			};
		};

        new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Fighter";
            pseudoHealth = 75;
			canStack = false;
			spawnMessage = "You acquired a pool cue, can shove further and can take 1 hit before getting damaged!";
			appearance = new ScriptObject()
            {
                class = "EventideClassAppearance";
                facePack["female"] = "fighterF";
                facePack["male"] = "fighterM";
            };
            items = new ScriptGroup()
			{
				new ScriptObject()
				{
                    class = "EventideClassItem";
					itemData = sm_poolCueItem.getID();
				};
			};
		};

        new ScriptObject()
		{
			class = "EventidePlayerClass";
			title = "Tinkerer";
			canStack = false;
			spawnMessage = "You acquired a monkey wrench and stungun. Use the wrench to repair generators faster!";
			appearance = new ScriptObject()
            {
                class = "EventideClassAppearance";
                facePack["female"] = "tinkererF";
                facePack["male"] = "tinkererM";
            };
            items = new ScriptGroup()
			{
				new ScriptObject()
				{
                    class = "EventideClassItem";
					itemData = MonkeyWrench.getID();
				};

                new ScriptObject()
				{
                    class = "EventideClassItem";
					itemData = StunGun.getID();
				};
			};
		};
	};
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

$Eventide_ClassGroupTemplates = new EventideClassGroupTemplates(Eventide_ClassGroupTemplates);

// Container for classes.
///

function EventideClassGroup::onAdd(%this)
{
	//Nothing but a template was given, auto-fill the data from an existing template if possible.
	if(%this.template && %this.getCount() == 0)
	{
		%existingTemplate = $Eventide_ClassGroupTemplates.getTemplate(%this.template);
		if(%existingTemplate)
		{
			%this = $Eventide_ClassGroupTemplates.cloneTemplate(%this.template);
		}
	}
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
        %this.appearance = new ScriptObject()
        {
            class = "EventideClassAppearance";
        };
    }
}

$Eventide_PlayerClasses = new ScriptGroup(Eventide_PlayerClasses)
{
	class = "EventideClassGroup";
	template = "default";
};

//
// Assigning a class to player.
//

function Player::assignClass(%obj, %eventidePlayerClass)
{
    //We can't give the player the class items otherwise.
    %client = %obj.client;
    if(!%client)
    {
        return;
    }

    //If the class doesn't exist, or isn't a class, we can stop right here.
    if(!isObject(%eventidePlayerClass) || %eventidePlayerClass.getClassName() !$= "EventidePlayerClass")
    {
        return;
    }

    //Set the player's psuedohealth, if applicable.
    %obj.psuedohealth = %eventidePlayerClass.psuedoHealth;

    //Give the player the class items.
    %items = %eventidePlayerClass.items;
    for(%i = 0; %i < %items.getCount(); %i++)
    {
        %itemDatablock = %items.getObject(%i).itemData;

        %obj.tool[%i] = %itemDatablock;
        messageClient(%client, 'MsgItemPickup', '', %i, %itemDatablock);
    }

    //Fix for the hoarder class: communicate to the client if the innventory size is bigger than 3 items.
    %maxItems = %eventidePlayerClass.maxItems;
    if(%maxItems > 3)
    {
        commandToClient(%client, 'PlayGui_CreateToolHud', %maxItems);
    }

    //Make the player's face match the class they were assigned.
    %player.createFaceConfig((%client.chest ? $Eventide_FacePacks["female"] : $Eventide_FacePacks["male"]));

    //Inform the player what class they got selected for.
    %client.centerprint(%formatString @ "Class: " @ %eventidePlayerClass.title @ "<br>" @ %eventidePlayerClass.spawnMessage, 4);
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
	%temporaryClassGroup = new ScriptGroup()
	{
		class = "EventideClassGroup";
		template = "Classic";
	};

	//Assign a class to each player.
	for(%i = 0; %i < %this.numMembers; %i ++)
	{
		%player = %this.member[%i].player;
		if(!isObject(%player))
		{
			return;
		}

		%class = %temporaryClassGroup.getObject(getRandom(0, (%temporaryClassGroup.getCount() - 1))); //Choose a class.

		%player.assignClass(%class); //Give the player the class.
		//TODO: Legacy class system support. Need to clean all that up eventually.
		%player.survivorclass = strlwr(%class.title);

		//Delete the class if it cannot be stacked.
		if(!%class.canStack)
		{
			%class.delete();
		}
	}

	//Clean up the temporary class group, we don't need it anymore.
	%temporaryClassGroup.delete();
}