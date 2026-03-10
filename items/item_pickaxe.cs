datablock ItemData(pickaxeItem)
{
	shapeFile = "./models/pickaxe/pickaxe.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;
	
	uiName = "Pickaxe";
	iconName = "./icons/icon_pickaxe";
	doColorShift = false;
	
	image = pickaxeImage;
	canDrop = true;
};

datablock ShapeBaseImageData(pickaxeImage)
{
	shapeFile = pickaxeItem.shapeFile;
	emap = false;
	mountPoint = 0;
	offset = "0.0 0.0 0.0";
	eyeOffset = 0;
	rotation = eulerToMatrix("0 0 0");
	isSpecial = true;
	
	className = "WeaponImage";
	item = PickaxeItem;
	
	armReady = true;
	doColorShift = false;
	
	stateName[0]					= "Activate";
	stateSound[0]					= "weaponSwitchSound";
	stateTimeoutValue[0]			= 0.15;
	stateSequence[0]				= "Ready";
	stateTransitionOnTimeout[0]		= "Ready";

	stateName[1]					= "Ready";
	stateAllowImageChange[1]		= true;
	stateScript[1]					= "onReady";
	stateTransitionOnTriggerDown[1]	= "Use";
	
	stateName[2]					= "Use";
	stateScript[2]					= "onUse";
	stateTransitionOnTriggerUp[2]	= "Ready";
};
pickaxeImage.registerImageOutputEvent("onPickaxeHit");

function pickaxeImage::getHintMessage(%this, %obj)
{
	return "Dig through shining rubble to unearth ancient artifacts...";
}

function pickaxeImage::onUse(%this, %obj)
{
	%obj.activateStuff();
	%this.eventRaycast(%obj);
}