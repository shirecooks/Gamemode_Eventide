AddDamageType("BarStool",'<bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_barStool> %1','%2 <bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_barStool> %1',1,1); 
datablock DebrisData(sm_barStoolLegDebris)
{
	shapeFile 			= "./models/d_barStoolLeg.dts";
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
datablock DebrisData(sm_barStoolSeat1Debris : sm_barStoolLegDebris)
{
	shapeFile 			= "./models/d_barStoolSeat1.dts";
};
datablock ExplosionData(sm_barStoolLegExplosion)
{
	debris 					= sm_barStoolLegDebris;
	debrisNum 				= 8;
	debrisNumVariance 		= 7;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
};
datablock ExplosionData(sm_barStoolSeat1Explosion : sm_chairSeat1Explosion)
{
	debris 					= sm_barStoolSeat1Debris;
	debrisNum 				= 3;
	debrisNumVariance 		= 2;
};
datablock ExplosionData(sm_barStoolSmashExplosion)
{
	debris 					= sm_woodFragDebris;
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
	subExplosion[0] 		= sm_barStoolSeat1Explosion;
	subExplosion[1] 		= sm_chairSeat1Explosion;
	subExplosion[2] 		= sm_chairRestExplosion;
	subExplosion[3] 		= sm_barStoolLegExplosion;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= true;
	camShakeFreq 			= "10.0 11.0 10.0";
	camShakeAmp 			= "6.0 8.0 6.0";
	camShakeDuration 		= 0.5;
	camShakeRadius 			= 20.0;
};
datablock ProjectileData(sm_barStoolSmashProjectile)
{
	explosion = sm_barStoolSmashExplosion;
};
datablock ItemData(sm_barStoolItem)
{
	category 			= "Weapon";
	className 			= "Weapon";

	shapeFile 			= "./models/barStool.dts";
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

	image 				= sm_barStoolImage;
	canDrop 			= true;
};
datablock ShapeBaseImageData(sm_barStoolImage)
{
	shapeFile 			= sm_barStoolItem.shapeFile;
	emap 				= true;

	mountPoint 			= 0;
	offset 				= "-0.53 0.47 0.65";
	eyeOffset 			= "0 0 0";
	rotation 			= "0 0 0 10";
	correctMuzzleVector = false;

	doColorShift 		= sm_barStoolItem.doColorShift;
	colorShiftColor 	= sm_barStoolItem.colorShiftColor;
	className 			= "WeaponImage";
	item 				= sm_barStoolItem;
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
function sm_barStoolImage::onSwing(%this,%obj,%slot)
{
	%obj.playThread(3,shiftDown);
	serverPlay3D("generic_heavyswing" @ getRandom(1,2) @ "_sound",%obj.getMuzzlePoint(0));
}

function sm_barStoolImage::onFire(%this,%obj,%slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;
	%startpos = %obj.getMuzzlePoint(0);
	%endpos = %obj.getMuzzleVector(0);

	for(%i = 0; %i <= %obj.getDataBlock().maxTools; %i++)
	if(%obj.tool[%i] $= %this.item.getID()) %itemslot = %i;
	
	%hit = containerRayCast(%startpos,vectorAdd(%startpos,VectorScale(%endpos,3)),$TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType,%obj);
	if(isObject(%hit))
	{
		%hitpos = posFromRaycast(%hit);
		%obj.barstoolhit++;

		if(%hit.getType() & $TypeMasks::PlayerObjectType)
		{
			if(minigameCanDamage(%obj,%hit) == 1)
			{
				if(%obj.barstoolhit < 3) %hit.Damage(%obj, %hit.getPosition(), 25, $DamageType::barStool);
				else
				{
					%hit.mountimage("sm_stunImage",3);
					%hit.Damage(%obj, %hit.getPosition(), 50, $DamageType::barStool);
				}
				
				%hit.applyImpulse(%hit.getposition(),vectorAdd(vectorScale(%obj.getMuzzleVector(0),1000),"0 0 1000"));
			}
		}		

		if(%obj.barstoolhit < 3)
		{
			serverPlay3D("chair_hit" @ getRandom(1,2) @ "_sound",%hitpos);
			%p = new Projectile()
			{
				dataBlock = "sm_chairHitProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();			
		}
		else
		{
			serverPlay3D("chair_smash" @ getRandom(1,2) @ "_sound",%hitpos);
			%p = new Projectile()
			{
				dataBlock = "sm_chairSmashProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();	

			if(isObject(%obj.client))
			{
				%obj.tool[%itemslot] = 0;
				messageClient(%obj.client,'MsgItemPickup','',%itemslot,0);
			}
			if(isObject(%obj.getMountedImage(%this.mountPoint))) %obj.unmountImage(%this.mountPoint);
			%obj.barstoolhit = 0;
		}
	}
}

function sm_barStoolImage::onMount(%db,%pl,%slot)
{
	parent::onMount(%db,%pl,%slot);
	%pl.playThread(2,armReadyLeft);
}
function sm_barStoolImage::onUnMount(%db,%pl,%slot)
{
	%pl.playThread(2,root);
	parent::onUnMount(%db,%pl,%slot);
}