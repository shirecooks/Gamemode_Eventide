//
// Miscellaneous.
//

//
// The rope attached to the projectile.
datablock StaticShapeData(anglerRope)
{
	shapeFile = "./models/hookrope.dts";
	isHookRope = true;
};

function anglerRope::onAdd(%this, %obj)
{
    %obj.setNodeColor("ALL", "0.5 0.5 0.5 1");
}

function anglerRope::onRemove(%this, %obj)
{
    %killer = %obj.startObject;
    %victim = %obj.endObject;

    if(isObject(%killer))
    {
        %killer.playManagedSound("Reel");
    }
    if(isObject(%victim) && %victim.getType() & $TypeMasks::PlayerObjectType)
    {
        %victim.setActionThread(root, 0);
    }

    if(isObject(%obj))
    {
        %obj.delete();
    }
}

function anglerRope::scaleBetweenPoints(%this, %obj, %startPoint, %endPoint)
{
    //Mystery code from Angler's original function. Readability is for losers.
	%direction = vectorNormalize(vectorSub(%endPoint, %startPoint));
	%relative = "0 1 0";

	%xyz = vectorNormalize(vectorCross(%relative, %direction));
	%u = mACos(vectorDot(%relative, %direction)) * -1;

	%obj.setTransform(vectorScale(vectorAdd(vectorAdd(%startPoint, "0 0 0.5"), %endPoint), 0.5) SPC %xyz SPC %u);
	%obj.setScale(0.5 SPC vectorDist(%startPoint, %endPoint) * 2 SPC 0.5);
}

//Continously scales the rope between a start object and end object.
function anglerRope::track(%this, %obj)
{
    %killer = %obj.startObject;
    %victim = %obj.endObject;

    if(!isObject(%obj) || !isObject(%killer) || !isObject(%victim))
    {
        %this.onRemove(%obj);
        return;
    }

    %startPoint = %obj.startObject.getPosition();
    %endPoint = %obj.endObject.getPosition();
    %this.scaleBetweenPoints(%obj, %startPoint, %endPoint);

    %obj.trackSchedule = %this.schedule(33, "track", %obj);
}

function anglerRope::reel(%this, %hookRope)
{
    %killer = %hookRope.startObject;
    %victim = %hookRope.endObject;

    %victimPosition = %victim.getHackPosition();
    %killerPosition = %killer.getHackPosition();

    if(!isObject(%hookRope))
    {
        return;
    }
    else if(!isObject(%killer) || !isObject(%victim) || %killer.getState() $= "Dead")
    {
        return %this.onRemove(%hookRope);
    }
    else if(VectorDist(%victimPosition, %killerPosition) < 2)
    {
        //If the victim is close enough to the killer, release them.
        return %this.onRemove(%hookRope);
    }
    else if((getSimTime() - %victim.lastReelTime) > PlayerAngler.maxReelTime)
    {
        //The victim has been reeled for too long, let them go.
        return %this.onRemove(%hookRope);
    }

    //Play an animation on the victim.
    %victim.setActionThread("sit", 0);

    //Drag the player towards the killer.
    %direction = VectorNormalize(VectorSub(%killer.getMuzzlePoint(0), %victimPosition));
    %victim.setVelocity(VectorScale(%direction, 10));

    //Occasionally, damage the victim and play a sound effect on the killer.
    if(((getSimTime() - %victim.lastReelTime) % 500) <= 5)
    {
        %killer.playManagedSound("Reel");
        %victim.damage(%killer, %killerPosition, 2, $DamageType::Default);
    }

    %killer.reelSchedule = %this.schedule(33, "reel", %hookRope);
}

//
//// The projectile.
datablock ProjectileData(anglerHookProjectile)
{
	projectileShapeName = "./models/anglerhookproj.dts";
	directDamage = 0;
	directDamageType = $DamageType::Default;

	impactImpulse = 10;
	verticalImpulse = 25;
	explosion = "";
	particleEmitter = "";
	sound = "";

	muzzleVelocity = 50;
	velInheritFactor = 0;

	armingDelay = 0;
	lifetime = 30000;
	fadeDelay = 1000;
	bounceElasticity = 0.5;
	bounceFriction = 0.5;
	isBallistic = false;

	hasLight = false;
	lightRadius = 1;
	lightColor = "0 0 0";
	gravityMod 	= true;
    gravityCoefficient = 1.0;

	uiName = "";
};

