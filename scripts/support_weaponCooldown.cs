datablock ShapeBaseImageData(cooldownImage)
{
    shapeFile = "base/data/shapes/empty.dts";

    stateName[0] = "Activate";
    stateTimeoutValue[0] = 0.01;
    stateTransitionOnTimeout[0] = "CooldownCheck";

    stateName[1] = "Ready";

	//The sudden jump in state number here it to allow for custom melee weapons to more easily 
	//implement their own logic between the "Activate" and "CooldownCheck" states.

    //Check if the item is on cooldown. If not, proceed to "Ready".
    stateName[25] = "CooldownCheck";
    stateScript[25] = "onCooldownCheck";
    stateAllowImageChange[25] = false;
    stateWaitForTimeout[25] = true;
    stateTimeOutValue[25] = 0.01;
    stateTransitionOnTimeout[25] = "CooldownRedirect";

    //Redirect to another state based on what was set in tq he previous "CooldownCheck" state.
    stateName[26] = "CooldownRedirect";
    stateAllowImageChange[26] = false;
    stateTransitionOnAmmo[26] = "Cooldown";
    stateTransitionOnNoAmmo[26] = "Ready";

    //The item is on cooldown and cannot be used.
    stateName[27] = "Cooldown";
    stateScript[27] = "onCooldown";
    stateAllowImageChange[27] = true;
    ////The cooldown ended while the item was equipped, transition to the "Ready" state.
    stateTransitionOnNoAmmo[27] = "CooldownRevert";

    //The item is transitioning from "Cooldown" to "Ready", we need to raise the arm back up.
    stateName[28] = "CooldownRevert";
    stateScript[28] = "onCooldownRevert";
    stateAllowImageChange[28] = false;
    stateWaitForTimeout[28] = true;
    stateTimeOutValue[28] = 0.01;
    stateTransitionOnTimeout[28] = "Ready";

    //Default cooldown of 30 seconds if not set by the weapon itself.
    cooldown = 30000;
};

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
    //     if((%obj.lastUseTime[objectClassImage] + %this.cooldown) > getSimTime())
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
    //         %hintStyle = %this.getHintStyle();
    //         %hintTime = (%hintStyle !$= "") ? getTextStyle(%hintStyle).displayTime : 6;

    //         %lastItemUseTime = %obj.lastUseTime[objectClassImage];
    //         %currentTime = getSimTime();

    //         if((%lastItemUseTime + msFromS(%hintTime)) < %currentTime)
    //         {
    //             %client.printFormatString("hint", "You can't use this " @ %this.item.uiName @ " for another " @ sFromMs((%lastItemUseTime + %this.cooldown) - %currentTime) @ " seconds.", sFromMs(%hintTime));
    //         }
    //     }
    // }

    // function objectClassImage::onCooldownRevert(%this, %obj)
    // {
    //     //Raise the item back up, it's ready.
    //     fixArmReady(%obj);
    // }
    %definitions = "function "@%objectClass@"::onCooldown(%this,%obj){%obj.playThread(1,root);if(%client=%obj.client){%hintStyle=%this.getHintStyle();%hintTime=(%hintStyle!$=\"\")?getTextStyle(%hintStyle).displayTime:6;%lastItemUseTime=%obj.lastUseTime["@%objectClass@"];%currentTime=getSimTime();if((%lastItemUseTime+msFromS(%hintTime))<%currentTime){%client.printFormatString(\"hint\",\"You can't use this \"@%this.item.uiName@\" for another \"@sFromMs((%lastItemUseTime+%this.cooldown)-%currentTime)@\" seconds.\",%hintTime);}}}";
    %definitions = %definitions @ "function "@%objectClass@"::onCooldownCheck(%this,%obj){fixArmReady(%obj);if((%obj.lastUseTime["@%objectClass@"]+%this.cooldown)>getSimTime()){%obj.setImageAmmo(%this.mountPoint,1);}else{%obj.setImageAmmo(%this.mountPoint,0);}}";
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

    //The variable storing the player's
    %objectClass = (%weapon.class !$= "") ? %weapon.class : %weapon.getName();
    %obj.lastUseTime[%objectClass] = getSimTime();

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