//
// Core and appearance.
//

datablock PlayerData(PlayerKartRacer : PlayerSurvivor)
{
    maxForwardSpeed = (PlayerEventide.maxForwardSpeed * 1.1);
	maxBackwardSpeed = (PlayerEventide.maxBackwardSpeed * 1.1);
	maxSideSpeed = (PlayerEventide.maxSideSpeed * 1.1);

    maxForwardCrouchSpeed = (PlayerEventide.maxForwardCrouchSpeed * 1.1);
	maxBackwardCrouchSpeed = (PlayerEventide.maxBackwardCrouchSpeed * 1.1);
	maxSideCrouchSpeed = (PlayerEventide.maxSideCrouchSpeed * 1.1);

	maxUnderwaterForwardSpeed = (PlayerEventide.maxUnderwaterForwardSpeed * 1.1);
	maxUnderwaterBackwardSpeed = (PlayerEventide.maxUnderwaterBackwardSpeed * 1.1);
	maxUnderwaterSideSpeed = (PlayerEventide.maxUnderwaterSideSpeed * 1.1);

    uiName = "Kart Racer";
};
//Inherits functions from `PlayerSurvivor`.
PlayerKartRacer.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerKartRacer::getFacePack(%this, %obj)
{
	%client = %obj.client;
	%facePack = (isObject(%client) && %client.chest) ? "racerF" : "racerM";
	return %facePack;
}

function PlayerKartRacer::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Give their torso a custom decal.
	%obj.setDecalName("scout");
}