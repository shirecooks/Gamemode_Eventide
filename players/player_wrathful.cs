//
// Melee weapons
//

//
// Particle effects.

datablock ProjectileData(KillerWrathfulHitProjectile : KillerSharpHitProjectile)
{
	uiName = "Wrathful's Hit";

	hitSound = "wrathfulHit";
	hitSoundAmount = 1;
};

datablock ProjectileData(KillerWrathfulRageHitProjectile : KillerWrathfulHitProjectile)
{
	uiName = "Wrathful's Rage Hit";

	hitSound = "wrathfulRageHit";
};

//
//// Normal melee.
datablock ShapeBaseImageData(MeleeWrathfulImage : eventideMeleeImage)
{
	class = "MeleeWrathfulImage";
    superClass = "eventideMeleeImage";

   	shapeFile = "base/data/shapes/empty.dts";
	
	hitProjectile = KillerWrathfulHitProjectile;
	hitObscureProjectile = "";
	meleeTrail = $Eventide_MeleeTrails["raggedClaw.trail"];
    meleeTrailScale = "6 5 0.4";

	swingSound = "wrathfulSwing";
	swingSoundAmount = 2;

    meleeRange = 1.0;
    customSwingAnimation = "w_attack1";
    fixedDamageAmount = 28;
};
MeleeWrathfulImage.inheritFunctionsFromSuperClass();

//
//// Ability and item to initiate charging.
datablock ItemData(wrathfulChargeAbilityItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "base/data/shapes/empty.dts";

	uiName = "Charge";
	iconName = "./icons/hicolor_dash";

	image = wrathfulChargeAbilityImage;
	canDrop = false;
};

datablock ShapeBaseImageData(wrathfulChargeAbilityImage)
{
	className = "WeaponImage";
	shapeFile = "base/data/shapes/empty.dts";

	mountPoint = $RightHandSlot;
   	offset = "0 0 0";
   	eyeOffset = 0;
   	rotation = eulerToMatrix("0 0 0");
   	correctMuzzleVector = false;

   	item = wrathfulChargeAbilityItem;
   	ammo = "";
   	projectile = "";
   	projectileType = Projectile;
   	melee = false;
   	armReady = false;

	hintStyle = "hint";
	hintMessage = "Click to charge for two seconds, winding everyone in your path.";

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
	stateTransitionOnAmmo[3] = "Charge";
	stateTransitionOnNoAmmo[3] = "CooldownCheckFail";

	stateName[4] = "CooldownCheckFail";
	stateScript[4] = "onCooldownCheckFail";
	stateTransitionOnTimeout[4] = "Ready";
	stateWaitForTimeout[4] = true;
	stateTimeoutValue[4] = 0.01;

	stateName[5] = "Charge";
	stateScript[5] = "onCharge";
	stateAllowImageChange[5] = true;
	stateWaitForTimeout[5] = true;
	stateTimeoutValue[5] = 0.01;
	stateTransitionOnTimeout[5] = "Ready";

    chargeCooldown = 24000;
    chargeTime = 2000;
};

function wrathfulChargeAbilityImage::onCooldownCheck(%this, %obj, %slot)
{
    %chargeCooldown = (%obj.getDatablock().chargeCooldown $= "") ? 24000 : %obj.getDatablock().chargeCooldown;
	if((%obj.lastChargeTime + %chargeCooldown) < getSimTime())
	{
		%obj.setImageAmmo(0, true);
	}
	else
	{
		%obj.setImageAmmo(0, false);
	}
}

function wrathfulChargeAbilityImage::onCooldownCheckFail(%this, %obj, %slot)
{
	//Play a fail animation.
	%obj.playThread(2, "undo");

	//Tell the killer they need to wait for longer.
	%client = %obj.client;
	if(%client)
	{
        %chargeCooldown = (%obj.getDatablock().chargeCooldown $= "") ? 24000 : %obj.getDatablock().chargeCooldown;
		%client.printFormatString("hint", "You can't charge again yet! Wait another " @ sFromMs(%chargeCooldown - (getSimTime() - %obj.lastChargeTime)) @ " seconds.");
	}
}

function wrathfulChargeAbilityImage::onCharge(%this, %obj, %slot)
{
    %obj.applyStatusEffect("WrathfulChargeEffect", "Wrathful", PlayerWrathful.chargeTime);
}

