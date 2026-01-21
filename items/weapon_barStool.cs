//
// Particle and debris data.
//

datablock DebrisData(barStoolLegDebris)
{
	shapeFile 			= "./models/barStool/barStoolDebrisLeg.dts";
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

datablock DebrisData(barStoolSeat1Debris : barStoolLegDebris)
{
	shapeFile 			= "./models/barStool/barStoolDebrisSeat1.dts";
};

datablock ExplosionData(barStoolLegExplosion)
{
	debris 					= barStoolLegDebris;
	debrisNum 				= 8;
	debrisNumVariance 		= 7;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
};

datablock ExplosionData(barStoolSeat1Explosion : chairSeat1Explosion)
{
	debris 					= barStoolSeat1Debris;
	debrisNum 				= 3;
	debrisNumVariance 		= 2;
};

datablock ExplosionData(barStoolSmashExplosion)
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
	subExplosion[0] 		= barStoolSeat1Explosion;
	subExplosion[1] 		= chairSeat1Explosion;
	subExplosion[2] 		= chairRestExplosion;
	subExplosion[3] 		= barStoolLegExplosion;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= true;
	camShakeFreq 			= "10.0 11.0 10.0";
	camShakeAmp 			= "6.0 8.0 6.0";
	camShakeDuration 		= 0.5;
	camShakeRadius 			= 20.0;
};

datablock ProjectileData(barStoolSmashProjectile)
{
	explosion = barStoolSmashExplosion;
};

//
// Item and image data.
//

AddDamageType("BarStool", '<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_barStool> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_barStool> %1', 1, 1); 

datablock ItemData(barStoolItem)
{
	category 			= "Weapon";
	className 			= "Weapon";

	shapeFile 			= "./models/barStool/barStool.dts";
	rotate 				= false;
	mass 				= 1;
	density 			= 2;
	elasticity 			= 0.02;
	friction 			= 0.9;
	emap 				= true;

	uiName 				= "Bar Stool";
	iconName 			= "./icons/icon_barStool";
	doColorShift 		= true;
	colorShiftColor 	= "0.56 0.4 0.2 1";

	image 				= barStoolImage;
	canDrop 			= true;
};

datablock ShapeBaseImageData(barStoolImage)
{
	shapeFile 			= barStoolItem.shapeFile;
	emap 				= true;

	mountPoint 			= 0;
	offset 				= "-0.53 0.47 0.65";
	eyeOffset 			= "0 0 0";
	rotation 			= "0 0 0 10";
	correctMuzzleVector = false;

	doColorShift 		= barStoolItem.doColorShift;
	colorShiftColor 	= barStoolItem.colorShiftColor;
	className 			= "WeaponImage";
	item 				= barStoolItem;
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

function barStoolImage::onSwing(%this, %obj)
{
	%obj.playThread(3, shiftDown);
	serverPlay3D("generic_heavyswing" @ getRandom(1, 2) @ "_sound", %obj.getMuzzlePoint(0));
}

function barStoolImage::onFire(%this, %obj, %slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;
	%startpos = %obj.getMuzzlePoint(0);
	%endpos = %obj.getMuzzleVector(0);
	
	%hit = containerRayCast(%startpos, vectorAdd(%startpos, VectorScale(%endpos, 3)), $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType, %obj);
	if(isObject(%hit))
	{
		%hitpos = posFromRaycast(%hit);
		%obj.barstoolhit++;

		if(%hit.getType() & $TypeMasks::PlayerObjectType)
		{
			if(minigameCanDamage(%obj, %hit) == 1)
			{
				if(%obj.barstoolhit < 3) %hit.Damage(%obj, %hit.getPosition(),  25, $DamageType::BarStool);
				else
				{
					%hit.mountimage("stunImage", 3);
					%hit.Damage(%obj, %hit.getPosition(), 50, $DamageType::BarStool);
				}
				
				%hit.applyImpulse(%hit.getposition(), vectorAdd(vectorScale(%obj.getMuzzleVector(0), 1000), "0 0 1000"));
			}
		}		

		if(%obj.barstoolhit < 3)
		{
			serverPlay3D("chair_hit" @ getRandom(1, 2) @ "_sound", %hitpos);
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
			serverPlay3D("chair_smash" @ getRandom(1, 2) @ "_sound", %hitpos);
			%p = new Projectile()
			{
				dataBlock = "chairSmashProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();	

			if(isObject(%obj.client))
			{
				%obj.tool[%slot] = 0;
				messageClient(%obj.client, 'MsgItemPickup', '', %slot, 0);
			}
			if(isObject(%obj.getMountedImage(%this.mountPoint))) %obj.unmountImage(%this.mountPoint);
			%obj.barstoolhit = 0;
		}
	}
}

function barStoolImage::onMount(%this, %obj)
{
	parent::onMount(%this, %obj);
	%obj.playThread(2, armReadyLeft);
}

function barStoolImage::onUnMount(%this, %obj)
{
	%obj.playThread(2, root);
	parent::onUnMount(%this, %obj);
}