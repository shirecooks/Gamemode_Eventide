//
// Particle and emitter effects.
//

//
// Generic darkness particles.
datablock ParticleData(DarkAmbientParticle)
{
	dragCoefficient = 1.75;
	windCoefficient = 0;
	gravityCoefficient = 0;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 1500;
	lifetimeVarianceMS = 500;
	textureName = "base/data/particles/dot";
	spinSpeed = 0;
	spinRandomMin = -300;
	spinRandomMax = 300;
	useInvAlpha = true;

	colors[0] = "0.75 0.53 0.88 .75";
	colors[1] = "0.5 0.33 0.68 0.25";
	sizes[0] = 0.2;
	sizes[1] = 0.4;
};

datablock ParticleEmitterData(DarkAmbientEmitter)
{
	ejectionPeriodMS = 10;
	periodVarianceMS = 0;
	ejectionVelocity = 1.5;
	velocityVariance = 1;
	ejectionOffset = 0.0;
	thetaMin = 0;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	overrideAdvance = false;
	particles = DarkAmbientParticle;

	uiName = "Darkness - Ambient";
};

datablock ExplosionData(DarkExplosion)
{
	lifeTimeMS = 250;

	particleEmitter = DarkAmbientEmitter;
	particleDensity = 75;
	particleRadius = 1;

	emitter[0] = DarkAmbientEmitter;

	faceViewer = true;
	explosionScale = "1 1 1";

	shakeCamera = false;
	camShakeFreq = "30 30 30";
	camShakeAmp = "7 2 7";
	camShakeDuration = 0.6;
	camShakeRadius = 2.5;

	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "1 1 1";
	lightEndColor = "1 1 1";

	uiName = "";
};

//
// Shire's glowing face, about to cast a curse.
datablock ParticleData(shireGlowingFaceParticle) 
{
	textureName				= "./particles/glowFace";
	lifetimeMS				= 500;
	lifetimeVarianceMS		= 0;
	dragCoefficient			= 0.0;
	windCoefficient			= 0.0;
	gravityCoefficient		= 0.0;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	spinRandomMin			= 0.0;
	spinRandomMax			= 0.0;
	colors[0]				= "1 1 1 0.5";
	colors[1]				= "0.1 0.1 0.1 0.1";
	colors[2]				= "0.0 0.0 0.0 0.0";
	sizes[0]				= 0.7;
	sizes[1]				= 0.7;
	sizes[2]				= 0.7;
	times[0]				= 0;
	times[1]				= 0.5;
	times[2]				= 1.0;
	useInvAlpha				= false;
};

datablock ParticleEmitterData(shireGlowingFaceEmitter) {
	uiName				= "Shire Glowing Face Emitter";
	particles			= "shireGlowingFaceParticle";
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 0.0;
	velocityVariance	= 0.0;
	ejectionOffset		= 0.35;
	thetaMin			= 0.0;
	thetaMax			= 0.0;
	phiReferenceVel		= 0.0;
	phiVariance			= 0.0;
};

datablock ShapeBaseImageData(shireGlowingFaceImage) 
{
	shapeFile			= "base/data/shapes/empty.dts";
	mountPoint			= 5;
	offset 				= "-0.09 -0.03 0.033";
	eyeOffset = "0 0 -1000";
	correctMuzzleVector	= false;

	stateName[0]				= "Glow";
	stateEmitter[0]				= shireGlowingFaceEmitter;
	stateEmitterTime[0]			= 1000;
	stateWaitForTimeout[0]		= true;
	stateTimeoutValue[0]		= 1000;
	stateTransitionOnTimeout[0]	= "Glow";
	stateScript[0]				= "onGlow";
};

//
// Blind particle and emitter.
datablock ParticleData(shireCurseParticle)
{
	dragCoefficient = 1;
	windCoefficient = 0;
	gravityCoefficient = 0;
	inheritedVelFactor = 1;
	constantAcceleration = 0;
	lifetimeMS = 115;
	lifetimeVarianceMS = 15;
	textureName = "base/data/particles/dot";
	spinSpeed = 0;
	spinRandomMin = -100;
	spinRandomMax = 100;
	useInvAlpha = true;

	colors[0] = "0.5 0.33 0.68 0.95";
	colors[1] = "0.5 0.33 0.68 0.25";
	sizes[0] = 1.5;
	sizes[1] = 1;
};

