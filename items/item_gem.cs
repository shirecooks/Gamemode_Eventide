//
// Base gem datablock and image.
//

datablock ItemData(gemItem : ritualItem)
{
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
gemItem.inheritFunctionsFromSuperClass("ritualItem");

datablock ShapeBaseImageData(gemImage : ritualImage)
{
	className = "WeaponImage";
	
	emap = true;
    mountPoint = 0;
    offset = "-0.1 0.125 0";
    
    item = gem1Item;
    ammo = " ";
    projectile = "";
    projectileType = Projectile;

    armReady = true;

	stateName[0] = "Activate";
};
gemImage.inheritFunctionsFromSuperclass("ritualImage");

//
// Red gem.
//

datablock ItemData(gem1Item : gemItem)
{
	uiName = "Red Gem";

	doColorShift = true;
	colorShiftColor = "1 0.5 0.5 1";	

	image = gem1Image;
};
gem1Item.inheritFunctionsFromSuperClass("gemItem");

datablock ShapeBaseImageData(gem1Image : ritualImage)
{
    shapeFile = "./models/gem/gem1.dts";

    doColorShift = gem1Item.doColorShift;
    colorShiftColor = gem1Item.colorShiftColor;
};
gem1Image.inheritFunctionsFromSuperclass("gemImage");

//
// Green gem.
//

datablock ItemData(gem2Item : gem1Item)
{
	shapeFile = "./models/gem/gem2.dts";
	uiName = "Green Gem";

	doColorShift = true;
	colorShiftColor = "0.5 1 0.5 1";

	image = gem2Image;
};
gem2Item.inheritFunctionsFromSuperClass("gemItem");

datablock ShapeBaseImageData(gem2Image : gem1Image)
{
    shapeFile = "./models/gem/gem2.dts";
    item = gem2Item;

    doColorShift = gem2Item.doColorShift;
    colorShiftColor = gem2Item.colorShiftColor;	
};
gem2Image.inheritFunctionsFromSuperClass("gemImage");

//
// Blue gem.
//

datablock ItemData(gem3Item : gem1Item)
{
	shapeFile = "./models/gem/gem3.dts";
	uiName = "Blue Gem";

	doColorShift = true;
	colorShiftColor = "0.5 0.5 1 1";

	image = gem3Image;
};
gem3Item.inheritFunctionsFromSuperClass("gem1Item");

datablock ShapeBaseImageData(gem3Image : gem1Image)
{
    shapeFile = "./models/gem/gem3.dts";
    item = gem3Item;

    doColorShift = gem3Item.doColorShift;
    colorShiftColor = gem3Item.colorShiftColor;		
};
gem3Image.inheritFunctionsFromSuperClass("gemImage");

//
// Yellow gem.
//

datablock ItemData(gem4Item : gem1Item)
{
	shapeFile = "./models/gem/gem4.dts";
	uiName = "Yellow Gem";

	doColorShift = true;
	colorShiftColor = "1 0.83 0.51 1";

	image = gem4Image;
};
gem4Item.inheritFunctionsFromSuperClass("gem1Item");

datablock ShapeBaseImageData(gem4Image : gem1Image)
{
    shapeFile = "./models/gem/gem4.dts";
    item = gem4Item;

    doColorShift = gem4Item.doColorShift;
    colorShiftColor = gem4Item.colorShiftColor;	
};
gem4Image.inheritFunctionsFromSuperClass("gemImage");

//
// Purple gem.
//

datablock ItemData(gem5Item : gem1Item)
{
	shapeFile = "./models/gem/gem5.dts";
	uiName = "Purple Gem";

	doColorShift = true;
	colorShiftColor = "0.78 0.51 1 1";

	image = gem5Image;
};
gem5Item.inheritFunctionsFromSuperClass("gem1Item");

datablock ShapeBaseImageData(gem5Image : gem1Image)
{
    shapeFile = "./models/gem/gem5.dts";
    item = gem5Item;

    doColorShift = gem5Item.doColorShift;
    colorShiftColor = gem5Item.colorShiftColor;	
};
gem5Image.inheritFunctionsFromSuperClass("gemImage");