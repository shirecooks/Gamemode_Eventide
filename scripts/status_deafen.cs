//
// Status effect.
//

function PlayerDeafenEffect::beginStatusEffect(%this, %obj)
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
        %client.printFormatString("hint", "Your eyes start ringing, you can barely hear anything.");
    }

    //Play an ear-ringing sound.
    %obj.playAccessoryMusic("musicData_earRinging", 1.0, "Deafen");

    //Make the player jolt in confusion.
    %obj.playThread(3, "plant");
    %obj.emote("WtfImage", 1);
}

function PlayerDeafenEffect::finalizeStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Stop the ear-ringing sound.
    %obj.stopAccessoryMusic("Deafen");

    //Undo the stun animation.
	%obj.playThread(3, "undo");
}

//
// Player helper functions.
//

function Player::deafen(%obj, %time)
{
    //Have the freeze time default to 1 second.
    if(%time $= "")
    {
        %time = 1000;
    }

    //Have the player enter the stun.
    %obj.applyStatusEffect("PlayerDeafenEffect", "Debuff", %time);
}

//
// Package to (partially) mute sounds from the player.
//

package Status_Deafen
{
    function GameConnection::Play3D(%client, %datablock, %position)
	{
        %player = %client.player;
		if(%player && %player.hasStatusEffect("PlayerDeafenEffect", "Debuff"))
        {
            return;
        }
		
		parent::Play3D(%client, %datablock, %position);
	}

	function GameConnection::Play2D(%client, %datablock)
	{
        %player = %client.player;
		if(%player && %player.hasStatusEffect("PlayerDeafenEffect", "Debuff"))
        {
            return;
        }
		
		parent::Play2D(%client, %datablock);
	}
};
if(isPackage(Status_Deafen))
{
    deactivatePackage(Status_Deafen);
}
activatePackage(Status_Deafen);