datablock ParticleData(MusketbulletTrailParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = -0.0;
	inheritedVelFactor   = 0.0;
	constantAcceleration = 0.0;
	lifetimeMS           = 625;
	lifetimeVarianceMS   = 55;
	textureName          = "base/data/particles/thinRing";
	spinSpeed		= 10.0;
	spinRandomMin		= -500.0;
	spinRandomMax		= 500.0;
	colors[0]     = "0.3 0.3 0.9 0.4";
	colors[1]     = "0.5 0.5 0.5 0.0";
	sizes[0]      = 0.15;
	sizes[1]      = 0.25;

	useInvAlpha = false;
};
datablock ParticleEmitterData(MusketbulletTrailEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 0.0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "MusketbulletTrailParticle";
};

datablock ParticleData(RevolverFlashParticle)
{
   dragCoefficient      = 3;
   gravityCoefficient   = -0.5;
   inheritedVelFactor   = 0.2;
   constantAcceleration = 0.0;
   lifetimeMS           = 25;
   lifetimeVarianceMS   = 15;
   textureName          = "base/data/particles/star1";
   spinSpeed		= 10.0;
   spinRandomMin	= -500.0;
   spinRandomMax	= 500.0;
   colors[0]   		= "0.9 0.9 0.0 0.9";
   colors[1]   		= "0.9 0.5 0.0 0.0";
   sizes[0]    		= 0.5;
   sizes[1]    		= 1.0;
   useInvAlpha		= false;
};

datablock ParticleEmitterData(RevolverFlashEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 1.0;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance  = false;
   particles 	    = "RevolverFlashParticle";
   uiName 	    = "";
};

datablock ParticleData(RevolverSmokeParticle)
{
   dragCoefficient      = 3;
   gravityCoefficient   = -0.5;
   inheritedVelFactor   = 0.2;
   constantAcceleration = 0.0;
   lifetimeMS           = 525;
   lifetimeVarianceMS   = 55;
   textureName          = "base/data/particles/cloud";
   spinSpeed		= 10.0;
   spinRandomMin	= -500.0;
   spinRandomMax	= 500.0;
   colors[0]    	= "0.5 0.5 0.5 0.9";
   colors[1]   		= "0.5 0.5 0.5 0.0";
   sizes[0]    		= 0.15;
   sizes[1]    		= 0.15;
   useInvAlpha 		= false;
};

datablock ParticleEmitterData(RevolverSmokeEmitter)
{
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 1.0;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance  = false;
   particles 	    = "RevolverSmokeParticle";
   uiName 	    = "";
};

datablock ParticleData(RevolverExplosionParticle)
{
	dragCoefficient      = 8;
	gravityCoefficient   = 1;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 700;
	lifetimeVarianceMS   = 400;
	textureName          = "base/data/particles/cloud";
	spinSpeed	     = 10.0;
	spinRandomMin	     = -50.0;
	spinRandomMax	     = 50.0;
	colors[0]    	     = "0.9 0.9 0.9 0.3";
	colors[1]  	     = "0.9 0.5 0.6 0.0";
	sizes[0]     	     = 0.25;
	sizes[1]   	     = 0.75;
	useInvAlpha 	     = true;
};

datablock ParticleEmitterData(RevolverExplosionEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 2;
   velocityVariance = 1.0;
   ejectionOffset   = 0.0;
   thetaMin         = 89;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance  = false;
   particles 	    = "RevolverExplosionParticle";
   useEmitterColors = true;
   uiName	    = "";
};

datablock ParticleData(RevolverExplosionRingParticle)
{
	dragCoefficient      = 8;
	gravityCoefficient   = -0.5;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 50;
	lifetimeVarianceMS   = 35;
	textureName          = "base/data/particles/star1";
	spinSpeed	     = 500.0;
	spinRandomMin	     = -500.0;
	spinRandomMax	     = 500.0;
	colors[0]     	     = "1 1 0.0 0.9";
	colors[1]     	     = "0.9 0.0 0.0 0.0";
	sizes[0]      	     = 1;
	sizes[1]     	     = 0;
	useInvAlpha 	     = false;
};

datablock ParticleEmitterData(RevolverExplosionRingEmitter)
{
   lifeTimeMS = 50;
   ejectionPeriodMS = 3;
   periodVarianceMS = 0;
   ejectionVelocity = 0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 89;
   thetaMax         = 90;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;
   particles = "RevolverExplosionRingParticle";
   useEmitterColors = true;
   uiName = "";
};

