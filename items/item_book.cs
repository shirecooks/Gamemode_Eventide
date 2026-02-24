datablock ItemData(bookItem : ritualItem)
{
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
bookItem.inheritFunctionsFromSuperClass("ritualItem");

datablock ShapeBaseImageData(bookImage : ritualImage)
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
bookImage.inheritFunctionsFromSuperclass("ritualImage");

function bookImage::onMount(%this, %obj)
{    
    Parent::onMount(%this, %obj);

    %obj.playThread(1, armReadyBoth);
}