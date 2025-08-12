//
// Particle and debris data.
//

datablock DebrisData(poolCueGripDebris)
{
	shapeFile 			= "./models/poolCueDebrisGrip.dts";
	lifetime 			= 2.8;
	spinSpeed			= 300.0;
	minSpinSpeed 		= -1200.0;
	maxSpinSpeed 		= 1200.0;
	elasticity 			= 0.5;
	friction 			= 0.2;
	numBounces 			= 3;
	staticOnMaxBounce 	= true;
	snapOnMaxBounce 	= false;
	fade 				= true;
	gravModifier 		= 4;
};

datablock DebrisData(poolCueShaftDebris : poolCueGripDebris)
{
	shapeFile 			= "./models/poolCueDebrisShaft.dts";
};

datablock DebrisData(poolCueEndDebris : poolCueGripDebris)
{
	shapeFile 			= "./models/poolCueDebrisEnd.dts";
};

datablock ExplosionData(poolCueGripExplosion)
{
	debris 					= poolCueGripDebris;
	debrisNum 				= 1;
	debrisNumVariance 		= 0;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
};

datablock ExplosionData(poolCueShaftExplosion : poolCueGripExplosion)
{
	debris 					= poolCueShaftDebris;
};

datablock ExplosionData(poolCueEndExplosion : poolCueGripExplosion)
{
	debris 					= poolCueEndDebris;
};

datablock ExplosionData(poolCueSmashExplosion)
{
	debris 					= woodFragDebris;
	debrisNum 				= 12;
	debrisNumVariance 		= 8;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
	explosionShape 			= "";
	lifeTimeMS 				= 150;
	subExplosion[0] 		= poolCueGripExplosion;
	subExplosion[1] 		= poolCueShaftExplosion;
	subExplosion[2] 		= poolCueEndExplosion;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= true;
	camShakeFreq 			= "10.0 11.0 10.0";
	camShakeAmp 			= "6.0 8.0 6.0";
	camShakeDuration 		= 0.5;
	camShakeRadius 			= 20.0;
};

datablock ProjectileData(poolCueSmashProjectile)
{
	explosion = poolCueSmashExplosion;
};

//
// Item and image data.
//

AddDamageType("poolCue", '<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_poolCue> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_poolCue> %1',  1,  1); 

datablock ItemData(poolCueItem)
{
	category 			= "Weapon";
	className 			= "Weapon";

	shapeFile 			= "./models/poolCue.dts";
	rotate 				= false;
	mass 				= 1;
	density 			= 2;
	elasticity 			= 0.02;
	friction 			= 0.9;
	emap 				= true;

	uiName 				= "Pool Cue";
	iconName 			= "./icons/icon_poolCue";
	doColorShift 		= true;
	colorShiftColor 	= "0.56 0.4 0.2 1";

	image 				= poolCueImage;
	canDrop 			= true;
	
	meleeRange			= 4.5;
	meleeHealth			= 2;
	meleeDamageHit		= 35;
	meleeDamageBreak	= 35;
	meleeDamageType 	= $DamageType::poolCue;
	meleeVelocity		= 7;
};

datablock ShapeBaseImageData(poolCueImage)
{
	shapeFile 			= poolCueItem.shapeFile;
	emap 				= true;

	mountPoint 			= 0;
	offset 				= "0 0 1.1";
	eyeOffset 			= "0 0 0";
	rotation 			= "0 0 0 10";
	correctMuzzleVector = false;

	doColorShift 		= poolCueItem.doColorShift;
	colorShiftColor 	= poolCueItem.colorShiftColor;
	className 			= "WeaponImage";
	item 				= poolCueItem;
	armReady 			= true;
	melee				= true;
	
	stateName[0] 					= "Activate";
	stateTimeoutValue[0] 			= 0.5;
	stateTransitionOnTimeout[0] 	= "Ready";
	
	stateName[1] 					= "Ready";
	stateTransitionOnTriggerDown[1] = "PreSwing";
	
	stateName[2] 					= "PreSwing";
	stateScript[2] 					= "onSwing";
	stateFire[2] 					= true;
	stateTransitionOnTimeout[2] 	= "Swing";
	stateTimeoutValue[2] 			= 0.07;
	
	stateName[3] 					= "Swing";
	stateScript[3] 					= "onFire";
	stateFire[3] 					= true;
	stateTransitionOnTimeout[3] 	= "Ready";
	stateTimeoutValue[3] 			= 0.45;
};

//
// Sequence callbacks.
//

function poolCueImage::onSwing(%this, %obj)
{
	%obj.playThread(2, "shiftDown");
	serverPlay3D("generic_heavyswing" @ getRandom(1, 2) @ "_sound", %obj.getMuzzlePoint($RightHandSlot));
}

function poolCueImage::onFire(%this, %obj, %slot)
{
	//The weapon cannot be swung if we're dead.
	if(!isObject(%obj) || %obj.getState() $= "Dead") 
	{
		return;
	}

	//No idea what the purpose of this is.
	for(%i = 0; %i <= %obj.getDataBlock().maxTools; %i++)
	{
		if(%obj.tool[%i] $= %this.item.getID()) 
		{
			%itemslot = %i;
		}
	}

	%startpos = %obj.getMuzzlePoint($RightHandSlot);
	%endpos = %obj.getMuzzleVector($RightHandSlot);
	
	//Determine if we've struck an object.
	%hit = containerRayCast(%startpos, VectorAdd(%startpos, VectorScale(%endpos, 4)), $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType, %obj);
	if(isObject(%hit))
	{
		%hitpos = posFromRaycast(%hit);
		%obj.poolcuehit++;	

		if(%obj.poolcuehit < 3)
		{
			//The pool cue is damaged but not broken. Play a creaking sound, and spawn some wood splinters.
			serverPlay3D("chair_hit" @ getRandom(1, 2) @ "_sound", %hitpos);
			%p = new Projectile()
			{
				dataBlock = "chairHitProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();

			//The object is a player, and we're in the same minigame. Hurt them and push them back.
			if((%hit.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj, %hit))
			{
				%hit.Damage(%obj,  %hit.getPosition(), 25, $DamageType::poolCue);						
				%hit.applyImpulse(%hit.getposition(), VectorAdd(VectorScale(%obj.getMuzzleVector($RightHandSlot), 500), "0 0 500"));			
			}
		}
		else
		{
			//The pool cue has broke. Play a cracking sound, and spawn some debris.
			serverPlay3D("poolcue_smash" @ getRandom(1, 2) @ "_sound", %hitpos);
			%p = new Projectile()
			{
				dataBlock = "poolCueSmashProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();	

			//Clear the pool cue from our inventory.
			if(isObject(%obj.client))
			{
				%obj.tool[%itemslot] = 0;
				messageClient(%obj.client, 'MsgItemPickup', '', %itemslot, 0);
			}
			if(isObject(%obj.getMountedImage(%this.mountPoint))) 
			{
				%obj.unmountImage(%this.mountPoint);
			}

			//The object is a player, and we're in the same minigame. Hurt them, stun them, and push them back.
			if((%hit.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj, %hit))
			{
				%hit.stun(2500);
				%hit.Damage(%obj, %hit.getPosition(), 50, $DamageType::poolCue);
				%hit.applyImpulse(%hit.getposition(), VectorAdd(VectorScale(%obj.getMuzzleVector($RightHandSlot), 1500), "0 0 750"));			
			}			
			
			%obj.poolcuehit = 0;
		}
	}
}