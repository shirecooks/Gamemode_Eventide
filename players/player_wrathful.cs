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
    %obj.lastChargeTime = (%obj.lastChargeTime $= "") ? (getSimTime() - wrathfulChargeAbilityImage.chargeCooldown) : %obj.lastChargeTime;

	if((getSimTime() - %obj.lastChargeTime) >= %this.chargeCooldown)
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
		%client.printFormatString("hint", "You can't charge again yet! Wait another " @ sFromMs(%this.chargeCooldown - (getSimTime() - %obj.lastChargeTime)) @ " seconds.");
	}
}

function wrathfulChargeAbilityImage::onCharge(%this, %obj, %slot)
{
	%playerDatablock = %obj.getDatablock();
    %playerDatablock.beginCharge(%obj);
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

    rageCooldown = 60000;
    rageTime = 30;
};

function wrathfulRageAbilityImage::onCooldownCheck(%this, %obj, %slot)
{
    %obj.lastRageTime = (%obj.lastRageTime $= "") ? (getSimTime() - wrathfulRageAbilityImage.rageCooldown) : %obj.lastRageTime;

	if((getSimTime() - %obj.lastRageTime) >= %this.rageCooldown)
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
		%client.printFormatString("hint", "You can't rage again yet! Wait another " @ sFromMs(%this.rageCooldown - (getSimTime() - %obj.lastRageTime)) @ " seconds.");
	}
}

