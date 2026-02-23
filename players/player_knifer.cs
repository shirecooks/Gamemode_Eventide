//
// Core and appearance.
//

datablock PlayerData(PlayerKnifer : PlayerSurvivor)
{
    uiName = "Knifer";
    leaveHatBlank = true;
};
//Inherits functions from `PlayerSurvivor`.
PlayerKnifer.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerKnifer::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    %obj.unHideNode("hoodie1");
    
	%obj.setDecalName("hoodie");
}

function PlayerKnifer::eventideBodyColors(%this, %obj)
{
	%this.super("eventideBodyColors", %this, %obj);

    //Set the color of the player's hat.
    %client = %obj.client;
    if(%client)
    {
        %obj.setNodeColor("hoodie1", %client.chestColor);
    }
    else
    {
        %obj.setNodeColor($Chest[0], "0.5 0.5 0.5 1");
        %obj.setNodeColor($Chest[1], "0.5 0.5 0.5 1");
        %obj.setNodeColor("hoodie1", "0.5 0.5 0.5 1");
    }
}