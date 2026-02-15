//
// Melee weapon.
//

//
// Melee weapon particle effects.
datablock ParticleData(KillerKatanaClankSprayParticle : KillerGenericSharpClankSprayParticle)
{
	colors[1]	= "1 0.7 0.6 1";
	sizes[0]	= 0.35;
	sizes[1]	= 0.9;

	lifetimeMS = 100;
	lifetimeVarianceMS = 10;
};

datablock ParticleEmitterData(KillerKatanaClankSprayEmitter : KillerGenericSharpClankSprayEmitter)
{
	particles = "KillerKatanaClankSprayParticle";
	
	ejectionPeriodMS = 20;
	ejectionVelocity = 10;
	thetaMin = 30;
	thetaMax = 45;

	uiName = "Katana Clank Spray";
};

datablock ParticleData(KillerKatanaClankChunkParticle : KillerGenericSharpClankChunkParticle)
{
	textureName = "base/data/particles/nut";
	useInvAlpha = false;

	colors[0]	= "1 0.9 0.8 1";
	colors[1]	= "1 0.3 0.2 1";
	sizes[0]	= 0.5;

	dragCoefficient = 0.5;
	lifetimeMS = 800;
	spinSpeed = 900;
};

datablock ParticleEmitterData(KillerKatanaClankChunkEmitter : KillerGenericSharpClankChunkEmitter)
{
	particles = "KillerKatanaClankChunkParticle";
	
	overrideAdvance = false;
	useEmitterColors = false;

	ejectionPeriodMS = 20;
	periodVarianceMS = 2;
	velocityVariance = 3.5;
	ejectionOffset = 0.2;
	thetaMin = 10;
	thetaMax = 35;
	
	uiName = "Katana Clank Chunk";
};

datablock ExplosionData(KillerKatanaClankExplosion)
{
	emitter[0] = KillerKatanaClankSprayEmitter;
	emitter[1] = KillerKatanaClankChunkEmitter;
	emitter[2] = KillerMacheteClankSparkEmitter;

	lifeTimeMS = 150;
	
	shakeCamera = true;
	camShakeFreq = "2 3 2";
	camShakeAmp = "1 1 1";
	camShakeDuration = 0.75;
	camShakeRadius = 15;
};

datablock ProjectileData(KillerKatanaClankProjectile)
{
	explosion = KillerKatanaClankExplosion;
	explodeOnDeath = true;

	uiName = "Katana Clank";
};

//
// Ability particle effects.

datablock ParticleData(RenownedAbilityParticle)
{
	dragCoefficient = 1;
	windCoefficient = 10;
	gravityCoefficient = 0.5;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 1500;
	lifetimeVarianceMS = 500;
	textureName = "base/data/particles/dot";
	spinSpeed = 0;
	spinRandomMin = -300;
	spinRandomMax = 300;
	useInvAlpha = true;

	colors[0] = "1 1 0.7 .75";
	colors[1] = "1 1 0.7 0.25";
	colors[3] = "1 1 0.7 0";
	sizes[0] = 0.2;
	sizes[1] = 0.4;
	sizes[2] = 0.6;
};

datablock ParticleEmitterData(RenownedAbilityEmitter)
{
	ejectionPeriodMS = 10;
	periodVarianceMS = 0;
	ejectionVelocity = 1.5;
	velocityVariance = 1;
	ejectionOffset = 0.25;
	thetaMin = 0;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	overrideAdvance = true;
	particles = RenownedAbilityParticle;

	uiName = "Renowned Ability";
};

//
// Baked-in melee weapon.
datablock ShapeBaseImageData(MeleeTantoImage : eventideMeleeImage)
{
   	shapeFile = $Eventide_BaseDirectory @ "/items/models/katana/Katana.dts";
	
	hitProjectile = KillerSharpHitProjectile;
	hitObscureProjectile = KillerKatanaClankProjectile;
	meleeTrail = $Eventide_MeleeTrails["base.trail"];

	swingSound = "generic_lightSwing";
	swingSoundAmount = 5;
};
MeleeTantoImage.inheritFunctionsFromSuperClass("eventideMeleeImage");

//
// Ability image and inventory item.
datablock ItemData(renownedPossessAbilityItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "base/data/shapes/empty.dts";
   	emap = false;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

	uiName = "Possession";
	iconName = "./icons/hicolor_headache";

	doColorShift = true;
	colorShiftColor = "0.56 0.56 0.62 1.000";

	image = renownedPossessAbilityImage;
	canDrop = false;
};

