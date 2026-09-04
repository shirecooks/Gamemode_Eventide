//
// Damage types.
//

AddDamageType("ButterflyKnifeWeak", '<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_butterflyknife> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_butterflyknife> %1', 1, 1);
AddDamageType("ButterflyKnifeBackstab", '<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_butterflyknife> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_butterflyknife> %1', 1, 1);

//
// Projectiles datablocks.
//

datablock ProjectileData(butterflyKnifeWeakProjectile)
{
    uiName = "";

    directDamage = 10;
    directDamageType = $DamageType::ButterflyKnifeWeak;

    explosion = swordExplosion;

    muzzleVelocity = 50;
    velInheritFactor = 1;

    armingDelay = 0;
    lifetime = 100;
    fadeDelay = 70;
    bounceElasticity = 0;
    bounceFriction = 0;
    isBallistic = false;
    gravityMod = 0.0;

    hasLight = false;
    lightRadius = 3.0;
    lightColor = "0 0 0.5";
};

datablock ProjectileData(butterflyKnifeChargedProjectile : butterflyKnifeWeakProjectile)
{
    directDamage = 0; //Damage must be determined by a function.
    directDamageType = $DamageType::ButterflyKnifeBackstab;
    criticalHitDamage = 75;
};

function butterflyKnifeWeakProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity)
{
    if(%col.getType() & $TypeMasks::PlayerObjectType)
    {
        //Integral for cooldown checking upon knife re-equip.
        %attacker = %obj.sourceObject;
        %attacker.lastKnifeTime = getSimTime();

        %slot = %obj.sourceSlot;

        //Put the player on cooldown after using the knife.
        //Triggers `stateTransitionOnAmmo[1]`.
        %attacker.weaponCooldown(%slot, "The blade was bent, and cannot be fixed for another " @ sFromMs(butterflyKnifeImage.cooldown) @ " seconds.", "The knife is fixed and ready for combat!", 6);
    }
    return Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
}

function butterflyKnifeChargedProjectile::onCollision(%this, %obj, %col, %fade, %position, %normal, %velocity)
{
    //Not a player or a bot, do nothing.
    if(!(%col.getType() & $TypeMasks::PlayerObjectType))
    {
        return Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
    }
    
    //Be forgiving, don't waste the knife attack if the target was a teammate.
    %attacker = %obj.sourceObject;
    if(!miniGameCanDamage(%attacker, %col))
    {
        return Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
    }

    //Essential for cooldown checking upon knife re-equip.
    %attacker.lastKnifeTime = getSimTime();

    %victimForwardVector = %col.getForwardVector(); //Already normalized.
    %collisionNormal = VectorNormalize(VectorScale(%velocity, -1));

    //%dot ~1: In front of the victim.
    //%dot ~0: At the victim's side.
    //%dot <0: Behind the victim.
    %dot = VectorDot(%victimForwardVector, %collisionNormal);

    //Do critical hit damage from behind, and weak damage from the side or front.
    %failstabDamage = butterflyKnifeWeakProjectile.directDamage;
    %criticalHitDamage = butterflyKnifeChargedProjectile.criticalHitDamage;
    if(%dot <= -0.5)
    {
        //Critical hit!
        %col.Damage(%obj, %position, %criticalHitDamage, butterflyKnifeChargedProjectile.directDamageType);

        //Stun the victim, if the attack didn't kill them.
        if(%col.getState() !$= "Dead")
        {
            %col.stun(2500);
        }

        //Fancy slice sound.
        serverPlay3D("melee_tanto" @ getRandom(1, 3) @ "_sound", %position);

        //Bloody explosion.
        %bloodExplosion = new Projectile()
        {
            dataBlock = bloodExplosionProjectile1;
            initialVelocity = %velocity;
            initialPosition = %position;
            sourceObject = %col;
            sourceSlot = 0;
            client = %col.client;
        };
        %bloodExplosion.setScale("2 2 2");
        %bloodExplosion.explode();
    }
    else
    {
        //Weak hit... :(
        %col.Damage(%obj, %position, %failstabDamage, butterflyKnifeWeakProjectile.directDamageType);
    }

    %slot = %obj.sourceSlot;

    //Put the player on cooldown after using the knife.
    %attacker.weaponCooldown(%slot, "The blade is bent, and cannot be fixed for another " @ sFromMs(butterflyKnifeImage.cooldown) @ " seconds.", "The knife is fixed and ready for combat!", 6);

    return Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
}

//
// Pickup model and image.
//

datablock ItemData(butterflyKnifeItem)
{
    category = "Weapon";
    className = "Weapon";

    shapeFile = "./models/butterflyKnife/butterflyKnife.dts";
    doColorShift = false;
    colorShiftColor = "0.400 0.196 0 1.000";

    mass = 1;
    density = 0.2;
    elasticity = 0.2;
    friction = 0.6;
    emap = false;

    uiName = "Butterfly Knife";
    iconName = "./icons/icon_butterflyknife";

    image = butterflyKnifeImage;
    canDrop = true;
};

