
//
// Status effect.
//

function PlayerFearEffect::beginStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Tell the player they're stunned.
    %client = %obj.client;
    if(%client)
    {
        %client.printFormatString("hint", "You're afraid, and your hands just won't work!");
    }

	//Create the particle effect.
	%obj.mountImage(stunImage, stunImage.mountSlot);

    //Mark the player as stunned.
    %obj.lockTools = true;

    //Clear the player's held tool as part of the status effect.
    %client = %obj.client;
    if(%client)
    {
        ServerCmdUnUseTool(%client);
    }
    else
    {
        %obj.unmountImage(0);
    }

    //Play the stun animation.
	%obj.playThread(3, "jump");
}

function PlayerFearEffect::finalizeStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	//Remove the particle effect.
	%obj.unmountImage(stunImage.mountSlot);

    //Mark the player as no longer being stunned.
	%obj.lockTools = false;

    //Undo the stun animation.
	%obj.playThread(3, "undo");
}

//
// Player helper functions.
//

function Player::fear(%obj, %time)
{
    //Have the freeze time default to 1 second.
    if(%time $= "")
    {
        %time = 1000;
    }

    //Have the player enter the stun.
    %obj.applyStatusEffect("PlayerFearEffect", "Debuff", %time);
}