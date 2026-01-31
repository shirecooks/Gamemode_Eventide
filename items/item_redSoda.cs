//
// Discarded soda can effect data.
//

datablock DebrisData(DietSpeedSodaDebris)
{
	elasticity = 0.5;
	gravModifier = 0.9;
	lifeTime = 5;
	maxSpinSpeed = 1000;
	numBounces = 2;
    bounceVariance = 2;
	fade = true;
	snapOnMaxBounce = false;
	staticOnMaxBounce = true;
	shapeFile = "./models/redSoda/sodacan.dts";
	velocity = 0;
};

datablock ExplosionData(DietSpeedSodaDebrisExplosion)
{
	debris = "DietSpeedSodaDebris";
	debrisNum = 1;
	debrisPhiMax = 360;
	debrisPhiMin = 180;
	debrisThetaMax = 180;
	debrisThetaMin = 160;
	debrisVelocity = 2;
};

datablock ProjectileData(DietSpeedSodaProjectile)
{
	explosion = "DietSpeedSodaDebrisExplosion";
};

//
// Image data.
//

//Inconsistent datablock name for compatibility.
datablock ItemData(SodaItem)
{
	category = "Tools";
	className = "Weapon";

	shapeFile = "./models/redSoda/sodacan.dts";
	doColorShift = false;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	canDrop = 1;
	emap = false;
	
	uiName = "Diet Speed Soda";
	iconName = "./icons/icon_redSoda";

	image = redSodaImage;
	canDrop = true;
};

datablock ShapeBaseImageData(redSodaImage)
{
	className = "WeaponImage";

	shapeFile = "./models/redSoda/sodacan.dts";
	emap = false;
	isSpecial = 1;

	mountPoint = 0;
	offset = "-0.01 0.1 0";
	armReady = false;
    
	doColorShift = false;

	item = blueSodaItem;
	ammo = false;
	projectile = "";
	cooldown = 32000;

	//The soda has been equipped.
    stateName[0] = "Activate";
    stateSequence[0] = "ready";
	stateAllowImageChange[0] = false;
    stateTimeoutValue[0] = 0.01;
    stateTransitionOnTimeout[0]	= "Ready";

	//The soda is inactive, simply being held.
    stateName[1] = "Ready";
    stateScript[1] = "onReady";
	stateAllowImageChange[1] = true;
    stateTransitionOnTriggerDown[1]	= "Open";

	//The soda has been opened, prepare to drink.
	stateName[6] = "Open";
	stateScript[6] = "onOpen";
	stateAllowImageChange[6] = false;
	stateWaitForTimeout[6] = true;
	stateTimeOutValue[6] = 1;
	stateTransitionOnTimeout[6] = "Drink";

	//Activate the status effect, prepare to discard the soda can.
	stateName[7] = "Drink";
	stateScript[7] = "onDrink";
	stateAllowImageChange[7] = false;
	stateWaitForTimeout[7] = true;
	stateTimeOutValue[7] = 0.5;
	stateTransitionOnTimeout[7] = "Discard";

    //Remove the soda can from the player's inventory, spawn debris.
	stateName[8] = "Discard";
	stateScript[8] = "onDiscard";
	stateAllowImageChange[8] = false;
	stateWaitForTimeout[8] = false;
};

//
// Sequence callbacks.
//

function redSodaImage::onReady(%this, %obj)
{

}

function redSodaImage::onOpen(%this, %obj)
{
	//Play the sound of the soda opening.
	serverPlay3D("soda_can_open_sound", %obj.getPosition());

	//Raise the can so it is ready to drink.
	%obj.playThread(1, armReadyRight);

	//Play an animation of cracking open the tab.
	%obj.playThread(2, shiftleft);
}

function redSodaImage::onDrink(%this, %obj)
{
	//Play the soda drinking sound and animation.
	serverPlay3D("drink_gulp" @ getRandom(1, 3) @ "_sound", %obj.getPosition());
	%obj.playThread(2, shiftUp);

	//Apply the speed boost status effect.
	%obj.applyStatusEffect("SpeedSodaEffect", "Powerup", 6000);

    //Remove the tool from the player's object, so they can't cancel out and re-equip for infinite speed boosts.
    %obj.removeToolFromInventory("", true);
}

function redSodaImage::onDiscard(%this, %obj)
{
    //Play a can dropping animation.
    %obj.playThread(0, shiftTo);
    %obj.playThread(2, plant);

    //Lower the arm again.
	%obj.playThread(1, root);

	//Spawn a debris soda can for the cool effect.
    %rightVector = VectorCross(%obj.getForwardVector(), "0 0 1");
    %pelvisPosition = VectorAdd(%obj.getHackPosition(), "0 0 -0.5");
    %rightHandLocation = VectorAdd(%pelvisPosition, VectorScale(%rightVector, 1));
    %sodaDebris = new Projectile()
	{
		dataBlock = "DietSpeedSodaProjectile";
		initialPosition = %rightHandLocation;
	};
    %sodaDebris.explode();

    //Remove the leftover image from the player's hand and communicate to the client.
    %obj.unmountImage(%this.mountPoint);
}