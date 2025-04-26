function Armor::killerMelee(%this,%obj,%radius)
{
	if(%obj.getState() $= "Dead" || %obj.isInvisible || %obj.getEnergyLevel() < %this.maxEnergy/8 || %obj.lastMeleeTime+1250 > getSimTime()) 
	{
		return;
	}
		
	%obj.lastMeleeTime = getSimTime();	
	%hackPos = %obj.getHackPosition();
	%obj.setEnergyLevel(%obj.getEnergyLevel()-%this.maxEnergy/6);	
								
	killerMelee_playActions(%this,%obj);

	initContainerRadiusSearch(%obj.getMuzzlePoint(0), %radius, $TypeMasks::PlayerObjectType);		
	while(%hit = containerSearchNext())
	{			
		if(!killerMelee_checkHitConditions(%this,%obj,%hit,%radius)) 
		{
			continue;
		}

		if(%obj.getdatablock().onKillerHit(%obj,%hit))
		{
			killerMelee_playHitActions(%this,%obj,%hit);
		}		
	}	
}

function Armor::onKillerHit(%this,%obj,%hit)
{
	return true;
}

function killerMelee_checkHitConditions(%this,%obj,%hit,%radius)
{

	if(%hit == %obj || %hit == %obj.effectbot || VectorDist(%obj.getPosition(),%hit.getPosition()) > %radius) 
	{
		return false;
	}

	%hackPos = %obj.getHackPosition();
	%typemasks = $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
	%obscure = containerRayCast(%obj.getEyePoint(),%hit.getHackPosition(),%typemasks, %obj);
	%dot = vectorDot(%obj.getEyeVector(),vectorNormalize(vectorSub(%hit.getHackPosition(),%hackPos)));				

	if(isObject(%obscure) && %this.hitobscureprojectile !$= "")
	{								
		%c = new Projectile()
		{
			dataBlock = %this.hitobscureprojectile;
			initialPosition = posfromraycast(%obscure);
			sourceObject = %obj;
			client = %obj.client;
		};
		
		MissionCleanup.add(%c);
		%c.explode();
		return false;
	}

	if(%dot < 0.4 || !(%hit.getType() & $TypeMasks::PlayerObjectType) || !minigameCanDamage(%obj,%hit))
	{
		return false;
	}

	return true;
}

function killerMelee_playActions(%this,%obj)
{
	%hackPos = %obj.getHackPosition();
	%meleeAnim = (%this.shapeFile $= EventideplayerDts.baseShape) ? getRandom(1,4) : getRandom(1,2);	

	if(%this.meleetrailskin !$= "") 
	{
		%meleetrailangle = %this.meleetrailangle[%meleeAnim];
		%obj.spawnKillerTrail(%this.meleetrailskin,%this.meleetrailoffset,%meleetrailangle,%this.meleetrailscale);		
	}	

	if(%this.killerMeleesound !$= "") 
	{
		serverPlay3D(%this.killerMeleesound @ getRandom(1,%this.killerMeleesoundamount) @ "_sound",%hackPos);
	}
	
	if(%this.killerWeaponSound !$= "") 
	{
		serverPlay3D(%this.killerWeaponSound @ getRandom(1,%this.killerWeaponSoundamount) @ "_sound",%hackPos);
	}

	%obj.playthread(2,"melee" @ %meleeAnim);
}

function killerMelee_playHitActions(%this,%obj,%hit)
{
	if(%hit.getdataBlock().isDowned) return;
	
	if(%this.killerMeleehitsound !$= "")
	{
		serverPlay3D(%this.killerMeleehitsound @ getRandom(1,%this.killerMeleehitsoundamount) @ "_sound",%hit.getHackPosition());
	}

	if(%this.hitprojectile !$= "")
	{
		%effect = new Projectile()
		{
			dataBlock = %this.hitprojectile;
			initialPosition = %hit.getHackPosition();
			initialVelocity = vectorNormalize(vectorSub(%hit.getHackPosition(), %obj.getEyePoint()));
			scale = %obj.getScale();
			sourceObject = %obj;
		};
		
		MissionCleanup.add(%effect);
		%effect.explode();
	}
	
	%hit.setvelocity(vectorscale(VectorNormalize(vectorAdd(%obj.getForwardVector(),"0" SPC "0" SPC "0.15")),15));								
	%hit.damage(%obj, %hit.getHackPosition(), 50*getWord(%obj.getScale(),2), $DamageType::Default);
	
	%obj.setTempSpeed(0.3);	
	%obj.schedule(2500,setTempSpeed,1);
}

function Player::spawnKillerTrail(%this, %skin, %offset, %angle, %scale)
{
	%shape = new StaticShape()
	{
		dataBlock = KillerTrailShape;
		scale = %scale;
	};
	
	%shape.setSkinName(%skin);
	
	%rotation = relativeVectorToRotation(%this.getLookVector(), %this.getUpVector());
	%clamped = mClampF(firstWord(%rotation), -89.9, 89.9) SPC restWords(%rotation);		
	%local = %this.getHackPosition() SPC %clamped;
	%combined = %offset SPC eulerToQuat(%angle);
	%actual = matrixMultiply(%local, %combined);
	
	%shape.setTransform(%actual);
	%shape.playThread(0, "rotate");
	%shape.schedule(1000, delete);
	MissionCleanup.add(%shape);		
}