//
//// Ability and item to initiate rage.
datablock ItemData(wrathfulRageAbilityItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "base/data/shapes/empty.dts";

	uiName = "Rage";
	iconName = "./icons/hicolor_headache";

	image = wrathfulRageAbilityImage;
	canDrop = false;
};

datablock ShapeBaseImageData(wrathfulRageAbilityImage)
{
	className = "WeaponImage";
	shapeFile = "base/data/shapes/empty.dts";

	mountPoint = $RightHandSlot;
   	offset = "0 0 0";
   	eyeOffset = 0;
   	rotation = eulerToMatrix("0 0 0");
   	correctMuzzleVector = false;

   	item = wrathfulRageAbilityItem;
   	ammo = "";
   	projectile = "";
   	projectileType = Projectile;
   	melee = false;
   	armReady = false;

	hintStyle = "hint";
	hintMessage = "Click to become enraged for 30 seconds, moving faster and punching harder.";

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
	stateTransitionOnAmmo[3] = "Rage";
	stateTransitionOnNoAmmo[3] = "CooldownCheckFail";

	stateName[4] = "CooldownCheckFail";
	stateScript[4] = "onCooldownCheckFail";
	stateTransitionOnTimeout[4] = "Ready";
	stateWaitForTimeout[4] = true;
	stateTimeoutValue[4] = 0.01;

	stateName[5] = "Rage";
	stateScript[5] = "onRage";
	stateAllowImageChange[5] = true;
	stateWaitForTimeout[5] = true;
	stateTimeoutValue[5] = 0.01;
	stateTransitionOnTimeout[5] = "Ready";
};

function wrathfulRageAbilityImage::onCooldownCheck(%this, %obj, %slot)
{
    %rageCooldown = (%obj.getDatablock().rageCooldown $= "") ? 60000 : %obj.getDatablock().rageCooldown;
	if((%obj.lastRageTime + %rageCooldown) < getSimTime())
	{
		%obj.setImageAmmo(0, true);
	}
	else
	{
		%obj.setImageAmmo(0, false);
	}
}

function wrathfulRageAbilityImage::onCooldownCheckFail(%this, %obj, %slot)
{
	//Play a fail animation.
	%obj.playThread(2, "undo");

	//Tell the killer they need to wait for longer.
	%client = %obj.client;
	if(%client)
	{
        %rageCooldown = (%obj.getDatablock().rageCooldown $= "") ? 60000 : %obj.getDatablock().rageCooldown;
        %client.printFormatString("hint", "You can't rage again yet! Wait another " @ sFromMs(%rageCooldown - (getSimTime() - %obj.lastRageTime)) @ " seconds.");
	}
}

function wrathfulRageAbilityImage::onRage(%this, %obj, %slot)
{
	%obj.applyStatusEffect("WrathfulPreRageEffect", "Wrathful", 2500);
}

//
// Ability and item for attacking while enraged.
datablock ShapeBaseImageData(MeleeWrathfulRageImage : MeleeWrathfulImage)
{
	class = "MeleeWrathfulRageImage";
    superClass = "MeleeWrathfulImage";

    customSwingAnimation = "";
    meleeTrailSkin = "";
    fixedDamageAmount = 36;
};
MeleeWrathfulRageImage.inheritFunctionsFromSuperClass();

function MeleeWrathfulRageImage::onSwing(%this, %obj, %slot)
{
    %currentTime = getSimTime();
	%killerDatablock = %obj.getDataBlock();

    //If we couldn't melee normally, don't melee now.
    if(%obj.getState() $= "Dead" || %obj.getEnergyLevel() < (%killerDatablock.maxEnergy / 8) || (%obj.lastMeleeTime + %this.meleeCooldown) > %currentTime) 
	{
		return;
	}

    %obj.lastMeleeTime = %currentTime;
    %obj.setEnergyLevel(%obj.getEnergyLevel() - (%killerDatablock.maxEnergy / 6));

    //Hold Wrathful in place (animation breaks otherwise) and deal some damage, after a delay.
    %windupTime = 400;
    %obj.freeze(%windupTime);
    %this.schedule(%windupTime, "afterWindup", %obj, %slot);

    //Play a punching animation.
    %obj.setActionThread(root);
    %obj.playThread(0, "w_handcannon");
}

