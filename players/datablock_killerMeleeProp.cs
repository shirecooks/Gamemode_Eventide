datablock ShapeBaseImageData(KillerMeleeImage)
{
    class = "KillerMelee";
    superClass = "";

	meleeRange = 4;
	meleeCooldown = 1750;
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

function KillerMeleeImage::onSwing(%this, %obj, %slot)
{	
	%currentTime = getSimTime();
	%killerDatablock = %obj.getDataBlock();

	if(%obj.getState() $= "Dead" || %obj.isInvisible || %obj.getEnergyLevel() < (%killerDatablock.maxEnergy / 8) || (%obj.lastMeleeTime + %this.meleeCooldown) > %currentTime) 
	{
		return;
	}

	%killerEyePoint = %obj.getEyePoint();
	%killerLookVector = VectorNormalize(%obj.getLookVector());
	%killerPosition = %obj.getHackPosition();
	%killerWeaponPosition = %obj.getMuzzlePoint(0);
	
	//Melee cooldown and energy decrease.
	%obj.lastMeleeTime = %currentTIme;	
	%obj.setEnergyLevel(%obj.getEnergyLevel() - (%killerDatablock.maxEnergy / 6));	

	//Melee swing sound effects.
	%obj.playVoiceLine("Attack"); //Killer grunt.
	
	//Air slice sound.
	%soundEffect = %this.swingSound @ %this.swingSoundAmount;
	ServerPlay3D(%soundEffect, %killerWeaponPosition);

	//Melee animation.
	%meleeAnim = getRandom(1, 4);
	%obj.playthread(2, "melee" @ %meleeAnim); //Weapon Swing animation.

	//Visual air slice.
	if(%this.meleeTrailSkin !$= "") 
	{
		%meleeTrailAngle = %this.meleeTrailAngle[%meleeAnim];
		%obj.spawnMeleeTrail(%this.meleeTrailSkin, %this.meleeTrailTime, %this.meleeTrailOffset, %meleeTrailAngle, %this.meleeTrailScale);
	}

	//Missing and/or striking the environment with the melee weapon.
	%typemasks = $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	%obstruction = ContainerRayCast(%killerEyePoint, VectorAdd(%killerEyePoint, VectorScale(%killerLookVector, %this.meleeRange)), %typemasks, %obj);
	if(isObject(%obstruction) && %this.hitObscureProjectile !$= "")
	{								
		%c = new Projectile()
		{
			dataBlock = %this.hitObscureProjectile;
			initialPosition = posFromRaycast(%obstruction);
			sourceObject = %obj;
			client = %obj.client;
		};
		%c.explode();
		return;
	}

	//Perform a container search for victims, and if any are found, determine if we can damage them.
	%killerWeaponPosition = %obj.getMuzzlePoint($RightHandSlot);
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
	
		//Hit sound effect.
		ServerPlay3D(%obj.voiceConfig.getUnmanagedSound("Hit"), %victimPosition);

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
		%hit.damage(%obj, %victimPosition, 25 * getWord(%killerScale, 2), $DamageType::Default);
		
		//Temporarily slow down the killer.
		%killerDatablock.setTempSpeed(%obj, 0.3);	
		%killerDatablock.schedule(1500, setTempSpeed, %obj, 1);
	}	
}