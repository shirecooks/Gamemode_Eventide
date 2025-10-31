
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

	//Create the particle effect.
	%obj.mountImage(stunImage, stunImage.mountSlot);

    //Mark the player as stunned.
    %obj.isStunned = true;

    //Clear the player's held tool as part of the status effect.
    ServerCmdUnUseTool(%obj.client);

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
	%obj.isStunned = false;

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

//
// Package to enable the main status effect of fear - no item use.
//

package Status_Fear
{
    function ServerCmdUseTool(%client, %slot)
    {
        %player = %client.Player;
        if(%player && %player.isStunned)
        {
            %client.printFormatString("hint", "You're afraid, and your hands just won't work!");
        }

        parent::ServerCmdUseTool(%client, %slot);
    }
};
if(isPackage(Status_Fear))
{
    deactivatePackage(Status_Fear);
}
activatePackage(Status_Fear);