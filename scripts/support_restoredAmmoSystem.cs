package Support_RestoredAmmoSystem
{
    function ItemData::onPickup(%this, %obj, %user, %amount)
    {
        %pickedUp = Parent::onPickup(%this, %obj, %user, %amount);
        if(%pickedUp)
        {
            %user.incInventory(%this, 1);
        }
        return %pickedUp;
    }

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
                %thrownItem = %item.onThrow(%player, 1);
                %thrownItem.setScale(%player.getScale());
                %thrownItem.setTransform(%muzzlepoint @ " " @ %playerRot);
                %thrownItem.setVelocity(VectorScale(%muzzlevector, 20 * %zScale));
                %thrownItem.miniGame = %client.miniGame;
                %thrownItem.bl_id = %client.getBLID();
                %thrownItem.setCollisionTimeout(%player);
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

    function ShapeBase::clearInventory(%this)
    {

    }
};
if(isPackage(Support_RestoredAmmoSystem))
{
    deactivatePackage(Support_RestoredAmmoSystem);
}
activatePackage(Support_RestoredAmmoSystem);