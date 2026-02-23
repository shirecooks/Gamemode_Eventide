datablock ItemData(monkeyWrenchItem)
{
	shapeFile = "base/data/shapes/wrench.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;
	
	uiName = "Eventer's Wrench";
	iconName = "base/client/ui/itemIcons/Wrench";
	
	doColorShift = true;
	colorShiftColor = "0.0 0.0 1.0 1.0";
	
	image = monkeyWrenchImage;
	canDrop = true;
};

datablock ShapeBaseImageData(monkeyWrenchImage)
{
	shapeFile = "base/data/shapes/wrench.dts";
	emap = false;
	mountPoint = 0;
	offset = "0.0 0.0 0.0";
	eyeOffset = 0;
	rotation = eulerToMatrix("0 0 0");
	isSpecial = true;

	doColorShift = monkeyWrenchItem.doColorShift;
	colorShiftColor = monkeyWrenchItem.colorShiftColor;
	
	className = "WeaponImage";
	item = monkeyWrenchItem;
	
	armReady = true;
	
	stateName[0]					= "Activate";
	stateSound[0]					= "weaponSwitchSound";
	stateTimeoutValue[0]			= 0.15;
	stateSequence[0]				= "Ready";
	stateTransitionOnTimeout[0]		= "Ready";

	stateName[1]					= "Ready";
	stateAllowImageChange[1]		= true;
	stateTransitionOnTriggerDown[1]	= "Use";
	
	stateName[2]					= "Use";
	stateScript[2]					= "onUse";
	stateTransitionOnTriggerUp[2]	= "Ready";
};

function MonkeyWrenchImage::onUse(%this, %obj)
{
	%obj.activateStuff();
}