function MeleeWrathfulRageImage::afterWindup(%this, %obj, %slot)
{
    if(%obj.getState() $= "Dead")
    {
        return;
    }

    %killerDatablock = %obj.getDataBlock();
    %killerLookVector = VectorNormalize(%obj.getLookVector());
	%killerPosition = %obj.getHackPosition();
	%killerWeaponPosition = %obj.getMuzzlePoint(0);

    //Shake the player's screen, make it feel like a big punch.
    %obj.spawnExplosion("impulseProjectile", %obj.getScale()); 

	//Perform a container search for victims, and if any are found, determine if we can damage them.
	%killerScale = %obj.getScale();
	initContainerRadiusSearch(%killerWeaponPosition, %this.meleeRange, $TypeMasks::PlayerObjectType);		
	while(%hit = containerSearchNext())
	{
		//If the victim is a special-purpose bot or can't be damaged, drop out.
		if(%hit == %obj || %hit == %obj.effectbot || !MinigameCanDamage(%obj, %hit)) 
		{
			continue;
		}

		%victimPosition = %hit.getHackPosition();

		//Check if the killer is facing the victim. If not, do nothing.
		%dot = VectorDot(%killerLookVector, VectorNormalize(VectorSub(%victimPosition, %killerPosition)));
		if(%dot < 0.4)
		{
			continue;
		}

		//Blood splatter or whatever else particle effect upon hitting the target.
		if(%this.hitProjectile !$= "")
		{
			%effect = new Projectile()
			{
				dataBlock = %this.hitProjectile;
				initialPosition = %victimPosition;
				initialVelocity = VectorNormalize(VectorSub(%victimPosition, %killerWeaponPosition));
				scale = %killerScale;
				sourceObject = %obj;
			};
			%effect.explode();
		}
		
		//Damage the target and give them a good shove.
		%hit.damage(%obj, %hit.getHackPosition(), %this.fixedDamageAmount, $DamageType::Default);
        %impulseVector = VectorAdd(VectorScale(VectorNormalize(VectorSub(%victimPosition, %killerWeaponPosition)), 15), "0 0 1.2");
        %inheritence = VectorAdd(%hit.getVelocity(), %impulseVector);
        %hit.setVelocity(%inheritence);
		
		//Temporarily slow down the killer.
		%killerDatablock.setTempSpeed(%obj, %this.slowdownSpeed);

        //Bring Wrathful back up to speed, and possibly more if he's still mad.
		%killerDatablock.schedule(%this.slowdownTime, setTempSpeed, %obj, 1.0);
	}

	//Air slice sound.
	%soundEffect = %this.swingSound @ %this.swingSoundAmount @ "_sound";
	ServerPlay3D(%soundEffect, %killerWeaponPosition);
}

//
// Playertype.
//

datablock TSShapeConstructor(PlayerWrathfulDTS)
{
    baseShape  = "./models/wrathful/wrathful.dts";
    sequence0  = "./models/wrathful/w_root.dsq root";
    sequence1  = "./models/wrathful/w_run.dsq run";
    sequence2  = "./models/wrathful/w_walk.dsq walk";
    sequence3  = "./models/wrathful/w_back.dsq back";
    sequence4  = "./models/wrathful/w_side.dsq side";
    sequence5  = "./models/wrathful/w_crouch.dsq crouch";
    sequence6  = "./models/wrathful/w_crouchRun.dsq crouchRun";
    sequence7  = "./models/wrathful/w_crouchBack.dsq crouchBack";
    sequence8  = "./models/wrathful/w_crouchSide.dsq crouchSide";
    sequence9  = "./models/wrathful/w_look.dsq look";
    sequence10 = "./models/wrathful/w_headside.dsq headside";
    sequence11 = "./models/wrathful/w_jump.dsq jump";
    sequence12 = "./models/wrathful/w_standJump.dsq standjump";
    sequence13 = "./models/wrathful/w_fall.dsq fall";
    sequence14 = "./models/wrathful/w_land.dsq land";
    sequence15 = "./models/wrathful/w_armAttack.dsq armAttack";
    sequence16 = "./models/wrathful/w_armReadyLeft.dsq armReadyLeft";
    sequence17 = "./models/wrathful/w_armReadyRight.dsq armReadyRight";
    sequence18 = "./models/wrathful/w_armReadyBoth.dsq armReadyBoth";
    sequence19 = "./models/wrathful/w_talk.dsq talk";
    sequence20 = "./models/wrathful/w_death1.dsq death1";
    sequence21 = "./models/wrathful/w_sit.dsq sit";
    sequence22 = "./models/wrathful/w_activate.dsq activate";
    sequence23 = "./models/wrathful/w_activate2.dsq activate2";
    sequence24 = "./models/wrathful/w_leftRecoil.dsq leftrecoil";
    sequence25 = "./models/wrathful/w_melee.dsq attack1";
    sequence26 = "./models/wrathful/w_charge.dsq charge";
    sequence27 = "./models/wrathful/w_chargecycle.dsq chargeCycle";
    sequence28 = "./models/wrathful/w_chargerecovery.dsq chargeEnd";
    sequence29 = "./models/wrathful/w_handcannon.dsq bigAttack1";
    sequence30 = "./models/wrathful/w_rage.dsq rage";
    sequence31 = "./models/wrathful/w_stomp.dsq stomp";
};