datablock ShapeBaseImageData(renownedPossessAbilityImage)
{
	className = "WeaponImage";
	shapeFile = "base/data/shapes/empty.dts";

	mountPoint = $RightHandSlot;
   	offset = "0 0 0";
   	eyeOffset = 0;
   	rotation = eulerToMatrix("0 0 0");
   	correctMuzzleVector = true;

   	item = "";
   	ammo = "";
   	projectile = "";
   	projectileType = Projectile;
   	melee = true;
   	armReady = true;

	hintStyle = "hint";
	hintMessage = "Aim at a survivor and release to gain control for a few seconds. Don't miss.";

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
	stateEmitter[6] = RenownedAbilityEmitter;
	stateEmitterNode[6] = "muzzlePoint";
	stateEmitterTime[6] = 1;
	stateTransitionOnTriggerUp[6] = "Possess";
	stateWaitForTimeout[6] = true;
	stateTimeoutValue[6] = 1.0;
	stateTransitionOnTimeout[6] = "Aiming";

	stateName[7] = "Possess";
	stateScript[7] = "attemptPossess";
	stateAllowImageChange[7] = false;
	stateWaitForTimeout[7] = true;
	stateTimeoutValue[7] = 0.1;
	stateTransitionOnTimeout[7] = "Ready";
};

function renownedPossessAbilityImage::onEnergyCheck(%this, %obj)
{
	if(%obj.getEnergyPercent() == 1)
	{
		%obj.setImageAmmo(%this.mountPoint, true);
	}
	else
	{
		%obj.setImageAmmo(%this.mountPoint, false);
	}
}

function renownedPossessAbilityImage::onEnergyCheckFail(%this, %obj)
{
	//Play a fail animation.
	%obj.playThread(2, "undo");

	//Tell the killer they need more energy.
	%client = %obj.client;
	if(%client)
	{
		%client.printFormatString("hint", "You don't have enough energy to possess! Wait until the bar fills up...");
	}
}

function renownedPossessAbilityImage::onAim(%this, %obj)
{
	//Play a sound effect.
	%obj.playManagedSound("Charged");

	//Make the hand "glow."
	%obj.setNodeColor("rhand", "0.8 0.8 0.5 1");
}

function renownedPossessAbilityImage::attemptPossess(%this, %obj)
{
	%start = %obj.getEyePoint();
	%range = getWord(%obj.getScale(), 2) * 40;
	%end = VectorAdd(%start, VectorScale(%obj.getLookVector(), %range));
	%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::ItemObjectType;

	%victim = containerRayCast(%start, %end, %mask, %obj);
	if(%victim != 0 && (%victim.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj, %victim))
	{
		//Tell the victim they can escape by rapidly clicking.
		%victimClient = %victim.client;
		%victimClientExists = isObject(%victimClient);
		if(%victimClientExists)
		{
			%victimClient.printFormatString("urgent", "You are being controlled, rapidly jump to break free!");
		}

		%killerClient = %obj.client;
		
		//Mark the victim as possessed, and give them a little visual effect.
		%victim.possessor = %obj;
		%victim.isPossessed = true;
		%victim.mountImage(RenownedPossessedImage, 3);

		//Play an animation on the killer and drain the remainder of their energy.
		%obj.possessedPlayer = %victim;
		%obj.possessTime = getSimTime();
		%obj.setEnergyLevel(0);
		%obj.playThread(2, "leftrecoil");
		//Give the killer a hint message, letting them know they need to bring the possessed survivor to themselves.
		if(%killerClient)
		{
			%killerClient.printFormatString("hint", "Bring the survivor to you or attack others!");
		}

		//Set the Renowned to control the victim.
		%obj.client.setControlObject(%victim);
		%this.possessTick(%obj, %victim);						
	}
	else 
	{
		//The killer missed, waste some energy.
		%obj.setEnergyLevel(%obj.getEnergyLevel() - (%obj.getDatablock().maxEnergy / 2));
	}

	//Reset the glowing hand.
	%client = %obj.client;
	if(%client)
	{
		%client.applyBodyColors();
	}
}

function renownedPossessAbilityImage::possessTick(%this, %obj, %victim)
{
	%victimPosition = %victim.getPosition();
	%killerPosition = %obj.getPosition();

	%victimDistance = VectorDist(%victimPosition, %killerPosition);
	%timeSincePossession = getSimTime() - %obj.possessTime;
	%killerClient = %obj.client;

	if(%victimDistance < 3 || %timeSincePossession > 4000)
	{
		//The victim has gotten too close or too much time has passed since possession, revert it.
		if(isObject(%killerClient))
		{
			ServerCmdUnUseTool(%killerClient);
		}
		%obj.possessTime = "";
		%obj.possessedPlayer = "";
		%obj.resetControlObject();
		%obj.playThread(2, "undo");
		serverPlay3D("renowned_spellBreak_sound", %killerPosition);

		%victim.isPossessed = false;
		%victim.possessor = "";
		%victim.unmountImage(3);
		%victim.resetControlObject();
		serverPlay3D("renowned_spellBreak_sound", %victimPosition);
		return;
	}

	cancel(%obj.possessTickSchedule);
	%obj.possessTickSchedule = %this.schedule(100, "possessTick", %obj, %victim);
}

//
// Possession image for victims.
//

