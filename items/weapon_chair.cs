//
// Particle and debris data.
//

datablock DebrisData(chairSeat1Debris)
{
	shapeFile 			= "./models/chair/chairDebrisSeat1.dts";
	lifetime 			= 2.8;
	spinSpeed			= 1200.0;
	minSpinSpeed 		= -3600.0;
	maxSpinSpeed 		= 3600.0;
	elasticity 			= 0.5;
	friction 			= 0.2;
	numBounces 			= 3;
	staticOnMaxBounce 	= true;
	snapOnMaxBounce 	= false;
	fade 				= true;
	gravModifier 		= 4;
};

datablock DebrisData(chairSeat2Debris : chairSeat1Debris)
{
	shapeFile 			= "./models/chair/chairDebrisSeat2.dts";
};

datablock DebrisData(chairSeat3Debris : chairSeat1Debris)
{
	shapeFile 			= "./models/chair/chairDebrisSeat3.dts";
};

datablock DebrisData(chairRestDebris : chairSeat1Debris)
{
	shapeFile 			= "./models/chair/chairDebrisRest.dts";
	spinSpeed			= 300.0;
	minSpinSpeed 		= -1200.0;
	maxSpinSpeed 		= 1200.0;
};

datablock DebrisData(chairLegDebris : chairRestDebris)
{
	shapeFile 			= "./models/chair/chairDebrisLeg.dts";
};

datablock ExplosionData(chairSeat1Explosion)
{
	debris 					= chairSeat1Debris;
	debrisNum 				= 3;
	debrisNumVariance 		= 1;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
};

datablock ExplosionData(chairSeat2Explosion : chairSeat1Explosion)
{
	debris 					= chairSeat2Debris;
};

datablock ExplosionData(chairSeat3Explosion : chairSeat1Explosion)
{
	debris 					= chairSeat2Debris;
	debrisNum 				= 4;
	debrisNumVariance 		= 2;
};

datablock ExplosionData(chairRestExplosion : chairSeat1Explosion)
{
	debris 					= chairRestDebris;
	debrisNum 				= 6;
	debrisNumVariance 		= 4;
};

datablock ExplosionData(chairLegExplosion : chairSeat1Explosion)
{
	debris 					= chairLegDebris;
	debrisNum 				= 6;
	debrisNumVariance 		= 4;
};

datablock ExplosionData(chairSmashExplosion)
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
	subExplosion[0] 		= chairSeat1Explosion;
	subExplosion[1] 		= chairSeat2Explosion;
	subExplosion[2] 		= chairSeat3Explosion;
	subExplosion[3] 		= chairRestExplosion;
	subExplosion[4] 		= chairLegExplosion;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= true;
	camShakeFreq 			= "10.0 11.0 10.0";
	camShakeAmp 			= "6.0 8.0 6.0";
	camShakeDuration 		= 0.5;
	camShakeRadius 			= 20.0;
};

datablock ProjectileData(chairSmashProjectile)
{
	explosion = chairSmashExplosion;
};

datablock ParticleData(chairExplosionParticle)
{
	dragCoefficient      = 1;
	gravityCoefficient   = 0.4;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 500;
	lifetimeVarianceMS   = 100;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]			= "0.8 0.8 0.6 0.3";
	colors[1]			= "0.8 0.8 0.6 0.0";
	sizes[0]			= 1.25;
	sizes[1]			= 2.25;
	useInvAlpha 		= true;
};

datablock ParticleEmitterData(chairExplosionEmitter)
{
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 4;
	velocityVariance	= 1.0;
	ejectionOffset  	= 0.0;
	thetaMin			= 89;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= chairExplosionParticle;
};

datablock ExplosionData(chairHitExplosion)
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
	particleEmitter 		= chairExplosionEmitter;
	particleDensity 		= 10;
	particleRadius 			= 0.2;
	lifeTimeMS 				= 150;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= true;
	camShakeFreq 			= "10.0 11.0 10.0";
	camShakeAmp 			= "3.0 4.0 3.0";
	camShakeDuration 		= 0.3;
	camShakeRadius 			= 20.0;
};

datablock ProjectileData(chairHitProjectile)
{
	explosion = chairHitExplosion;
};

//
// Item and image data.
//

AddDamageType("Chair", '<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_chair> %1','%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_chair> %1', 1, 1); 