function anglerHookProjectile::clearRope(%this, %obj)
{
    %hookRope = %obj.hookRope;
    %hookRope.Datablock.onRemove(%hookRope);
}

function anglerHookProjectile::onAdd(%this, %obj)
{
    %hookRope = new StaticShape()
    {
        dataBlock = anglerRope;

        sourceObject = %obj.sourceObject;
        client = %obj.sourceObject.client;
    };
    %hookRope.startObject = %obj.sourceObject;
    %hookRope.endObject = %obj;

    %obj.hookRope = %hookRope;
    %hookRope.Datablock.track(%hookRope);
    %this.castTick(%obj);
}

function anglerHookProjectile::castTick(%this, %obj)
{
    if(!isObject(%obj))
    {
        return;
    }

    if(VectorDist(%obj.initialPosition, %obj.getPosition()) > PlayerAngler.throwLength)
    {
        %this.clearRope(%obj);
        return;
    }

    %obj.castSchedule = %this.schedule(33, "castTick", %obj);
}

function anglerHookProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity)
{
    %killer = %obj.sourceObject;

    //Play a sound effect at the site of impact.
    ServerPlay3D("chain_clash_sound", %pos);

    if(!(%col.getType() & $TypeMasks::PlayerObjectType) || %col.getDatablock().isKiller || !minigameCanDamage(%killer, %col))
    {
        //If it wasn't a player (we can damage), so we don't care.
        %this.clearRope(%obj);
        return;
    }

    %hookRope = %obj.hookRope;

    //Play a sound effect, we got a hit.
    %killer.playManagedSound("Reel");

    //Attach the rope to the player instead of the hook, which is due to despawn.
    %hookRope.endObject = %col;
    %col.lastReelTime = getSimTime();

    //Start reeling in the victim.
    %hookRope.Datablock.reel(%hookRope);

    //Despawn the original hook.
    parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
}

//
// Melee weapons.
//

datablock ShapeBaseImageData(MeleeAnglerImage : eventideMeleeImage)
{
   	shapeFile = "./models/anglerhook.dts";
	
	hitProjectile = KillerSharpHitProjectile;
	hitObscureProjectile = KillerGenericSharpClankProjectile;

	meleeTrail = $Eventide_MeleeTrails["ragged.trail"];
    meleeTrailScale = "6 5 0.4";

    customSwingAnimation = "melee";
    customSwingAnimationCount = 2;

    swingSound = "anglerSwing";
	swingSoundAmount = 5;
};
MeleeAnglerImage.inheritFunctionsFromSuperClass("eventideMeleeImage");

//
//// Ability and item to initiate casting.
datablock ItemData(anglerCastAbilityItem)
{
	category = "Weapon";
	className = "Weapon";

	shapeFile = "./models/anglerhook.dts";

	uiName = "Cast";
	iconName = "./icons/hicolor_meathook";

	image = anglerCastAbilityImage;
	canDrop = false;
};

datablock ShapeBaseImageData(anglerCastAbilityImage)
{
	className = "WeaponImage";
	shapeFile = anglerCastAbilityItem.shapeFile;

	mountPoint = $RightHandSlot;
   	offset = "0 0 0";
   	eyeOffset = 0;
   	rotation = eulerToMatrix("0 0 0");
   	correctMuzzleVector = false;

   	item = anglerCastAbilityItem;
   	ammo = "";
   	projectile = "";
   	projectileType = Projectile;
   	melee = false;
   	armReady = true;

	hintStyle = "hint";
	hintMessage = "Click to throw a hook that reels other people in. Don't miss.";

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
	stateTransitionOnAmmo[3] = "Cast";
	stateTransitionOnNoAmmo[3] = "CooldownCheckFail";

	stateName[4] = "CooldownCheckFail";
	stateScript[4] = "onCooldownCheckFail";
	stateTransitionOnTimeout[4] = "Ready";
	stateWaitForTimeout[4] = true;
	stateTimeoutValue[4] = 0.01;

	stateName[5] = "Cast";
	stateScript[5] = "onCast";
	stateAllowImageChange[5] = true;
	stateWaitForTimeout[5] = true;
	stateTimeoutValue[5] = 0.01;
	stateTransitionOnTimeout[5] = "Ready";
};

