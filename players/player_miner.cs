//
// Core and appearance.
//

datablock PlayerData(PlayerMiner : PlayerSurvivor)
{
    uiName = "Miner";
};
//Inherits functions from `PlayerSurvivor`.
PlayerMiner.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerMiner::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Give their torso a custom decal.
	%obj.setDecalName("worm_engineer");

    //Give the Hoarder a hardhat.
    %obj.unhideNode("scouthat");
}

function PlayerMiner::eventideBodyColors(%this, %obj)
{
    %this.super("eventideBodyColors", %this, %obj);

    //Color the hardhat, and overalls.
    %obj.setNodeColor("scouthat", "1.0 0.98431372549 0.0 1.0");

    %overallsColor = "0.23529411764 0.23529411764 0.23529411764 1.0"; //Matches the color of the decal.
    %obj.setNodeColor("pants", %overallsColor);
    %obj.setNodeColor("skirt", %overallsColor);
}   