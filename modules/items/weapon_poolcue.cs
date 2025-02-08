AddDamageType("PoolCue",'<bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_poolCue> %1','%2 <bitmap:Add-Ons/Gamemode_Eventide/modules/items/icons/ci_poolCue> %1',1,1); 

datablock DebrisData(sm_poolCueGripDebris)
{
	shapeFile 			= "./models/d_poolCueGrip.dts";
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
datablock DebrisData(sm_poolCueShaftDebris : sm_poolCueGripDebris)
{
	shapeFile 			= "./models/d_poolCueShaft.dts";
};
datablock DebrisData(sm_poolCueEndDebris : sm_poolCueGripDebris)
{
	shapeFile 			= "./models/d_poolCueEnd.dts";
};
datablock ExplosionData(sm_poolCueGripExplosion)
{
	debris 					= sm_poolCueGripDebris;
	debrisNum 				= 1;
	debrisNumVariance 		= 0;
	debrisPhiMin 			= 0;
	debrisPhiMax 			= 360;
	debrisThetaMin 			= 0;
	debrisThetaMax 			= 180;
	debrisVelocity 			= 12;
	debrisVelocityVariance 	= 6;
};
datablock ExplosionData(sm_poolCueShaftExplosion : sm_poolCueGripExplosion)
{
	debris 					= sm_poolCueShaftDebris;
};
datablock ExplosionData(sm_poolCueEndExplosion : sm_poolCueGripExplosion)
{
	debris 					= sm_poolCueEndDebris;
};
datablock ExplosionData(sm_poolCueSmashExplosion)
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
	subExplosion[0] 		= sm_poolCueGripExplosion;
	subExplosion[1] 		= sm_poolCueShaftExplosion;
	subExplosion[2] 		= sm_poolCueEndExplosion;
	faceViewer     			= true;
	explosionScale 			= "1 1 1";
	shakeCamera 			= true;
	camShakeFreq 			= "10.0 11.0 10.0";
	camShakeAmp 			= "6.0 8.0 6.0";
	camShakeDuration 		= 0.5;
	camShakeRadius 			= 20.0;
};
datablock ProjectileData(sm_poolCueSmashProjectile)
{
	explosion = sm_poolCueSmashExplosion;
};
datablock ItemData(sm_poolCueItem)
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

	image 				= sm_poolCueImage;
	canDrop 			= true;
	
	meleeRange			= 4.5;
	meleeHealth			= 2;
	meleeDamageHit		= 35;
	meleeDamageBreak	= 35;
	meleeDamageType 	= $DamageType::PoolCue;
	meleeVelocity		= 7;
};
datablock ShapeBaseImageData(sm_poolCueImage)
{
	shapeFile 			= sm_poolCueItem.shapeFile;
	emap 				= true;

	mountPoint 			= 0;
	offset 				= "0 0 1.1";
	eyeOffset 			= "0 0 0";
	rotation 			= "0 0 0 10";
	correctMuzzleVector = false;

	doColorShift 		= sm_poolCueItem.doColorShift;
	colorShiftColor 	= sm_poolCueItem.colorShiftColor;
	className 			= "WeaponImage";
	item 				= sm_poolCueItem;
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
function sm_poolCueImage::onSwing(%this,%obj)
{
	%obj.playThread(2,"shiftDown");
	serverPlay3D("generic_heavyswing" @ getRandom(1,2) @ "_sound",%obj.getMuzzlePoint(0));
}
function sm_poolCueImage::onFire(%this,%obj,%slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;
	%startpos = %obj.getMuzzlePoint(0);
	%endpos = %obj.getMuzzleVector(0);

	for(%i = 0; %i <= %obj.getDataBlock().maxTools; %i++)
	if(%obj.tool[%i] $= %this.item.getID()) %itemslot = %i;
	
	%hit = containerRayCast(%startpos,vectorAdd(%startpos,VectorScale(%endpos,4)),$TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType,%obj);
	if(isObject(%hit))
	{
		%hitpos = posFromRaycast(%hit);
		%obj.poolcuehit++;	

		if(%obj.poolcuehit < 3)
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

			if((%hit.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj,%hit))
			{
				%hit.Damage(%obj, %hit.getPosition(), 25, $DamageType::barStool);						
				%hit.applyImpulse(%hit.getposition(),vectorAdd(vectorScale(%obj.getMuzzleVector(0),500),"0 0 500"));			
			}
		}
		else
		{
			serverPlay3D("poolcue_smash" @ getRandom(1,2) @ "_sound",%hitpos);
			%p = new Projectile()
			{
				dataBlock = "sm_poolCueSmashProjectile";
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
			if(isObject(%obj.getMountedImage(%this.mountPoint))) 
			{
				%obj.unmountImage(%this.mountPoint);
			}

			if((%hit.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj,%hit))
			{
				%hit.mountimage("sm_stunImage",3);
				%hit.Damage(%obj, %hit.getPosition(), 50, $DamageType::barStool);
				%hit.applyImpulse(%hit.getposition(),vectorAdd(vectorScale(%obj.getMuzzleVector(0),1500),"0 0 750"));			
			}			
			
			%obj.poolcuehit = 0;
		}
	}
}