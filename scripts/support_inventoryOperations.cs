function Player::removeItemFromInventory(%obj, %slot)
{
    //If a slot is not specified, default to the currently held tool.
    if(%slot $= "")
    {
        %slot = %obj.currTool;
    }

    //If they don't have a tool in that slot, do nothing.
    if(%obj.tool[%slot] == 0)
    {
        return;
    }

    //If the player is currently holding that tool, forcibly unequip it.
    if(%slot == %obj.currTool)
    {
        %obj.unmountImage(%slot);
    }

    //Remove the item from the player's inventory.
    %obj.weaponCount--;
    %obj.decInventory(%obj.tool[%slot], 1);
    %obj.tool[%slot] = 0;
    %client = %obj.client;
	if(isObject(%client)) 
    {
        messageClient(%client, 'MsgItemPickup', '', %slot, 0, true);
    }
}

function Player::addItemToInventory(%obj, %image)
{
    for(%i = 0; %i < %obj.getDatablock().maxTools; %i++)
    {
        //Search for an empty slot in the player's inventory.
        %tool = %obj.tool[%i];
        if(%tool == 0)
        {
            //We found an empty slot, add the item there.
            %obj.weaponCount++;
            %obj.incInventory(%obj.tool[%slot], 1);
            %obj.tool[%i] = %image;
            %client = %obj.client;
            if(isObject(%client))
            {
                messageClient(%client, 'MsgItemPickup', '', %i, %image);
            }

            break;
        }
    }
}