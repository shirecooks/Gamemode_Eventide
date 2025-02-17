datablock ItemData(DCamera)
{
	shapeFile = "./models/DCamera/DCamera.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;
	
	uiName = "Camera";
	iconName = "./icons/icon_camera";
	doColorShift = false;
	
	image = DCameraImage;
	canDrop = true;
};

datablock ExplosionData(DCameraExplosion)
{
   explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionSphere1.dts";
   lifeTimeMS = 150;

   explosionScale = "1 1 1";

   lightStartRadius = 0;
   lightEndRadius = 0;
   impulseRadius = 0;
   impulseForce = 0;
   damageRadius = 0;
   radiusDamage = 0;

   uiName = "DCamera";
};

datablock ShapeBaseImageData(DCameraImage)
{
	shapeFile = "./models/DCamera/DCamera.dts";
	emap = false;
	mountPoint = 0;
	offset = "-0.1 0.0 0.0";
	eyeOffset = 0;
	rotation = eulerToMatrix("0 0 0");
	
	className = "WeaponImage";
	item = DCamera;
	
	armReady = true;
	doColorShift = false;
	
	stateName[0]                     = "Equip";
    stateTimeoutValue[0]             = 0;
    stateTransitionOnTriggerDown[0] = "Initiate";
    stateSound[0]					= weaponSwitchSound;

    stateName[1]                = "Initiate";    
    stateTimeoutValue[1]        = 0;
    stateTransitionOnTimeout[1] = "Wait";
    stateScript[1]              = "onInitiate";

    stateName[2]                = "Wait";    
    stateTimeoutValue[2]        = 0.1;
    stateTransitionOnTimeout[2] = "Detonate";

    stateName[3]                = "Detonate";    
	stateExplosion[3]			= DCameraExplosion;
    stateTimeoutValue[3]        = 0;
    stateScript[3]              = "onDetonate";
};

function DCameraImage::onInitiate(%this, %obj, %slot)
{
    %obj.playthread(2,"shiftRight");
}

function DCameraImage::onDetonate(%this, %obj, %slot)
{
	%obj.unmountImage(%slot);
    
    %soundpitch = getRandom(50,200);
    $oldTimescale = getTimescale();
    setTimescale((%soundpitch*0.01) * $oldTimescale);
    serverPlay3D("camera_flash_sound",%obj.getPosition());
    setTimescale($oldTimescale);

    // Flash nearby players
    initContainerRadiusSearch(%obj.getPosition(), 15, $TypeMasks::PlayerObjectType);
    while (%nearbyplayer = containerSearchNext()) 
    {                
        %nearbyplayer.setwhiteout(%nearbyplayer.getDataBlock().isKiller ? 4 : 0.375); // Flash nearby players
    }

    //Create camera light
    %cameralight = new fxLight() { datablock = "brightLight"; };
    %cameralight.setTransform(%obj.getMuzzlePoint(0));
    %cameralight.schedule(50,delete);

    // Remove camera from inventory
    if(isObject(%obj.client))
    {
        for(%i = 0; %i <= %obj.getDataBlock().maxTools; %i++) 
        {
            if(%obj.tool[%i] $= %this.item.getID()) 
            {            
                %obj.tool[%i] = 0;
                messageClient(%obj.client,'MsgItemPickup','',%i,0);
                break; 
            }
        }          
    }    
}

function DCameraImage::onMount(%this,%obj,%slot)
{
	%obj.playThread(0,"armReady");
}

function DCameraImage::onUnMount(%this,%obj,%slot)
{
	%obj.playThread(0,"root");
}

