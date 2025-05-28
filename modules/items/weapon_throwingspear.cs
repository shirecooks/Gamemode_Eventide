//Throwingspear.cs
datablock AudioProfile(ThrowingspearStickSound)
{
   filename    = "./models/throwingspear/swordHit.wav";
   description = AudioClose3d;
   preload = true;
};
datablock AudioProfile(ThrowingspearFireSound)
{
   filename    = "./models/throwingspear/spearFire.wav";
   description = AudioClose3d;
   preload = true;
};


//spear trail
datablock ParticleData(ThrowingspearTrailParticle)
{
    dragCoefficient        = 3.0;
    windCoefficient        = 0.0;
    gravityCoefficient    = 0.0;
    inheritedVelFactor    = 0.0;
    constantAcceleration    = 0.0;
    lifetimeMS        = 600;
    lifetimeVarianceMS    = 0;
    spinSpeed        = 10.0;
    spinRandomMin        = -50.0;
    spinRandomMax        = 50.0;
    useInvAlpha        = true;
    animateTexture        = false;
    //framesPerSec        = 1;

    textureName        = "base/data/particles/ring";
    //animTexName        = " ";

    // Interpolation variables
    colors[0]    = "0.75 0.75 0.75 0.3";
    colors[1]    = "0.75 0.75 0.75 0.2";
    colors[2]    = "1 1 1 0.0";
    sizes[0]    = 0.15;
    sizes[1]    = 0.35;
    sizes[2]    = 0.05;
    times[0]    = 0.0;
    times[1]    = 0.5;
    times[2]    = 1.0;
};

datablock ParticleEmitterData(ThrowingspearTrailEmitter)
{
   ejectionPeriodMS = 5;
   periodVarianceMS = 0;

   ejectionVelocity = 0; //0.25;
   velocityVariance = 0; //0.10;

   ejectionOffset = 0;

   thetaMin         = 0.0;
   thetaMax         = 90.0;  

   particles = ThrowingspearTrailParticle;

   useEmitterColors = true;
   uiName = "Throwing Spear Trail";
};


//effects
datablock ParticleData(ThrowingspearExplosionParticle)
{
    dragCoefficient      = 8;
    gravityCoefficient   = -0.3;
    inheritedVelFactor   = 0.2;
    constantAcceleration = 0.0;
    lifetimeMS           = 500;
    lifetimeVarianceMS   = 300;
    textureName          = "base/data/particles/cloud";
    spinSpeed        = 10.0;
    spinRandomMin        = -50.0;
    spinRandomMax        = 50.0;
    colors[0]     = "0.5 0.5 0.5 0.9";
    colors[1]     = "0.5 0.5 0.5 0.0";
    sizes[0]      = 0.45;
    sizes[1]      = 0.0;

   useInvAlpha = true;
};

datablock ParticleEmitterData(ThrowingspearExplosionEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 3;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 180;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "ThrowingspearExplosionParticle";

   useEmitterColors = true;

   uiName = "Throwing Spear Smoke";
};

datablock ParticleData(ThrowingspearExplosionParticle2)
{
    dragCoefficient      = 5;
    gravityCoefficient   = 0.1;
    inheritedVelFactor   = 0.2;
    constantAcceleration = 0.0;
    lifetimeMS           = 500;
    lifetimeVarianceMS   = 300;
    textureName          = "base/data/particles/chunk";
    spinSpeed        = 10.0;
    spinRandomMin        = -50.0;
    spinRandomMax        = 50.0;
    colors[0]     = "0.9 0.9 0.6 0.9";
    colors[1]     = "0.9 0.5 0.6 0.0";
    sizes[0]      = 0.25;
    sizes[1]      = 0.0;
};

datablock ParticleEmitterData(ThrowingspearExplosionEmitter2)
 {
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 5;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 80;
   thetaMax         = 80;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "ThrowingspearExplosionParticle2";

   useEmitterColors = true;
};

