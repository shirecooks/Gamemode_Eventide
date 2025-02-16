datablock fxLightData(CandleLight)
{
	uiName = "Candle Light";

	LightOn = true;
	radius = 7.5;
	brightness = 1;
	color = "1 0.75 0 1";

	FlareOn			= true;
    FlareBitmap = "./models/candleLightFlare";
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
};

datablock ItemData(candleItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/candle.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Candle";
	iconName = "./icons/icon_candle";
	doColorShift = false;
	colorShiftColor = "1 1 1 1";

	image = candleImage;
	canDrop = true;
};

datablock ShapeBaseImageData(candleImage)
{
    shapeFile = candleItem.shapeFile;
    emap = true;

    mountPoint = 0;
    offset = "0 0 0.0";
    correctMuzzleVector = false;
    eyeOffset = "0 0 0";
    className = "WeaponImage";

    item = candleItem;
    ammo = " ";
    projectile = "";
    projectileType = Projectile;

    staticShape = "brickCandleStaticShape";
    isRitual = true;

    melee = true;
    doRetraction = false;
    doColorShift = candleItem.doColorShift;
    colorShiftColor = candleItem.colorShiftColor;

    stateName[0]                     = "Activate";
    stateSound[0]                  = "WeaponSwitchSound";
};

datablock StaticShapeData(brickCandleStaticShape)
{
	isInvincible = true;
	isRitual = true;
	shapeFile = "./models/candlestatic.dts";
	placementSound = "candle_place_sound";
};

function brickCandleStaticShape::Light(%this,%obj)
{   
    if(!isObject(%obj)) return;

    %obj.light = new fxlight()
    {
        dataBlock = "CandleLight";
        enable = true;
    };
    %obj.playAudio(1,"candleignite_sound");
    %obj.light.settransform(vectoradd(%obj.gettransform(),"0 0 1.125") SPC getwords(%obj.gettransform(),3,6));
}

function brickCandleStaticShape::onRemove(%this,%obj)
{
    if(isObject(%obj.light)) %obj.light.delete();
}

function brickCandleStaticShape::onAdd(%this,%obj)
{
    Parent::onAdd(%this,%obj);
	%obj.playaudio(3,%this.placementSound);
    %this.schedule(500,Light,%obj);
}