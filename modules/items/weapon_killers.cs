datablock ShapeBaseImageData(meleeTantoImage)
{
   	// Basic Item properties
   	shapeFile = "./models/Katana.dts";
   	emap = true;
   	mountPoint = 0;
   	offset = "0 0 0";
   	eyeOffset = 0;
   	rotation = eulerToMatrix( "0 0 0" );
   	correctMuzzleVector = true;
   	className = "WeaponImage";
   	item = "";
   	ammo = "";
   	projectile = "";
   	projectileType = Projectile;
   	melee = false;
   	armReady = true;
   	doColorShift = false;

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 1;
	stateTransitionOnTimeout[0]       = "Ready";
	stateScript[0]                  = "onAim";
	stateSound[0]					= weaponSwitchSound;

	stateName[1]                     = "Ready";
	stateTimeoutValue[1]             = 3;
	stateTransitionOnTimeout[1]       = "ReadyDown";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateWaitForTimeout[1]			= false;
	stateAllowImageChange[1]         = true;
	stateSequence[1]	= "Ready";

	stateName[7]                     = "ReadyDown";
	stateSound[7]					= weaponSwitchSound;
	stateTransitionOnTriggerDown[7]  = "AimBeat";
	stateAllowImageChange[7]         = true;
	stateScript[7]                  = "onDrop";
	stateSequence[7]	= "Ready";

	stateName[2]                    = "Fire";
	stateTransitionOnTimeout[2]     = "Smoke";
	stateTimeoutValue[2]            = 0.15;
	stateScript[2]                  = "onFireDown";
	stateWaitForTimeout[2]			= true;
   	stateSequence[2]                = "Fire";

	stateName[3] 			= "Smoke";
	stateScript[3]                  = "onFire";
	stateFire[3]                    = true;
	stateAllowImageChange[3]        = false;
	stateSequence[3]                = "Fire";
	stateTimeoutValue[3]            = 0.15;
	stateSound[3]			= "";
	stateTransitionOnTimeout[3]     = "CoolDown";

   	stateName[5] = "CoolDown";
   	stateTimeoutValue[5]            = 0.9;
	stateTransitionOnTimeout[5]     = "Reload";
   	stateSequence[5]                = "clickDown";

	stateName[4]			= "Reload";
	stateTransitionOnTriggerUp[4]     = "Ready";
	stateSequence[4]	= "TrigDown";

   	stateName[6]   = "NoAmmo";
   	stateTransitionOnAmmo[6] = "Ready";

	stateName[8]                     = "AimBeat";
	stateTimeoutValue[8]             = 0.05;
	stateTransitionOnTimeout[8]       = "Fire";
	stateAllowImageChange[8]         = true;
	stateScript[8]                  = "onAim";
	stateSequence[8]	= "clickDown";
};

datablock ParticleData(kidsHammerParticle)
{
	dragCoefficient      = 0;
	gravityCoefficient   = -1;
	inheritedVelFactor   = 0;
	constantAcceleration = -2.0;
	spinRandomMin        = -300;
	spinRandomMax        = 300;
	lifetimeMS           = 1000;
	lifetimeVarianceMS   = 250;
	textureName          = "./models/ban.png";
	colors[0]            = "1 0 0 0";
	colors[1]            = "1 1 1 1";
	colors[2]            = "1 0 0 0.5";
	colors[3]            = "1 0 0 0";
	sizes[0]             = 0.1;
	sizes[1]             = 1.5;
	sizes[2]             = 0.7;
	sizes[3]             = 0.2;
	times[0]             = 0;
	times[1]             = 0.4;
	times[2]             = 0.8;
	times[3]             = 1;
};
datablock ParticleEmitterData(kidsHammerParticleEmitter)
{
	ejectionPeriodMS = 200;
	periodVarianceMS = 100;
	ejectionVelocity = 5;
	velocityVariance = 0;
	ejectionOffset   = 0.4;
	thetaMin         = 5;
	thetaMax         = 25;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance  = false;
	particles        = "kidsHammerParticle";

	uiName = "Kid's Hammer Emitter";
};

