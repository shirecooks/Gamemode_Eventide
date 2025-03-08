datablock ItemData(RadioItem)
{
	category = "Weapon";
	className = "Weapon";
	shapeFile = "./models/radio.dts";
	iconName = "./icons/RadioIcon.png";
	uiName = "Radio";
	image = RadioImage;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	canDrop = true;
};

datablock ShapeBaseImageData(RadioImage)
{
	className = "WeaponImage";
	item = "RadioItem";		
	shapefile = RadioItem.shapeFile;
	mountpoint = 0;

	stateName[0] = "Activate";
	stateSound[0] = "radio_change_sound";
	stateTimeoutValue[0] = "0";
	stateTransitionOnTimeout[0] = "Ready";		
	stateName[1] = "Ready";
	stateTimeoutValue[1] = 0;
};

function RadioImage::onMount(%this, %obj, %slot)
{	
	if(!%obj.radioInformed && isObject(%obj.client))
	{
		%obj.client.centerPrint("<font:Impact:25>\c3Keep the radio to<br>\c3team chat with other survivors",3);
		%obj.radioInformed = true;
	}
}

function RadioItem::onAdd(%this, %obj)
{
	Parent::onAdd(%this,%obj);
	%obj.playaudio(3,"radio_unmount_sound");
}