function anglerCastAbilityImage::onCooldownCheck(%this, %obj)
{
    %castCooldown = (%obj.getDatablock().castCooldown $= "") ? 20000 : %obj.getDatablock().castCooldown;
	if((%obj.lastCastTime + %castCooldown) < getSimTime())
	{
		%obj.setImageAmmo(%this.mountPoint, true);
	}
	else
	{
		%obj.setImageAmmo(%this.mountPoint, false);
	}
}

function anglerCastAbilityImage::onCooldownCheckFail(%this, %obj)
{
	//Play a fail animation.
	%obj.playThread(2, "undo");

	//Tell the killer they need to wait for longer.
	%client = %obj.client;
	if(%client)
	{
        %castCooldown = (%obj.getDatablock().castCooldown $= "") ? 20000 : %obj.getDatablock().castCooldown;
		%client.printFormatString("hint", "You can't cast again yet! Wait another " @ sFromMs(%castCooldown - (getSimTime() - %obj.lastCastTime)) @ " seconds.");
	}
}

function anglerCastAbilityImage::onCast(%this, %obj)
{
    %obj.lastCastTime = getSimTime();

    //Throwing sound.
    %obj.playManagedSound("Cast");

    //Temporarily unmount the hook, so the Angler can throw it.
    %obj.unmountImage(0);

    //Throw the hook from the Angler's hand toward the focal point.
    %focalPoint = VectorAdd(%obj.getEyePoint(), VectorScale(%obj.getLookVector(), 1000));
    %muzzlePoint = %obj.getMuzzlePoint(0);
    %aimVector = VectorNormalize(VectorSub(%focalPoint, %muzzlePoint));

    %hookProjectile = new Projectile()
    {
        dataBlock = anglerHookProjectile;
        initialVelocity = VectorScale(%aimVector, anglerHookProjectile.muzzleVelocity);
        initialPosition = %muzzlePoint;

        sourceObject = %obj;
        client = %obj.client;
    };

    //Play a throwing animation.
    %obj.playThread(2, "shiftAway");
	%obj.playThread(3, "jump");
}

//
// Playertype.
//

datablock TSShapeConstructor(AnglerDTS) 
{
	baseShape = "./models/angler.dts";
	sequence0 = "./models/angler.dsq";
	sequence1 = "./models/angler_melee.dsq";
};

datablock PlayerData(PlayerAngler : PlayerKiller) 
{
	uiName = "Angler Player";
	shapeFile = AnglerDTS.baseShape;

	maxDamage = 1200;

	maxTools = 2;
	maxWeapons = 2;
	
	killerlight = "NoFlareRLight";	

	killerNearMusic = "musicData_Eventide_AnglerNear";
	killerChaseMusic = "musicData_Eventide_AnglerChase";

    killerWeaponImage = MeleeAnglerImage;
    voicePack = "angler";

    throwLength = 16;
    maxReelTime = 10000;
};
PlayerAngler.inheritFunctionsFromSuperClass("PlayerKiller");

function PlayerAngler::onNewDataBlock(%this, %obj)
{
    %this.super("onNewDatablock", %this, %obj);

    %voiceConfig = %obj.voiceConfig;
    %voiceConfig.setLineCooldown("Cast", 0);
    %voiceConfig.setLineCooldown("Reel", 0);
}

//
// Appearance.
//

function PlayerAngler::eventideBodyParts(%this, %obj)
{
    %obj.setScale("1.2 1.2 1.2");
    %obj.unHideNode("ALL");
}

function PlayerAngler::eventideBodyColors(%this, %obj)
{
    %skinColor = "0.16 0.27 0.43 1";
	%obj.setNodeColor("chest", %skinColor);
	%obj.setNodeColor("pants", %skinColor);
	%obj.setNodeColor("Lhand", %skinColor);
	%obj.setNodeColor("Rhand", %skinColor);
	%obj.setNodeColor("Larm", %skinColor);
	%obj.setNodeColor("Rarm", %skinColor);
	%obj.setNodeColor("LShoe", %skinColor);
	%obj.setNodeColor("RShoe", %skinColor);
	%obj.setNodeColor("HeadSkin", %skinColor);
}