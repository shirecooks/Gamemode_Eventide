datablock ItemData(bookItem : ritualItem)
{
	class = "bookItem";
	superClass = "ritualItem";

	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/book/book.dts";
	iconName = "./icons/icon_book";
    uiName = "Book";

	image = bookImage;

	ritualType = "Book";
	maxRitualsOnCircle = 1;
	possibleOffset1 = "0 0.65 0.1 0 0 0";
};
bookItem.inheritFunctionsFromSuperClass();

datablock ShapeBaseImageData(bookImage)
{
    className = "WeaponImage";

    shapeFile = "./models/book/book.dts";
    emap = true;

    mountPoint = $RightHandSlot;
    offset = "-0.5 0 0";
    eyeOffset = "0 0 0";

    item = bookItem;
    ammo = " ";
    projectile = "";
    projectileType = Projectile;

    armReady = true;

    stateName[0] = "Activate";
};

function bookImage::onMount(%this, %obj, %slot)
{    
    Parent::onMount(%this, %obj, %slot);

    %obj.playThread(1,"armReadyBoth");
}