function wrathfulRageAbilityImage::onRage(%this, %obj, %slot)
{
	%playerDatablock = %obj.getDatablock();
    %playerDatablock.beginRagePhaseOne(%obj);
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
        %hit.setVelocity(VectorScale(VectorNormalize(VectorAdd(%obj.getForwardVector(), "0 0 0.6")), 3));
		%hit.damage(%obj, %hit.getHackPosition(), %this.fixedDamageAmount, $DamageType::Default);
		
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
    sequence26 = "./models/wrathful/w_melee.dsq";
    sequence27 = "./models/wrathful/w_charge.dsq";
    sequence28 = "./models/wrathful/w_chargecycle.dsq";
    sequence29 = "./models/wrathful/w_chargerecovery.dsq";
    sequence30 = "./models/wrathful/w_handcannon.dsq";
    sequence31 = "./models/wrathful/w_rage.dsq";
    sequence32 = "./models/wrathful/w_stomp.dsq";
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

function PlayerWrathful::beginCharge(%this, %obj)
{
    //Disable the player's control over Wrathful, so they only move in a straight line while charging.
    %client = %obj.client;
    if(isObject(%client))
    {
        %client.setControlObject(%client.camera);
	    %client.camera.setMode("Corpse", %obj);
    }

    %obj.isCharging = true;
    %obj.lastChargeTime = getSimTime();

    //Begin the charging state by switch Wrathful to the charging datablock.
    %newDatablock = PlayerWrathfulCharging;
    %obj.setDatablock(%newDatablock);
    %newDatablock.chargeTick(%obj);

    //Play the charging animation.
    %obj.playThread(1, "w_charge");
}

datablock PlayerData(PlayerWrathfulCharging : PlayerWrathful) 
{
    class = "PlayerWrathfulCharging";
    superClass = "PlayerWrathful";
    uiName = "";
};
//Inherit functions from `PlayerWrathful`.
PlayerWrathfulCharging.inheritFunctionsFromSuperClass();

//
// Charge loop, used to mimick the player moving running forward.
function PlayerWrathfulCharging::chargeTick(%this, %obj)
{
    if(!isObject(%obj) || %obj.getState() $= "Dead")
    {
        return;
    }

    //Wrathful can only charge for two seconds at most.
    if((getSimTime() - %obj.lastChargeTime) > wrathfulChargeAbilityImage.chargeTime)
    {
        %this.endCharge(%obj);
        return;
    }

    %forwardVector = %obj.getForwardVector();
    %chargeVelocity = VectorScale(%forwardVector, (2 * PlayerWrathful.maxForwardSpeed));
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

        //Do some damage.
        if(!%col.isStunned)
        {
            %col.damage(%obj, %obj.getHackPosition(), 36, $DamageType::Default);
        }

        //Give them the `fear` condition (no items).
        if(%col.getState() !$= "Dead")
        {
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
            %this.endCharge(%obj);
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

function PlayerWrathfulCharging::endCharge(%this, %obj)
{
    //Give the player control of their character again.
    %client = %obj.client;
    if(isObject(%client))
    {
        %client.setControlObject(%obj);
	    %client.camera.setMode("Observer");
    }

    //End the charge by switching the player back to the usual player datablock.
    cancel(%obj.chargeSchedule);
    %obj.isCharging = false;
    %obj.lastChargeTime = getSimTime();
    %obj.setDatablock(PlayerWrathful);

    //Play the reset animation.
    %obj.setActionThread("root");
    %obj.playThread(1, "w_chargerecovery");
}

//
// Raging functionality.
//

function PlayerWrathful::beginRagePhaseOne(%this, %obj)
{
    //Disable the player's control over Wrathful, so they have to watch the roar play out.
    %client = %obj.client;
    if(isObject(%client))
    {
        %client.setControlObject(%client.camera);
	    %client.camera.setMode("Corpse", %obj);
    }

    %obj.isRaging = true;

    //Play the raging animation.
    %obj.playThread(1, "w_rage");

    //"Do the roar."
    %obj.playVoiceLine("Rage");

    %this.schedule(2500, "beginRagePhaseTwo", %obj);
}

function PlayerWrathful::beginRagePhaseTwo(%this, %obj)
{
    if(!isObject(%obj) || %obj.getState() $= "Dead")
    {
        return;
    }

    //Give the player control of their character again.
    %client = %obj.client;
    if(isObject(%client))
    {
        %client.setControlObject(%obj);
	    %client.camera.setMode("Observer");
    }

    //Reset the raging animation.
    %obj.playThread(1, "root");

    %obj.lastRageTime = getSimTime();

    //Begin the charging state by switch Wrathful to the charging datablock.
    %newDatablock = PlayerWrathfulRaging;
    %obj.setDatablock(%newDatablock);
    %obj.rageSchedule = %newDatablock.schedule(30000, "endRage", %obj);
}

datablock PlayerData(PlayerWrathfulRaging : PlayerWrathful) 
{
    class = "PlayerWrathfulRaging";
    superClass = "PlayerWrathful";
    uiName = "";

    killerWeaponImage = MeleeWrathfulRageImage;

    maxForwardSpeed = 9.24;
	maxBackwardSpeed = 5.5;
	maxSideSpeed = 7.92;
};
//Inherit functions from `PlayerWrathful`.
PlayerWrathfulRaging.inheritFunctionsFromSuperClass();

function PlayerWrathfulRaging::endRage(%this, %obj)
{
    if(!isObject(%obj) || %obj.getState() $= "Dead")
    {
        return;
    }

    //Give the player control of their character again.
    %client = %obj.client;
    if(isObject(%client))
    {
        %client.setControlObject(%obj);
	    %client.camera.setMode("Observer");
    }

    //End the charge by switching the player back to the usual player datablock.
    %obj.isRaging = false;
    %obj.lastRageTime = getSimTime();
    %obj.setDatablock(PlayerWrathful);

    //Play the reset animation.
    %obj.emote("HateImage", 1);
}

//
// Package, for everything.
//

package Player_Wrathful
{
    function Observer::onTrigger(%this, %obj, %trigger, %state)
    {
        %client = %obj.getControllingClient();
        %player = %client.player;
        if(!%player)
        {
            return Parent::onTrigger(%this, %obj, %trigger, %state);
        }
        else if(!%player.isCharging && ! %player.isRaging)
        {
            return Parent::onTrigger(%this, %obj, %trigger, %state);
        }
    }

    function serverCmdUseTool(%client, %slot)
    {
        %player = %client.player;
        if(%player && (%player.isCharging || %player.isRaging))
        {
            return;
        }
        parent::ServerCmdUseTool(%client, %slot);
    }

    function ServerCmdUnUseTool(%client)
    {
        %player = %client.player;
        if(%player && (%player.isCharging || %player.isRaging))
        {
            return;
        }
        parent::ServerCmdUnUseTool(%client);
    }
};
if(isPackage(Player_Wrathful))
{
    deactivatePackage(Player_Wrathful);
}
activatePackage(Player_Wrathful);