datablock ParticleEmitterData(shireCurseEmitter)
{
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;
	ejectionVelocity = 3;
	velocityVariance = 2.5;
	ejectionOffset = 0.3;
	thetaMin = 0;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	overrideAdvance = false;
	particles = shireCurseParticle;
};

//
// Zombie particles and emitters.
// datablock ParticleData(GlowFaceZombieParticle) 
// {
// 	textureName				= "./particles/glowFaceZombie";
// 	lifetimeMS				= 500;
// 	lifetimeVarianceMS		= 0;
// 	dragCoefficient			= 0.0;
// 	windCoefficient			= 0.0;
// 	gravityCoefficient		= 0.0;
// 	inheritedVelFactor		= 0.0;
// 	constantAcceleration	= 0.0;
// 	spinRandomMin			= 0.0;
// 	spinRandomMax			= 0.0;
// 	colors[0]				= "1.0 1.0 1.0 0.5";
// 	colors[1]				= "0.1 0.1 0.1 0.1";
// 	colors[2]				= "0.0 0.0 0.0 0.0";
// 	sizes[0]				= 0.7;
// 	sizes[1]				= 0.7;
// 	sizes[2]				= 0.7;
// 	times[0]				= 0;
// 	times[1]				= 0.5;
// 	times[2]				= 1.0;
// 	useInvAlpha				= false;
// };

// datablock ParticleEmitterData(GlowFaceZombieEmitter) {
// 	uiName				= "";
// 	particles			= "GlowFaceZombieParticle";
// 	ejectionPeriodMS	= 1;
// 	periodVarianceMS	= 0;
// 	ejectionVelocity	= 0.0;
// 	velocityVariance	= 0.0;
// 	ejectionOffset		= 0.4;
// 	thetaMin			= 0.0;
// 	thetaMax			= 0.0;
// 	phiReferenceVel		= 0.0;
// 	phiVariance			= 0.0;
// };

// datablock ParticleData(ZombieBodyParticle) 
// {
// 	textureName				= "./particles/ZombieBody";
// 	lifetimeMS				= 500;
// 	lifetimeVarianceMS		= 0;
// 	dragCoefficient			= 0.0;
// 	windCoefficient			= 0.0;
// 	gravityCoefficient		= 0.0;
// 	inheritedVelFactor		= 0.0;
// 	constantAcceleration	= 0.0;
// 	spinRandomMin			= 0.0;
// 	spinRandomMax			= 0.0;
// 	colors[0]				= "0 0 0 0.4";
// 	colors[1]				= "0 0 0 0.1";
// 	colors[2]				= "0 0 0 0";
// 	sizes[0]				= 2.6;
// 	sizes[1]				= 2.6;
// 	sizes[2]				= 2.6;
// 	times[0]				= 0;
// 	times[1]				= 0.5;
// 	times[2]				= 1.0;
// 	useInvAlpha				= true;
// };

// datablock ParticleEmitterData(ZombieBodyEmitter) {
// 	uiName				= "";
// 	particles			= "ZombieBodyParticle";
// 	ejectionPeriodMS	= 10;
// 	periodVarianceMS	= 0;
// 	ejectionVelocity	= 0.0;
// 	velocityVariance	= 0.0;
// 	ejectionOffset		= 0.0;
// 	thetaMin			= 0.0;
// 	thetaMax			= 0.0;
// 	phiReferenceVel		= 0.0;
// 	phiVariance			= 0.0;
// };

// datablock ParticleEmitterData(DarkAmbientZombieEmitter)
// {
// 	ejectionPeriodMS = 5;
// 	periodVarianceMS = 0;
// 	ejectionVelocity = 2.5;
// 	velocityVariance = 1.5;
// 	ejectionOffset = 1.25;
// 	thetaMin = 0;
// 	thetaMax = 180;
// 	phiReferenceVel = 180;
// 	phiVariance = 360;
// 	overrideAdvance = false;
// 	particles = DarkAmbientParticle;

// 	uiName = "Darkness - Ambient";
// };

// datablock ShapeBaseImageData(DarkCastZombieImage : DarkCastImage)
// {
// 	mountPoint = 2;

