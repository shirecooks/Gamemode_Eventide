datablock DebrisData(RumBottleCorkDebris)
{
	shapeFile = "./models/cork.dts";
	lifetime = 5.0;
	minSpinSpeed = -400.0;
	maxSpinSpeed = 200.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 3;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;
	gravModifier = 2;
};
datablock ParticleData(drunkBuffGlowParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.0;
	inheritedVelFactor   = 0.15;
	constantAcceleration = 0.0;
	lifetimeMS           = 200;
	lifetimeVarianceMS   = 0;
	textureName          = "Add-ons/Projectile_Pong/square";
	spinSpeed		   = 0;
	spinRandomMin		= -500;
	spinRandomMax		= 500;
	colors[0]     = "0.5 0.5 1 0.2";
	colors[1]     = "0.6 0.6 1 0.1";
	colors[2]     = "0.7 0.7 1 0";

	sizes[0]      = 0.7;
	sizes[1]      = 2;
	sizes[2]      = 0.3;

	times[0] = 0.0;
	times[1] = 0.25;
	times[2] = 1;

	useInvAlpha = false;
};
datablock ParticleData(drunkBuffEmitParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.0;
	inheritedVelFactor   = 0.15;
	constantAcceleration = 0.0;
	lifetimeMS           = 200;
	lifetimeVarianceMS   = 0;
	textureName          = "Add-ons/Projectile_Pong/square";
	spinSpeed		   = 0.0;
	spinRandomMin		= 0.0;
	spinRandomMax		= 0.0;
	colors[0]     = "0.5 0.5 1 0.2";
	colors[1]     = "0.6 0.6 1 0.1";
	colors[2]     = "0.7 0.7 1 0";

	sizes[0]      = 0.3;
	sizes[1]      = 0.6;
	sizes[2]      = 0.1;

	times[0] = 0.0;
	times[1] = 0.25;
	times[2] = 1;

	useInvAlpha = false;
};
datablock ParticleEmitterData(drunkBuffGlowEmitter)
{
	ejectionPeriodMS = 15;
	periodVarianceMS = 1;
	ejectionVelocity = 0;
	velocityVariance = 0.0;
	ejectionOffset   = 0.1;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = drunkBuffGlowParticle;

	useEmitterColors = false;
};
datablock ParticleEmitterData(drunkBuffEmitEmitter)
{
	ejectionPeriodMS = 5;
	periodVarianceMS = 1;
	ejectionVelocity = 7;
	velocityVariance = 3;
	ejectionOffset   = 0.5;
	thetaMin         = 0;
	thetaMax         = 90;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = drunkBuffEmitParticle;

	useEmitterColors = false;
};
datablock ShapeBaseImageData(drunkBuffGlowImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = $HeadSlot;
	offset = "0 0 -0.65";
	eyeOffset = "0 0 999";

	stateName[0]				= "Ready";
	stateTimeoutValue[0]		= 0.01;
	stateTransitionOnTimeout[0]	= "FireA";

	stateName[1]				= "FireA";
	stateEmitter[1]				= drunkBuffGlowEmitter;
	stateEmitterTime[1]			= 999;
	stateTimeoutValue[1]		= 999;
	stateTransitionOnTimeout[1]	= "Done";
	stateWaitForTimeout[1]		= true;

	stateName[2]				= "Done";
	stateTimeoutValue[2]		= 0.01;
	stateTransitionOnTimeout[1]	= "FireA";
};
datablock ShapeBaseImageData(drunkBuffEmitImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = $HeadSlot;
	offset = "0 0 -0.65";
	eyeOffset = 0;

	stateName[0]				= "Ready";
	stateTimeoutValue[0]		= 0.01;
	stateTransitionOnTimeout[0]	= "FireA";

	stateName[1]				= "FireA";
	stateEmitter[1]				= drunkBuffEmitEmitter;
	stateEmitterTime[1]			= 999;
	stateTimeoutValue[1]		= 999;
	stateTransitionOnTimeout[1]	= "Done";
	stateWaitForTimeout[1]		= true;

	stateName[2]				= "Done";
	stateTimeoutValue[2]		= 0.01;
	stateTransitionOnTimeout[1]	= "FireA";
};
datablock ItemData(RumBottleItem)
{
	category = "Weapon";
	className = "Weapon";
	
	shapeFile = "./models/rum/bottle.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	
	uiName = "Pirate's Rum";
	iconName = "./models/rum/icon_rum";
	doColorShift = true;
	colorShiftColor = "0.392 0.196 0 1";
	
	image = RumBottleImage;
	canDrop = true;
};
datablock ShapeBaseImageData(RumBottleImage)
{
	shapeFile = "./models/rum/bottle.dts";
	emap = true;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = eulerToMatrix( "0 0 0" );
	
	className = "WeaponImage";
	item = RumBottleItem;
	
	armReady = true;
	
	doColorShift = RumBottleItem.doColorShift;
	colorShiftColor = RumBottleItem.colorShiftColor;
	
	casing = RumBottleCorkDebris;
	shellExitDir		= "1.0 1.0 1.0";
	shellExitOffset		= "0 0 0";
	shellExitVariance	= 5;	
	shellVelocity		= 10;
	
	stateName[0]					= "Activate";
	stateSound[0]					= weaponSwitchSound;
	stateTimeoutValue[0]			= 0.15;
	stateTransitionOnTimeout[0]		= "Ready";

	stateName[1]					= "Ready";
	stateSequence[1]				= "Ready";
	stateAllowImageChange[1]		= true;
	stateTransitionOnTriggerDown[1]	= "Use";
	
	stateName[2]					= "Use";
	stateScript[2]					= "onUse";
	stateTransitionOnTriggerUp[2]	= "Done";
	
	stateName[3]					= "Done";
	stateTransitionOnAmmo[3]		= "Ready";
	stateTransitionOnNoAmmo[3]		= "UnUse";
	
	stateName[4]					= "UnUse";
	stateEjectShell[4]				= true;
	stateSound[4]					= "bottle_pop";
	stateScript[4]					= "onUnUse";
	stateSequence[4]				= "Activate";
	stateTimeoutValue[4]			= 0.5;
	stateTransitionOnTimeout[4]		= "Hack";
	stateWaitForTimeout[4]			= true;
	
	stateName[5]					= "Hack";
	stateScript[5]					= "onHack";
};
function RumBottleImage::onUnUse(%this, %obj, %slot)
{
	messageClient(%obj.client,'MsgItemPickup','',%obj.currTool,0);
	%obj.tool[%obj.currTool] = 0;
	%obj.weaponCount--;
	%obj.playThread(2, "shiftAway");
	schedule(500,0,rumBuff,%obj);
}
function RumBottleImage::onHack(%this, %obj, %slot)
{
	%obj.playThread(1, "root");
	%obj.schedule(32, "unMountImage", %slot);
	serverCmdUnUseTool(%obj.client);
}
function RumBottleImage::onUse(%this, %obj, %slot)
{
	if(!%obj.rumBuff)
	{
		%obj.setImageAmmo(%slot, false);
	}
	else
	{
		%obj.setImageAmmo(%slot, true);
		messageClient(%obj.client,'',"\c6I think you have already had enough to drink");
	}
}
package swol_rum
{
	function rumBuff(%player)
	{
		if(!isObject(%player))
		return;
		%cl = %player.client;
		%player.rumBuff = 1;
		%player.rumStep = $PI/8;
		rumAnimation(%player,0);
		%player.mountImage(drunkBuffEmitImage,3);
	}
	function rumLoop(%player,%tick)
	{
		%max = 76;
		if(!isObject(%player))
		return;
		cancel(%player.rumLoop);
		%player.setWhiteOut(mClampF(((%max-%tick)+(%max/6))/%max/2,0.3,1));
		%tick++;
		if(%tick > %max)
		{
			rumCooldown(%player);
			return;
		}
		%player.rumStep = ($PI)*(mClampF(%tick/%max,0.4,5));
		commandToClient(%player.client,'SetVignette',false,"1 1 1" SPC (%max-%tick)/%max);
		%player.rumLoop = schedule(125,0,rumLoop,%player,%tick);
	}
	function rumAnimation(%player,%tick)
	{
		if(!isObject(%player))
		return;
		cancel(%player.rumLoop);
		%max = 50;
		%tick++;
		commandToClient(%player.client,'SetVignette',false,"1 1 1" SPC %tick/%max);
		%player.setWhiteOut(mClampF(%tick/%max/5,0,1));
		if(%tick >= %max)
		{
			rumLoop(%player,0);
			%player.rumBuff = 1;
			%player.mountImage(drunkBuffGlowImage,2);
			return;
		}
		%player.rumLoop = schedule(25,0,rumAnimation,%player,%tick);
	}
	function rumCooldown(%player)
	{
		%cl = %player.client;
		%player.rumBuff = 0;
		%player.rumStep = 0;
		%player.unMountImage(2);
		%player.unMountImage(3);
		commandToClient(%cl,'SetVignette',$EnvGuiServer::VignetteMultiply,$EnvGuiServer::VignetteColor);
	}
	function armor::damage(%this,%obj,%sourceObject,%pos,%damage,%damageType)
	{
		if(%obj.rumBuff)
		{
			%damage /= 6*%obj.rumStep;
		}
		parent::damage(%this,%obj,%sourceObject,%pos,%damage,%damageType);
	}
};
activatePackage(swol_rum);