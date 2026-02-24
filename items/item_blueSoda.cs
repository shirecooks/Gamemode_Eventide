//
// Image data.
//

datablock ItemData(blueSodaItem)
{
	category = "Tools";
	className = "Weapon";

	shapeFile = "./models/blueSoda/blueSoda.dts";
	doColorShift = false;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	canDrop = 1;
	emap = false;
	
	uiName = "Speed Soda";
	iconName = "./icons/icon_blueSoda";

	image = blueSodaImage;
	canDrop = true;
};

datablock ShapeBaseImageData(blueSodaImage : cooldownImage)
{
	className = "WeaponImage";

	shapeFile = "./models/blueSoda/blueSoda.dts";
	emap = false;
	isSpecial = 1;

	mountPoint = 0;
	offset = "0.157 0.1 0";
	rotation = eulerToMatrix("0 0 90");
	armReady = false;

	doColorShift = false;

	item = blueSodaItem;
	ammo = false;
	projectile = "";

	//The soda is inactive, simply being held.
	//This state NEEDS to start at 1, not 0. `implementCooldownCallbacks` reserves the first slot.
    stateName[1] = "Ready";
    stateScript[1] = "onReady";
	stateAllowImageChange[1] = true;
    stateTransitionOnTriggerDown[1]	= "Open";

	//The soda has been opened, prepare to drink.
	stateName[2] = "Open";
	stateScript[2] = "onOpen";
	stateAllowImageChange[2] = false;
	stateWaitForTimeout[2] = true;
	stateTimeOutValue[2] = 1;
	stateTransitionOnTimeout[2] = "Drink";

	//Activate the status effect, put the soda on cooldown.
	stateName[3] = "Drink";
	stateScript[3] = "onDrink";
	stateAllowImageChange[3] = false;
	stateWaitForTimeout[3] = true;
	stateTimeOutValue[3] = 0.5;
	stateTransitionOnTimeout[3] = "Activate";
	
	cooldown = 32000;
};
blueSodaImage.implementCooldownCallbacks();

function blueSodaImage::getHintMessage(%this, %obj)
{
	return "Drink for a short sugar rush, moving faster. Free refills!";
}

//
// Sequence callbacks.
//

function blueSodaImage::onOpen(%this, %obj)
{
	//Play the sound of the soda opening.
	serverPlay3D("sodaCan_open_sound", %obj.getMuzzlePoint(0));

	//Raise the can so it is ready to drink.
	%obj.playThread(1, armReadyRight);

	//Play an animation of cracking open the tab.
	%obj.playThread(2, shiftleft);
}

function blueSodaImage::OnDrink(%this, %obj)
{
	%slot = %obj.currTool;
	
	//Play the soda drinking sound and animation.
	serverPlay3D("drink_gulp" @ getRandom(1, 3) @ "_sound", %obj.getEyePoint());
	%obj.playThread(2, shiftUp);

	//Essential for cooldown purposes.
	%obj.lastSpeedSodaTime = getSimTime();

	//Apply the speed boost status effect.
	%obj.applyStatusEffect("SpeedSodaEffect", "Powerup", 6000);

	//Apply the weapon cooldown, to be resolved in 32 seconds.
	%cooldown = %this.cooldown;
	%obj.weaponCooldown(%slot, "The can is empty, and won't be refilled for " @ sFromMs(%cooldown) @ " seconds.", "Another Speed Soda is ready to consume!", 6);
}