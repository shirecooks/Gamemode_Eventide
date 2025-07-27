//
// Core and appearance.
//

datablock PlayerData(PlayerRunner : PlayerSurvivor)
{
    class = "PlayerRunner";
    superClass = "PlayerSurvivor";

    maxForwardSpeed = (PlayerEventide.maxForwardSpeed * 1.1);
	maxBackwardSpeed = (PlayerEventide.maxBackwardSpeed * 1.1);
	maxSideSpeed = (PlayerEventide.maxSideSpeed * 1.1);

    maxForwardCrouchSpeed = (PlayerEventide.maxForwardCrouchSpeed * 1.1);
	maxBackwardCrouchSpeed = (PlayerEventide.maxBackwardCrouchSpeed * 1.1);
	maxSideCrouchSpeed = (PlayerEventide.maxSideCrouchSpeed * 1.1);

	maxUnderwaterForwardSpeed = (PlayerEventide.maxUnderwaterForwardSpeed * 1.1);
	maxUnderwaterBackwardSpeed = (PlayerEventide.maxUnderwaterBackwardSpeed * 1.1);
	maxUnderwaterSideSpeed = (PlayerEventide.maxUnderwaterSideSpeed * 1.1);

    uiName = "Runner";
};
//Inherits functions from `PlayerSurvivor`.
PlayerRunner.inheritFunctionsFromSuperClass();

function PlayerRunner::onNewDatablock(%this, %obj)
{
	%client = %obj.client;
	%clientExists = isObject(%client);

	//Face pack initialization, very basic for now. 
	//This can't use the usual datablock field method, since it needs to be dynamic.
	%facePack = (%clientExists && %client.chest) ? $Eventide_FacePacks["runnerF"] : $Eventide_FacePacks["runnerM"];
	%obj.createFaceConfig(%facePack);

	//Voice Pack initialization.
	%voicePack = (%clientExists && %client.chest) ? $Eventide_VoicePacks["female"] : $Eventide_VoicePacks["male"];
	%obj.createVoiceConfig(%voicePack);

	//Store some information used for voice-lines and chase management.
	%obj.chasingKillers = new SimSet();
	%obj.nearbyKillers = new SimSet();
}

function PlayerRunner::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Give their torso a custom decal.
	%obj.setDecalName("scout");
}