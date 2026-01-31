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
	hitObscureProjectile = "";

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

	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1]  = "Swing";
	stateAllowImageChange[1] = true;
	stateSequence[1] = "Ready";

	stateName[2] = "Swing";
	stateScript[2] = "onSwing";
	stateFire[2] = false;
	stateAllowImageChange[2] = false;
	stateWaitForTimeout[2] = true;
	stateTimeoutValue[2] = 0.15;
	stateTransitionOnTimeout[2] = "Ready";
};

function eventideMeleeImage::onSwing(%this, %obj)
{	
	%currentTime = getSimTime();
	%killerDatablock = %obj.getDataBlock();

	if(%obj.getState() $= "Dead" || %obj.getEnergyLevel() < (%killerDatablock.maxEnergy / 8) || (%obj.lastMeleeTime + %this.meleeCooldown) > %currentTime) 
	{
		return;
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

	//Missing and/or striking the environment with the melee weapon.
	%typemasks = $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	%obstruction = ContainerRayCast(%killerWeaponPosition, VectorAdd(%killerWeaponPosition, VectorScale(%killerLookVector, %this.meleeRange)), %typemasks, %obj);
	if(isObject(%obstruction) && %this.hitObscureProjectile !$= "")
	{							
		//Spawn a debris explosion.	
		%impactProjectile = new Projectile()
		{
			dataBlock = %this.hitObscureProjectile;
			initialPosition = posFromRaycast(%obstruction);
			sourceObject = %obj;
			client = %obj.client;
		};
		%impactProjectile.explode();

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

		return;
	}

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
		%damageAmount = %this.fixedDamageAmount;
		if(%damageAmount $= "")
		{
			%damageAmount = (25 * getWord(%obj.getScale(), 2));
		}
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