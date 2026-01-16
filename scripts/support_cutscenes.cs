//
// Functions and a package too lock the player's inputs for cutscenes.
//

function Observer::checkInputsLocked(%this, %obj)
{
    %client = %obj.getControllingClient();
    %player = %client.player;
    if(!%player)
    {
        return false;
    }

    return (%client.lockInputs || (%player && %player.lockInputs));
}

function GameConnection::checkInputsLocked(%client)
{
    %player = %client.player;
    if(!%player)
    {
        return false;
    }

    return (%player.lockInputs || %client.lockInputs);
}

function Player::checkInputsLocked(%obj)
{
    %client = %obj.client;

    return (%obj.lockInputs || (%client && %client.lockInputs));
}

//
//// Lock tools specifically.
function Observer::checkToolsLocked(%this, %obj)
{
    %client = %obj.getControllingClient();
    %player = %client.player;

    return (!%player || %player.lockTools || %client.lockTools);
}

function GameConnection::checkToolsLocked(%client)
{
    %player = %client.player;

    return (!%player || %player.lockTools || %client.lockTools);
}

function Player::checkToolsLocked(%obj)
{
    %client = %obj.client;

    return (!%player || %player.lockTools || (%client && %client.lockTools));
}

package Support_Client
{
    function Observer::onTrigger(%this, %obj, %trigger, %state)
    {
        if(%this.checkInputsLocked(%obj))
        {
            return;
        }

        return Parent::onTrigger(%this, %obj, %trigger, %state);
    }

    function serverCmdUseTool(%client, %slot)
    {
        if(%client.checkInputsLocked() || %client.checkToolsLocked())
        {
            return;
        }

        return parent::ServerCmdUseTool(%client, %slot);
    }

    function ServerCmdUnUseTool(%client)
    {
        if(%client.checkInputsLocked() || %client.checkToolsLocked())
        {
            return;
        }

        parent::ServerCmdUnUseTool(%client);
    }

    function ServerCmdPlantBrick(%client)
    {
        if(%client.checkInputsLocked())
        {
            return;
        }

        return parent::ServerCmdPlantBrick(%client);
    }
};
if(isPackage(Support_Client))
{
    deactivatePackage(Support_Client);
}
activatePackage(Support_Client);

//
// Automated camera orbit functions.
//

function Player::createCameraOrbit(%obj)
{
    %client = %obj.client;
    if(!%client)
    {
        return;
    }

    %camera = %client.camera;
    %client.setControlObject(%camera);
	%camera.setMode("Corpse", %obj);
}

function Player::restoreCameraFromOrbit(%obj)
{
    %client = %obj.client;
    if(!%client)
    {
        return;
    }

    %client.setControlObject(%obj);
	%client.camera.setMode("Observer");
}

//
// Shaky cam, cutscenes and abilities.
//

datablock ExplosionData(camShakeExplosion)
{
   explosionShape = "";
   lifeTimeMS = 150;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = true;
   camShakeFreq = "10.0 10.0 10.0";
   camShakeAmp = "10.0 10.0 10.0";
   camShakeDuration = 0.5;
   camShakeRadius = 0.1;

   damageRadius = 0;
   radiusDamage = 0;

   impulseRadius = 0;
   impulseForce = 0;
};

datablock ProjectileData(camShakeProjectile)
{
	projectileShapeName = "";
	directDamage        = 0;
	directDamageType    = $DamageType::Default;
	radiusDamageType    = $DamageType::Default;
	
	
	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce  = 0;
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;
	
	impactImpulse	   = 0;
	verticalImpulse	   = 0;
	explosion           = camShakeExplosion;
	
	muzzleVelocity      = 0;
	velInheritFactor    = 1;
	
	armingDelay         = 00;
	lifetime            = 1;
	fadeDelay           = 0;
	bounceElasticity    = 0.0;
	bounceFriction      = 0.0;
	isBallistic         = false;
	gravityMod		   = 0.0;
	
	hasLight    = false;
	lightRadius = 10;
	lightColor  = "0.0 1.0 1.0";
	
	explodeOnDeath = 1;
	
	uiName = "Camera Shake";
};

function Player::shakeCamera(%obj, %intensity)
{
    %obj.spawnExplosion(camShakeProjectile, VectorScale(%obj.getScale(), %intensity));
}