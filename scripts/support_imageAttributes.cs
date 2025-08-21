//
// Item attribute getters and setters.
//

function Player::setImageAttribute(%obj, %attribute, %value, %invPosition)
{
    //If an inventory slot is not specified, select the active one.
    if(%invPosition $= "")
    {
        %invPosition = %obj.currTool;
    }

    //If there is no item in the inventory slot, do nothing.
    if(%obj.tool[%invPosition] $= "")
    {
        return -1;
    }

    //Assign the attribute alongside the item slot in the player's inventory.
    %obj.tool[%invPosition, %attribute] = %value;

    //Maintain a list of attributes available to the inventory item.
    %attributeList = %obj.tool[%invPosition, "index"];
    if(%attributeList $= "")
    {
        %obj.tool[%invPosition, "index"] = %attribute;
    }
    else
    {
        //Check if the value is already in the list. Skip adding it if so.
        for(%i = 0; %i < getWordCount(%attributeList); %i++)
        {
            if(getWord(%attributeList, %i) $= %attribute)
            {
                return 1;
            }
        }
        //The attribute isn't already in the index, add it.
        %obj.tool[%invPosition, "index"] = %attributeList SPC %attribute;
    }

    return 1;
}

function Player::getImageAttribute(%obj, %attribute, %invPosition)
{
    if(%invPosition $= "")
    {
        %invPosition = %obj.currTool;
    }

    if(%obj.tool[%invPosition] $= "")
    {
        return -1;
    }

    return %obj.tool[%invPosition, %attribute];
}

//
// Package to manage hashing and attribute persistance.
//

package Support_ImageAttributes
{
    //Establish a "hash" (identifier) on the inventory item if it's new and doesn't already have one.
    function Weapon::onUse(%this, %player, %invPosition)
    {
        %hash = %player.tool[%invPosition, "hash"];
        if(%hash $= "")
        {
            %player.tool[%invPosition, "hash"] = getRandom(-2147483648, 2147483647);
        }
        return Parent::onUse(%this, %player, %invPosition);
    }

    //Allow the item to retain its attributes when it is dropped by a player.
    function ServerCmdDropTool(%client, %invPosition)
    {
        %player = %client.Player;
        if (!isObject(%player))
        {
            return;
        }
        %item = %player.tool[%invPosition];
        if (isObject(%item))
        {
            if (%item.canDrop == 1)
            {
                %zScale = getWord(%player.getScale(), 2);
                %muzzlepoint = VectorAdd(%player.getPosition(), "0 0" SPC 1.5 * %zScale);
                %muzzlevector = %player.getEyeVector();
                %muzzlepoint = VectorAdd(%muzzlepoint, %muzzlevector);
                %playerRot = rotFromTransform(%player.getTransform());

                //Code addition: add hash and attributes to thrown object, if they were present on the inventory slot.
                %thrownItem = new Item()
                {
                    dataBlock = %item;
                    hash = %player.tool[%invPosition, "hash"];
                    attributes["index"] = %player.tool[%invPosition, "index"];
                };
                if(%thrownItem.hash !$= "") //If the hash exists, it is possible there are attributes.
                {
                    %attributeList = %player.tool[%invPosition, "index"];
                    for(%i = 0; %i < getWordCount(%attributeList); %i++)
                    {
                        %attribute = getWord(%attributeList, %i);
                        %thrownItem.attributes[%attribute] = %player.tool[%invPosition, %attribute]; //Add the attribute to the Item object.
                        %player.tool[%invPosition, %attribute] = ""; //Clear the attribute from the player's inventory, to avoid confusion.
                    }
                }

                %thrownItem.setScale(%player.getScale());
                MissionCleanup.add(%thrownItem);
                %thrownItem.setTransform(%muzzlepoint @ " " @ %playerRot);
                %thrownItem.setVelocity(VectorScale(%muzzlevector, 20 * %zScale));
                %thrownItem.schedulePop();
                %thrownItem.miniGame = %client.miniGame;
                %thrownItem.bl_id = %client.getBLID();
                %thrownItem.setCollisionTimeout(%player);
                if (%item.className $= "Weapon")
                {
                    %player.weaponCount--;
                }
                %player.tool[%invPosition] = 0;
                messageClient(%client, 'MsgItemPickup', '', %invPosition, 0);
                if (%player.getMountedImage(%item.image.mountPoint) > 0)
                {
                    if (%player.getMountedImage(%item.image.mountPoint).getId() == %item.image.getId())
                    {
                        %player.unmountImage(%item.image.mountPoint);
                    }
                }
            }
        }
    }

    //Transfer an item's attributes to the player's inventory when they pick it up, if attributes are present.
    function ItemData::onPickup(%this, %obj, %user, %amount)
    {
        if (%obj.canPickup == 0)
        {
            return;
        }
        %player = %user;
        %client = %player.client;
        %data = %player.getDataBlock();
        if (!isObject(%client))
        {
            return;
        }
        %mg = %client.miniGame;
        if (isObject(%mg))
        {
            if (%mg.WeaponDamage == 1)
            {
                if (getSimTime() - %client.lastF8Time < 5000)
                {
                    return;
                }
            }
        }
        %canUse = 1;
        if (miniGameCanUse(%player, %obj) == 1)
        {
            %canUse = 1;
        }
        if (miniGameCanUse(%player, %obj) == 0)
        {
            %canUse = 0;
        }
        if (!%canUse)
        {
            if (isObject(%obj.spawnBrick))
            {
                %ownerName = %obj.spawnBrick.getGroup().name;
            }
            %msg = %ownerName @ " does not trust you enough to use this item.";
            if ($lastError == $LastError::Trust)
            {
                %msg = %ownerName @ " does not trust you enough to use this item.";
            }
            else if ($lastError == $LastError::MiniGameDifferent)
            {
                if (isObject(%client.miniGame))
                {
                    %msg = "This item is not part of the mini-game.";
                }
                else
                {
                    %msg = "This item is part of a mini-game.";
                }
            }
            else if ($lastError == $LastError::MiniGameNotYours)
            {
                %msg = "You do not own this item.";
            }
            else if ($lastError == $LastError::NotInMiniGame)
            {
                %msg = "This item is not part of the mini-game.";
            }
            commandToClient(%client, 'CenterPrint', %msg, 1);
            return;
        }
        %freeslot = -1;
        for (%i = 0; %i < %data.maxTools; %i++)
        {
            if (%player.tool[%i] == 0)
            {
                %freeslot = %i;
                break;
            }
        }
        if (%freeslot != -1)
        {
            //Code addition: transferring attributes from item object to inventory.
            if(%obj.hash !$= "")
            {
                %player.tool[%freeslot, "hash"] = %obj.hash;

                %attributeList = %obj.attributes["index"];
                for(%i = 0; %i < getWordCount(%attributeList); %i++)
                {
                    %attribute = getWord(%attributeList, %i);
                    %player.tool[%freeslot, %attribute] = %obj.attributes[%attribute];
                }
            }

            if (%obj.isStatic())
            {
                %obj.Respawn();
            }
            else
            {
                %obj.delete();
            }
            %player.tool[%freeslot] = %this;
            if (%user.client)
            {
                messageClient(%user.client, 'MsgItemPickup', '', %freeslot, %this.getId());
            }

            return 1;
        }
    }
};
if(isPackage(Support_ImageAttributes))
{
    deactivatePackage(Support_ImageAttributes);
}
activatePackage(Support_ImageAttributes);