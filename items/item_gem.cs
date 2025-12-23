//
// Base gem datablock.
//

datablock ItemData(gemItem : ritualItem)
{
	class = "gemItem";
	superClass = "ritualItem";

	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/gem/gem1.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	iconName = "./icons/icon_gem";

	image = "";

	ritualType = "Gem";
	maxRitualsOnCircle = 4;
	possibleOffset1 = "2.28 2.875 0.1 0 0 0";
	possibleOffset2 = "-2.28 2.875 0.1 0 0 0";
	possibleOffset3 = "2.1 -2.725 0.1 0 0 0";
	possibleOffset4 = "-2.1 -2.725 0.1 0 0 0";

	placeSound = "gem_place";
	placeSoundAmount = 1;
};
gemItem.inheritFunctionsFromSuperClass();

//
// Red gem.
//

datablock ItemData(gem1Item : gemItem)
{
	class = "gem1Item";
	superClass = "gemItem";

	category = "Weapon";
	className = "Weapon";

	uiName = "Red Gem";
	iconName = "./icons/icon_gem";

	doColorShift = true;
	colorShiftColor = "1 0.5 0.5 1";	

	image = gem1Image;
};
gem1Item.inheritFunctionsFromSuperClass();

datablock ShapeBaseImageData(gem1Image)
{
    shapeFile = "./models/gem/gem1.dts";
    emap = true;

    mountPoint = 0;
    offset = "-0.1 0.125 0";
	offsetrotation = "0 0 0";
    correctMuzzleVector = false;
    eyeOffset = "0 0 0";
    className = "WeaponImage";

    item = gem1Item;
    ammo = " ";
    projectile = "";
    projectileType = Projectile;

    armReady = true;

    doColorShift = gem1Item.doColorShift;
    colorShiftColor = gem1Item.colorShiftColor;

    stateName[0]                     = "Activate";
};

//
// Green gem.
//

datablock ItemData(gem2Item : gem1Item)
{
	class = "gem2Item";
	superClass = "gem1Item";

	shapeFile = "./models/gem/gem2.dts";
	uiName = "Green Gem";

	doColorShift = true;
	colorShiftColor = "0.5 1 0.5 1";

	image = gem2Image;
};
gem2Item.inheritFunctionsFromSuperClass();

datablock ShapeBaseImageData(gem2Image : gem1Image)
{
    shapeFile = "./models/gem/gem2.dts";
    item = gem2Item;

    doColorShift = gem2Item.doColorShift;
    colorShiftColor = gem2Item.colorShiftColor;	
};

//
// Blue gem.
//

datablock ItemData(gem3Item : gem1Item)
{
	class = "gem3Item";
	superClass = "gem1Item";

	shapeFile = "./models/gem/gem3.dts";
	uiName = "Blue Gem";

	doColorShift = true;
	colorShiftColor = "0.5 0.5 1 1";

	image = gem3Image;
};
gem3Item.inheritFunctionsFromSuperClass();

datablock ShapeBaseImageData(gem3Image : gem1Image)
{
    shapeFile = "./models/gem/gem3.dts";
    item = gem3Item;

    doColorShift = gem3Item.doColorShift;
    colorShiftColor = gem3Item.colorShiftColor;		
};

//
// Yellow gem.
//

datablock ItemData(gem4Item : gem1Item)
{
	class = "gem4Item";
	superClass = "gem1Item";

	shapeFile = "./models/gem/gem4.dts";
	uiName = "Yellow Gem";

	doColorShift = true;
	colorShiftColor = "1 0.83 0.51 1";

	image = gem4Image;
};
gem4Item.inheritFunctionsFromSuperClass();

datablock ShapeBaseImageData(gem4Image : gem1Image)
{
    shapeFile = "./models/gem/gem4.dts";
    item = gem4Item;

    doColorShift = gem4Item.doColorShift;
    colorShiftColor = gem4Item.colorShiftColor;	
};

//
// Purple gem.
//

datablock ItemData(gem5Item : gem1Item)
{
	class = "gem5Item";
	superClass = "gem1Item";

	shapeFile = "./models/gem/gem5.dts";
	uiName = "Purple Gem";

	doColorShift = true;
	colorShiftColor = "0.78 0.51 1 1";

	image = gem5Image;
};
gem5Item.inheritFunctionsFromSuperClass();

datablock ShapeBaseImageData(gem5Image : gem1Image)
{
    shapeFile = "./models/gem/gem5.dts";
    item = gem5Item;

    doColorShift = gem5Item.doColorShift;
    colorShiftColor = gem5Item.colorShiftColor;	
};