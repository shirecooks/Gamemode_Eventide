datablock ShapeBaseImageData(eventideMeleeImage)
{
	meleeTrailSkin = $Eventide_MeleeTrails["base.trail"];
	meleeTrailTime = 1000;
	meleeTrailOffset = "0.3 1.4 0.7"; 
	meleeTrailAngle1 = "0 90 0";
	meleeTrailAngle2 = "0 -90 0";
	meleeTrailAngle3 = "0 0 0";
	meleeTrailAngle4 = "0 180 0";
	useCustomMeleeTrail = true;

	hitProjectile = "";
	hitObscureProjectile = "KillerGenericSharpClankProjectile";

	meleeRange = 0.5;
	meleeCooldown = 1750;
	slowdownSpeed = 0.3;
	slowdownTime = 1500;

   	shapeFile = "base/data/shapes/empty.dts";
   	emap = true;
   	mountPoint = 0;
   	offset = "0 0 0";
   	eyeOffset = 0;
   	rotation = eulerToMatrix("0 0 0");
   	correctMuzzleVector = true;
   	className = "WeaponImage";
   	item = "";
   	ammo = "";
   	projectile = "";
   	projectileType = Projectile;
   	melee = true;
   	armReady = false;
   	doColorShift = false;

	stateName[0] = "Activate";
	stateWaitForTimeout[0] = true;
	stateTimeoutValue[0] = 0.01;
	stateTransitionOnTimeout[0] = "Ready";

	//The sudden jump in state number here it to allow for custom melee weapons to more easily 
	//implement their own logic between the "Activate" and "Ready" states.
	stateName[29] = "Ready";
	stateTransitionOnTriggerDown[29]  = "Swing";
	stateAllowImageChange[29] = true;
	stateSequence[29] = "Ready";

	stateName[30] = "Swing";
	stateScript[30] = "onSwing";
	stateFire[30] = false;
	stateAllowImageChange[30] = false;
	stateWaitForTimeout[30] = true;
	stateTimeoutValue[30] = 0.15;
	stateTransitionOnTimeout[30] = "Activate";
};

//
// Bonus abstract class for combining the weapon cooldown system with the Eventide melee system.
//

datablock ShapeBaseImageData(eventideCooldownMeleeImage : eventideMeleeImage)
{
	stateName[0] = "Activate";
    stateTimeoutValue[0] = 0.01;
    stateTransitionOnTimeout[0] = "CooldownCheck";

	//The sudden jump in state number here it to allow for custom melee weapons to more easily 
	//implement their own logic between the "Activate" and "CooldownCheck" states.

    //Check if the item is on cooldown. If not, proceed to "Ready".
    stateName[25] = "CooldownCheck";
    stateScript[25] = "onCooldownCheck";
    stateAllowImageChange[25] = false;
    stateWaitForTimeout[25] = true;
    stateTimeOutValue[25] = 0.01;
    stateTransitionOnTimeout[25] = "CooldownRedirect";

    //Redirect to another state based on what was set in tq he previous "CooldownCheck" state.
    stateName[26] = "CooldownRedirect";
    stateAllowImageChange[26] = false;
    stateTransitionOnAmmo[26] = "Cooldown";
    stateTransitionOnNoAmmo[26] = "Ready";

    //The item is on cooldown and cannot be used.
    stateName[27] = "Cooldown";
    stateScript[27] = "onCooldown";
    stateAllowImageChange[27] = true;
    ////The cooldown ended while the item was equipped, transition to the "Ready" state.
    stateTransitionOnNoAmmo[27] = "CooldownRevert";

    //The item is transitioning from "Cooldown" to "Ready", we need to raise the arm back up.
    stateName[28] = "CooldownRevert";
    stateScript[28] = "onCooldownRevert";
    stateAllowImageChange[28] = false;
    stateWaitForTimeout[28] = true;
    stateTimeOutValue[28] = 0.01;
    stateTransitionOnTimeout[28] = "Ready";

    //Default cooldown of 30 seconds if not set by the weapon itself.
    cooldown = 30000;
};

//
// Swing functionality.
//