datablock ShapeBaseImageData(renownedPossessedImage)
{
	className = "ItemImage";
	shapeFile = "base/data/shapes/empty.dts";

	mountPoint = $HeadSlot;
   	offset = "0 0 0";
   	eyeOffset = 0;
   	rotation = eulerToMatrix("0 0 0");
   	correctMuzzleVector = true;

	stateName[0] = "Activate";
	stateAllowImageChange[0] = false;
	stateEmitter[0] = RenownedAbilityEmitter;
	stateEmitterNode[0] = "muzzlePoint";
	stateEmitterTime[0] = 5000;
	stateTimeoutValue[0] = 5000;
	stateTransitionOnTimeout[0] = "Activate";
};

function renownedPossessedImage::onMount(%this, %obj)
{
	%client = %obj.client;
	if(%client)
	{
		%client.setControlObject(%client.Camera);
	}
}

function renownedPossessedImage::onUnMount(%this, %obj)
{
	%obj.resetControlObject();
}

//
// Playertype.
//

datablock PlayerData(PlayerRenowned : PlayerKiller) 
{
	uiName = "Renowned Player";	
	
	// Weapon: Katana
	killerWeaponImage = MeleeTantoImage;

	facePack = "renowned";
	voicePack = "renowned";

	maxDamage = 762;

	maxForwardSpeed = 7.32;
	maxBackwardSpeed = 4.18;
	maxSideSpeed = 6.27;

	killerNearMusic = musicData_Eventide_RenownedNear;
	killerChaseMusic = musicData_Eventide_RenownedChase;
};
PlayerRenowned.inheritFunctionsFromSuperClass("PlayerKiller");

//
// Appearance.
//

function PlayerRenowned::eventideBodyParts(%this, %obj)
{
	%obj.hideNode("ALL");

	%obj.unHideNode("chest");	
	%obj.unHideNode("rhand");
	%obj.unHideNode("lhand");
	%obj.unHideNode("rarm");
	%obj.unHideNode("larm");
	%obj.unHideNode("headskin");
	%obj.unHideNode("pants");
	%obj.unHideNode("rshoe");
	%obj.unHideNode("lshoe");
    %obj.unHideNode("renownedeyes");	
	%obj.setDecalName("renowneddecal");

	%obj.setHeadUp(0);

	%this.clearHatmodHat(%obj);

    //Custom player scale.
    %obj.setScale("1.05 1.05 1.05");
}

function PlayerRenowned::eventideBodyColors(%this, %obj)
{
    %skinColor = "0.83 0.73 0.66 1";
	%pantsColor = "0.075 0.075 0.075 1";
	%shirtColor = "0.541 0.698 0.553 1";

    //Set core body part colors.
	%obj.setNodeColor("headskin", %skinColor);
	%obj.setNodeColor("chest", %shirtColor);
	%obj.setNodeColor("Rhand", %skinColor);
	%obj.setNodeColor("Lhand", %skinColor);
	%obj.setNodeColor("lshoe", %pantsColor);
	%obj.setNodeColor("rshoe", %pantsColor);
	%obj.setNodeColor("pants", %pantsColor);
	%obj.setNodeColor("rarm", %shirtColor);
	%obj.setNodeColor("larm", %shirtColor);

    //Set blood colors.
	%obj.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("lhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("rhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_back", "0.7 0 0 1");
}

//
// Custom behaviors.
//

function Player::resetControlObject(%obj)
{
	switch$(%obj.getClassName())
	{
		case "Player": 	
			%obj.client.setControlObject(%obj);
			%obj.client.camera.setMode("Observer");
		case "AIPlayer": 
			%obj.setControlObject(%obj);
	}
}

//
// Possession break-free feature.
//

package Player_Renowned
{
	function Observer::onTrigger(%this, %obj, %trigger, %state)
	{
		%client = %obj.getControllingClient();
		%victim = %client.player;
		%killer = %victim.possessor;
		if(!%victim)
		{
			return Parent::onTrigger(%this, %obj, %trigger, %state);
		}
		else if(!%victim.isPossessed)
		{
			return Parent::onTrigger(%this, %obj, %trigger, %state);
		}

		//They jumped, progress their break-away threshold.
		if(%trigger == 2 && %state)
		{
			%obj.possessionResistance++;
			%victim.playthread(2, "activate2");
			%client.counterPrint(mFloor(%obj.possessionResistance / 2), "Rapidly jump to break free!", 0);
		}

		//They hit the max number of presses needed to break free...
		if(%obj.possessionResistance >= 15)
		{
			//Break the victim free, tell them what happened.
			%victm.unmountImage(3);
			%victim.possessor = "";
			%victim.isPossessed = false;
			%client.printFormatString("urgent", "You broke free!");

			//Stun the killer, tell them what happened.
			%killer.client.printFormatString("urgent", "Your victim broke free!");
			%killer.possessedPlayer = "";
			%killer.stun();
		}
	}
};
if(isPackage(Player_Renowned))
{
	deactivatePackage(Player_Renowned);
}
activatePackage(Player_Renowned);