//
// Core and appearance.
//

datablock PlayerData(PlayerSheriff : PlayerSurvivor)
{
    uiName = "Sheriff";
};
//Inherits functions from `PlayerSurvivor`.
PlayerSheriff.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerSheriff::onNewDatablock(%this, %obj)
{
	%client = %obj.client;
	%clientExists = isObject(%client);

	//Face pack initialization, very basic for now. 
	//This can't use the usual datablock field method, since it needs to be dynamic.
	%facePack = (%clientExists && %client.chest) ? $Eventide_FacePacks["female"] : $Eventide_FacePacks["sheriffM"];
	%obj.createFaceConfig(%facePack);

	//Voice Pack initialization.
	%voicePack = (%clientExists && %client.chest) ? $Eventide_VoicePacks["female"] : $Eventide_VoicePacks["male"];
	%obj.createVoiceConfig(%voicePack);

	//Store some information used for voice-lines and chase management.
	%obj.chasingKillers = new SimSet();
	%obj.nearbyKillers = new SimSet();
}

function PlayerSheriff::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Give their torso a custom decal.
	%obj.setDecalName("Mod-Police");
}