datablock ItemData(chairItem)
{
	category 			= "Weapon";
	className 			= "Weapon";

	shapeFile 			= "./models/chair/chair.dts";
	rotate 				= false;
	mass 				= 1;
	density 			= 0.2;
	elasticity 			= 0.2;
	friction 			= 0.6;
	emap 				= true;

	uiName 				= "Chair";
	iconName 			= "./icons/icon_chair";
	doColorShift 		= true;
	colorShiftColor 	= "0.56 0.4 0.2 1";

	image 				= chairImage;
	canDrop 			= true;
};

datablock ShapeBaseImageData(chairImage)
{
	shapeFile 			= chairItem.shapeFile;
	emap 				= true;

	mountPoint 			= 0;
	offset 				= "-0.53 0.3 0.72";
	eyeOffset 			= "0 0 0";
	rotation 			= "0 1 0 180";
	correctMuzzleVector = false;

	doColorShift 		= chairItem.doColorShift;
	colorShiftColor 	= chairItem.colorShiftColor;
	className 			= "WeaponImage";
	item 				= chairItem;
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
	stateTimeoutValue[3] 			= 0.6;
};

//
// Sequence callbacks.
//

function chairImage::onSwing(%this, %obj, %slot)
{
	%obj.playThread(3, shiftDown);
	serverPlay3D("generic_heavyswing" @ getRandom(1, 2) @ "_sound", %obj.getMuzzlePoint($RightHandSlot));
}

function chairImage::onFire(%this, %obj, %slot)
{
	//Can't swing the chair if we're dead.
	if(!isObject(%obj) || %obj.getState() $= "Dead") 
	{
		return;
	}

	//Not a clue what this is doing, honestly.
	for(%i = 0; %i <= %obj.getDataBlock().maxTools; %i++)
	{
		if(%obj.tool[%i] $= %this.item.getID())
		{
			%itemslot = %i;
		}
	}

	%startpos = %obj.getMuzzlePoint($RightHandSlot);
	%endpos = %obj.getMuzzleVector($RightHandSlot);

	//Draw a line from the player's hand three units formward, and determine if we hit anything.
	%hit = containerRayCast(%startpos, VectorAdd(%startpos, VectorScale(%endpos, 3)), $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType, %obj);
	if(isObject(%hit))
	{
		%hitpos = posFromRaycast(%hit);
		%obj.chairhit++;

		//If the struck object is a player or bot, and we're both in the same minigame...
		if(%hit.getType() & $TypeMasks::PlayerObjectType)
		{
			if(minigameCanDamage(%obj, %hit) == 1)
			{
				if(%obj.chairhit < 3) 
				{
					//The chair isn't damaged enough, simply hurt the victim.
					%hit.Damage(%obj, %hit.getPosition(), 25, $DamageType::barStool);
				}
				else
				{
					//The chair has taken three hits, make the chair explode and stun the victim.
					%hit.stun(2500);
					%hit.Damage(%obj, %hit.getPosition(), 50, $DamageType::barStool);
				}
				
				//Knock the victim back.
				%hit.applyImpulse(%hit.getPosition(), VectorAdd(VectorScale(%obj.getMuzzleVector($RightHandSlot), 1000), "0 0 1000"));
			}
		}		

		if(%obj.chairhit < 3)
		{
			//Play a chair creak sound effect on hit.
			serverPlay3D("chair_hit" @ getRandom(1,2) @ "_sound", %hitpos);

			//Spawn some splinters from the chair.
			%p = new Projectile()
			{
				dataBlock = "chairHitProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();			
		}
		else
		{
			//Play chair break sound effect.
			serverPlay3D("chair_smash" @ getRandom(1, 2) @ "_sound", %hitpos);

			//Spawn chair debris.
			%p = new Projectile()
			{
				dataBlock = "chairSmashProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();	

			//Unequip chair from the player's inventory.
			if(isObject(%obj.client))
			{
				%obj.tool[%itemslot] = 0;
				messageClient(%obj.client,'MsgItemPickup', '', %itemslot, 0);
			}

			if(isObject(%obj.getMountedImage(%this.mountPoint))) 
			{
				%obj.unmountImage(%this.mountPoint);
			}

			%obj.chairhit = 0;
		}
	}
}

function chairImage::onMount(%this, %obj ,%slot)
{
	parent::onMount(%this, %obj, %slot);
	%obj.playThread(2, armReadyLeft);
}

function chairImage::onUnMount(%this, %obj, %slot)
{
	%obj.playThread(2, root);
	parent::onUnMount(%this, %obj, %slot);
}