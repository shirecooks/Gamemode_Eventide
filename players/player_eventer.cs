//
// Core and appearance.
//

datablock PlayerData(PlayerEventer : PlayerSurvivor)
{
    uiName = "Eventer";
};
//Inherits functions from `PlayerSurvivor`.
PlayerEventer.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerEventer::getFacePack(%this, %obj)
{
	%client = %obj.client;
	%facePack = (isObject(%client) && %client.chest) ? "eventerF" : "eventerM";
	return %facePack;
}

function PlayerEventer::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

	//Give the builder a blue hardhat.
    %obj.setNodeColor("scoutHat", "0.0 0.0 1.0 1.0");

    //Give their torso a custom decal.
	%obj.setDecalName("civilian");
}