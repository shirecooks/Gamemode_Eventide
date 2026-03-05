//
// Particle and emitter data.
//

datablock ParticleData(revolverBulletTrailParticle)
{
	dragCoefficient = 3;
	gravityCoefficient = -0.0;
	inheritedVelFactor = 0.0;
	constantAcceleration = 0.0;
	lifetimeMS = 625;
	lifetimeVarianceMS = 55;
	textureName = "base/data/particles/thinRing";
	spinSpeed = 10.0;
	spinRandomMin = -500.0;
	spinRandomMax = 500.0;
	colors[0] = "0.3 0.3 0.9 0.4";
	colors[1] = "0.5 0.5 0.5 0.0";
	sizes[0] = 0.15;
	sizes[1] = 0.25;

	useInvAlpha = false;
};

datablock ParticleEmitterData(revolverBulletTrailEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 0.0;
   velocityVariance = 0.0;
   ejectionOffset = 0.0;
   thetaMin = 0;
   thetaMax = 90;
   phiReferenceVel = 0;
   phiVariance = 360;
   overrideAdvance = false;
   particles = "revolverBulletTrailParticle";
};

datablock ParticleData(revolverFlashParticle)
{
   dragCoefficient = 3;
   gravityCoefficient = -0.5;
   inheritedVelFactor = 0.2;
   constantAcceleration = 0.0;
   lifetimeMS = 25;
   lifetimeVarianceMS = 15;
   textureName = "base/data/particles/star1";
   spinSpeed = 10.0;
   spinRandomMin = -500.0;
   spinRandomMax = 500.0;
   colors[0] = "0.9 0.9 0.0 0.9";
   colors[1] = "0.9 0.5 0.0 0.0";
   sizes[0] = 0.5;
   sizes[1] = 1.0;
   useInvAlpha = false;
};

datablock ParticleEmitterData(revolverFlashEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 1.0;
   velocityVariance = 1.0;
   ejectionOffset = 0.0;
   thetaMin = 0;
   thetaMax = 90;
   phiReferenceVel = 0;
   phiVariance = 360;
   overrideAdvance = false;
   particles = "revolverFlashParticle";
   uiName = "";
};

datablock ParticleData(revolverSmokeParticle)
{
   dragCoefficient = 3;
   gravityCoefficient = -0.5;
   inheritedVelFactor = 0.2;
   constantAcceleration = 0.0;
   lifetimeMS = 525;
   lifetimeVarianceMS = 55;
   textureName = "base/data/particles/cloud";
   spinSpeed = 10.0;
   spinRandomMin = -500.0;
   spinRandomMax = 500.0;
   colors[0] = "0.5 0.5 0.5 0.9";
   colors[1] = "0.5 0.5 0.5 0.0";
   sizes[0] = 0.15;
   sizes[1] = 0.15;
   useInvAlpha = false;
};

datablock ParticleEmitterData(revolverSmokeEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 1.0;
   velocityVariance = 1.0;
   ejectionOffset = 0.0;
   thetaMin = 0;
   thetaMax = 90;
   phiReferenceVel = 0;
   phiVariance = 360;
   overrideAdvance = false;
   particles = "revolverSmokeParticle";
   uiName = "";
};

datablock ParticleData(revolverExplosionParticle)
{
	dragCoefficient = 8;
	gravityCoefficient = 1;
	inheritedVelFactor = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS = 700;
	lifetimeVarianceMS = 400;
	textureName = "base/data/particles/cloud";
	spinSpeed = 10.0;
	spinRandomMin = -50.0;
	spinRandomMax = 50.0;
	colors[0] = "0.9 0.9 0.9 0.3";
	colors[1] = "0.9 0.5 0.6 0.0";
	sizes[0] = 0.25;
	sizes[1] = 0.75;
	useInvAlpha = true;
};

datablock ParticleEmitterData(revolverExplosionEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 2;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin = 89;
   thetaMax = 90;
   phiReferenceVel = 0;
   phiVariance = 360;
   overrideAdvance = false;
   particles = "revolverExplosionParticle";
   useEmitterColors = true;
   uiName = "";
};

datablock ParticleData(revolverExplosionRingParticle)
{
	dragCoefficient = 8;
	gravityCoefficient = -0.5;
	inheritedVelFactor = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS = 50;
	lifetimeVarianceMS = 35;
	textureName = "base/data/particles/star1";
	spinSpeed = 500.0;
	spinRandomMin = -500.0;
	spinRandomMax = 500.0;
	colors[0] = "1 1 0.0 0.9";
	colors[1] = "0.9 0.0 0.0 0.0";
	sizes[0] = 1;
	sizes[1] = 0;
	useInvAlpha = false;
};

datablock ParticleEmitterData(revolverExplosionRingEmitter)
{
   lifeTimeMS = 50;
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 0;
   velocityVariance = 0.0;
   ejectionOffset = 0.0;
   thetaMin = 89;
   thetaMax = 90;
   phiReferenceVel = 0;
   phiVariance = 360;
   overrideAdvance = false;
   particles = "revolverExplosionRingParticle";
   useEmitterColors = true;
   uiName = "";
};

datablock ExplosionData(revolverExplosion)
{
   soundProfile = "ricochet_sound";
   lifeTimeMS = 150;
   particleEmitter = revolverExplosionEmitter;
   particleDensity = 5;
   particleRadius = 0.2;
   emitter[0] = revolverExplosionRingEmitter;
   faceViewer = true;
   explosionScale = "1 1 1";
   shakeCamera = false;
   camShakeFreq = "10.0 11.0 10.0";
   camShakeAmp = "1.0 1.0 1.0";
   camShakeDuration = 0.5;
   camShakeRadius = 10.0;
   lightStartRadius = 2;
   lightEndRadius = 2;
   lightStartColor = "0.5 0.8 0.9";
   lightEndColor = "0 0 0";
};

