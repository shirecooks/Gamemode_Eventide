//
// Core and appearance.
//

datablock PlayerData(PlayerMender : PlayerSurvivor)
{
    uiName = "Mender";
    maskMountPoint = 2;
};
//Inherits functions from `PlayerSurvivor`.
PlayerMender.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerMender::getFacePack(%this, %obj)
{
	%client = %obj.client;
	%facePack = (isObject(%client) && %client.chest) ? "menderF" : "menderM";
	return %facePack;
}

function PlayerMender::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

	%obj.unhideNode("surgicalMask");

    //Give their torso a custom decal.
	%obj.setDecalName("sweater");
}