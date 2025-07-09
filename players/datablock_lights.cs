datablock fxLightData(NegativePlayerLight)
{
	uiName = "Player\'s Negative Light";
	LightOn = 1;
	radius = 15;
	Brightness = -5;
	color = "1 1 1 1";
	FlareOn = 0;
	FlareTP = 1;
	FlareBitmap = "base/lighting/corona";
	FlareColor = "1 1 1";
	ConstantSizeOn = 1;
	ConstantSize = 1;
	NearSize = 3;
	FarSize = 0.5;
	NearDistance = 10;
	FarDistance = 30;
	FadeTime = 0.1;
	BlendMode = 0;
};

datablock fxLightData(NoFlareGLight)
{
	uiName = "No Flare Green";
	LightOn = true;
	radius = 15;
	brightness = 5;
	color = "0.1 1 0.1";
	FlareOn			= false;
	FlareTP			= false;
	Flarebitmap		= "";
	FlareColor		= "1 1 1";
	ConstantSizeOn	= false;
	ConstantSize	= 1;
	NearSize		= 1;
	FarSize			= 0.5;
	NearDistance	= 10.0;
	FarDistance		= 30.0;
	FadeTime		= 0.1;
};
datablock fxLightData(NoFlareRLight : NoFlareGLight)
{
	uiName = "No Flare Red";
	color = "1 0.1 0.1";
};
datablock fxLightData(NoFlarePLight : NoFlareGLight)
{
	uiName = "No Flare Purple";
	color = "1 0.05 0.5";
};
datablock fxLightData(NoFlareYLight : NoFlareGLight)
{
	uiName = "No Flare Yellow";
	color = "1 1 0.1";
};
datablock fxLightData(NoFlareBLight : NoFlareGLight)
{
	uiName = "No Flare Blue";
	color = "0.1 0.1 1";
};