// 	stateName[0]               = "Wait";
// 	stateTimeoutValue[0]       = 1;
// 	stateEmitter[0]            = DarkAmbientZombieEmitter;
// 	stateEmitterTime[0]        = 5000;
// 	stateEmitterTime[0]        = 5;
// 	stateTransitionOnTimeout[0]= "Wait";
//     stateSound[0]               = "shire_charged_sound";	
// };

// datablock ShapeBaseImageData(DarkCastZombieHandRImage : DarkCastImage)
// {
// 	mountPoint = 0;
// };
// datablock ShapeBaseImageData(DarkCastZombieHandLImage : DarkCastImage)
// {
// 	mountPoint = 0;
// };

// datablock ShapeBaseImageData(GlowFaceZombieImage) 
// {
// 	shapeFile			= "base/data/shapes/empty.dts";
// 	mountPoint			= 6;
// 	correctMuzzleVector	= false;

// 	stateName[0]				= "Glow";
// 	stateEmitter[0]				= GlowFaceZombieEmitter;
// 	stateEmitterTime[0]			= 1000;
// 	stateWaitForTimeout[0]		= true;
// 	stateTimeoutValue[0]		= 1000;
// 	stateTransitionOnTimeout[0]	= "Glow";
// 	stateScript[0]				= "onGlow";
// };

// datablock ShapeBaseImageData(ZombieBodyImage) 
// {
// 	shapeFile			= "base/data/shapes/empty.dts";
// 	mountPoint			= 2;
// 	offset = "0 0 -0.55";
// 	eyeOffset = "0 0 -1000";
// 	correctMuzzleVector	= false;
// 	stateName[0]				= "Glow";
// 	stateEmitter[0]				= ZombieBodyEmitter;
// 	stateEmitterTime[0]			= 1000;
// 	stateWaitForTimeout[0]		= true;
// 	stateTimeoutValue[0]		= 1000;
// 	stateTransitionOnTimeout[0]	= "Glow";
// 	stateScript[0]				= "onGlow";
// };

//
// Baked-in melee weapon.
datablock ShapeBaseImageData(MeleeShireAxeImage : eventideMeleeImage)
{
   	shapeFile = $Eventide_BaseDirectory @ "/items/models/axe/axe.dts";
	
	hitProjectile = KillerSharpHitProjectile;
	hitObscureProjectile = KillerGenericSharpClankProjectile;
	meleeTrail = $Eventide_MeleeTrails["magic.trail"];

	swingSound = "generic_lightSwing";
	swingSoundAmount = 5;
};
MeleeShireAxeImage.inheritFunctionsFromSuperClass("eventideMeleeImage");

//
//// Ability and item to initiate howl.
datablock ItemData(shireHowlAbilityItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "base/data/shapes/empty.dts";

	uiName = "Howl";
	iconName = "./icons/hicolor_headache";

	image = shireHowlAbilityImage;
	canDrop = false;
};

datablock ShapeBaseImageData(shireHowlAbilityImage)
{
	className = "WeaponImage";
	shapeFile = "base/data/shapes/empty.dts";

	mountPoint = $RightHandSlot;
   	offset = "0 0 0";
   	eyeOffset = 0;
   	rotation = eulerToMatrix("0 0 0");
   	correctMuzzleVector = false;

   	item = shireHowlAbilityItem;
   	ammo = "";
   	projectile = "";
   	projectileType = Projectile;
   	melee = false;
   	armReady = false;

	stateName[0] = "Activate";
	stateWaitForTimeout[0] = true;
	stateTimeoutValue[0] = 0.01;
	stateTransitionOnTimeout[0] = "Ready";

	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "CooldownCheck";

	stateName[2] = "CooldownCheck";
	stateScript[2] = "onCooldownCheck";
	stateAllowImageChange[1] = false;
	stateWaitForTimeout[2] = true;
	stateTimeoutValue = 0.01;
	stateTransitionOnTimeout[2] = "Switch";

	stateName[3] = "Switch";
	stateTransitionOnAmmo[3] = "Howl";
	stateTransitionOnNoAmmo[3] = "CooldownCheckFail";

	stateName[4] = "CooldownCheckFail";
	stateScript[4] = "onCooldownCheckFail";
	stateTransitionOnTimeout[4] = "Ready";
	stateWaitForTimeout[4] = true;
	stateTimeoutValue[4] = 0.01;

	stateName[5] = "Howl";
	stateScript[5] = "onHowl";
	stateAllowImageChange[5] = true;
	stateWaitForTimeout[5] = true;
	stateTimeoutValue[5] = 0.01;
	stateTransitionOnTimeout[5] = "Ready";
};

