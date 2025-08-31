//
// Melee weapon.
//

//
// Particle and emitter effects.
datablock ParticleData(KillerKatanaClankSprayParticle : KillerAxeClankSprayParticle)
{
	colors[1]	= "1 0.7 0.6 1";
	sizes[0]	= 0.35;
	sizes[1]	= 0.9;

	lifetimeMS = 100;
	lifetimeVarianceMS = 10;
};
datablock ParticleEmitterData(KillerKatanaClankSprayEmitter : KillerAxeClankSprayEmitter)
{
	particles = "KillerKatanaClankSprayParticle";
	
	ejectionPeriodMS = 20;
	ejectionVelocity = 10;
	thetaMin = 30;
	thetaMax = 45;

	uiName = "Katana Clank Spray";
};

datablock ParticleData(KillerKatanaClankChunkParticle : KillerAxeClankChunkParticle)
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
datablock ParticleEmitterData(KillerKatanaClankChunkEmitter : KillerAxeClankChunkEmitter)
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
	emitter[3] = KillerBloodSprayEmitter;
	emitter[4] = KillerBloodDropletEmitter;

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
// Item image.
datablock ShapeBaseImageData(MeleeTantoImage : KillerMeleeImage)
{
	class = "MeleeTantoImage";
    superClass = "KillerMeleeImage";

   	shapeFile = "./models/Katana.dts";
	
	hitProjectile = KillerSharpHitProjectile;
	hitObscureProjectile = KillerKatanaClankProjectile;	
	meleeTrail = $Eventide_MeleeTrails["base.trail"];

	swingSound = "generic_lightSwing";
	swingSoundAmount = 5;
};
MeleeTantoImage.inheritFunctionsFromSuperClass();

//
// Playertype.
//

datablock PlayerData(PlayerRenowned : PlayerKiller) 
{
    class = "PlayerRenowned";
    superClass = "PlayerKiller";

	uiName = "Renowned Player";	
	
	// Weapon: Katana
	meleeWeaponImage = MeleeTantoImage;

	facePack = "renowned";
	voicePack = "renowned";

	maxDamage = 762;

	maxForwardSpeed = 7.32;
	maxBackwardSpeed = 4.18;
	maxSideSpeed = 6.27;
	
	rightclickicon = "color_headache";
	leftclickicon = "color_melee";
	rightclickspecialicon = "";
	leftclickspecialicon = "";

	killerNearMusic = musicData_Eventide_RenownedNear;
	killerChaseMusic = musicData_Eventide_RenownedChase;
};
PlayerRenowned.inheritFunctionsFromSuperClass();

//
// Appearance
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

	%obj.mountImage("renownedEyesImage", 3);

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

function PlayerRenowned::possessAnimation(%this, %obj, %after)
{
	if(!%after)
	{
		%obj.playthread(2, "armReadyLeft");
		%obj.castTime = getSimTime();

		%obj.schedule(500, "setNodeColor", "lhand", "0.8 0.8 0.5 1");
		%obj.schedule(500, "mountImage", "RenownedCastImage", 3);
	}
	else
	{
		%obj.unmountImage(3);
		%this.eventideBodyColors(%obj);
	}
}

function PlayerRenowned::attemptPossess(%this, %obj)
{
	%start = %obj.getEyePoint();
	%range = getWord(%obj.getScale(), 2) * 40;
	%end = VectorAdd(%start, VectorScale(%obj.getEyeVector(), %range));
	%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::ItemObjectType;

	%victim = containerRayCast(%start, %end, %mask, %obj);
	if(isObject(%victim) && minigameCanDamage(%obj, %victim))
	{
		//Tell the victim they can escape by rapidly clicking.
		%victimClient = %victim.client;
		%victimClientExists = isObject(%victimClient);
		if(%victimClientExists)
		{
			%victimClient.centerprint("<color:FFFFFF><font:Impact:40>You are being controlled, rapidly jump to break free!", 5);
		}
		
		//Mark the victim as possessed, and give them a little visual effect.
		%victim.possessor = %obj;
		%victim.isPossessed = true;
		%victim.mountImage("RenownedPossessedImage", 3);
		if(%victimClientExists)
		{
			%victimClient.setControlObject(%victimClient.Camera);
		}

		//Play an animation on the killer and drain the remainder of their energy.
		%obj.possessedPlayer = %victim;
		%obj.possessTime = getSimTime();
		%obj.setEnergyLevel(0);
		%obj.playthread(2, "leftrecoil");

		//Set the Renowned to control the victim.
		%obj.client.setControlObject(%victim);
		%this.possessTick(%obj, %victim);						
	}
	else 
	{
		//The killer missed, waste some energy.
		%obj.setEnergyLevel(%obj.getEnergyLevel() - 50);
	}
}

function PlayerRenowned::possessTick(%this, %obj, %victim)
{
	%victimPosition = %victim.getPosition();
	%killerPosition = %obj.getPosition();

	%victimDistance = VectorDist(%victimPosition, %killerPosition);
	%timeSincePossession = getSimTime() - %obj.possessTime;

	if(%victimDistance < 3 || %timeSincePossession > 4000)
	{
		//The victim has gotten too close or too much time has passed since possession, revert it.
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
	}

	cancel(%obj.possessTickSchedule);
	%obj.possessTickSchedule = %this.schedule(100, "possessTick", %obj, %victim);
}

function PlayerRenowned::onTrigger(%this, %obj, %trig, %press) 
{
	%this.super("onTrigger", %this, %obj, %trig, %press);
	
	if(%trig == 4)
	{
		if(%press && %obj.getEnergyLevel() == %this.maxEnergy && (%obj.castTime + 500) < getSimTime())
		{
			if(%press)
			{
				%this.possessAnimation(%obj, false);
			}
			else
			{						
				%this.possessAnimation(%obj, true);
				%this.attemptPossess(%obj);
			}
		}
		else 
		{
			//Not enough energy.
			%obj.playThread(2, "undo");
		}
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
		if(!isObject(%victim))
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
			%victim.possessor = "";
			%victim.isPossessed = false;
			%victim.resetControlObject();
			%client.centerprint("<color:FFFFFF><font:Impact:40>You broke free!", 3);

			//Stun the killer, tell them what happened.
			%killer.client.centerprint("<font:Impact:30>\c3Your victim broke free!", 3);
			%killer.possessedPlayer = "";
			%killer.mountImage("sm_stunImage", 3);
		}
	}
};
if(isPackage(Player_Renowned))
{
	deactivatePackage(Player_Renowned);
}
activatePackage(Player_Renowned);