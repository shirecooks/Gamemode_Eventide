
//
// Status effect.
//

function PlayerFrozenEffect::beginStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Stop the player from moving.
    %playerDatablock = %obj.getDataBlock();
    %playerDatablock.setTempSpeed(%obj, 0.0);
}

function PlayerFrozenEffect::finalizeStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Allow the player to move again. The `setTempSpeed` function does this automatically when no multiplier is supplied.
	%playerDatablock = %obj.getDataBlock();
    %playerDatablock.setTempSpeed(%obj);
}

//
// Player helper functions.
//

function Player::freeze(%obj, %time)
{
    //Have the freeze time default to 1 second.
    if(%time $= "")
    {
        %time = 1000;
    }

    //Have the player enter the stun.
    %obj.applyStatusEffect("PlayerFrozenEffect", "Debuff", %time);
}