function Player::weaponCooldown(%obj, %slot, %startMessage, %endMessage, %time)
{
    //Default time to 6 seconds, if a custom one is not provided.
    if(!%time)
    {
        %time = 6;
    }

    //Send a message to the client letting them know the weapon cannot be used.
    %client = %obj.client;
    if(%client)
    {
        %client.printFormatString("hint", %startMessage, %time);
    }

    //No weapon equipped, no cooldown can be done.
    %weapon = %obj.getMountedImage(%slot);
    if(!isObject(%weapon))
    {
        return;
    }

    //Trigger the Cooldown state if the item is currently equipped.
    %obj.setImageAmmo(%slot, 1);
    
    //Set the cooldown to end in the time specified by the weapon itself.
    %obj.schedule(%weapon.cooldown, "_weaponCooldownEnd", %slot, %weapon.getName(), %endMessage, %time);
}

function Player::_weaponCooldownEnd(%obj, %slot, %imageName, %message, %time)
{
    //Send a message to the client letting them know the weapon is now usable.
    %client = %obj.client;
    if(%client)
    {
        %client.printFormatString("hint", %message, %time);
    }

    %weapon = %obj.getMountedImage(%slot);
    if(!isObject(%weapon) || %weapon.getName() !$= %imageName)
    {
        return;
    }

    //Revert the Cooldown state if the item is currently equipped.
    %obj.setImageAmmo(%slot, 0);
}