function Player::weaponCooldown(%obj, %mountPoint, %startMessage, %endMessage, %time)
{
    //Default time to 6 seconds, if a custom one is not provided.
    if(!%time)
    {
        %time = 6;
    }

    //Send a message to the client letting them know the weapon cannot be used.
    %client = %obj.client;
    if(%client && %startMessage !$= "" && %time !$= "")
    {
        %client.printFormatString("hint", %startMessage, %time);
    }

    //No weapon equipped, no cooldown can be done.
    %weapon = %obj.getMountedImage(%mountPoint);
    if(!isObject(%weapon))
    {
        return;
    }

    //Trigger the Cooldown state if the item is currently equipped.
    %obj.setImageAmmo(%mountPoint, true);
    
    //Set the cooldown to end in the time specified by the weapon itself.
    %obj.schedule(%weapon.cooldown, "_weaponCooldownEnd", %mountPoint, %weapon.getName(), %endMessage, %time);
}

function Player::_weaponCooldownEnd(%obj, %mountPoint, %imageName, %message, %time)
{
    //Send a message to the client letting them know the weapon is now usable.
    %client = %obj.client;
    if(%client && %message !$= "")
    {
        %client.printFormatString("hint", %message, %time);
    }

    %weapon = %obj.getMountedImage(%mountPoint);
    if(!isObject(%weapon) || %weapon.getName() !$= %imageName)
    {
        return;
    }

    //Revert the Cooldown state if the item is currently equipped.
    %obj.setImageAmmo(%mountPoint, false);
}