//
// Hat.
//

datablock ShapeBaseImageData(stallerHoodImage)
{
	shapeFile = "Add-Ons/Gamemode_Eventide/players/models/grimhood.dts";
	mountPoint = $HeadSlot;

	eyeOffset = "0 0 -1000";
	emap = 0;
	
	doColorShift = true;
	colorShiftColor = "0.1 0.1 0.1 1";
};

//
// Cloak/decloak emitter effects.
//

datablock ParticleData(stallerCloakSmokeParticle)
{
   dragCoefficient = 1.0;
   constantAcceleration = 0.0;
   gravityCoefficient = -3.0;
   inheritedVelFactor = 1.0;
   spinSpeed = 0;

   lifetimeMS = 400;
   lifetimeVarianceMS = 100;

   textureName = "base/data/particles/cloud";
   useInvAlpha = true;

   colors[0] = "0 0 0 0.5";
   colors[1] = "0.1 0.1 0.1 1";
   colors[2] = "0.15 0.15 0.15 0";

   sizes[0] = 3.0;
   sizes[1] = 2.0;
   sizes[2] = 1.5;

   times[0] = 0.0;
   times[1] = 0.5;
   times[2] = 1.0;
};

datablock ParticleData(stallerCloakInvertedStarParticle : stallerCloakSmokeParticle)
{
	textureName = "Add-Ons/Gamemode_Eventide/players/icons/invertedStar";

	lifetimeMS = 800;
	gravityCoefficient = -0.5;

	colors[0] = "1 0 0 1";
	colors[1] = "1 0 0 1";
	colors[2] = "1 0 0 0";

	sizes[0] = 0.75;
	sizes[1] = 0.75;
	sizes[2] = 0.75;
};

datablock ParticleEmitterData(stallerCloakSmokeEmitter)
{
	lifeTimeMS = 300;
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;

	ejectionVelocity = 1.0;
	velocityVariance = 0.0;

	ejectionOffset = 2.0; //How far away from the origin point particles can spawn.

	thetaMin = 0; //Particles spawn up.
	thetaMax = 90; //Particles spawn down.

	//Make the particles spawn around the origin point, never on.
	phiReferenceVel = 720;
	phiVariance = 360;

	overrideAdvance = false;
	useEmitterColors = false;
	orientParticles = false;
	particles = "stallerCloakSmokeParticle";
};

datablock ParticleEmitterData(stallerCloakInvertedStarEmitter : stallerCloakSmokeEmitter)
{
	ejectionPeriodMS = 45;
	ejectionOffset = 3.5; //How far away from the origin point particles can spawn.
	particles = "stallerCloakInvertedStarParticle";
};

datablock ExplosionData(stallerCloakExplosion)
{
	lifeTimeMS = 800;
	particleDensity = 100;
	particleRadius = 1.0;
	emitter[0] = stallerCloakSmokeEmitter;
	emitter[1] = stallerCloakInvertedStarEmitter;

	damageRadius = 0; 
	radiusDamage = 0;
	impulseRadius = 0;
	impulseForce = 0;
	playerBurnTime = 0;

	shakeCamera = false;
	explosionShape = "base/data/shapes/empty.dts";
};

datablock ProjectileData(stallerCloakProjectile)
{
	directDamage = 0;
	directDamageType = $DamageType::Default;
	radiusDamageType = $DamageType::Default;
	impactImpulse = 0;
	verticalImpulse = 0;

	explosion = stallerCloakExplosion;
	waterExplosion = stallerCloakExplosion;
	particleEmitter = "";
	particleWaterEmitter = "";
	splash = "";

	armingDelay = 0;
	lifeTime = 1;
	isBallistic = false;
	explodeOnDeath = true;
	
	muzzleVelocity = 0;
	velInheritFactor = 1.0;

	hasLight = false;
	projectileShapeName = "base/data/shapes/empty.dts";
};

//
// Core and appearance.
//

datablock PlayerData(PlayerStaller : PlayerSurvivor)
{
    class = "PlayerStaller";
    superClass = "PlayerSurvivor";

    uiName = "Staller Player";
	hoodMountPoint = 3;
};
//Inherits functions from `PlayerSurvivor`.
PlayerStaller.inheritFunctionsFromSuperClass();

function PlayerStaller::onNewDatablock(%this, %obj)
{
    %this.superClass.super("onNewDatablock", %this, %obj);

	//Edge-case: delete any voice-configs that may be present from a previous datablock.
	if(isObject(%obj.voiceConfig))
	{
		%obj.voiceConfig.delete();
	}
	//Another edge-case: delete any face-configs that may be present from a previous datablock.
	if(isObject(%obj.faceConfig))
	{
		%obj.faceConfig.delete();
	}
    
	//Store some information used for voice-lines and chase management.
	%obj.chasingKillers = new SimSet();
	%obj.nearbyKillers = new SimSet();
}

function PlayerStaller::eventideBodyParts(%this, %obj)
{
	%obj.hideNode("ALL");

	%obj.unHideNode("chest");	
	%obj.unHideNode("rhand");
	%obj.unHideNode("lhand");
	%obj.unHideNode("rarm");
	%obj.unHideNode("larm");
	%obj.unHideNode("headskin");
	%obj.unHideNode("skirt");
	%obj.setHeadUp(0);

	//Hood.
	%obj.mountImage("stallerHoodImage", %this.hoodMountPoint);

    //Custom player scale.
    %obj.setScale("1.05 1.05 1.05");
}

