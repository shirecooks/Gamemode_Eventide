datablock fxLightData(candleLight)
{
	uiName = "Candle Light";

	LightOn = true;
	radius = 7.5;
	brightness = 1;
	color = "1 0.75 0 1";

	FlareOn			= true;
    FlareBitmap = "./particles/candle_lightFlare";
    FlareColor = "0.375 0.3 0";
	AnimColor		= false;
	AnimBrightness	= true;
	AnimOffsets		= true;
	AnimRotation	= false;
	LinkFlare		= false;
	LinkFlareSize	= true;
	MinColor		= "1 1 0";
	MaxColor		= "0 0 1";
	MinBrightness	= 1.0;
	MaxBrightness	= 6.0;
	MinRadius		= 0.1;
	MaxRadius		= 10;
	StartOffset		= "0 0 0";
	EndOffset		= "0 0 0";
	MinRotation		= 0;
	MaxRotation		= 359;

	SingleColorKeys	= false;
	RedKeys			= "AWTCFAH";
	GreenKeys		= "AWTCFAH";
	BlueKeys		= "AWTCFAH";
	
	BrightnessKeys	= "DEDEDFGF";
	RadiusKeys		= "AZAAAAA";
	OffsetKeys		= "AZAAAAA";
	RotationKeys	= "AZAAAAA";

	ColorTime		= 1.0;
	BrightnessTime	= 1.0;
	RadiusTime		= 1.0;
	OffsetTime		= 1.0;
	RotationTime	= 1.0;

	LerpColor		= true;
	LerpBrightness	= true;
	LerpRadius		= true;
	LerpOffset		= false;
	LerpRotation	= false;

	lightOffset = "0 0 1.125";
};

datablock ItemData(candleItem : ritualItem)
{
	class = "candleItem";
	superClass = "ritualItem";

	shapeFile = "./models/candle/candle.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

	uiName = "Candle";
	iconName = "./icons/icon_candle";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";

	image = candleImage;

	emitterDatablock = "brickDeployExplosionEmitter";

	ritualType = "Candle";
	maxRitualsOnCircle = 4;
	possibleOffset1 = "0 -3.5 0.375 0 0 0";
	possibleOffset2 = "0 3.5 0.375 0 0 0";
	possibleOffset3 = "-3.25 1.1 0.375 0 0 0";
	possibleOffset4 = "3.25 1.1 0.375 0 0 0";
};
candleItem.inheritFunctionsFromSuperClass();

datablock ShapeBaseImageData(candleImage)
{
    shapeFile = candleItem.shapeFile;

    mountPoint = 0;
    offset = "0 0 0";
    correctMuzzleVector = false;
    eyeOffset = "0 0 0";

	item = candleItem;
    ammo = " ";
    projectile = "";
    projectileType = Projectile;
    armReady = true;

    doColorShift = candleItem.doColorShift;
    colorShiftColor = candleItem.colorShiftColor;

    stateName[0] = "Activate";
};

function candleItem::placeOnRitualCircle(%this, %obj, %circle)
{
	%result = %this.super("placeOnRitualCircle", %this, %obj, %circle);

	if(%result)
	{
		//Light the candle after a short delay.
		%this.schedule(500, createLight, %obj, candleLight, "0 0 1.125");
	}
	
	return %result;
}