datablock PlayerData(PlayerWrathful : PlayerKiller) 
{
    class = "PlayerWrathful";
    superClass = "PlayerKiller";

    shapeFile = PlayerWrathfulDTS.baseShape;
    uiName = "Wrathful Player";

    maxTools = 2;
    maxWeapons = 2;

    killerWeaponImage = MeleeWrathfulImage;

    killerNearMusic = musicData_Eventide_WrathfulNear;
	killerChaseMusic = musicData_Eventide_WrathfulChase;

    voicePack = "wrathful";

    rechargeRate = 0.375;	
	maxDamage = 1250;
	maxForwardSpeed = 7.7;
	maxBackwardSpeed = 4.4;
	maxSideSpeed = 6.6;

    chargeTime = 2000;
    chargeSpeedMultiplier = 2.0;
    chargeCooldown = 24000;

    rageCooldown = 60000;
    rageTime = 30000;

    stompCooldown = 36000;
};
//Inherit functions from `PlayerKiller`.
PlayerWrathful.inheritFunctionsFromSuperClass();

//
// Appearance.
//

function PlayerWrathful::eventideBodyParts(%this, %obj)
{
    %obj.unHideNode("ALL");
}

function PlayerWrathful::eventideBodyColors(%this, %obj)
{
    //The nodes have colored baked in, we don't need this.
}

//
// Charging functionality.
//

//
//// Status effect for charging functionality.
function WrathfulChargeEffect::beginStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Give a cutscene-esque effect by making the player only able to observe Wrathful, instead of control.
    %obj.lockInputs = true;
    %obj.createCameraOrbit();

    //Make Wrathful charge forward until the status effect runs out.
    %obj.originalDatablock = %obj.getDatablock();
    %obj.setDatablock(PlayerWrathfulCharging);

    //Play the charging animation.
    %obj.setActionThread("chargeCycle", 1);
    %obj.playThread(1, "charge");

    //Start the charging loop.
    PlayerWrathfulCharging.chargeTick(%obj);
}

function WrathfulChargeEffect::finalizeStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	//Give the player control of Wrathful again.
    %obj.lockInputs = false;
    %obj.restoreCameraFromOrbit();

    //Restore the Wrathful back to it's usual self. The extra datablock is needed so `onObjectCollision` isn't running constantly.
    if(%obj.originalDatablock == PlayerWrathful.getID())
    {
        //Prevents a new killer loop from being started when switching back to old datablock.
        %obj.shallowDatablockChanges = 2;
    }
    %obj.setDatablock(%obj.originalDatablock);

    //End the charging animation.
    %obj.setActionThread("root", 1);
    %obj.playThread(1, "chargeEnd");

    //End the charging loop.
    cancel(%obj.chargeSchedule);

    //Necessary for the item cooldown.
    %playerDatablock = %obj.getDatablock();
    %obj.lastChargeTime = getSimTime();
    %obj.weaponCooldown(%obj.currTool, "You're tired, and need to wait " @ mCeil(%playerDatablock.chargeCooldown / 1000) @ " seconds to charge again.", "Your charge ability is ready!", %playerDatablock.chargeCooldown);
}

//
// Charge loop, used to mimick the player moving running forward.
datablock PlayerData(PlayerWrathfulCharging : PlayerWrathful) 
{
    class = "PlayerWrathfulCharging";
    superClass = "PlayerWrathful";

    uiName = "";
};
//Inherit functions from `PlayerWrathful`.
PlayerWrathfulCharging.inheritFunctionsFromSuperClass();

function PlayerWrathfulCharging::chargeTick(%this, %obj)
{
    if(!isObject(%obj) || %obj.getState() $= "Dead")
    {
        return;
    }

    %forwardVector = %obj.getForwardVector();
    %chargeVelocity = VectorScale(%forwardVector, (%this.chargeSpeedMultiplier * %this.maxForwardSpeed));
    %obj.setVelocity(%chargeVelocity);

    cancel(%obj.chargeSchedule);
    %obj.chargeSchedule = %this.schedule(33, "chargeTick", %obj);
}