function PlayerStaller::eventideBodyColors(%this, %obj)
{
    %skinColor = "0.5 0.5 0.5 1";
	%clothingColor = "0.1 0.1 0.1 1";
	%shirtColor = "0.541 0.698 0.553 1";

    //Set core body part colors.
	%obj.setNodeColor("headskin", "0 0 0 1");
	%obj.setNodeColor("chest", %clothingColor);
	%obj.setNodeColor("skirt", %clothingColor);
	%obj.setNodeColor("rarm", %clothingColor);
	%obj.setNodeColor("larm", %clothingColor);
	%obj.setNodeColor("Rhand", %skinColor);
	%obj.setNodeColor("Lhand", %skinColor);

    //Set blood node colors, only shown upon taking damage.
	%bloodColor = "0.4 0 0 1";
	%obj.setNodeColor("lhand_blood", %bloodColor);
	%obj.setNodeColor("rhand_blood", %bloodColor);
	%obj.setNodeColor("chest_blood_front", %bloodColor);
	%obj.setNodeColor("chest_blood_back", %bloodColor);
}

//
// Cloaking mechanic and datablock.
//

function PlayerStaller::spawnCloakEffect(%this, %obj)
{
	%effectProjectile = new Projectile()
	{
		dataBlock = stallerCloakProjectile;
		initialVelocity = VectorAdd(%obj.getVelocity(), "0 0 0.1"); //Particle positioning messes up if velocity is exactly 0. Let's fix it!
		initialPosition = %obj.getTransform();
		sourceObject = %obj;
		sourceSlot = 3;
		client = %obj.client;
	};
	%effectProjectile.explode();
}

datablock PlayerData(PlayerStallerCloaked : PlayerStaller)
{
    class = "PlayerStallerCloaked";
    superClass = "PlayerStaller";

	maxForwardCrouchSpeed = PlayerStaller.maxForwardSpeed;
	maxSideCrouchSpeed = PlayerStaller.maxSideSpeed;
	maxBackwardCrouchSpeed = PlayerStaller.maxBackwardSpeed;

    uiName = "";
	isEventideClass = true;

	maxEnergy = 100;
	rechargeRate = -0.4375; //32 ticks per second * 14 = 8 seconds of cloak at full energy.
};
//Inherits functions from `PlayerStaller`.
PlayerStallerCloaked.inheritFunctionsFromSuperClass();

function PlayerStallerCloaked::eventideBodyParts(%this, %obj)
{
	//Hide every node of the Staller's body, to make them invisible.
	%obj.hideNode("ALL"); 

	//Get rid of the Staller's hood separately, as it is not yet part of the playertype.
	%obj.unmountImage(%this.hoodMountPoint);
}

function PlayerStallerCloaked::eventideBodyColors(%this, %obj)
{
	//Invisible, so nothing is needed here.
}

//
// Cloaking.

//Triggering a cloak.
function PlayerStaller::onTrigger(%this, %obj, %trigger, %state)
{
	%returnValue = %this.super("onTrigger", %this, %obj, %trigger, %state);
	if(%trigger == 3 && %state)
	{
		%this.cloak(%obj);
	}
	return %returnValue;
}

//Cloak effects.
function PlayerStaller::cloak(%this, %obj)
{
	//Energy needs to be full to cloak, for balance reasons.
	if(%obj.getEnergyLevel() != %this.maxEnergy)
	{
		return;
	}
	//Prevent cloak spam by adding a 1-second delay.
	else if((getSimTime() - %obj.lastCloakTime) < 1000)
	{
		return;
	}

	//Store last cloaking time.
	%obj.lastCloakTime = getSimTime();

	//Change the player to the cloaked datablock, so all the nodes will be hidden and the crouching speed will be increased.
	%targetDatablock = PlayerStallerCloaked;
	%obj.setDataBlock(%targetDatablock);

	//Make a cloud of smoke and inverted stars.
	%this.spawnCloakEffect(%obj);

	//Play the cloaking sound effect.
	serverPlay3D("staller_cloak_sound", %obj.getHackPosition());

	//Start the tick loop, to determine when the player has run out of energy.
	cancel(%obj.cloakTickSchedule);
	%targetDatablock.cloakTick(%obj);
}

function PlayerStallerCloaked::onNewDatablock(%this, %obj)
{
	Parent::onNewDatablock(%this, %obj);

	//Unghost the player from everyone, so nobody can see them using items or anything.
	%obj.adjustObjectScopeToAll(false, %obj.client);
}

//Cloaking tick, handles uncloaking if the player runs out of energy.
function PlayerStallerCloaked::cloakTick(%this, %obj)
{
	//If the player no longer exists, we no longer need to do this.
	if(!isObject(%obj))
	{
		return;
	}

	//If the player has run out of energy, uncloak them.
	if(%obj.getEnergyLevel() == 0)
	{
		return %this.uncloak(%obj);
	}

	//Loop this check, as energy drains continuously
	cancel(%obj.cloakTickSchedule);
	%obj.cloakTickSchedule = %this.schedule(33, cloakTick, %obj);
}

//
// Decloak.

//Triggering a cloak.
function PlayerStallerCloaked::onTrigger(%this, %obj, %trigger, %state)
{
	%returnValue = %this.super("onTrigger", %this, %obj, %trigger, %state);
	if(%trigger == 3 && !%state)
	{
		%this.uncloak(%obj);
	}
	return %returnValue;
}

function PlayerStallerCloaked::uncloak(%this, %obj)
{
	//Cancel the looping check for energy, if it exists.
	cancel(%obj.cloakTickSchedule);

	//Make a cloud of smoke and inverted stars.
	%this.spawnCloakEffect(%obj);

	//Play the decloaking sound effect.
	serverPlay3D("staller_uncloak_sound", %obj.getHackPosition());

	//Reghost the player to everyone, so they are no longer invisible.
	%obj.adjustObjectScopeToAll(true);

	//Resume the normal datablock and appearance code.
	%obj.setDataBlock(PlayerStaller);
}