function shireHowlAbilityImage::getHintMessage(%this, %obj)
{
	return "Click to let out a howl, dazzling everyone nearby.";
}

function shireHowlAbilityImage::onCooldownCheck(%this, %obj)
{
    %howlCooldown = (%obj.getDatablock().howlCooldown $= "") ? 45000 : %obj.getDatablock().howlCooldown;
	if((%obj.lastHowlTime + %howlCooldown) < getSimTime())
	{
		%obj.setImageAmmo(%this.mountPoint, true);
	}
	else
	{
		%obj.setImageAmmo(%this.mountPoint, false);
	}
}

function shireHowlAbilityImage::onCooldownCheckFail(%this, %obj)
{
	//Play a fail animation.
	%obj.playThread(2, "undo");

	//Tell the killer they need to wait for longer.
	%client = %obj.client;
	if(%client)
	{
        %howlCooldown = (%obj.getDatablock().howlCooldown $= "") ? 45000 : %obj.getDatablock().howlCooldown;
        %client.printFormatString("hint", "You can't howl again yet! Wait another " @ sFromMs(%howlCooldown - (getSimTime() - %obj.lastHowlTime)) @ " seconds.");
	}
}

function shireHowlAbilityImage::onHowl(%this, %obj)
{
    %obj.lastHowlTime = getSimTime();
    
    //Play the audible howl.
	%obj.playVoiceLine("Howl");

	//Shake Shire's camera, emulate an ear-piercing howl.
	%obj.shakeCamera(0.35);

    //Search for players to scare. 16 stud radius.
    initContainerRadiusSearch(%obj.getHackPosition() , 32, $TypeMasks::PlayerObjectType);
    while(%victim = containerSearchNext())
    {
        //Don't scare ourselves, don't scare other killers.
        if(%victim.getID() == %obj.getID())
        {
            continue;
        }
        else if(%victim.getDatablock().isKiller)
        {
            continue;
        }

		%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::ItemObjectType;
        %isVisible = (containerRayCast(%obj.getEyePoint(), %victim.getHackPosition(), %mask, %obj).getID() == %victim.getID());
        if(%isVisible)
        {
            //The victim is in range and is in line-of-sight, scare them.
            %victim.fear(%obj.getDatablock().howlTime);
        }
    }
}

//
// Ability and item to initiate curse.
datablock ItemData(shireCurseAbilityItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "base/data/shapes/empty.dts";
   	emap = false;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

	uiName = "Curse";
	iconName = "./icons/hicolor_dash";

	doColorShift = true;
	colorShiftColor = "0.56 0.56 0.62 1.000";

	image = shireCurseAbilityImage;
	canDrop = false;
};

datablock ShapeBaseImageData(shireCurseAbilityImage)
{
	className = "WeaponImage";
	shapeFile = "base/data/shapes/empty.dts";

	mountPoint = $RightHandSlot;
   	offset = "0 0 0";
   	eyeOffset = 0;
   	rotation = eulerToMatrix("0 0 0");
   	correctMuzzleVector = true;

   	item = shireCurseAbilityItem;
   	ammo = "";
   	projectile = "";
   	projectileType = Projectile;
   	melee = true;
   	armReady = true;

	stateName[0] = "Activate";
	stateWaitForTimeout[0] = true;
	stateTimeoutValue[0] = 0.01;
	stateTransitionOnTimeout[0] = "Ready";

	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "EnergyCheck";

	stateName[2] = "EnergyCheck";
	stateScript[2] = "onEnergyCheck";
	stateAllowImageChange[1] = false;
	stateWaitForTimeout[2] = true;
	stateTimeoutValue = 0.01;
	stateTransitionOnTimeout[2] = "Switch";

	stateName[3] = "Switch";
	stateTransitionOnAmmo[3] = "Aim";
	stateTransitionOnNoAmmo[3] = "EnergyCheckFail";

	stateName[4] = "EnergyCheckFail";
	stateScript[4] = "onEnergyCheckFail";
	stateTransitionOnTimeout[4] = "Ready";
	stateWaitForTimeout[4] = true;
	stateTimeoutValue[4] = 0.01;

	stateName[5] = "Aim";
	stateScript[5] = "onAim";
	stateAllowImageChange[5] = true;
	stateWaitForTimeout[5] = true;
	stateTimeoutValue[5] = 0.01;
	stateTransitionOnTimeout[5] = "Aiming";

	stateName[6] = "Aiming";
	stateEmitter[6] = DarkAmbientEmitter;
	stateEmitterNode[6] = "muzzlePoint";
	stateEmitterTime[6] = 1;
	stateTransitionOnTriggerUp[6] = "Curse";
	stateWaitForTimeout[6] = true;
	stateTimeoutValue[6] = 1.0;
	stateTransitionOnTimeout[6] = "Aiming";

	stateName[7] = "Curse";
	stateScript[7] = "attemptCurse";
	stateAllowImageChange[7] = false;
	stateWaitForTimeout[7] = true;
	stateTimeoutValue[7] = 0.1;
	stateTransitionOnTimeout[7] = "Ready";
};

