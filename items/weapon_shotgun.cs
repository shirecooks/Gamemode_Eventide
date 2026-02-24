//
// Particle and debris data.
//

datablock DebrisData(slugFragDebris)
{
	shapeFile 			= "./models/woodFrag/woodFrag.dts";
	lifetime 			= 2.8;
	spinSpeed			= 1200.0;
	minSpinSpeed 		= -3600.0;
	maxSpinSpeed 		= 3600.0;
	elasticity 			= 0.5;
	friction 			= 0.2;
	numBounces 			= 3;
	staticOnMaxBounce 	= true;
	snapOnMaxBounce 	= false;
	fade 				= true;
	gravModifier 		= 4;
};

datablock ParticleData(SlugExplosionParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = 0.4;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 800;
	lifetimeVarianceMS   = 100;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]			= "0.6 0.6 0.6 0.3";
	colors[1]			= "0.5 0.5 0.5 0.0";
	sizes[0]			= 0.75;
	sizes[1]			= 1.5;
	useInvAlpha 		= true;
};

datablock ParticleEmitterData(SlugExplosionEmitter)
{
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 2;
	velocityVariance	= 1.0;
	ejectionOffset  	= 0.0;
	thetaMin			= 89;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= SlugExplosionParticle;
};

datablock ExplosionData(SlugSubExplosion)
{
	debris 					= slugFragDebris;
	debrisNum 				= 6;
	debrisNumVariance 		= 2;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
	explosionShape 			= "";
	particleEmitter 		= SlugExplosionEmitter;
	particleDensity 		= 20;
	particleRadius 			= 0.4;
	lifeTimeMS 				= 150;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= false;
	camShakeFreq 			= "";
	camShakeAmp 			= "";
	camShakeDuration 		= 0;
	camShakeRadius 			= 0.0;
};

datablock explosionData(SlugExplosion : gunExplosion)
{
	soundProfile = "";
	particleEmitter = "";
	subExplosion[0] = SlugSubExplosion;
};

datablock ParticleData(SlugProjectileParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 120;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;

	textureName		= "base/data/particles/dot";

	colors[0]	= "1 0.6 0.4 0.99";
	colors[1]	= "1 0.3 0 0.99";
	colors[2]	= "1 0.1 0 0.99";
	sizes[0]	= 0.2;
	sizes[1]	= 0.15;
	sizes[2]	= 0.0;
	times[0]	= 0.0;
	times[1]	= 0.5;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(SlugProjectileEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 0;
	velocityVariance = 0;
	ejectionOffset = 0;

	thetaMin         = 0.0;
	thetaMax         = 90.0;  

	particles = SlugProjectileParticle;
	useEmitterColors = true;
};

datablock DebrisData(shotgunShellDebris)
{
	shapeFile = "./models/shotgunSlug/shotgunSlug.dts";
	lifetime = 6.0;
	lifetimeVariance = 1.0;
	minSpinSpeed = -400.0;
	maxSpinSpeed = 200.0;
	elasticity = 0.5;
	friction = 0.2;
	numBounces = 3;
	staticOnMaxBounce = true;
	snapOnMaxBounce = false;
	fade = true;

	gravModifier = 4;
};

//
// Projectile data.
//

AddDamageType("shotGun",'<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_shotgun> %1','%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_shotgun> %1', 1, 1);
datablock ProjectileData(shotgunProjectile : gunProjectile)
{
	projectileShapeName 	= "base/data/shapes/empty.dts";
	directDamage        	= 50;
	explosion 				= SlugExplosion;
	directDamageType    	= $DamageType::shotGun;
	radiusDamageType    	= $DamageType::shotGun;
	particleEmitter     	= SlugProjectileEmitter;
	uiName 					= "";
	muzzleVelocity 			= 200;
	verticalImpulse 		= 20;
	impactImpulse			= 20;
	sProjectile = 1;
};

function shotgunProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	if(%col.getType() & $TypeMasks::PlayerObjectType)
	{
		%col.stun();
	}
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
}

//
// Shotgun slug item.
//

