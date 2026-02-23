//
// Core and appearance.
//

datablock PlayerData(PlayerFreekiller : PlayerSurvivor)
{
    uiName = "Freekiller";
    maxDamage = 175;
    shoveForce = 2;
};
//Inherits functions from `PlayerSurvivor`.
PlayerFreekiller.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerFreekiller::getFacePack(%this, %obj)
{
	%client = %obj.client;
	%facePack = (isObject(%client) && %client.chest) ? "freekillerF" : "freekillerM";
	return %facePack;
}

function PlayerFreekiller::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Give their torso a custom decal.
	%obj.setDecalName("francis");
}