datablock ShapeBaseImageData(butterflyKnifeImage : cooldownImage)
{
    className = "WeaponImage";

    shapeFile = butterflyKnifeItem.shapeFile;
    armReady = true;
    emap = false;

    mountPoint = $RightHandSlot;
    offset = "0 0 0";

    item = ButterflyKnifeItem;
    ammo = false;
    projectile = "";
    cooldown = 32000;

    melee = false;
    correctMuzzleVector = true;

    doColorShift = true;
    colorShiftColor = "0.400 0.196 0 1.000";

    //The knife has been equipped.
    stateName[0] = "Activate";
    stateSequence[0] = "activate";
    stateSound[0] = butterflyknife_equip_sound;
    stateTimeoutValue[0] = 0.5;

    //The knife is inactive, simply being held.
    stateName[1] = "Ready";
    stateSequence[1] = "ready";
    stateScript[1] = "onReady";
    stateTransitionOnTriggerDown[1]	= "Raising";
    stateAllowImageChange[1] = true;

    //The knife is being raised...
    stateName[2] = "Raising";
    stateScript[2] = "onRaising";
    stateAllowImageChange[2] = true;
    //Timeout before the knife is fully raised.
    stateTransitionOnTimeout[2]	= "Raised";
    stateTimeoutValue[2] = 0.7;
    ////The knife was released before it was fully raised. Weak stab.
    stateWaitForTimeout[2] = false;
    stateTransitionOnTriggerUp[2] = "unchargedStab";

    //The knife was released prematurely.
    stateName[3] = "unchargedStab";
    stateScript[3] = "unchargedStab";
    stateAllowImageChange[3] = false;
    ////Return it to the inactive state after a short delay.
    stateTransitionOnTimeout[3]	= "CooldownCheck";
    stateTimeoutValue[3] = 0.2;

    //Knife is fully raised, ready to release.
    stateName[4] = "Raised";
    stateTransitionOnTriggerUp[4] = "Fire";
    stateAllowImageChange[4] = true;
    stateWaitForTimeout[4] = false;

    //The knife was raised and released.
    stateName[5] = "Fire";
    stateFire[5] = true; //Fire the weapon.
    stateScript[5] = "onFire";
    stateAllowImageChange[5] = false;
    ////Return it to the inactive state after a short delay.
    stateTransitionOnTimeout[5]	= "Cooldown";
    stateWaitForTimeout[5] = true;
    stateTimeoutValue[5] = 0.2;
};
butterflyKnifeImage.implementCooldownCallbacks();

function butterflyKnifeImage::getHintMessage(%this, %obj)
{
	return "You know the drill: go for the back.";
}

//
// Animations and damage logic.
//

function butterflyKnifeImage::onReady(%this, %obj)
{
    //Needed to end the swing animation if the player released the knife before it was fully raised.
    %obj.playThread(2, root);
}

function butterflyKnifeImage::onRaising(%this, %obj)
{
    //Play the raising animation.
	%obj.playThread(2, spearReady);
}

function butterflyKnifeImage::unchargedStab(%this, %obj)
{
    %slot = %obj.currTool;
    
    //Play the weak jabbing animation.
	%obj.playThread(2, armattack);

    //Knife projectile velocity determination.
    %aimVector = %obj.getMuzzleVector(%slot);
    %playerVelocity = %obj.getVelocity();

    %projectileDirection = VectorScale(%aimVector, butterflyKnifeWeakProjectile.muzzleVelocity);
    %projectileVelocity = VectorScale(%playerVelocity, butterflyKnifeWeakProjectile.velInheritFactor);
    %projectileFinalVelocity = VectorAdd(%projectileDirection, %projectileVelocity);

    //Spawn a weak projectile.
    %weakProjectile = new Projectile()
    {
        dataBlock = butterflyKnifeWeakProjectile;
        initialVelocity = %projectileFinalVelocity;
        initialPosition = %obj.getMuzzlePoint(%this.mountPoint);
        sourceObject = %obj;
        sourceSlot = %slot;
        client = %obj.client;
    };
}

function butterflyKnifeImage::onFire(%this, %obj)
{
    %slot = %obj.currTool;
    
    //Play the overhand stabbing animation.
	%obj.playthread(2, spearThrow);

    //Knife projectile velocity determination.
    %aimVector = %obj.getMuzzleVector(%slot);
    %playerVelocity = %obj.getVelocity();

    %projectileDirection = VectorScale(%aimVector, butterflyKnifeChargedProjectile.muzzleVelocity);
    %projectileVelocity = VectorScale(%playerVelocity, butterflyKnifeChargedProjectile.velInheritFactor);
    %projectileFinalVelocity = VectorAdd(%projectileDirection, %projectileVelocity);

    //Spawn a stronger projectile that does a critical hit from behind.
    %chargedProjectile = new Projectile()
    {
        dataBlock = butterflyKnifeChargedProjectile;
        initialVelocity = %projectileFinalVelocity;
        initialPosition = %obj.getMuzzlePoint(%this.mountPoint);
        sourceObject = %obj;
        sourceSlot = %slot;
        client = %obj.client;
    };
}