datablock ItemData(shotgunSlugItem)
{
	className = "Weapon";

	shapeFile = "./models/shotgunSlug/shotgunSlug.dts";
	iconName = "./icons/icon_shotgunSlug";
	emap = 1;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	canDrop = 1;
	
	uiName = "Shotgun Slug";
	
	image = shotgunSlugImage;
};

function shotgunSlugItem::onPickup(%this, %obj, %user, %amount)
{
	%pickedUp = Parent::onPickup(%this, %obj, %user, %amount);
	%shotgunMountPoint = shotgunImage.mountPoint;
	if(%pickedUp && (%user.getMountedImage(%shotgunMountPoint) == shotgunImage.getID()) && (%user.getImageLoaded(%shotgunMountPoint) == false))
	{
		//If the user has the shotgun equipped and it isn't loaded, set a flag so it reloads.
		%user.setImageAmmo(%shotgunMountPoint, true);
	}
	return %pickedUp;
}

datablock shapeBaseImageData(shotgunSlugImage)
{
	className = "WeaponImage";
	item = shotgunSlugItem;

	shapeFile = "./models/shotgunSlug/shotgunSlug.dts";
	emap = 1;

	correctMuzzleVector = 0;
	mountPoint = 0;
	armReady = 1;

	stateName[0] = "Activate";
};

//
// Shotgun item and image data.
//

datablock ItemData(shotgunItem)
{
	category = "Weapon";
	className = "Weapon";
	
	shapeFile = "./models/shotgun/shotgun.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	uiName = "Old Shotgun";
	iconName = "./icons/icon_shotgun";

	image = shotgunImage;
	canDrop = true;
};

datablock shapeBaseImageData(shotgunImage)
{
	className = "WeaponImage";

	shapeFile = "./models/shotgun/shotgun.dts";
	emap = true;

	correctMuzzleVector = true;
	mountPoint = 0;
	offset = "-0.014 0 -0.08";

	armReady = true;

	item = shotgunItem;
	ammo = shotgunSlugItem;

	projectile = shotgunProjectile;
	projectileType = Projectile;

	casing = shotgunShellDebris;
	shellExitDir        = "0.2 -1.3 1.0";
	shellExitOffset     = "0 0 0";
	shellExitVariance   = 12.0;	
	shellVelocity       = 7.0;

	stateName[0] = "Activate";
	stateSequence[0] = "root";
	stateWaitForTimeout[0] = true;
	stateTimeOutValue[0] = 0.01;
	stateTransitionOnTimeout[0] = "AmmoCheck";

	stateName[1] = "AmmoCheck";
	stateScript[1] = "onAmmoCheck";
	stateWaitForTimeout[1] = true;
	stateTimeOutValue[1] = 0.01;
	stateTransitionOnTimeout[1] = "Ready";

	stateName[2] = "Ready";
	stateSequence[2] = "root";
	stateTransitionOnTriggerDown[2] = "AttemptFire";
	stateWaitForTimeout[2] = true;
	stateTimeOutValue[2] = 0.01;
	stateTransitionOnAmmo[2] = "ReloadPhase1";

	stateName[3] = "AttemptFire";
	stateTransitionOnLoaded[3] = "Fire";
	stateTransitionOnNotLoaded[3] = "dryFire";

	stateName[4] = "Fire";
	stateScript[4] = "onFire";
	stateFire[4] = true;
	stateAllowImageChange[4] = false;
	stateSequence[4] = "trig";
	stateEmitter[4] = gunFlashEmitter;
	stateEmitterTime[4] = 0.05;
	stateWaitForTimeout[4] = true;
	stateTimeOutValue[4] = 1.2;
	stateTransitionOnTimeout[4] = "AmmoCheck";

	stateName[5] = "dryFire";
	stateScript[5] = "onDryFire";
	stateSequence[5] = "trig";
	stateTransitionOnTriggerUp[5] = "AmmoCheck";

	stateName[6] = "ReloadPhase1";
	stateScript[6] = "onReloadPhase1";
	stateSequence[6] = "ReloadPhase1";
	stateTimeOutValue[6] = 0.5;
	stateTransitionOnTimeout[6] = "ReloadPhase2";

	stateName[7] = "ReloadPhase2";
	stateScript[7] = "onReloadPhase2";
	stateSequence[7] = "unload";
	stateTransitionWaitForTimeout[7] = true;
	stateTimeOutValue[7] = 0.5;
	stateTransitionOnTimeout[7] = "ReloadPhase3";
	stateEjectShell[7]	= true;
	
	stateName[8] = "ReloadPhase3";
	stateScript[8] = "onReloadPhase3";
	stateSequence[8] = "load";
	stateTransitionWaitForTimeout[8] = true;
	stateTimeOutValue[8] = 0.01;
	stateTransitionOnTimeout[8] = "ReloadPhase4";

	stateName[9] = "ReloadPhase4";
	stateScript[9] = "onReloadPhase4";
	stateSequence[9] = "unbreak";
	stateTransitionWaitForTimeout[9] = true;
	stateTimeOutValue[9] = 0.5;
	stateTransitionOnTimeout[9] = "Ready";
};

