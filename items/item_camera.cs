//
// Discarded soda can effect data.
//

datablock DebrisData(hoarderCameraDebris)
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
	shapeFile = "./models/camera/camera.dts";
	velocity = 0;
};

datablock ExplosionData(hoarderCameraDebrisExplosion)
{
	debris = hoarderCameraDebris;
	debrisNum = 1;
	debrisPhiMax = 360;
	debrisPhiMin = 180;
	debrisThetaMax = 180;
	debrisThetaMin = 160;
	debrisVelocity = 2;
};

datablock ProjectileData(hoarderCameraProjectile)
{
	explosion = hoarderCameraDebrisExplosion;
};

//
// Item and image data.
//

datablock ItemData(hoarderCameraItem)
{
	shapeFile = "./models/camera/camera.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;
	
	uiName = "Camera";
	iconName = "./icons/icon_camera";
	doColorShift = false;
	
	image = hoarderCameraImage;
	canDrop = true;
};

datablock ShapeBaseImageData(hoarderCameraImage)
{
	shapeFile = "./models/camera/camera.dts";
	emap = false;
	mountPoint = 0;
	offset = "-0.1 0.0 0.0";
	eyeOffset = 0;
	rotation = eulerToMatrix("0 0 0");
	
	className = "WeaponImage";
	item = hoarderCameraItem;
	
	armReady = true;
	doColorShift = false;
	
	//The soda has been equipped.
    stateName[0] = "Activate";
    stateTimeoutValue[0] = 0.01;
    stateTransitionOnTimeout[0]	= "Ready";

	//The soda is inactive, simply being held.
    stateName[1] = "Ready";
    stateScript[1] = "onReady";
	stateAllowImageChange[1] = true;
    stateTransitionOnTriggerDown[1]	= "Flash";

	//Activate the status effect, prepare to discard the camera.
	stateName[2] = "Flash";
	stateScript[2] = "onFlash";
	stateAllowImageChange[2] = false;
	stateWaitForTimeout[2] = true;
	stateTimeOutValue[2] = 0.5;
	stateTransitionOnTimeout[2] = "Discard";

    //Remove the camera from the player's inventory, spawn debris.
	stateName[8] = "Discard";
	stateScript[8] = "onDiscard";
	stateAllowImageChange[8] = false;
	stateWaitForTimeout[8] = false;
};

function hoarderCameraImage::getHintMessage(%this, %obj)
{
	return "Blind enemies for a second or so. Make sure they're looking your way.";
}

//
// Sequence callbacks, animations.
//

function hoarderCameraImage::onFlash(%this, %obj)
{
    %obj.playThread(2, "shiftRight");
    
    //Play the sound effect.
    %soundPitch = getRandom(50, 200);
    %oldTimescale = getTimescale();
    setTimescale((%soundPitch * 0.01) * %oldTimescale);
    serverPlay3D("camera_flash_sound", %obj.getPosition()); //The camera will be laying at the player's feet shortly.
    setTimescale(%oldTimescale);

    //Blind any hunter or their stooges.
    initContainerRadiusSearch(%obj.getPosition(), 15, $TypeMasks::PlayerObjectType);
    while(%nearbyPlayer = containerSearchNext()) 
    {
        %isEnemy = minigameCanDamage(%obj, %nearbyPlayer);

        //Check if the player's flashlight muzzle is in view of the killer.
        %enemyEye = %nearbyPlayer.getEyeVector();
        %enemyPosition = %nearbyPlayer.getPosition();
        %flashlightMuzzle = %obj.getMuzzlePoint($LeftHandSlot);

        //Draw a line between us and the killer.
        %line = VectorNormalize(VectorSub(%flashlightMuzzle, %enemyPosition));
        //Compare the killer's eye to the line.
        %dot = VectorDot(%enemyEye, %line);

        if(%player == %obj || %dot < 0.7)
        {
            //Don't blind the flashlight owner, or someone who isn't looking at the flashlight.
            continue; 
        }

        %nearbyPlayer.setWhiteOut(%isEnemy ? 1.0 : 0.375);
		%nearbyPlayer.setEnergyLevel(%isEnemy ? 0 : 80);
    }

    //Create the camera flash.
    %cameraFlash = new fxLight() 
    { 
        datablock = "brightLight"; 
    };
    %cameraFlash.setTransform(%obj.getMuzzlePoint(0));
    %cameraFlash.schedule(50, delete);

    //Remove the item from the player's inventory, but don't unmount so we can finish the animation.
    %obj.removeItemFromInventory("", true);
}

function hoarderCameraImage::onDiscard(%this, %obj)
{
    //Play a camera dropping animation.
    %obj.playThread(0, shiftTo);
    %obj.playThread(2, plant);

    //Lower the arm again.
	%obj.playThread(1, root);

	//Spawn a debris camera for the cool effect.
    %rightVector = VectorCross(%obj.getForwardVector(), "0 0 1");
    %pelvisPosition = VectorAdd(%obj.getHackPosition(), "0 0 -0.5");
    %rightHandLocation = VectorAdd(%pelvisPosition, VectorScale(%rightVector, 1));
    new Projectile()
	{
		dataBlock = hoarderCameraProjectile;
		initialPosition = %rightHandLocation;
	}.explode();

    //Remove the leftover image from the player's hand and communicate to the client.
    %obj.unmountImage(%slot);
}