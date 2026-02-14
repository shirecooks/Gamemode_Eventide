//
// Item and image data.
//

datablock ItemData(medpackItem)
{
    category = "Weapon";
    className = "Weapon";
    
    shapeFile = "./models/medpack/medpack.dts";
    rotate = false;
    mass = 1;
    density = 0.2;
    elasticity = 0.2;
    friction = 0.6;
    emap = true;
    
    uiName = "Medpack";
    iconName = "./icons/icon_medpack";
    doColorShift = false;
    
    image = medpackImage;
    canDrop = true;
    l4ditemtype = "heal_full";
};

datablock ShapeBaseImageData(medpackImage)
{
    shapeFile = medpackItem.shapeFile;
    emap = true;
    mountPoint = 0;
    offset = "0 0 0";
    eyeOffset = 0;
    rotation = eulerToMatrix("0 0 0");
    
    className = "WeaponImage";
    item = medpackItem;
    
    armReady = true;
    doColorShift = false;
    
    stateName[0]					= "Activate";
    stateScript[0]					= "onActivate";
    stateTimeoutValue[0]			= 0.15;
    stateSequence[0]				= "Ready";
    stateTransitionOnTimeout[0]		= "Ready";

    stateName[1]					= "Ready";
    stateAllowImageChange[1]		= true;
    stateScript[1]					= "onReady";
    stateTransitionOnTriggerDown[1]	= "Begin";
    
    stateName[2] = "Begin";
    stateScript[2] = "onBegin";
    stateTimeoutValue[2] = 0.01;
    stateTransitionOnTimeout[2] = "HealLoop";

    stateName[3] = "HealLoop";
    stateScript[3] = "onHealLoop";
    stateTransitionOnAmmo[3] = "Complete";
    stateAllowImageChange[3] = false;
    stateTimeoutValue[3] = 0.1;
    stateTransitionOnTimeout[3] = "HealLoop";

    stateName[4] = "Cancel";
    stateScript[4] = "onCancel";
    stateTimeoutValue[4] = 0.01;
    stateTransitionOnTimeout[4] = "Ready";

    stateName[5] = "Complete";
    stateScript[5] = "onComplete";

    healTime = 4.0;
};

function medpackImage::onMount(%this, %obj, %slot)
{
    Parent::onMount(%this, %obj, %slot);
    %obj.setImageAmmo(%this.mountPoint, false);
    %obj.playThread(1, armReadyBoth);
    serverPlay3D("medpack_deploy1_sound", %obj.getHackPosition());
}

//
// The healing process, step-by-step.
//

function medpackImage::onBegin(%this, %obj)
{
    %obj.lastHealTime = getSimTime();
    %obj.isHealing = true;

    %obj.playAudio(3, "medpack_use1_sound");
    %obj.createCameraOrbit(true);
}

function medpackImage::onHealLoop(%this, %obj)
{
    if(%obj.getState() $= "Dead")
    {
        return;
    }

    %timeSinceStart = (getSimTime() - %obj.lastHealTime);

    //Play the healing animation and update the player's UI only once per second.
    %oneSecondInterval = (%timeSinceStart % 1000) < 200;
    if(%oneSecondInterval)
    {
        %obj.playThread(2, "activate2");

        %client = %obj.client;
        if(%client)
        {
            %ticksSinceStart = mFloor(%timeSinceStart / 1000);
            %totalHealTicks = mFloor(%this.healTime);

            %client.printCounterFormatString(%ticksSinceStart, %totalHealTicks);
        }
    }

    //If the player has been healing for the time specified by `healTime`, restore their health and remove the medkit.
    if(%timeSinceStart > (%this.healTime * 1000))
    {
        %obj.setImageAmmo(%this.mountPoint, true);
        return;
    }
}

function medpackImage::onComplete(%this, %obj)
{
    %obj.setDamageLevel(0.0);
    %obj.removeItemFromInventory();
    serverPlay3D("health_restored_sound", %obj.getHackPosition());
}

//
// If the player is interrupted during healing. Only possible through being damaged at the moment.

function medpackImage::onCancel(%this, %obj)
{
    %obj.playAudio(3, "medpack_abort_sound");
    %obj.isHealing = false;
    %obj.restoreCameraFromOrbit(true);
}

function medpackImage::onUnMount(%this, %obj, %slot)
{
    Parent::onUnMount(%this, %obj, %slot);
    if(%obj.isHealing)
    {
        %this.onCancel(%obj);
    }
}

//
// Package for the damage interrupt feature.
//

package Item_Medpack
{
    function Armor::onDamage(%this, %obj, %delta)
    {
        if(%obj.isHealing && %obj.getMountedImage(medpackImage.mountPoint) == medpackImage.getID())
        {
            %obj.unmountImage(medpackImage.mountPoint);
            %obj.mountImage(medpackImage, medpackImage.mountPoint);
        }
        Parent::onDamage(%this, %obj, %delta);
    }
};
if(isPackage(Item_Medpack))
{
    deactivatePackage(Item_Medpack);
}
activatePackage(Item_Medpack);