datablock ExplosionData(ThrowingspearExplosion)
{
   //explosionShape = "";
   lifeTimeMS = 150;

   soundProfile = ThrowingspearStickSound; 

   particleEmitter = ThrowingspearExplosionEmitter2;
   particleDensity = 10;
   particleRadius = 0.2;

   emitter[0] = "";

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = false;
   camShakeFreq = "10.0 11.0 10.0";
   camShakeAmp = "1.0 1.0 1.0";
   camShakeDuration = 0.5;
   camShakeRadius = 10.0;

   // Dynamic light
   lightStartRadius = 2;
       lightEndRadius = 0;
       lightStartColor = "1 0 0";
       lightEndColor = "0 0 0";

   //impulse
   impulseRadius = 0;
   impulseForce = 100;

   //radius damage
   radiusDamage        = 0;
   damageRadius        = 0;
};

datablock ExplosionData(ThrowingspearExplosion2)
{
    //explosionShape = "";
   lifeTimeMS = 150;

   emitter[0] = ThrowingspearExplosionEmitter;

   faceViewer     = true;
   explosionScale = "1 1 1";

   shakeCamera = false;
   camShakeFreq = "7.0 8.0 7.0";
   camShakeAmp = "1.0 1.0 1.0";
   camShakeDuration = 0.5;
   camShakeRadius = 15.0;

   // Dynamic light
       lightStartRadius = 0;
       lightEndRadius = 1;
       lightStartColor = "0.3 0.6 0.7";
       lightEndColor = "0 0 0";

   //impulse
   impulseRadius = 0;
   impulseForce = 100;

   //radius damage
   radiusDamage        = 0;
   damageRadius        = 0;
};

//projectile
AddDamageType("ThrowingSpearDirect",   '<bitmap:add-ons/Weapon_Throwing_Spear/CI_spear> %1',       '%2 <bitmap:add-ons/Weapon_Throwing_Spear/CI_spear> %1',1,1);

datablock ProjectileData(ThrowingspearProjectile)
{
   projectileShapeName = "./models/throwingspear/spearProjectile.dts";

   directDamage        = 30;
   directDamageType    = $DamageType::ThrowingSpearDirect;

   radiusDamage        = 0;
   damageRadius        = 0;
   radiusDamageType    = $DamageType::ThrowingSpearDirect;

   explosion             = ThrowingspearExplosion2;
   stickExplosion        = ThrowingspearExplosion;
   bloodExplosion        = ThrowingspearExplosion2;
   particleEmitter       = ThrowingspearTrailEmitter;
   explodeOnPlayerImpact = true;
   explodeOnDeath        = true;  

   armingDelay         = 8000;
   lifetime            = 8000;
   fadeDelay           = 8000;

   isBallistic         = true;
   bounceAngle         = 170; //stick almost all the time
   minStickVelocity    = 10;
   bounceElasticity    = 0.2;
   bounceFriction      = 0.01;   
   gravityMod = 1;

   hasLight    = true;
   lightRadius = 3.0;
   lightColor  = "0 0 0.5";

   muzzleVelocity      = 64;
   velInheritFactor    = 1;

   uiName = "Spear";
};
package ThrowingSpearPackage
{
    
    function ThrowingspearProjectile::onCollision(%this,%obj,%col,%fade,%pos,%normal)
    {
		if(%col.getDataBlock().isKiller)
		{
			%col.mountimage("sm_stunImage",3);
			serverPlay3d("poolcue_smash" @ getRandom(1,2) @ "_sound",%pos);
		}
        serverPlay3D(ThrowingspearExplosionSound,%obj.getTransform());
        parent::onCollision(%this,%obj,%col,%fade,%pos,%normal);
    }
    function Armor::onCollision(%this, %obj, %col, %a, %b, %c, %d, %e, %f)
    {
        if(%col.dataBlock $= "ThrowingSpearItem" && %col.canPickup)
        {
            for(%i=0;%i<%this.maxTools;%i++)
            {
                %item = %obj.tool[%i];
                if(%item $= 0 || %item $= "")
                {
                    %freeSlot = 1;
                    break;
                }
            }

            if(%freeSlot)
            {
                %obj.pickup(%col);
                return;
            }
        }
        Parent::onCollision(%this, %obj, %col, %a, %b, %c, %d, %e, %f);
    }
};
activatePackage(ThrowingSpearPackage);

