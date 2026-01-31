//
// Core and appearance.
//

datablock PlayerData(PlayerFighter : PlayerSurvivor)
{
    uiName = "Fighter";
    maxDamage = 175;
    shoveForce = 2;
};
//Inherits functions from `PlayerSurvivor`.
PlayerFighter.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerFighter::onNewDatablock(%this, %obj)
{
	%client = %obj.client;
	%clientExists = isObject(%client);

	//Face pack initialization, very basic for now. 
	//This can't use the usual datablock field method, since it needs to be dynamic.
	%facePack = (%clientExists && %client.chest) ? $Eventide_FacePacks["fighterF"] : $Eventide_FacePacks["fighterM"];
	%obj.createFaceConfig(%facePack);

	//Voice Pack initialization.
	%voicePack = (%clientExists && %client.chest) ? $Eventide_VoicePacks["female"] : $Eventide_VoicePacks["male"];
	%obj.createVoiceConfig(%voicePack);

	//Store some information used for voice-lines and chase management.
	%obj.chasingKillers = new SimSet();
	%obj.nearbyKillers = new SimSet();
}

function PlayerFighter::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Give their torso a custom decal.
	%obj.setDecalName("francis");
}