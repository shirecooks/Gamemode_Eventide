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

datablock ShapeBaseImageData(blueSodaImage)
{
	className = "WeaponImage";

	shapeFile = "./models/blueSoda/blueSoda.dts";
	emap = false;
	isSpecial = 1;

	mountPoint = 0;
	offset = "-0.01 0.1 0";
	armReady = false;

	doColorShift = false;

	item = blueSodaItem;
	ammo = false;
	projectile = "";
	cooldown = 32000;

	//The soda has been equipped.
    stateName[0] = "Activate";
    stateSequence[0] = "ready";
	stateAllowImageChange[0] = false;
    stateTimeoutValue[0] = 0.01;
    stateTransitionOnTimeout[0]	= "CooldownCheck";

	//Check if the soda is on cooldown. If not, proceed to "Ready".
    stateName[1] = "CooldownCheck";
    stateScript[1] = "onCooldownCheck";
    stateAllowImageChange[1] = false;
    stateWaitForTimeout[1] = true;
    stateTimeOutValue[1] = 0.01;
    stateTransitionOnTimeout[1] = "CooldownRedirect";

	//Redirect to another state based on what was set in the previous "CooldownCheck" state.
    stateName[2] = "CooldownRedirect";
    stateAllowImageChange[2] = false;
    stateTransitionOnNoAmmo[2] = "Ready";
	stateTransitionOnAmmo[2] = "Cooldown";

	//The soda is on cooldown and cannot be used.
    stateName[3] = "Cooldown";
    stateScript[3] = "onCooldown";
    stateAllowImageChange[3] = false;
    ////The cooldown ended while the soda was equipped, transition to the "Ready" state.
    stateTransitionOnNoAmmo[3] = "CooldownRevert";

	//The soda is transitioning from "Cooldown" to "Ready", we need to raise the arm back up.
    stateName[4] = "CooldownRevert";
    stateScript[4] = "onCooldownRevert";
    stateAllowImageChange[4] = false;
    stateWaitForTimeout[4] = true;
    stateTimeOutValue[4] = 0.01;
    stateTransitionOnTimeout[4] = "Ready";

	//The soda is inactive, simply being held.
    stateName[5] = "Ready";
    stateScript[5] = "onReady";
	stateAllowImageChange[5] = true;
    stateTransitionOnTriggerDown[5]	= "Open";

	//The soda has been opened, prepare to drink.
	stateName[6] = "Open";
	stateScript[6] = "onOpen";
	stateAllowImageChange[6] = false;
	stateWaitForTimeout[6] = true;
	stateTimeOutValue[6] = 1;
	stateTransitionOnTimeout[6] = "Drink";

	//Activate the status effect, put the soda on cooldown.
	stateName[7] = "Drink";
	stateScript[7] = "onDrink";
	stateAllowImageChange[7] = false;
	stateWaitForTimeout[7] = true;
	stateTimeOutValue[7] = 0.5;
	stateTransitionOnTimeout[7] = "Cooldown";
};

//
// Sequence callbacks.
//

function blueSodaImage::onCooldownCheck(%this, %obj, %slot)
{
    //The soda's animation always needs to be reset at this point.
    %obj.playThread(2, root);

    //If the soda has not passed it's cooldown time limit, transition to the "Cooldown" state.
    //Otherwise, transition to the "Ready" state.
	%cooldownEndTime = (%obj.lastSpeedSodaTime + %this.cooldown);
	%currentTime = getSimTime();
    if(%cooldownEndTime > %currentTime)
    {
        %obj.setImageAmmo(%slot, true);
    }
    else
    {
        %obj.setImageAmmo(%slot, false);
    }
}

function blueSodaImage::onCooldown(%this, %obj, %slot)
{
	//The soda can was raised during the drinking animation, lower it again.
	%obj.playThread(1, root);
}

function blueSodaImage::onCooldownRevert(%this, %obj, %slot)
{
    //Raise the arm back up after being lowered.
    fixArmReady(%obj);
}

function blueSodaImage::onReady(%this, %obj, %slot)
{

}

function blueSodaImage::onOpen(%this, %obj, %slot)
{
	//Play the sound of the soda opening.
	serverPlay3D("soda_can_open_sound", %obj.getPosition());

	//Raise the can so it is ready to drink.
	%obj.playThread(1, armReadyRight);

	//Play an animation of cracking open the tab.
	%obj.playThread(2, shiftleft);
}

function blueSodaImage::OnDrink(%this, %obj, %slot)
{
	//Play the soda drinking sound and animation.
	serverPlay3D("soda_gulp" @ getRandom(1,3) @ "_sound", %obj.getPosition());
	%obj.playThread(2, shiftUp);

	//Essential for cooldown purposes.
	%obj.lastSpeedSodaTime = getSimTime();

	//Apply the speed boost status effect.
	%obj.applyStatusEffect("SpeedSodaEffect", "Powerup", 6000);

	//Apply the weapon cooldown, to be resolved in 32 seconds.
	%cooldown = %this.cooldown;
	%obj.weaponCooldown(%slot, "The can is empty, and won't be refilled for " @ sFromMs(%cooldown) @ " seconds.", "Another Speed Soda is ready to consume!", 6);
}