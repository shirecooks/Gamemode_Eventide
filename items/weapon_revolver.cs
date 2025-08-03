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

datablock ItemData(RevolverItem)
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

datablock ShapeBaseImageData(revolverImage)
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

   doColorShift = true;
   colorShiftColor = RevolverItem.colorShiftColor;

   cooldown = 42000;

   //The revolver has been equipped.
   stateName[0] = "Activate";
   stateSequence[0] = "activate";
   stateSound[0] = weaponSwitchSound;
   stateWaitForTimeout[0] = true;
   stateTimeoutValue[0] = 0.01;
   stateTransitionOnTimeout[0] = "CooldownCheck";

   //Check if the revolver is on cooldown. If not, proceed to "Ready".
   stateName[1] = "CooldownCheck";
   stateScript[1] = "onCooldownCheck";
   stateAllowImageChange[1] = false;
   stateWaitForTimeout[1] = true;
   stateTimeOutValue[1] = 0.01;
   stateTransitionOnTimeout[1] = "CooldownRedirect";

   //Redirect to another state based on what was set in the previous "CooldownCheck" state.
   stateName[2] = "CooldownRedirect";
   stateAllowImageChange[2] = false;
   stateTransitionOnAmmo[2] = "Cooldown";
   stateTransitionOnNoAmmo[2] = "Ready";

   //The revolver is on cooldown and cannot be used.
   stateName[3] = "Cooldown";
   stateSequence[3] = "ready";
   stateScript[3] = "onCooldown";
   stateAllowImageChange[3] = true;
   ////The cooldown ended while the revolver was equipped, transition to the "Ready" state.
   stateTransitionOnNoAmmo[3] = "CooldownRevert";

   //The revolver is transitioning from "Cooldown" to "Ready", we need to raise the arm back up.
   stateName[4] = "CooldownRevert";
   stateSequence[4] = "Reload";
   stateScript[4] = "onCooldownRevert";
   stateAllowImageChange[4] = false;
   stateWaitForTimeout[4] = true;
   stateTimeOutValue[4] = 0.01;
   stateTransitionOnTimeout[4] = "Ready";

   //The revolver is inactive, simply being held.
   stateName[5] = "Ready";
   stateSequence[5] = "ready";
   stateScript[5] = "onReady";
   stateTransitionOnTriggerDown[5]	= "Fire";
   stateAllowImageChange[5] = true;

   //The revolver has been fired, spawn a bullet.
   stateName[6] = "Fire";
   stateScript[6] = "onFire";
   stateSequence[6] = "Fire";
   stateSound[6] = "revolver_shot_sound";
   stateAllowImageChange[6] = false;
   stateWaitForTimeout[6] = true;
   stateTimeoutValue[6] = 0.7;
   stateTransitionOnTimeout[6] = "Smoke";
   stateFire[6] = true;
   stateEmitter[6] = revolverFlashEmitter;
   stateEmitterNode[6] = "muzzleNode";
   stateEmitterTime[6] = 0.05;
   stateEjectShell[6] = true;

   //Make the revolver barrel smoke after the shot.
   stateName[7] = "Smoke";
   stateScript[7] = "onSmoke";
   stateAllowImageChange[7] = true;
   stateWaitForTimeout[7] = true;
   stateTimeoutValue[7] = 0.01;
   stateTransitionOnTimeout[7] = "CooldownCheck";
   stateEmitter[7] = revolverSmokeEmitter;
   stateEmitterTime[7] = 0.02;
   stateEmitterNode[7] = "muzzleNode";
};

//
// Sequence callbacks.
//

function revolverImage::onCooldownCheck(%this, %obj, %slot)
{
   //If the revolver has not passed it's cooldown time limit, transition to the "Cooldown" state.
   //Otherwise, transition to the "Ready" state.
   if((%obj.lastRevolverTime + %this.cooldown) > getSimTime())
   {
      %obj.setImageAmmo(%slot, 1);
   }
   else
   {
      %obj.setImageAmmo(%slot, 0);
   }
}

function revolverImage::onCooldown(%this, %obj, %slot)
{
   //Lower the revolver, it cannot be used.
   %obj.playThread(1, root);
}

function revolverImage::onCooldownRevert(%this, %obj, %slot)
{
   //Raise the arm back up after being lowered.
   fixArmReady(%obj);

   //Since we have a new bullet, play a reloading sound.
   %obj.playAudio(0, "revolver_reload_sound");
}

function revolverImage::onReady(%this, %obj, %slot)
{
   
}

function revolverImage::onFire(%this, %obj, %slot)
{
   //Play a firing animation.
   %obj.playThread(2, jump);

   //Shake the player's camera to mimic recoil.
   %obj.spawnExplosion("impulseProjectile", %obj.getScale()); 

   //Create a muzzle flash.
   %revolverlight = new fxLight() 
   {
      datablock = "orangeLight"; 
   };
   %revolverlight.setTransform(%obj.getMuzzlePoint(0));
   %revolverlight.schedule(50, delete);

   //Essential for cooldown checking upon revolver re-equip.
   %obj.lastRevolverTime = getSimTime();

   //Initiate the revolver's cooldown.
   //Triggers `stateTransitionOnAmmo[1]`.
   %cooldown = mCeil(%this.cooldown / 1000);
   %obj.weaponCooldown(%slot, "The bullet was spent, and you won't get another for " @ %cooldown @ " seconds.", "You have a new bullet, and your revolver is ready to fire!", 6);

	Parent::onFire(%this, %obj, %slot);	
}