function SimObject::implementCooldownCallbacks(%this)
{
    %objectClass = getSafeVariableName((%this.class !$= "") ? %this.class : %this.getName());
    if(%objectClass $= "")
    {
        error("ERROR: implementCooldownCallbacks() - Target object does not have a namespace.");
        return;
    }

    //
    // Define necessary logic callbacks.
    //

    // function objectClassImage::onCooldownCheck(%this, %obj)
    // {
    //     //Rest the item's animation.
    //     fixArmReady(%obj);

    //     //If the item has not passed it's cooldown time limit, transition to the "Cooldown" state.
    //     //Otherwise, transition to the "Ready" state.
    //     if((%obj.lastItemUseTime + %this.cooldown) > getSimTime())
    //     {
    //         %obj.setImageAmmo(%this.mountPoint, 1);
    //     }
    //     else
    //     {
    //         %obj.setImageAmmo(%this.mountPoint, 0);
    //     }
    // }

    // function objectClassImage::onCooldown(%this, %obj)
    // {
    //     //Lower the item, it cannot be used.
    //     %obj.playThread(1, root);

    //     if(%client = %obj.client)
    //     {
    //         %client.printFormatString("hint", "You can't use this " @ %this.item.uiName @ " for another " @ sFromMs((%obj.lastItemUseTime + %this.cooldown) - getSimTime()) @ " seconds.", 6);
    //     }
    // }

    // function objectClassImage::onCooldownRevert(%this, %obj)
    // {
    //     //Raise the item back up, it's ready.
    //     fixArmReady(%obj);
    // }
    %definitions = "function "@%objectClass@"::onCooldown(%this,%obj){%obj.playThread(1,root);if(%client=%obj.client){%client.printFormatString(\"hint\",\"You can't use this \"@%this.item.uiName@\" for another \"@sFromMs((%obj.last"@%objectClass@"UseTime+%this.cooldown)-getSimTime())@\" seconds.\",6);}}";
    %definitions = %definitions @ "function "@%objectClass@"::onCooldownCheck(%this,%obj){fixArmReady(%obj);if((%obj.last"@%objectClass@"UseTime+%this.cooldown)>getSimTime()){%obj.setImageAmmo(%this.mountPoint,1);}else{%obj.setImageAmmo(%this.mountPoint,0);}}";
    %definitions = %definitions @ "function "@%objectClass@"::onCooldownRevert(%this,%obj){fixArmReady(%obj);}";
    eval(%definitions);

    //Return the datablock for chaining by other scripts.
    return %this;
}

function Player::weaponCooldown(%obj, %mountPoint, %startMessage, %endMessage, %time)
{
    //No weapon equipped, no cooldown can be done.
    %weapon = %obj.getMountedImage(%mountPoint);
    if(!isObject(%weapon))
    {
        return;
    }

    //Default time to 6 seconds, if a custom one is not provided.
    if(!%time)
    {
        %time = 6;
    }

    //Take advantage of the GVarAccess.dso to set the player's last use time of the item, which is used to determine
    //if the cooldown has ended.
    %objectClass = getSafeVariableName((%weapon.class !$= "") ? %weapon.class : %weapon.getName());
    eval("%obj.last" @ %objectClass @ "UseTime = getSimTime();");

    //Trigger the Cooldown state if the item is currently equipped.
    %obj.setImageAmmo(%mountPoint, true);

    //Send a message to the client letting them know the weapon cannot be used.
    %client = %obj.client;
    if(%client && %startMessage !$= "" && %time !$= "")
    {
        %client.printFormatString("hint", %startMessage, %time);
    }
    
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