//////////
// item //
//////////
datablock ItemData(ThrowingspearItem)
{
    category = "Weapon";  // Mission editor category
    className = "Weapon"; // For inventory system

     // Basic Item Properties
    shapeFile = "./models/throwingspear/spear.dts";
    mass = 1;
    density = 0.2;
    elasticity = 0.2;
    friction = 0.6;
    emap = true;

    //gui stuff
    uiName = "Throwing Spear";
    iconName = "./models/throwingspear/icon_Throwing_Spear";
    doColorShift = true;
    colorShiftColor = "0.400 0.196 0 1.000";

     // Dynamic properties defined by the scripts
    image = ThrowingspearImage;
    canDrop = true;
};

//function spear::onUse(%this,%user)
//{
//    //mount the image in the right hand slot
//    %user.mountimage(%this.image, $RightHandSlot);
//}

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(ThrowingspearImage)
{
   // Basic Item properties
   shapeFile = "./models/throwingspear/spear.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "0 0 0";
   //eyeOffset = "0.1 0.2 -0.55";

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.  
   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = ThrowingspearItem;
   ammo = " ";
   projectile = ThrowingspearProjectile;
   projectileType = Projectile;

   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;

   //casing = " ";
   doColorShift = true;
   colorShiftColor = "0.400 0.196 0 1.000";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.

   // Initial start up state
        stateName[0]            = "Activate";
    stateTimeoutValue[0]        = 0.1;
    stateTransitionOnTimeout[0]    = "Ready";
    stateSequence[0]        = "ready";
    stateSound[0]                    = weaponSwitchSound;

    stateName[1]            = "Ready";
    stateTransitionOnTriggerDown[1]    = "Charge";
    stateAllowImageChange[1]    = true;
    
    stateName[2]                    = "Charge";
    stateTransitionOnTimeout[2]    = "Armed";
    stateTimeoutValue[2]            = 0.7;
    stateWaitForTimeout[2]        = false;
    stateTransitionOnTriggerUp[2]    = "AbortCharge";
    stateScript[2]                  = "onCharge";
    stateAllowImageChange[2]        = false;
    
    stateName[3]            = "AbortCharge";
    stateTransitionOnTimeout[3]    = "Ready";
    stateTimeoutValue[3]        = 0.3;
    stateWaitForTimeout[3]        = true;
    stateScript[3]            = "onAbortCharge";
    stateAllowImageChange[3]    = false;

    stateName[4]            = "Armed";
    stateTransitionOnTriggerUp[4]    = "Fire";
    stateAllowImageChange[4]    = false;

    stateName[5]            = "Fire";
    stateTransitionOnTimeout[5]    = "Done";
    stateTimeoutValue[5]        = 0.5;
    stateFire[5]            = true;
    stateSequence[5]        = "fire";
    stateScript[5]            = "onFire";
    stateWaitForTimeout[5]        = true;
    stateAllowImageChange[5]    = false;

    stateName[6]                    = "Done";
    stateScript[6]                    = "onDone";


};

function ThrowingspearImage::onCharge(%this, %obj, %slot)
{
    %obj.playthread(2, spearReady);
    %obj.ThrowingSpearSlot = %obj.currTool;
}

function ThrowingspearImage::onAbortCharge(%this, %obj, %slot)
{
    %obj.playthread(2, root);
}

function ThrowingSpearImage::onFire(%this, %obj, %slot)
{
    %obj.playthread(2, ThrowingspearThrow);
    serverPlay3D(ThrowingspearFireSound, %obj.getTransform());
    Parent::OnFire(%this, %obj, %slot);

    %currSlot = %obj.ThrowingSpearSlot;
    %obj.tool[%currSlot] = 0;
    %obj.weaponCount--;
    messageClient(%obj.client,'MsgItemPickup','',%currSlot,0);
    serverCmdUnUseTool(%obj.client);
}

function ThrowingSpearImage::onDone(%this,%obj,%slot)
{
    %obj.unMountImage(%slot);
}

