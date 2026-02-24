//
// Core and appearance.
//

datablock PlayerData(PlayerSailor : PlayerSurvivor)
{
    uiName = "Sailor";
    leaveHatBlank = true;
};
//Inherits functions from `PlayerSurvivor`.
PlayerSailor.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerSailor::getFacePack(%this, %obj)
{
    %client = %obj.client;

    if(isObject(%client) && %client.chest)
    {
        %facePack = "sailorF";
    }
    else
    {
        %facePack = "sailorM" @ getRandom(1, 3);
    }
    
	return %facePack;
}

function PlayerSailor::eventideBodyParts(%this, %obj, %client)
{
	%this.super("eventideBodyParts", %this, %obj, %client);

    //Give the Miner a hardhat.
    %obj.unhideNode("bicorn");

    //Give their torso a custom decal.
    if(%client.chest)
    {
        %decalName = "femalePirateVest" @ getRandom(1, 3);
    }
    else
    {
        %decalName = "malePirateVest" @ getRandom(1, 3);
    }
	%obj.setDecalName(%decalName);
}

function PlayerSailor::eventideBodyColors(%this, %obj, %client)
{
    %this.super("eventideBodyColors", %this, %obj, %client);

    //Color the hat.
    %obj.setNodeColor("bicorn", "0.0784314 0.0784314 0.0784314 1");
}   