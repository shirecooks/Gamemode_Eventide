/// This function randomizes the Eventide items that are spawned in the mini-game.
/// It iterates through all bricks in the Eventide_ItemSpawns simset and randomizes
/// the item that is spawned for each brick. If the brick is a ritual brick, it is
/// added to a separate simset and then a random item is chosen from a script group
/// containing all of the ritual items. The random item is then set on the brick.
/// The emitter for each brick is set to SparkleGroundEmitter and the brick is then
/// removed from the simset.
/// @param %minigame The mini-game object that the items are being spawned in.
function MiniGameSO::randomizeEventideItems(%minigame)
{	    
    if(!isObject(Eventide_ItemSpawns) || !Eventide_ItemSpawns.getCount()) return;
    
    %ritualBrickSet = new SimSet(); // Create a simset to hold the ritual bricks
       
    // Main function to spawn items, clear effects and prepare ritual brick for the next condition check
    for(%g = 0; %g < Eventide_ItemSpawns.getCount(); %g++) if(isObject(%brick = Eventide_ItemSpawns.getObject(%g)))
    {
        %brick.setItem("none");
        %brick.setEmitter("none");

        switch$(strlwr(%brick.getname()))
        {
            case "_ritual": // This main loop only iterates through one brick at a time which causes problems 
                            // if we try to randomize ritual spawns, only add it to the simset for the next loop
                            %ritualBrickSet.add(%brick);

            case "_item":   switch(getRandom(1,12)) // Randomly pick an item to spawn, anything greater than 6 will be none
                            {
                                case 1: %brick.setItem($MinigameLocalChat ? "RadioItem" : "ShotgunSlugItem");
                                case 2: %brick.setItem("ZombieMedpackItem");
                                case 3: %brick.setItem("SodaItem");
                                case 4: %brick.setItem("FlareItem");
								case 5: %brick.setItem("GauzeItem");
								case 6: %brick.setItem("AirhornItem");
                                case 7: %brick.setItem("ZombiePillsItem");
                                default: %brick.setItem("none");
                            }

            case "_weapon": switch(getRandom(1,16)) // Randomly pick a weapon to spawn, anything greater than 8 will be none
                            {
                                case 1: %brick.setItem("sm_barStoolItem");
                                case 2: %brick.setItem("sm_bottleItem");
                                case 3: %brick.setItem("sm_chairItem");
                                case 4: %brick.setItem("sm_poolCueItem");
                                case 5: %brick.setItem("sm_chairItem");                                
                                case 6: %brick.setItem("FlareGunItem");
								case 7: %brick.setItem("DCamera");
                                case 8: %brick.setItem("StunGun");
                                default: %brick.setItem("none");
                            }
        }

        if(isObject(%brick.item)) %brick.setEmitter("SparkleGroundEmitter");
    }

    // Create the ritual script group to be used for the randomization of ritual spawns
    %ritualScriptGroup = new ScriptGroup();
    %ritualScriptGroup.add(new ScriptObject("script_candleitem"));
    %ritualScriptGroup.add(new ScriptObject("script_candleitem"));
    %ritualScriptGroup.add(new ScriptObject("script_candleitem"));
    %ritualScriptGroup.add(new ScriptObject("script_candleitem"));
    %ritualScriptGroup.add(new ScriptObject("script_bookItem"));
    %ritualScriptGroup.add(new ScriptObject("script_daggerItem"));
    
    // Only iterate through the ritual script group if there are still items in the group
    while(%ritualScriptGroup.getCount()) 
    {                 
        // If there are less than 6 items in the ritual brick set, end the loop
        if(%ritualBrickSet.getCount() < 6) break;     
        
        %randomritual = %ritualScriptGroup.getObject(getRandom(0,%ritualScriptGroup.getCount()-1)); // Get a random script object
        %randomritualitem = strreplace(%randomritual.getName(),"script_",""); // Get the item name from the script object
        %randomritualbrick = %ritualBrickSet.getObject(getRandom(0,%ritualBrickSet.getCount()-1)); // Get a random brick
        %randomritualbrick.setItem(%randomritualitem); // Set the item of the random brick to the item of the random script object
		%randomritualbrick.setEmitter("SparkleGroundEmitter"); // Set the emitter of the random brick
        
        // Delete the script object and remove the ritual brick from the set
        %randomritual.delete();
        %ritualBrickSet.remove(%randomritualbrick);
    }

     // Always clean up ScriptGroup and SimSet, so adding them to the mission cleanup won't be necessary
    %ritualScriptGroup.delete();
    %ritualBrickSet.delete();
}

