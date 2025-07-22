//
// Hat.
//

datablock ShapeBaseImageData(stallerHoodImage)
{
	shapeFile = "Add-Ons/Gamemode_Eventide/players/models/grimhood.dts";
	mountPoint = $HeadSlot;

	eyeOffset = "0 0 -1000";
	emap = 0;
	
	doColorShift = true;
	colorShiftColor = "0.1 0.1 0.1 1";
};

//
// Core and appearance.
//

datablock PlayerData(PlayerStaller : PlayerSurvivor)
{
    class = "PlayerStaller";
    superClass = "PlayerSurvivor";

    uiName = "Staller Player";
	hoodMountPoint = 3;
};
//Inherits functions from `PlayerSurvivor`.
PlayerStaller.inheritFunctionsFromSuperClass();

function PlayerStaller::onNewDatablock(%this, %obj)
{
    %this.superClass.super("onNewDatablock", %this, %obj);

	//Edge-case: delete any voice-configs that may be present from a previous datablock.
	if(isObject(%obj.voiceConfig))
	{
		%obj.voiceConfig.delete();
	}
	//Another edge-case: delete any face-configs that may be present from a previous datablock.
	if(isObject(%obj.faceConfig))
	{
		%obj.faceConfig.delete();
	}
    
	//Store some information used for voice-lines and chase management.
	%obj.chasingKillers = new SimSet();
	%obj.nearbyKillers = new SimSet();
}

function PlayerStaller::eventideBodyParts(%this, %obj)
{
	%obj.hideNode("ALL");

	%obj.unHideNode("chest");	
	%obj.unHideNode("rhand");
	%obj.unHideNode("lhand");
	%obj.unHideNode("rarm");
	%obj.unHideNode("larm");
	%obj.unHideNode("headskin");
	%obj.unHideNode("skirt");
	%obj.setHeadUp(0);

	//Hood.
	%obj.mountImage("stallerHoodImage", %this.hoodMountPoint);

    //Custom player scale.
    %obj.setScale("1.05 1.05 1.05");
}

function PlayerStaller::eventideBodyColors(%this, %obj)
{
    %skinColor = "0.5 0.5 0.5 1";
	%clothingColor = "0.1 0.1 0.1 1";
	%shirtColor = "0.541 0.698 0.553 1";

    //Set core body part colors.
	%obj.setNodeColor("headskin", "0 0 0 1");
	%obj.setNodeColor("chest", %clothingColor);
	%obj.setNodeColor("skirt", %clothingColor);
	%obj.setNodeColor("rarm", %clothingColor);
	%obj.setNodeColor("larm", %clothingColor);
	%obj.setNodeColor("Rhand", %skinColor);
	%obj.setNodeColor("Lhand", %skinColor);

    //Set blood node colors, only shown upon taking damage.
	%bloodColor = "0.7 0 0 1";
	%obj.setNodeColor("lhand_blood", %bloodColor);
	%obj.setNodeColor("rhand_blood", %bloodColor);
	%obj.setNodeColor("chest_blood_front", %bloodColor);
	%obj.setNodeColor("chest_blood_back", %bloodColor);
}

//
// Cloaking mechanic and datablock.
//

datablock PlayerData(PlayerStallerCloaked : PlayerStaller)
{
    class = "PlayerStallerCloaked";
    superClass = "PlayerStaller";

	maxForwardCrouchSpeed = PlayerStaller.maxForwardSpeed;
	maxSideCrouchSpeed = PlayerStaller.maxSideSpeed;
	maxBackwardCrouchSpeed = PlayerStaller.maxBackwardSpeed;

    uiName = "";
	isEventideClass = true;

	maxEnergy = 100;
	rechargeRate = -0.4375; //32 ticks per second * 14 = 8 seconds of cloak at full energy.
};
//Inherits functions from `PlayerStaller`.
PlayerStallerCloaked.inheritFunctionsFromSuperClass();

function PlayerStallerCloaked::eventideBodyParts(%this, %obj)
{
	//Hide every node of the Staller's body, to make them invisible.
	%obj.hideNode("ALL"); 

	//Get rid of the Staller's hood separately, as it is not yet part of the playertype.
	%obj.unmountImage(%this.hoodMountPoint);
}

function PlayerStallerCloaked::eventideBodyColors(%this, %obj)
{
	//Invisible, so nothing is needed here.
}

//Cloaking.
function PlayerStaller::onTrigger(%this, %obj, %trigger, %state)
{
	%returnValue = %this.super("onTrigger", %this, %obj, %trigger, %state);
	if(%trigger == 3 && %state)
	{
		%this.cloak(%obj);
	}
	return %returnValue;
}

function PlayerStaller::cloak(%this, %obj)
{
	//Energy needs to be full to cloak, for balance reasons.
	if(%obj.getEnergyLevel() != %this.maxEnergy)
	{
		return;
	}
	//Prevent cloak spam by adding a 1-second delay.
	else if((getSimTime() - %obj.lastCloakTime) < 1000)
	{
		return;
	}

	//Store last cloaking time.
	%obj.lastCloakTime = getSimTime();

	//Play the cloaking sound effect.
	serverPlay3D("staller_cloak_sound", %obj.getHackPosition());

	//Change the player to the cloaked datablock, so all the nodes will be hidden and the crouching speed will be increased.
	%targetDatablock = PlayerStallerCloaked;
	%obj.setDataBlock(%targetDatablock);

	//Start the tick loop, to determine when the player has run out of energy.
	cancel(%obj.cloakTickSchedule);
	%targetDatablock.cloakTick(%obj);
}

function PlayerStallerCloaked::onNewDatablock(%this, %obj)
{
	Parent::onNewDatablock(%this, %obj);

	//Unghost the player from everyone, so nobody can see them using items or anything.
	%obj.adjustObjectScopeToAll(false, %obj.client);
}

//Cloaking tick, handles uncloaking if the player runs out of energy.
function PlayerStallerCloaked::cloakTick(%this, %obj)
{
	//If the player no longer exists, we no longer need to do this.
	if(!isObject(%obj))
	{
		return;
	}

	//If the player has run out of energy, uncloak them.
	if(%obj.getEnergyLevel() == 0)
	{
		return %this.uncloak(%obj);
	}

	//Loop this check, as energy drains continuously
	cancel(%obj.cloakTickSchedule);
	%obj.cloakTickSchedule = %this.schedule(33, cloakTick, %obj);
}

//Decloak.
function PlayerStallerCloaked::onTrigger(%this, %obj, %trigger, %state)
{
	%returnValue = %this.super("onTrigger", %this, %obj, %trigger, %state);
	if(%trigger == 3 && !%state)
	{
		%this.uncloak(%obj);
	}
	return %returnValue;
}

function PlayerStallerCloaked::uncloak(%this, %obj)
{
	//Cancel the looping check for energy, if it exists.
	cancel(%obj.cloakTickSchedule);

	//Play the decloaking sound effect.
	serverPlay3D("staller_uncloak_sound", %obj.getHackPosition());

	//Reghost the player to everyone, so they are no longer invisible.
	%obj.adjustObjectScopeToAll(true);

	//Resume the normal datablock and appearance code.
	%obj.setDataBlock(PlayerStaller);
}