function PlayerWrathfulCharging::onObjectCollision(%this, %obj, %col)
{
    %wasImpact = false;

    if(%col.getType() & $TypeMasks::PlayerObjectType)
    {
        %wasImpact = true;

        %hostPosition = %obj.getHackPosition();
        %targetPosition = %col.getHackPosition();

        //For whatever reason, Wrathful collides with everyone in a 15 mile radius. Fine, be that way.
        if(VectorDist(%hostPosition, %targetPosition) > 2)
        {
            return true;
        }

        //The Wrathful has struck a player, fling them away.
        %impulseVector = VectorAdd(VectorScale(VectorNormalize(VectorSub(%targetPosition, %hostPosition)), 3), "0 0 0.6");
        %inheritence = VectorAdd(%col.getVelocity(), %impulseVector);
        %col.setVelocity(%inheritence);

        //Do some damage, and give them the Fear status effect (no items.)
        if(%col.getState() !$= "Dead" && !%col.hasStatusEffect("PlayerFearEffect", "Debuff"))
        {
            %col.damage(%obj, %obj.getHackPosition(), 36, $DamageType::Default);
            %col.fear(5000);
        }
    }
    else if(%col.getType() & $TypeMasks::FxBrickObjectType)
    {
        //Wrathful has struck a brick. If it's too tall to step on, end the charge.
        %playerAltitude = getWord(%obj.position, 2);
        %brickAltitude = getWord(%col.position, 2) + (%col.Datablock.brickSizeZ / 20); //`getPosition()` returns the center of the brick, so we need this trickery to get the top.
        if((%brickAltitude - %playerPosition) > 0.8) //0.8 Torque Units is the maximum step height players can seamlessly walk onto.
        {
            %wasImpact = true;
            %obj.clearStatusEffect("WrathfulChargeEffect", "Wrathful");
        }
    }

    if(%wasImpact)
    {
        //Dust explosion whenever Wrathful hits a player or brick.
        new Projectile()
        {
            dataBlock = tumbleImpactAProjectile;
            initialVelocity = "0 0 0";
            initialPosition = %obj.getTransform();
            sourceObject = %obj;
            sourceSlot = 0;
            client = %obj.client;
        }.explode();
    }

    return true;
}

//
// Raging functionality.
//

//
//// Status effect for charging functionality.
function WrathfulPreRageEffect::beginStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    //Give a cutscene-esque effect by making the player only able to observe Wrathful, instead of control.
    %obj.lockInputs = true;
    %obj.createCameraOrbit();

    //Play the charging animation.
    %obj.playThread(1, "rage");

    //"Do the roar."
    %obj.playVoiceLine("Rage");
}

function WrathfulPreRageEffect::finalizeStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	//Give the player control of Wrathful again.
    %obj.lockInputs = false;
    %obj.restoreCameraFromOrbit();

    //
    // Start the real rage status effect.

    %obj.lastRageTime = getSimTime();

    //Mount a more powerful melee weapon.
    %obj.killerWeaponImage = MeleeWrathfulRageImage;
    %obj.mountImage(%obj.killerWeaponImage, 0);

    //Increase Wrathful's speed.
    %obj.getDatablock().setTempSpeed(%obj, 1.2);
    %obj.defaultSpeed = 1.2;

    //Mount a rage-music audio emitter.
    %obj.playAccessoryMusic("rage_sound", 1.0, "Rage");

    //Use a separate status effect to expire this one.
    %obj.applyStatusEffect("WrathfulRageEffect", "Wrathful", PlayerWrathful.rageTime);
}

function WrathfulRageEffect::finalizeStatusEffect(%this, %obj)
{
	//If the player is dead, do nothing.
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

    %obj.lastRageTime = getSimTime();

    //Reset the melee weapon back to normal.
    %obj.killerWeaponImage = "";
    %obj.mountImage(PlayerWrathful.killerWeaponImage, 0);

    //Reset Wrathful's speed.
    %obj.defaultSpeed = "";
    %obj.getDatablock().setTempSpeed(%obj);

    //Stop the rage-music audio emitter.
    %obj.stopAccessoryMusic("Rage");

    //Play a little visual, let survivors know the rage is over.
	%obj.playThread(2, "undo");
    %obj.emote("HateImage", 1);
}