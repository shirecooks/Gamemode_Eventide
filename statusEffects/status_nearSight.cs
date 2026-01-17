
//
// Status effect.
//

function PlayerNearSightEffect::beginStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Tell the player they're blinded.
    %client = %obj.client;
    if(%client)
    {
        %client.printFormatString("hint", "Your vision narrows...");
    }

    //Darken the player's vision.
    %client = %obj.client;
    if(%client)
    {
        %client.originalFOV = %client.getControlCameraFov();
        commandToClient(%client, 'setVignette', 0, "1.0 0.3 0.3 1.0");
    }

    //Start a loop that keeps the player's vision narrow.
    %this.nearSightTick(%obj);
}

function PlayerNearSightEffect::nearSightTick(%this, %obj)
{
    %client = %obj.client;
    if(%client)
    {
        %client.setControlCameraFov(40);
    }

    %obj.nearSightSchedule = %this.schedule(200, "nearSightTick", %obj);
}

function PlayerNearSightEffect::finalizeStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Reset the player's vision.
    %client = %obj.client;
    if(%client)
    {
        %client.setControlCameraFov(%client.originalFOV);
        commandToClient(%client, 'setVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor);
    }

    cancel(%obj.nearSightSchedule);
}

//
// Player helper functions.
//

function Player::nearSight(%obj, %time)
{
    //Have the freeze time default to 1 second.
    if(%time $= "")
    {
        %time = 1000;
    }

    //Have the player enter the stun.
    %obj.applyStatusEffect("PlayerNearSightEffect", "Debuff", %time);
}