datablock ShapeBaseImageData(meleeMacheteImage : meleeTantoImage)
{
   shapeFile = "./models/machete.dts";
   mountPoint = 0;
};

datablock ShapeBaseImageData(defaultswordImage : meleeTantoImage)
{
   shapeFile = "./models/defaultsword.dts";
   mountPoint = 0;
};

datablock ShapeBaseImageData(meleeKnifeImage : meleeTantoImage)
{
   shapeFile = "./models/knife.dts";
   mountPoint = 0;
};

datablock ShapeBaseImageData(meleePuppetMasterDaggerImage : meleeTantoImage)
{
   shapeFile = "./models/puppetmasterdagger.dts";
   mountPoint = 0;
};

datablock ShapeBaseImageData(meleeAxeImage : meleeTantoImage)
{
   shapeFile = "./models/Huntress Axe Temp.dts";
};

datablock ShapeBaseImageData(meleeAxeLImage : meleeTantoImage)
{
   shapeFile = "./models/Huntress Axe Temp.dts";
   mountPoint = 1;
};

datablock ShapeBaseImageData(ShovelImage : meleeTantoImage)
{
   shapeFile = "./models/Shovel.dts";
   mountPoint = 1;
};

datablock ShapeBaseImageData(blackKnifeImage : meleeTantoImage)
{
	shapeFile = "./models/BlackKnife.dts";
	mountPoint = 1;

	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0.5;
	stateTransitionOnTimeout[0] = "Ready";
	stateSound[0] = "";

	stateName[1] = "Ready";
	stateTransitionOnAmmo[1] = "Fading";
	stateTransitionOnNoAmmo[1] = "Visible";
	stateSound[1] = "";

	stateName[2] = "Visible";
	stateSequence[2] = "Root";
	stateTransitionOnAmmo[2] = "Fading";
	stateSound[2] = "";

	stateName[3] = "Fading";
	stateSequence[3] = "fade";
	stateTimeoutValue[3] = 0.2;
	stateTransitionOnTimeout[3] = "Faded";
	stateSound[3] = "";

	stateName[4] = "Faded";
	stateSequence[4] = "faded";
	stateTransitionOnNoAmmo[4] = "Unfading";
	stateSound[4] = "";

	stateName[5] = "Unfading";
	stateSequence[5] = "unfade";
	stateTimeoutValue[5] = 0.2;
	stateTransitionOnTimeout[5] = "Visible";
	stateSound[5] = "";
};

datablock ShapeBaseImageData(kidsHammerImage : meleeTantoImage)
{
	shapeFile = "base/data/shapes/hammer.dts";
	doColorShift = true;
	colorShiftColor = "1 0 0 1";
	mountPoint = $RightHandSlot;

	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0.15;
	stateTransitionOnTimeout[0]       = "Ready";
	stateEmitter[0] = kidsHammerParticleEmitter;
	stateEmitterTime[0] = 600;

	stateName[1] = "Ready";
	stateAllowImageChange[1] = true;
	stateSequence[1] = "Ready";
};

datablock ShapeBaseImageData(ZweihanderImage : meleeTantoImage)
{
   shapeFile = "./models/zweihander.dts";
   mountPoint = 0;
   doColorShift = true;
   colorShiftColor = (100/255) SPC (100/255) SPC (100/255) SPC (255/255);
};


function meleeTantoImage::onFire(%this,%obj,%slot)
{
	return;
}

function meleePuppetMasterDaggerImage::onFire(%this,%obj,%slot)
{
	meleeTantoImage::onFire(%this,%obj,%slot);
}

function meleeAxeImage::onFire(%this,%obj,%slot)
{
	meleeTantoImage::onFire(%this,%obj,%slot);
}

function meleeMacheteImage::onFire(%this,%obj,%slot)
{
	meleeTantoImage::onFire(%this,%obj,%slot);
}

function ZweihanderImage::onFire(%this,%obj,%slot)
{
	meleeTantoImage::onFire(%this,%obj,%slot);
}