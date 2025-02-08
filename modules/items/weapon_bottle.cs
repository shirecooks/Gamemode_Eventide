AddDamageType("Bottle",'<bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_bottle> %1','%2 <bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_bottle> %1',1,1); 
AddDamageType("BrokenBottle",'<bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_bottle_broken> %1','%2 <bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_bottle_broken> %1',1,1); 
datablock DebrisData(bottleShard1Debris)
{
	shapeFile 			= "./models/d_glassShard1.dts";
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
datablock DebrisData(bottleShard2Debris : bottleShard1Debris)
{
	shapeFile 			= "./models/d_glassShard2.dts";
};
datablock DebrisData(bottleShard3Debris : bottleShard1Debris)
{
	shapeFile 			= "./models/d_glassShard3.dts";
};
datablock ExplosionData(bottleShard1Explosion)
{
	debris 					= bottleShard1Debris;
	debrisNum 				= 8;
	debrisNumVariance 		= 1;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 5;
	debrisVelocityVariance 	= 2;
};
datablock ExplosionData(bottleShard2Explosion : bottleShard1Explosion)
{
	debris 					= bottleShard2Debris;
	debrisNum 				= 32;
	debrisNumVariance 		= 3;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
};
datablock ExplosionData(bottleSmashExplosion)
{
	debris 					= bottleShard3Debris;
	debrisNum 				= 18;
	debrisNumVariance 		= 8;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
	explosionShape 			= "";
	lifeTimeMS 				= 150;
	subExplosion[0] 		= bottleShard1Explosion;
	subExplosion[1] 		= bottleShard2Explosion;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= true;
	camShakeFreq 			= "4.0 5.0 4.0";
	camShakeAmp 			= "3.0 4.0 3.0";
	camShakeDuration 		= 0.1;
	camShakeRadius 			= 10.0;
};
datablock ProjectileData(bottleSmashProjectile)
{
	explosion = bottleSmashExplosion;
};
datablock ProjectileData(bottleMiniSmashProjectile)
{
	explosion = bottleShard2Explosion;
};
datablock ParticleData(bottleExplosionParticle)
{
	dragCoefficient      = 3;
	gravityCoefficient   = 0.4;
	inheritedVelFactor   = 0.2;
	constantAcceleration = 0.0;
	lifetimeMS           = 300;
	lifetimeVarianceMS   = 100;
	textureName          = "base/data/particles/cloud";
	spinSpeed			= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	colors[0]			= "0.8 0.8 0.6 0.3";
	colors[1]			= "0.8 0.8 0.6 0.0";
	sizes[0]			= 0.75;
	sizes[1]			= 1.25;

	useInvAlpha = true;
};

datablock ParticleEmitterData(bottleExplosionEmitter)
{
	ejectionPeriodMS	= 1;
	periodVarianceMS	= 0;
	ejectionVelocity	= 2;
	velocityVariance	= 1.0;
	ejectionOffset  	= 0.0;
	thetaMin			= 89;
	thetaMax			= 90;
	phiReferenceVel		= 0;
	phiVariance			= 360;
	overrideAdvance		= false;
	particles			= bottleExplosionParticle;
};
datablock ExplosionData(bottleHitExplosion)
{
	debris 					= bottleShard3Debris;
	debrisNum 				= 5;
	debrisNumVariance 		= 3;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
	explosionShape 			= "";
	particleEmitter 		= bottleExplosionEmitter;
	particleDensity 		= 10;
	particleRadius 			= 0.2;
	lifeTimeMS 				= 150;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= true;
	camShakeFreq 			= "4.0 5.0 4.0";
	camShakeAmp 			= "3.0 4.0 3.0";
	camShakeDuration 		= 0.1;
	camShakeRadius 			= 20.0;
};
datablock ProjectileData(bottleHitProjectile)
{
	explosion = bottleHitExplosion;
};
datablock ItemData(bottleItem)
{
	category 			= "Weapon";
	className 			= "Weapon";

	shapeFile 			= "./models/bottle.dts";
	rotate 				= false;
	mass 				= 1;
	density 			= 0.2;
	elasticity 			= 0.2;
	friction 			= 0.6;
	emap 				= true;

	uiName 				= "Bottle";
	iconName 			= "./icons/icon_bottle";
	doColorShift 		= true;
	colorShiftColor 	= "0.4 0.2 0 1";

	image 				= bottleImage;
	canDrop 			= true;
};
datablock ShapeBaseImageData(bottleImage)
{
	shapeFile 			= bottleItem.shapeFile;
	emap 				= true;

	mountPoint 			= 0;
	offset 				= "0 0 0.3";
	eyeOffset 			= "0 0 0";
	rotation 			= "1 0 0 180";
	correctMuzzleVector = false;

	doColorShift 		= bottleItem.doColorShift;
	colorShiftColor 	= bottleItem.colorShiftColor;
	className 			= "WeaponImage";
	item 				= bottleItem;
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
	stateTimeoutValue[3] 			= 0.38;
};

function bottleImage::onSwing(%this,%obj)
{
	%obj.playThread(2,"shiftTo");
	$oldTimescale = getTimescale();
	setTimescale((getRandom(75,150)*0.01) * $oldTimescale);
	serverPlay3D("melee_swing_sound",%obj.getMuzzlePoint(0));
	setTimescale($oldTimescale);
}
function bottleImage::onFire(%this,%obj,%slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;
	%startpos = %obj.getMuzzlePoint(0);
	%endpos = %obj.getMuzzleVector(0);

	for(%i = 0; %i <= %obj.getDataBlock().maxTools; %i++)
	if(%obj.tool[%i] $= %this.item.getID()) %itemslot = %i;
	
	%hit = containerRayCast(%startpos,vectorAdd(%startpos,VectorScale(%endpos,2)),$TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType,%obj);
	if(isObject(%hit))
	{
		%hitpos = posFromRaycast(%hit);
		%obj.bottlehit++;

		if(%hit.getType() & $TypeMasks::PlayerObjectType)
		{
			if(minigameCanDamage(%obj,%hit) == 1)
			{				
				if(%obj.bottlehit < 3) %hit.Damage(%obj, %hit.getPosition(), 10, $DamageType::Bottle);
				else
				{
					%hit.mountimage("sm_stunImage",3);
					%hit.Damage(%obj, %hit.getPosition(), 20, $DamageType::BottleBroken);
				}
				
				%hit.applyImpulse(%hit.getposition(),vectorAdd(vectorScale(%obj.getMuzzleVector(0),500),"0 0 500"));						
			}
		}		

		if(%obj.bottlehit < 3)
		{
			serverPlay3D("bottle_hitplayer" @ getRandom(1,2) @ "_sound",%hitpos);
			%p = new Projectile()
			{
				dataBlock = "bottleHitProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();
		}
		else
		{
			serverPlay3D("bottle_smash_sound",%hitpos);
			%p = new Projectile()
			{
				dataBlock = "bottleSmashProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			MissionCleanup.add(%p);
			%p.explode();	

			if(isObject(%obj.client))
			{
				%obj.tool[%itemslot] = 0;
				messageClient(%obj.client,'MsgItemPickup','',%itemslot,0);
			}
			if(isObject(%obj.getMountedImage(%this.mountPoint))) 
			{
				%obj.unmountImage(%this.mountPoint);
			}
			%obj.bottlehit = 0;
		}
	}
}