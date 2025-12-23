datablock ItemData(PickaxeItem)
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
	
	image = PickaxeImage;
	canDrop = true;
};

datablock ShapeBaseImageData(PickaxeImage)
{
	shapeFile = "./models/pickaxe.dts";
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

function PickaxeImage::onUse(%this,%obj,%slot)
{
	%obj.activateStuff();
}

function PickaxeImage::onMount(%this,%obj,%slot)
{
	%obj.playThread(0, "armReady");
}

function PickaxeImage::onUnMount(%this,%obj,%slot)
{
	%obj.playThread(0, "root");
}