function shireCurseAbilityImage::getHintMessage(%this, %obj)
{
	return "Aim at a survivor and release to gain curse them, blinding and deafening for a few seconds. Don't miss.";
}

function shireCurseAbilityImage::onEnergyCheck(%this, %obj)
{
    %curseCooldown = (%obj.getDatablock().curseCooldown $= "") ? 40000 : %obj.getDatablock().curseCooldown;
	if((%obj.lastCurseTime + %curseCooldown) < getSimTime())
	{
		%obj.setImageAmmo(%this.mountPoint, true);
	}
	else
	{
		%obj.setImageAmmo(%this.mountPoint, false);
	}
}

function shireCurseAbilityImage::onEnergyCheckFail(%this, %obj)
{
	//Play a fail animation.
	%obj.playThread(2, "undo");

	//Tell the killer they need to wait for longer.
	%client = %obj.client;
	if(%client)
	{
        %curseCooldown = (%obj.getDatablock().curseCooldown $= "") ? 40000 : %obj.getDatablock().curseCooldown;
        %client.printFormatString("hint", "You can't curse again yet! Wait another " @ sFromMs(%curseCooldown - (getSimTime() - %obj.lastCurseTime)) @ " seconds.");
	}
}

function shireCurseAbilityImage::onAim(%this, %obj)
{
	//Make the hand "glow."
	%obj.setNodeColor("rhand", "0.8 0.8 0.5 1");

    //Add a glowing eye.
    %obj.mountImage("shireGlowingFaceImage", 1);	
}

function shireCurseAbilityImage::attemptCurse(%this, %obj)
{
    %obj.lastCurseTime = getSimTime();

    //Play a casting animation.
	%obj.playthread(2, "leftrecoil");

    //Play a sound effect.
    serverPlay3d("magicCast_sound", %obj.getMuzzlePoint(0));

    //Cast a projectile that will apply the status effect to anyone it hits.
    %focalPoint = VectorAdd(%obj.getEyePoint(), VectorScale(%obj.getLookVector(), 1000));
    %muzzlePoint = %obj.getMuzzlePoint(0);
    %aimVector = VectorNormalize(VectorSub(%focalPoint, %muzzlePoint));
    new Projectile()
    {
        dataBlock = "shireCastProjectile";
        initialVelocity = VectorScale(%aimVector, shireCastProjectile.muzzleVelocity);
        initialPosition = %muzzlePoint;
        sourceObject = %obj;
        client = %obj.client;
    };

	//Reset the glowing hand.
	%client = %obj.client;
	if(%client)
	{
		%client.applyBodyColors();
	}

    //Unmount the glowing eye.
    %obj.unmountImage(1);
}

//
//// Projectile to cast curse from.
datablock ProjectileData(shireCastProjectile)
{
	projectileShapeName = "base/data/shapes/empty.dts";
    uiName = "";

	directDamage = 0;
    radiusDamage = 0;

	explosion = DarkExplosion;

	muzzleVelocity = 70;
	velInheritFactor = 1.0;

	gravityMod = 1.0;
    collideWithPlayers = true;
	armingDelay         = 0;
	lifetime            = 30000;
	fadeDelay           = 29500;
	bounceElasticity    = 0.99;
	bounceFriction      = 0.00;
	isBallistic         = true;

    impactImpulse = 10;
	verticalImpulse = 25;

	hasLight = false;
	lightRadius = 1;
	lightColor = "0 0 0";

    particleEmitter = DarkAmbientEmitter;
	sound = "magicHum_sound";
};

