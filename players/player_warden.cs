//
// Core and appearance.
//

datablock PlayerData(PlayerWarden : PlayerSurvivor)
{
    uiName = "Warden";
};
//Inherits functions from `PlayerSurvivor`.
PlayerWarden.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerWarden::getFacePack(%this, %obj)
{
	%client = %obj.client;
	%facePack = (isObject(%client) && %client.chest) ? "female" : "wardenM";
	return %facePack;
}

function PlayerWarden::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Give their torso a custom decal.
	%obj.setDecalName("Mod-Police");
}