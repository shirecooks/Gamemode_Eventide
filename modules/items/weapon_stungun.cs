datablock ItemData(StunGun)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/glitchgun.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Stun Gun";
	iconName = "./icons/icon_glitchgun";
	doColorShift = true;
	colorShiftColor = "0.0 1.0 1.0 1.0";
	image = StunGunImage;
	canDrop = true;
};

datablock ShapeBaseImageData(StunGunImage)
{
    shapeFile = "./models/glitchgun.dts";
    emap = true;
    mountPoint = 0;
    offset = "0 0 0";
    eyeoffset = "0";
    rotation = "0 0 0";

    correctMuzzleVector = true;
    className = "WeaponImage";
    item = StunGun;

    projectile = "";
    casing = "";
    
    armReady = true;

    doColorShift = true;
    colorShiftColor = StunGun.colorShiftColor;

    stateName[0]                     = "Equip";
    stateTimeoutValue[0]             = 0;
    stateTransitionOnTriggerDown[0] = "Initiate";
    stateSound[0]					= weaponSwitchSound;

    stateName[1]                = "Initiate";    
    stateTimeoutValue[1]        = 0;
    stateTransitionOnTimeout[1] = "Wait";
    stateSound[1]               = "beep_key_sound";
    stateScript[1]              = "onInitiate";

    stateName[2]                = "Wait";    
    stateTimeoutValue[2]        = 1;
    stateTransitionOnTimeout[2] = "Detonate";

    stateName[3]                = "Detonate";    
    stateTimeoutValue[3]        = 0;
    stateSound[3]               = "flaregun_dryfire_sound";
    stateScript[3]              = "onDetonate";
};

function StunGunImage::onInitiate(%this, %obj, %slot)
{
    %obj.playthread(2,"shiftRight");
}

function StunGunImage::onDetonate(%this, %obj, %slot)
{
	serverPlay3D("stungun_fire_sound",%obj.getPosition());

    for(%i = 0; %i <= %obj.getDataBlock().maxTools; %i++)
	if(%obj.tool[%i] $= %this.item.getID()) %itemslot = %i;

    if(isObject(%obj.client))
    {
        %obj.tool[%itemslot] = 0;
        messageClient(%obj.client,'MsgItemPickup','',%itemslot,0);
    }
    if(isObject(%obj.getMountedImage(%this.mountPoint))) %obj.unmountImage(%this.mountPoint);     

    for(%i = 0; %i < clientgroup.getCount(); %i++)//Can't use container radius search anymore :(
    {        
        if(isObject(%nearbyplayer = clientgroup.getObject(%i).player))
        {    
            if(%nearbyplayer.getClassName() !$= "Player" || VectorDist(%nearbyplayer.getPosition(), %obj.getPosition()) > 15) 
            continue;         
                           
            %obj.setwhiteout(1);

            if(%nearbyplayer == %obj) continue;

            if(%nearbyplayer.getDataBlock().isKiller) %nearbyplayer.mountimage("sm_stunImage",3);                        
        }
    }      
}