function shotgunImage::getHintMessage(%this, %obj)
{
	return "Single-shot, tight spread. Don't miss.";
}

//
// Sequence callbacks.
//

function shotgunImage::onAmmoCheck(%this, %obj)
{
	%mountPoint = this.mountPoint;

	if(%obj.getImageAttribute("loaded") == true)
	{
		%obj.setImageLoaded(%mountPoint, true);
	}
	else
	{
		%obj.setImageLoaded(%mountPoint, false);
	}

	if(%obj.getInventory(shotgunSlugItem) >= 1 && %obj.getImageLoaded(%mountPoint) == false)
	{
		%obj.setImageAmmo(%mountPoint, true);
	}
	else
	{
		%obj.setImageAmmo(%mountPoint, false);
	}
}

function shotgunImage::onReady(%this, %obj)
{

}

function shotgunImage::onFire(%this, %obj)
{
	//Play the recoil animation.
	%obj.playThread(2, "jump");
	%obj.spawnExplosion("camShakeProjectile", %obj.getScale()); 

	%mountPoint = %this.mountPoint;

	//Play a firing sound.
	serverPlay3D("shotgun_fire_sound", %obj.getMuzzlePoint(%mountPoint));

	//Disable loaded, so the gane can't immediately fire again.
	%obj.setImageLoaded(%mountPoint, 0);
	%obj.setImageAttribute("loaded", false);

	Parent::onFire(%this, %obj);
}

function shotgunImage::onDryFire(%this, %obj)
{
	serverPlay3D("flaregun_dryfire_sound", %obj.getMuzzlePoint(%this.mountPoint));
}

function shotgunImage::onReloadPhase1(%this, %obj)
{
	%obj.playThread(2, "shiftLeft");
	serverPlay3D("shotgun_break_sound", %obj.getMuzzlePoint(%this.mountPoint));
}

function shotgunImage::onReloadPhase2(%this, %obj)
{
	%obj.playThread(2, "jump");

	//After 400 milliseconds, play a shell-drop sound effect by the player's side, relative to where they are facing.
	schedule(400, 0, "serverPlay3D", "shotgun_shellDrop" @ getRandom(1, 2) @ "_sound", MatrixMulPoint(%obj.getTransform(), "-1 -2 0"));
}

function shotgunImage::onReloadPhase3(%this, %obj)
{
	serverPlay3D("shotgun_load_sound", %obj.getMuzzlePoint(%this.mountPoint));
}

function shotgunImage::onReloadPhase4(%this, %obj)
{
	%mountPoint = %this.mountPoint;
	
	%obj.playThread(2, "shiftRight");
	serverPlay3D("shotgun_unbreak_sound", %obj.getMuzzlePoint(%mountPoint));

	//Remove a shotgun slug from the player's inventory.
	for(%i = 0; %i < %obj.getDatablock().maxTools; %i++)
	{
		if(%obj.tool[%i] == ShotgunSlugItem.getID())
		{
			%obj.removeItemFromInventory(%i);
			break;
		}
	}

	%obj.setImageAmmo(%mountPoint, false); //Disable ammo to flag that the gun does not need to reload.
	%obj.setImageLoaded(%mountPoint, true); //Enable loaded to flag that the gun can fire.
	%obj.setImageAttribute("loaded", true); //In case the gun is unequipped, store a variable on the player marking it as loaded.
}