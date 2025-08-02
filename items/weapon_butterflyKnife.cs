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
        %cooldown = mCeil(butterflyKnifeImage.cooldown / 1000);

        //Put the player on cooldown after using the knife.
        //Triggers `stateTransitionOnAmmo[1]`.
        %attacker.weaponCooldown(%slot, "The blade was bent, and cannot be fixed for another " @ %cooldown @ " seconds.", "The knife is fixed and ready for combat!", 6);
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

    //Integral for cooldown checking upon knife re-equip.
    %attacker = %obj.sourceObject;
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
        if(miniGameCanDamage(%obj, %col))
        {
            %col.Damage(%obj, %position, %criticalHitDamage, butterflyKnifeChargedProjectile.directDamageType);
        }

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
        if(miniGameCanDamage(%obj, %col))
        {
            %col.Damage(%obj, %position, %failstabDamage, butterflyKnifeWeakProjectile.directDamageType);
        }
    }

    %slot = %obj.sourceSlot;
    %cooldown = mCeil(butterflyKnifeImage.cooldown / 1000);

    //Put the player on cooldown after using the knife.
    //Triggers `stateTransitionOnAmmo[1]`.
    %attacker.weaponCooldown(%slot, "The blade was bent, and cannot be fixed for another " @ %cooldown @ " seconds.", "The knife is fixed and ready for combat!", 6);

    return Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
}

//
// Pickup model and image.
//

datablock ItemData(butterflyKnifeItem)
{
    category = "Weapon";
    className = "Weapon";

    shapeFile = "./models/bknife2.dts";
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

datablock ShapeBaseImageData(butterflyKnifeImage)
{
    className = "WeaponImage";

    shapeFile = "./models/bknife2.dts";
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
    stateTransitionOnTimeout[0]	= "CooldownCheck";

    //Check if the knife is on cooldown. If not, proceed to "Ready".
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

    //The knife is on cooldown and cannot be used.
    stateName[3] = "Cooldown";
    stateSequence[3] = "ready";
    stateScript[3] = "onCooldown";
    stateAllowImageChange[3] = true;
    ////The cooldown ended while the knife was equipped, transition to the "Ready" state.
    stateTransitionOnNoAmmo[3] = "CooldownRevert";

    //The knife is transitioning from "Cooldown" to "Ready", we need to raise the arm back up.
    stateName[4] = "CooldownRevert";
    stateScript[4] = "onCooldownRevert";
    stateAllowImageChange[4] = false;
    stateWaitForTimeout[4] = true;
    stateTimeOutValue[4] = 0.01;
    stateTransitionOnTimeout[4] = "Ready";

    //The knife is inactive, simply being held.
    stateName[5] = "Ready";
    stateSequence[5] = "ready";
    stateScript[5] = "onReady";
    stateTransitionOnTriggerDown[5]	= "Raising";
    stateAllowImageChange[5] = true;

    //The knife is being raised...
    stateName[6] = "Raising";
    stateScript[6] = "onRaising";
    stateAllowImageChange[6] = true;
    //Timeout before the knife is fully raised.
    stateTransitionOnTimeout[6]	= "Raised";
    stateTimeoutValue[6] = 0.7;
    ////The knife was released before it was fully raised. Weak stab.
    stateWaitForTimeout[6] = false;
    stateTransitionOnTriggerUp[6] = "unchargedStab";

    //The knife was released prematurely.
    stateName[7] = "unchargedStab";
    stateScript[7] = "unchargedStab";
    stateAllowImageChange[7] = false;
    ////Return it to the inactive state after a short delay.
    stateTransitionOnTimeout[7]	= "CooldownCheck";
    stateTimeoutValue[7] = 0.2;

    //Knife is fully raised, ready to release.
    stateName[8] = "Raised";
    stateTransitionOnTriggerUp[8] = "Fire";
    stateAllowImageChange[8] = true;
    stateWaitForTimeout[8] = false;

    //The knife was raised and released.
    stateName[9] = "Fire";
    stateFire[9] = true; //Fire the weapon.
    stateScript[9] = "onFire";
    stateAllowImageChange[9] = false;
    ////Return it to the inactive state after a short delay.
    stateTransitionOnTimeout[9]	= "CooldownCheck";
    stateWaitForTimeout[9] = true;
    stateTimeoutValue[9] = 0.2;
};

//
// Animations and damage logic.
//

function butterflyKnifeImage::onCooldownCheck(%this, %obj, %slot)
{
    //The knife's animation always needs to be reset at this point.
    %obj.playThread(2, root);

    //If the knife has not passed it's cooldown time limit, transition to the "Cooldown" state.
    //Otherwise, transition to the "Ready" state.
    if((%obj.lastKnifeTime + %this.cooldown) > getSimTime())
    {
        %obj.setImageAmmo(%slot, 1);
    }
    else
    {
        %obj.setImageAmmo(%slot, 0);
    }
}

function butterflyKnifeImage::onCooldown(%this, %obj, %slot)
{
    //Lower the knife, it cannot be used.
    %obj.playThread(1, root);
}

function butterflyKnifeImage::onCooldownRevert(%this, %obj, %slot)
{
    //Raise the arm back up after being lowered.
    fixArmReady(%obj);
}

function butterflyKnifeImage::onReady(%this, %obj, %slot)
{

}

function butterflyKnifeImage::onRaising(%this, %obj, %slot)
{
    //Play the raising animation.
	%obj.playthread(2, spearReady);
}

function butterflyKnifeImage::unchargedStab(%this, %obj, %slot)
{
    //Play the weak jabbing animation.
	%obj.playthread(2, armattack);

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
        initialPosition = %obj.getMuzzlePoint(%slot);
        sourceObject = %obj;
        sourceSlot = %slot;
        client = %obj.client;
    };
}

function butterflyKnifeImage::onFire(%this, %obj, %slot)
{
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
        initialPosition = %obj.getMuzzlePoint(%slot);
        sourceObject = %obj;
        sourceSlot = %slot;
        client = %obj.client;
    };
}