//
// Speed Soda status effect callbacks.
//

function SpeedSodaEffect::beginStatusEffect(%this, %player)
{
	//Speed up the player by 25%.
	%player.getDataBlock().setTempSpeed(%player, 1.25);

	//Play a jolting animation so te killer knows the effect is active.
	%player.playThread(3, jump);
}

function SpeedSodaEffect::finalizeStatusEffect(%this, %player)
{
	//Return the player to normal speed.
	%player.getDataBlock().setTempSpeed(%player, 1.0);
	
	//Play a head shake animation to let the killer knows the effect has run out.
	%player.playThread(3, undo);
}