datablock ExplosionData(RevolverExplosion)
{
   soundProfile = "ricochet_sound";
   lifeTimeMS = 150;
   particleEmitter = RevolverExplosionEmitter;
   particleDensity = 5;
   particleRadius = 0.2;
   emitter[0] = RevolverExplosionRingEmitter;
   faceViewer     = true;
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


AddDamageType("Revolver",   '<bitmap:Add-Ons/Weapon_DefaultishGuns/icons/CI_Revolver> %1',    '%2 <bitmap:Add-Ons/Weapon_DefaultishGuns/icons/CI_Revolver> %1',0.2,1);
datablock ProjectileData(RevolverProjectile)
{
   projectileShapeName = "./models/bullet.dts";
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
   explosion           = RevolverExplosion;
   particleEmitter     = "MusketbulletTrailEmitter";
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

datablock ItemData(RevolverItem)
{
	category = "Weapon";
	className = "Weapon";
	shapeFile = "./models/revolver.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;
	uiName = "Revolver";
	Name = "./icons/icon_revolver";
	doColorShift = true;
	colorShiftColor = "0.56 0.56 0.62 1.000";
	image = RevolverImage;
	canDrop = true;
};

datablock ShapeBaseImageData(RevolverImage)
{
   shapeFile = "./models/revolver.dts";
   emap = true;
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = 0;
   rotation = eulerToMatrix( "0 0 0" ); 
   correctMuzzleVector = true;
   className = "WeaponImage";
   item = BowItem;
   ammo = " ";
   projectile = RevolverProjectile;
   projectileType = Projectile;
   melee = false;
   isSpecial = true;
   armReady = true;
   minShotTime = 200;
   doColorShift = true;
   colorShiftColor = RevolverItem.colorShiftColor = "0.56 0.56 0.62 1.000";

	stateName[0]                     = "Activate";
	stateTimeoutValue[0]             = 0.1;
	stateTransitionOnTimeout[0]      = "Ready";
	stateSound[0]			 = weaponSwitchSound;

	stateName[1]                     = "Ready";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateAllowImageChange[1]         = true;
	stateSequence[1]		 = "Ready";

	stateName[2]                     = "Fire";
	stateTransitionOnTimeout[2]      = "Smoke";
	stateTimeoutValue[2]             = 0.7;
	stateFire[2]                     = true;
	stateAllowImageChange[2]         = false;
	stateSequence[2]                 = "Fire";
	stateWaitForTimeout[2]		 = true;
	stateEmitter[2]			 = RevolverFlashEmitter;
	stateEmitterTime[2]		 = 0.05;
	stateEmitterNode[2]		 = "muzzleNode";
	stateScript[2]                   = "onFire";
	stateSound[2]			 = "revolvershot_sound";
	stateEjectShell[2]       	 = true;

	stateName[3]			 = "Smoke";
	stateEmitter[3]			 = RevolverSmokeEmitter;
	stateEmitterTime[3]		 = 0.02;
	stateEmitterNode[3]		 = "muzzleNode";
	stateTimeoutValue[3]             = 0.01;
	stateSound[3]			 = "";
	stateScript[3]                   = "onSmoke";
	stateTransitionOnTimeout[3]      = "Reload";

	stateName[4]			 = "Reload";
	stateSequence[4]                 = "Reload";
	stateTransitionOnTriggerUp[4]    = "Ready";
	stateSequence[4]	 	 = "Ready";

	cooldown = 42000;
};
function RevolverImage::onFire(%this,%obj,%slot)
{
	if(%obj.getDamagePercent() < 1.0)
		%obj.playThread(2, plant);
		%obj.spawnExplosion("impulseProjectile",%obj.getScale()); 
		%obj.lastRevolverTime = getSimTime();
		%obj.schedule(%this.cooldown,notifyRevolverReady);
				// Create Light Effect
		%revolverlight = new fxLight() { datablock = "orangeLight"; };
		%revolverlight.setTransform(%obj.getMuzzlePoint(0));
		%revolverlight.schedule(50,delete);

	Parent::onFire(%this,%obj,%slot);	
}

function RevolverImage::onSmoke(%this,%obj,%slot)
{
	if(%obj.getDamagePercent() < 1.0)
	{
		serverCmdUnUseTool(%obj.client);
	}
}

function RevolverImage::onMount(%this, %obj, %slot)
{
	if((%obj.lastRevolverTime+%this.cooldown) > getSimTime())
		{
		centerprint(%obj.client,"<font:arial:13><color:ff7744>Can't use this yet!" ,1);
		serverCmdUnUseTool(%obj.client);
		%obj.playThread(2, undo);
		return;
		}
}

function RevolverProjectile::onCollision(%data, %proj, %col, %fade, %pos, %normal)
{
	if(%col.getClassName() $= "Player")
	{
		%col.mountimage("sm_stunImage",3);
	}
	parent::onCollision(%data, %proj, %col, %fade, %pos, %normal);
}

function Player::notifyRevolverReady(%obj)
{
	centerprint(%obj.client,"<font:arial:13><color:44ff44>Revolver ability is ready!" ,1);
	serverPlay3D("revolverreload_sound",%obj.getPosition());
}