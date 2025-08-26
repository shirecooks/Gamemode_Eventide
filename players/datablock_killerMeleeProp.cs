datablock ShapeBaseImageData(KillerMeleeImage)
{
    class = "KillerMelee";
    superClass = "";

	meleeRange = 4;
	meleeCooldown = 1750;
	meleeTrailSkin = "base";
	meleeTrailOffset = "0.3 1.4 0.7"; 
	meleeTrailAngle1 = "0 90 0";
	meleeTrailAngle2 = "0 -90 0";
	meleeTrailAngle3 = "0 0 0";
	meleeTrailAngle4 = "0 180 0";
	meleeTrailScale = "4 4 2";

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
   	melee = false;
   	armReady = true;
   	doColorShift = false;

	stateName[0] = "Activate";
	stateTimeoutValue[0] = 1;
	stateTransitionOnTimeout[0] = "Ready";
	stateScript[0] = "onAim";
	stateSound[0] = weaponSwitchSound;

	stateName[1] = "Ready";
	stateTimeoutValue[1] = 3;
	stateTransitionOnTimeout[1] = "ReadyDown";
	stateTransitionOnTriggerDown[1]  = "Fire";
	stateWaitForTimeout[1] = false;
	stateAllowImageChange[1] = true;
	stateSequence[1] = "Ready";

	stateName[7] = "ReadyDown";
	stateSound[7] = weaponSwitchSound;
	stateTransitionOnTriggerDown[7] = "AimBeat";
	stateAllowImageChange[7] = true;
	stateScript[7] = "onDrop";
	stateSequence[7] = "Ready";

	stateName[2] = "Fire";
	stateTransitionOnTimeout[2] = "Smoke";
	stateTimeoutValue[2] = 0.15;
	stateScript[2] = "onFireDown";
	stateWaitForTimeout[2] = true;
   	stateSequence[2] = "Fire";

	stateName[3] = "Smoke";
	stateScript[3] = "onSwing";
	stateFire[3] = true;
	stateAllowImageChange[3] = false;
	stateSequence[3] = "Fire";
	stateTimeoutValue[3] = 0.15;
	stateSound[3] = "";
	stateTransitionOnTimeout[3] = "CoolDown";

   	stateName[5] = "CoolDown";
   	stateTimeoutValue[5] = 0.9;
	stateTransitionOnTimeout[5] = "Reload";
   	stateSequence[5] = "clickDown";

	stateName[4] = "Reload";
	stateTransitionOnTriggerUp[4] = "Ready";
	stateSequence[4] = "TrigDown";

   	stateName[6] = "NoAmmo";
   	stateTransitionOnAmmo[6] = "Ready";

	stateName[8] = "AimBeat";
	stateTimeoutValue[8] = 0.05;
	stateTransitionOnTimeout[8] = "Fire";
	stateAllowImageChange[8] = true;
	stateScript[8] = "onAim";
	stateSequence[8] = "clickDown";
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
	%obj.playManagedSound("Swing", true); //Air slice sound.

	//Melee animation.
	%meleeAnim = getRandom(1, 4);
	%obj.playthread(2, "melee" @ %meleeAnim); //Weapon Swing animation.

	//Visual air slice.
	if(%this.meleeTrailSkin !$= "") 
	{
		%meleeTrailAngle = %this.meleeTrailAngle[%meleeAnim];

		%shape = new StaticShape()
		{
			dataBlock = KillerTrailShape;
			scale = %this.meleeTrailScale;
		};
		%shape.setSkinName(%this.meleeTrailSkin);
		
		%rotation = relativeVectorToRotation(%killerLookVector, %obj.getUpVector());
		%clamped = mClampF(firstWord(%rotation), -89.9, 89.9) SPC restWords(%rotation);		
		%local = %killerPosition SPC %clamped;
		%combined = %this.meleeTrailOffset SPC eulerToQuat(%meleeTrailAngle);
		%actual = matrixMultiply(%local, %combined);
		
		%shape.setTransform(%actual);
		%shape.playThread(0, "rotate");
		%shape.schedule(1000, delete);
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