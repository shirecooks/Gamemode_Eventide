datablock itemData(overcoatItem)
{
	category     = "item";
	shapeFile    = "./models/overcoat.dts";
	mass         = 1;
	density      = 0.2;
	elasticity   = 0.2;
	friction     = 0.6;
	emap         = true;
	doColorShift = false;
	canDrop      = true;
	image        = overcoatImage;
	uiName   = "Overcoat";
};

datablock shapeBaseImageData(overcoatImage)
{
	shapeFile = "./models/overcoat.dts";
	emap = true;
	mountPoint = 0;
	className = "WeaponImage";
	item = overcoatItem;
	rotation = eulerToMatrix("0 0 180");
	offset = "0 0.25 0";
	melee = false;
	armReady = true;
	doColorShift = false;
	
	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0.1;
	stateTransitionOnTimeout[0] = "Ready";
	stateSound[0] = weaponSwitchSound;
	
	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1] = true;
	
	stateName[2] = "Fire";
	stateTransitionOnTriggerUp[2] = "Ready";
	stateTimeoutValue[2] = "0.2";
	stateFire[2] = true;
	stateAllowImageChange[2] = true;
	stateScript[2] = "onFire";
};

datablock shapeBaseImageData(overcoatMountedImage)
{
	shapeFile = "./models/overcoat.dts";
	emap = true;
	mountPoint = 7;
	offset = "0.01 0 0.64";
	eyeOffset = "0 0 10";
	rotation = eulerToMatrix("0 0 180");
	scale = "1 1 1";
	correctMuzzleVector = true;
	doColorShift = false;
	
	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0.1;
	stateTransitionOnTimeout[0] = "Idle";
	stateSound[0] = weaponSwitchSound;	
	stateName[1] = "Idle";
	stateAllowImageChange[1] = true;
};

function overcoatImage::onFire(%this, %obj, %slot)
{
	if(isObject(%obj) && %obj.getMountedImage(1) !$= nametoID("overcoatMountedImage"))
	{
		%obj.mountImage("overcoatMountedImage", 1);
	}	
}

function overcoatMountedImage::onMount(%this,%obj,%slot)
{
	%obj.hideNode("armor");
}