function eventideMeleeImage::onSwing(%this, %obj)
{	
	%currentTime = getSimTime();
	%killerDatablock = %obj.getDataBlock();

	if(%obj.getState() $= "Dead" || %obj.getEnergyLevel() < (%killerDatablock.maxEnergy / 8) || (%obj.lastMeleeTime + %this.meleeCooldown) > %currentTime) 
	{
		return 0;
	}

	%killerLookVector = VectorNormalize(%obj.getLookVector());
	%killerPosition = %obj.getHackPosition();
	%killerWeaponPosition = %obj.getMuzzlePoint(0);
	
	//Melee cooldown and energy decrease.
	%obj.lastMeleeTime = %currentTime;	
	%obj.setEnergyLevel(%obj.getEnergyLevel() - (%killerDatablock.maxEnergy / 6));	

	//Melee swing sound effects.
	%obj.playVoiceLine("Attack"); //Killer grunt.

	//Melee animation.
	%meleeAnim = getRandom(1, 4);

	%customSwingAnimation = %this.customSwingAnimation;
	if(%customSwingAnimation !$= "")
	{
		%customSwingAnimationCount = %this.customSwingAnimationCount;
		%customSwingAnimationCount = (%customSwingAnimationCount $= "") ? "" : getRandom(1, %customSwingAnimationCount);

		%obj.playThread(2, %customSwingAnimation @ %customSwingAnimationCount);
	}
	else
	{
		%obj.playThread(2, "melee" @ %meleeAnim); //Weapon Swing animation.
	}

	%killerEyePosition = %obj.getEyePoint();
	%damageAmount = %this.fixedDamageAmount;
	if(%damageAmount $= "")
	{
		%damageAmount = (25 * getWord(%obj.getScale(), 2));
	}

	//Missing and/or striking the environment with the melee weapon.
	%hitObscureProjectile = %this.hitObscureProjectile;
	%typemasks = $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	%obstruction = ContainerRayCast(%killerEyePosition, VectorAdd(%killerEyePosition, VectorScale(%killerLookVector, mClampF(%this.meleeRange * 3, 3, $maxInt))), %typemasks);
	if(isObject(%obstruction) && %hitObscureProjectile !$= "")
	{							
		//Spawn a debris explosion.	
		%impactProjectile = new Projectile()
		{
			dataBlock = %hitObscureProjectile;
			initialPosition = posFromRaycast(%obstruction);
			sourceObject = %obj;
			client = %obj.client;
		};
		%impactProjectile.explode();

		%client = %obj.client;
		%minigameCanDamage = minigameCanDamage(%obj, %obstruction);
		if(%obstruction.getType() & $TypeMasks::FxBrickObjectType)
		{
			if(%minigameCanDamage)
			{
				%obstruction.onBlownUp(%client, %obj);
				transmitBrickExplosion(%pos, 40, 0.02, %respawnTime, %col);
			}
			else
			{
				%obstruction.onProjectileHit(%hitObscureProjectile, %client);
			}
		}
		else if(%minigameCanDamage)
		{
			%obstruction.damage(%obj, %killerPosition, %damageAmount, $DamageType::Default);
		}

		//Dynamic weapon recoil animation based on which direction the weapon was swung.
		%recoilAnimationDelay = 50;
		%obj.schedule(%recoilAnimationDelay, playThread, 3, "plant");

		if(%meleeAnim <= 2)
		{
			%recoilAnimation = (%meleeAnim == 1) ? "shiftTo" : "shiftAway";

			%obj.schedule(%recoilAnimationDelay, playThread, 2, "armReadyRight");
			%obj.schedule((%recoilAnimationDelay + 500), playThread, 2, "root");
		}
		else
		{
			%recoilAnimation = "wrench";

			%obj.schedule(%recoilAnimationDelay, playThread, 2, "root");
		}
		%obj.schedule(%recoilAnimationDelay, playThread, 1, %recoilAnimation);

		return 0;
	}

	%victims = "";

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

		%victims = (%victims $= "") ? %hit : (%victims SPC %hit);
		%victimPosition = %hit.getHackPosition();

		//Check if the killer is facing the victim. If not, do nothing.
		%dot = VectorDot(%killerLookVector, VectorNormalize(VectorSub(%victimPosition, %killerPosition)));
		if(%dot < 0.4)
		{
			continue;
		}

		//Mark on the weapon that it hit something.
		%currentHits = %obj.getImageAttribute("hits");
		%currentHits = %currentHits !$= "" ? %currentHits : 0;
		%obj.setImageAttribute("hits", %currentHits++);

		//Mark on the weapon the last person who was hit.
		%lastPersonHit = %obj.getImageAttribute("lastPersonHit");
		%lastPersonHit = %currentHits !$= "" ? %lastPersonHit : "";
		%obj.setImageAttribute("lastPersonHit", %hit.getID());

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
		%hit.setVelocity(VectorScale(VectorNormalize(VectorAdd(%obj.getForwardVector(), "0 0 0.15")), 15));
		%hit.damage(%obj, %hit.getHackPosition(), %damageAmount, $DamageType::Default);
		
		//Temporarily slow down the killer.
		%killerDatablock.setTempSpeed(%obj, %this.slowdownSpeed);
		%killerDatablock.schedule(%this.slowdownTime, setTempSpeed, %obj);
	}

	//Air slice sound.
	%soundEffect = %this.swingSound @ %this.swingSoundAmount @ "_sound";
	ServerPlay3D(%soundEffect, %killerWeaponPosition);

	//Visual air slice.
	if(%this.meleeTrailSkin !$= "") 
	{
		%meleeTrailAngle = %this.meleeTrailAngle[%meleeAnim];
		%obj.spawnMeleeTrail(%this.meleeTrailSkin, %this.meleeTrailTime, %this.meleeTrailOffset, %meleeTrailAngle, %this.meleeTrailScale);
	}

	return %victims;
}

//
// Package for playing sound from the hit projectiles.
//

package Weapon_KillerMelee
{
	function ProjectileData::onExplode(%this, %obj, %pos)
	{
		%returnValue = Parent::onExplode(%this, %obj, %pos);
		if(%this.hitSound !$= "" && %this.hitSoundAmount !$= "")
		{
			%soundEffect = %this.hitSound @ getRandom(1, %this.hitSoundAmount) @ "_sound";
			schedule(33, 0, ServerPlay3D, %soundEffect, %pos); //Won't play immediately for some reason. Oh well, let's fix that.
		}
		return %returnValue;
	}
};
if(isPackage(Weapon_KillerMelee))
{
	deactivatePackage(Weapon_KillerMelee);
}
activatePackage(Weapon_KillerMelee);