//
// Projectile data.
//

AddDamageType("Revolver", '<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_revolver> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/CI_Revolver> %1', 0.2, 1);

datablock ProjectileData(revolverProjectile)
{
   projectileShapeName = "./models/revolver/bullet.dts";
   directDamage        = 60;
   directDamageType    = $DamageType::Revolver;
   radiusDamageType    = $DamageType::Revolver;
   brickExplosionRadius = 0;
   brickExplosionImpact = true;
   brickExplosionForce  = 10;
   brickExplosionMaxVolume = 1;
   brickExplosionMaxVolumeFloating = 2;
   impactImpulse	     = 400;
   verticalImpulse	  = 400;
   explosion           = revolverExplosion;
   particleEmitter     = "revolverBulletTrailEmitter";
   muzzleVelocity      = 120;
   velInheritFactor    = 1;
   armingDelay         = 00;
   lifetime            = 4000;
   fadeDelay           = 3500;
   bounceElasticity    = 0.5;
   bounceFriction      = 0.20;
   isBallistic         = false;
   gravityMod = 0.0;
   hasLight    = false;
   lightRadius = 3.0;
   lightColor  = "0 0 0.5";
   uiName = "";
};

function revolverProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	if(%col.getType() & $TypeMasks::PlayerObjectType)
   {
      %attacker = %obj.sourceObject;
      if(miniGameCanDamage(%attacker, %col))
      {
         %col.stun(2500);
      }
   }
	parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
}

//
// Image data.
//

datablock ItemData(revolverItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/revolver/revolver.dts";
   emap = false;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

	uiName = "Revolver";
   iconName = "./icons/icon_revolver";

	doColorShift = true;
	colorShiftColor = "0.56 0.56 0.62 1.000";

	image = revolverImage;
	canDrop = true;
};

datablock ShapeBaseImageData(revolverImage : cooldownImage)
{
   className = "WeaponImage";

   shapeFile = "./models/revolver/revolver.dts";
   emap = false;
   isSpecial = true;

   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0;
   rotation = eulerToMatrix("0 0 0"); 
   correctMuzzleVector = true;

   item = RevolverItem;
   ammo = false;
   projectile = revolverProjectile;
   projectileType = Projectile;
   melee = false;

   armReady = true;

   doColorShift = revolverItem.doColorShift;
   colorShiftColor = revolverItem.colorShiftColor;

   //The revolver has been equipped.
   stateName[0] = "Activate";
   stateSequence[0] = "activate";
   stateWaitForTimeout[0] = true;
   stateTimeoutValue[0] = 0.01;
   stateTransitionOnTimeout[0] = "CooldownCheck";

   //The revolver is inactive, simply being held.
   stateName[1] = "Ready";
   stateSequence[1] = "ready";
   stateScript[1] = "onReady";
   stateTransitionOnTriggerDown[1]	= "Fire";
   stateAllowImageChange[1] = true;

   //The revolver has been fired, spawn a bullet.
   stateName[2] = "Fire";
   stateScript[2] = "onFire";
   stateSequence[2] = "Fire";
   stateSound[2] = "revolver_shot_sound";
   stateAllowImageChange[2] = false;
   stateWaitForTimeout[2] = true;
   stateTimeoutValue[2] = 0.7;
   stateTransitionOnTimeout[2] = "Smoke";
   stateFire[2] = true;
   stateEmitter[2] = revolverFlashEmitter;
   stateEmitterNode[2] = "muzzleNode";
   stateEmitterTime[2] = 0.05;
   stateEjectShell[2] = true;

   //Make the revolver barrel smoke after the shot.
   stateName[3] = "Smoke";
   stateScript[3] = "onSmoke";
   stateAllowImageChange[3] = true;
   stateWaitForTimeout[3] = true;
   stateTimeoutValue[3] = 0.01;
   stateTransitionOnTimeout[3] = "CooldownCheck";
   stateEmitter[3] = revolverSmokeEmitter;
   stateEmitterTime[3] = 0.02;
   stateEmitterNode[3] = "muzzleNode";

   cooldown = 42000;
};
revolverImage.implementCooldownCallbacks();

function revolverImage::getHintMessage(%this, %obj)
{
	return "Ironically, only has a single bullet.";
}

//
// Sequence callbacks.
//

function revolverImage::onReady(%this, %obj)
{
   serverPlay3D("revolver_reload_sound", %obj.getMuzzlePoint(revolverImage.mountPoint));
}

function revolverImage::onFire(%this, %obj)
{
   %slot = %obj.currTool;

   //Play a firing animation.
   %obj.playThread(2, jump);

   //Shake the player's camera to mimic recoil.
   %obj.shakeCamera(0.5);

   //Create a muzzle flash.
   %revolverlight = new fxLight() 
   {
      datablock = "orangeLight"; 
   };
   %revolverlight.setTransform(%obj.getMuzzlePoint(0));
   %revolverlight.schedule(50, delete);

   //Initiate the revolver's cooldown.
   %obj.weaponCooldown(%slot, "The bullet is spent, and you won't get another for " @ sFromMs(%this.cooldown) @ " seconds.", "You have a new bullet, and your revolver is ready to fire!", 6);

	Parent::onFire(%this, %obj);
}