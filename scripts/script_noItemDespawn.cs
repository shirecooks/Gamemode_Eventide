package Script_NoItemDespawn
{
    function ServerCmdDropTool(%client, %position)
    {
        %player = %client.Player;
        if (!isObject(%player))
        {
            return;
        }
        %item = %player.tool[%position];
        if (isObject(%item))
        {
            if (%item.canDrop == 1)
            {
                %zScale = getWord(%player.getScale(), 2);
                %muzzlepoint = VectorAdd(%player.getPosition(), "0 0" SPC 1.5 * %zScale);
                %muzzlevector = %player.getEyeVector();
                %muzzlepoint = VectorAdd(%muzzlepoint, %muzzlevector);
                %playerRot = rotFromTransform(%player.getTransform());
                %thrownItem = new Item()
                {
                    dataBlock = %item;
                };
                %thrownItem.setScale(%player.getScale());
                MissionCleanup.add(%thrownItem);
                %thrownItem.setTransform(%muzzlepoint @ " " @ %playerRot);
                %thrownItem.setVelocity(VectorScale(%muzzlevector, 20 * %zScale));
                %thrownItem.miniGame = %client.miniGame;
                %thrownItem.bl_id = %client.getBLID();
                %thrownItem.setCollisionTimeout(%player);
                %thrownItem.schedulePop();
                if (%item.className $= "Weapon")
                {
                    %player.weaponCount--;
                }
                %player.tool[%position] = 0;
                messageClient(%client, 'MsgItemPickup', '', %position, 0);
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

    function Item::schedulePop(%obj)
    {
        %minigame = %obj.miniGame;
        if(!isObject(%minigame))
        {
            return parent::schedulePop(%obj);
        }

        %minigame.spawnedItems.add(%obj);
    }

    function MiniGameSO::onAdd(%obj)
    {
        parent::onAdd(%obj);

        %obj.spawnedItems = new SimGroup();
    }

    function MiniGameSO::Reset(%obj, %client)
    {
        %obj.spawnedItems.delete();
        %obj.spawnedItems = new SimGroup();

        parent::Reset(%obj, %client);
    }

    function MiniGameSO::endGame(%obj)
    {
        %obj.spawnedItems.delete();
    }
};
if(isPackage(Script_NoItemDespawn))
{
    deactivatePackage(Script_NoItemDespawn);
}
activatePackage(Script_NoItemDespawn);