datablock ShapeBaseImageData(shireCurseBlindImage)
{
	shapeFile = "base/data/shapes/empty.dts";

	mountPoint = $Headslot;
	offset = "0 0 0";
	eyeOffset = 0;
	rotation = "0 0 0";

	projectileType = Projectile;

	stateName[0] = "Blind";
	stateTimeoutValue[0] = 1;
	stateEmitter[0] = "shireCurseEmitter";
	stateEmitterTime[0] = 0.50;
	stateTransitionOnTimeout[0] = "Loop";

	stateName[1] = "Loop";
	stateTransitionOnTimeout[1] = "Blind";
	stateTimeoutValue[1] = 0.01;
};

function shireCastProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
    if(%col.getType() & $TypeMasks::PlayerObjectType)
    {
        if(!%col.getDatablock().isKiller)
        {
            %killerDatablock = %obj.sourceObject.getDatablock();
			%col.curse(%killerDatablock.curseTime);
        }
    }

    parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
}

//
//// Status effect create by projectile.
function ShireCurseEffect::beginStatusEffect(%this, %obj)
{
	//Disorient the player by blinding and deafening them.
	%obj.setDamageflash(0.5);
	%obj.nearSight(%this.duration);
	%obj.deafen(%this.duration);

	//Visual effect where the player have darkness over their face. Also play a spooky sound.
	%obj.playAudio(3, "magicPossession_sound");
	%obj.mountImage("shireCurseBlindImage", 3);
}

function ShireCurseEffect::finalizeStatusEffect(%this, %obj)
{
	//Take the darkness off the player's face.
	%obj.unmountImage(3);
}

function Player::curse(%obj, %time)
{
    //Have the freeze time default to 1 second.
    if(%time $= "")
    {
        %time = 1000;
    }

    //Have the player enter the stun.
    %obj.applyStatusEffect("ShireCurseEffect", "Debuff", %time);
}

//
// Playertype.
//

datablock PlayerData(PlayerShire : PlayerKiller) 
{
	uiName = "Shire Player";	
	
	killerWeaponImage = MeleeShireAxeImage;

	facePack = "shire";
	voicePack = "shire";

	maxDamage = 1000;

	maxForwardSpeed = 7.32;
	maxBackwardSpeed = 4.18;
	maxSideSpeed = 6.27;

	killerNearMusic = musicData_Eventide_ShireNear;
	killerChaseMusic = musicData_Eventide_ShireChase;

    maxWeapons = 2;
    maxTools = 2;

    howlTime = 6000;
    curseTime = 5000;
};
PlayerShire.inheritFunctionsFromSuperClass("PlayerKiller");

//
// Appearance.
//

function PlayerShire::eventideBodyParts(%this, %obj)
{
    %obj.hideNode("ALL");

	%obj.unhideNode("pants");
	%obj.unhideNode("headskin");
	%obj.unhideNode("larm");
	%obj.unhideNode("rarm");
	%obj.unhideNode("rshoe");
	%obj.unhideNode("lshoe");
	%obj.unhideNode("lhand");
	%obj.unhideNode("rhand");
	%obj.unhideNode("femchest");
    %obj.unHideNode("hoodie2");

	%obj.setDecalName("robe");
}

function PlayerShire::eventideBodyColors(%this, %obj)
{
    %hoodieColor = "0.22 0.11 0.3 1";
	%pantsColor = "0.075 0.075 0.075 1";
	%skinColor = "1 1 1 1";

    %obj.setNodeColor("rarm", %hoodieColor);
	%obj.setNodeColor("larm", %hoodieColor);
	%obj.setNodeColor("femchest", %hoodieColor);
	%obj.setNodeColor("pants", %pantsColor);
	%obj.setNodeColor("rshoe", %pantsColor);
	%obj.setNodeColor("lshoe", %pantsColor);
	%obj.setNodeColor("rhand", %skinColor);
	%obj.setNodeColor("lhand", %skinColor);
	%obj.setNodeColor("headskin", %skinColor);
	%obj.setNodeColor("hoodie2", %hoodieColor);
}