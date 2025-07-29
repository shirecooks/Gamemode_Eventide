//
// Damage types.
//

AddDamageType("ButterflyKnifeWeak", '<bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_butterflyknife> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_butterflyknife> %1', 1, 1);
AddDamageType("ButterflyKnifeBackstab", '<bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_butterflyknife> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_butterflyknife> %1', 1, 1);

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
    directDamage = 0; //Damge must be determined by a function.
    directDamageType = $DamageType::ButterflyKnifeBackstab;
    criticalHitDamage = 75;
};

function butterflyKnifeChargedProjectile::onCollision(%this, %obj, %col, %fade, %position, %normal, %velocity)
{
    //Not a player or a bot, do nothing.
    if(!(%col.getType() & $TypeMasks::PlayerObjectType))
    {
        return Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
    }

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

        //Stun the victim.
        if(%col.getState() !$= "Dead")
        {
            %col.mountImage("sm_stun", 3);
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

    image = butterflyknifeImage;
    canDrop = true;
};

datablock ShapeBaseImageData(butterflyknifeImage)
{
    className = "WeaponImage";

    shapeFile = "./models/bknife2.dts";
    armReady = true;
    emap = false;

    mountPoint = $RightHandSlot;
    offset = "0 0 0";

    item = ButterflyKnifeItem;
    ammo = " ";
    projectile = butterflyknifekillProjectile;

    melee = false;
    correctMuzzleVector = true;

    doColorShift = true;
    colorShiftColor = "0.400 0.196 0 1.000";

    //The knife has been equipped.
    stateName[0] = "Activate";
    stateTimeoutValue[0] = 0.5;
    stateTransitionOnTimeout[0]	= "Ready";
    stateSequence[0] = "activate";
    stateSound[0] = butterflyknife_equip_sound;

    //The knife is inactive, simply being held.
    stateName[1] = "Ready";
    stateSequence[1] = "ready";
    stateScript[1] = "onReady";
    stateTransitionOnTriggerDown[1]	= "Raising";
    stateAllowImageChange[1] = true;

    //The knife is being raised...
    stateName[2] = "Raising";
    stateSequence[2]= "ready";
    stateScript[2] = "onRaising";
    stateAllowImageChange[2] = true;
    //Timeout before the knife is fully raised.
    stateTransitionOnTimeout[2]	= "Raised";
    stateTimeoutValue[2] = 0.7;
    ////The knife was released before it was fully raised. Deliver a weak, 10 damage stab.
    stateWaitForTimeout[2] = false;
    stateTransitionOnTriggerUp[2] = "unchargedStab";

    //The knife was released prematurely.
    stateName[3] = "unchargedStab";
    stateSequence[3] = "ready";
    stateScript[3] = "unchargedStab";
    stateAllowImageChange[3] = false;
    ////Return it to the inactive state after a short delay.
    stateTransitionOnTimeout[3]	= "Ready";
    stateTimeoutValue[3] = 0.2;

    //Knife is fully raised, ready to release.
    stateName[4] = "Raised";
    stateTransitionOnTriggerUp[4] = "Fire";
    stateAllowImageChange[4] = true;
    stateWaitForTimeout[4] = false;

    //The knife was raised and released.
    stateName[5] = "Fire";
    stateSequence[5] = "ready";
    stateFire[5] = true; //Fire the weapon.
    stateScript[5] = "onFire";
    stateAllowImageChange[5]	= false;
    ////Return it to the inactive state after a short delay.
    stateTransitionOnTimeout[5]	= "Ready";
    stateTimeoutValue[5] = 0.2;
};

//
// Animations and damage logic.
//

function butterflyknifeImage::onReady(%this, %obj, %slot)
{
    //Play the idle animation.
	%obj.playthread(2, root);
}

function butterflyknifeImage::onRaising(%this, %obj, %slot)
{
    //Play the raising animation.
	%obj.playthread(2, spearReady);
}

function butterflyknifeImage::unchargedStab(%this, %obj, %slot)
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

function butterflyknifeImage::onFire(%this, %obj, %slot)
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