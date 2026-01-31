//
// Hat.
//

datablock ShapeBaseImageData(menderMaskImage) 
{
	shapeFile = "Add-Ons/Gamemode_Eventide/players/models/SurgicalMask.dts";
	mountPoint = $HeadSlot;
	offset = "0 0 0";
	eyeOffset = "0 0 -1000";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	doColorShift = false;
	emap = 0;
};

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

function PlayerMender::onNewDatablock(%this, %obj)
{
	%client = %obj.client;
	%clientExists = isObject(%client);

	//Face pack initialization, very basic for now. 
	//This can't use the usual datablock field method, since it needs to be dynamic.
	%facePack = (%clientExists && %client.chest) ? $Eventide_FacePacks["menderF"] : $Eventide_FacePacks["menderM"];
	%obj.createFaceConfig(%facePack);

	//Voice Pack initialization.
	%voicePack = (%clientExists && %client.chest) ? $Eventide_VoicePacks["female"] : $Eventide_VoicePacks["male"];
	%obj.createVoiceConfig(%voicePack);

	//Store some information used for voice-lines and chase management.
	%obj.chasingKillers = new SimSet();
	%obj.nearbyKillers = new SimSet();
}

function PlayerMender::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Surgical mask.
	%obj.mountImage("menderMaskImage", %this.maskMountPoint);

    //Give their torso a custom decal.
	%obj.setDecalName("sweater");
}