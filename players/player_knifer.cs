//
// Core and appearance.
//

datablock PlayerData(PlayerKnifer : PlayerSurvivor)
{
    uiName = "Knifer";
};
//Inherits functions from `PlayerSurvivor`.
PlayerKnifer.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerKnifer::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    %nodesToHideCount = 0;
    %nodesToHide[128] = "0"; //Instantiate the array with a dummy value.

    //Determine which nodes to hide. Changes between players and bots.
    %client = %obj.client;
    if(%client)
    {
        //Force a hoodie by hiding appropriate and showing appropriate nodes.
        %playerHat = $hat[%client.hat];
        if(%playerHat !$= "none")
        {
            %nodesToHide[%nodesToHideCount++] = %playerHat;
        }
        %playerAccent = getWord($accentsAllowed[$hat[%client.hat]], %client.accent);
        if(%playerAccent !$= "none")
        {
            %nodesToHide[%nodesToHideCount++] = %playerAccent;
        }
    }
    else
    {
        %nodesToHide[%nodesToHideCount++] = "plume";
        %nodesToHide[%nodesToHideCount++] = "triplume";
        %nodesToHide[%nodesToHideCount++] = "septplume";
        %nodesToHide[%nodesToHideCount++] = "visor";
        %nodesToHide[%nodesToHideCount++] = "bicorn";
        %nodesToHide[%nodesToHideCount++] = "cophat";
        %nodesToHide[%nodesToHideCount++] = "flarehelmet";
        %nodesToHide[%nodesToHideCount++] = "pointyhelmet";
        %nodesToHide[%nodesToHideCount++] = "scouthat";
        %nodesToHide[%nodesToHideCount++] = "helmet";
        %nodesToHide[%nodesToHideCount++] = "knithat";
    }

    //Hide those nodes...
    for(%i = 0; %i < %nodesToHideCount; %i++)
    {
        %obj.hideNode(%nodesToHide[%i]);
    }

    //This is everything we DO want to show.
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