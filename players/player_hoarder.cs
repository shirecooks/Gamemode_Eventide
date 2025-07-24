//
// Core and appearance.
//

datablock PlayerData(PlayerHoarder : PlayerSurvivor)
{
    class = "PlayerHoarder";
    superClass = "PlayerSurvivor";

    uiName = "Hoarder";
    maxWeapons = 5;
	maxTools = 5;
};
//Inherits functions from `PlayerSurvivor`.
PlayerHoarder.inheritFunctionsFromSuperClass();

function PlayerHoarder::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Give their torso a custom decal.
	%obj.setDecalName("hawaiianshirt");
}