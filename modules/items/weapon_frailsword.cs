datablock DebrisData(frailswordDebris)
{
	elasticity = 0.5;
	gravModifier = 0.9;
	lifeTime = 5;
	maxSpinSpeed = 1000;
	numBounces = 2;
	fade = true;
	snapOnMaxBounce = false;
	staticOnMaxBounce = true;
	shapeFile = "./models/frailsword.dts";
	velocity = 0;
};

datablock ExplosionData(frailswordExplosion)
{
	debris = "frailswordDebris";
	debrisNum = 1;
	debrisPhiMax = 360;
	debrisPhiMin = 180;
	debrisThetaMax = 180;
	debrisThetaMin = 0;
	debrisVelocity = 2;
};

datablock ProjectileData(frailswordProjectile)
{
	explosion = "frailswordExplosion";
};

datablock ItemData(sm_frailswordItem)
{
	category 			= "Weapon";
	className 			= "Weapon";

	shapeFile 			= "./models/frailsword.dts";
	rotate 				= false;
	mass 				= 1;
	density 			= 0.2;
	elasticity 			= 0.2;
	friction 			= 0.6;
	emap 				= true;

	uiName 				= "Frail Sword";
	iconName 			= "./icons/icon_frailsword";
	doColorShift = true;
	colorShiftColor = "0.471 0.471 0.471 1.000";

	image 				= sm_frailswordImage;
	canDrop 			= true;
};
datablock ShapeBaseImageData(sm_frailswordImage)
{
	shapeFile 			= sm_frailswordItem.shapeFile;
	emap 				= true;

	mountPoint 			= 0;
	offset 				= "0 0 0";
	correctMuzzleVector = false;

	doColorShift = true;
	colorShiftColor = "0.471 0.44 0.41 1.000";
	className 			= "WeaponImage";
	item 				= sm_frailswordItem;
	armReady 			= true;
	melee				= true;
	
	stateName[0] 					= "Activate";
	stateSound[0]                    = "frailswordDraw_sound";
	stateTimeoutValue[0] 			= 0.5;
	stateTransitionOnTimeout[0] 	= "Ready";
	
	stateName[1] 					= "Ready";
	stateScript[1]                  = "onReady";
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

function sm_frailswordImage::onSwing(%this, %obj, %slot)
{	
	%obj.playthread(2, "shiftAway");
	%obj.schedule(75,spawnKillerTrail,PlayerRenowned.meleetrailskin,"0.4 1.2 0.375","0 -90 0","3 2.5 1");
}
function sm_frailswordImage::onReady(%this, %obj, %slot)
{
	%obj.playthread(1, "armReady");
}

function sm_frailswordImage::onFire(%this,%obj,%slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;
	%obj.playthread(2, "shiftTo");
	%startpos = %obj.getMuzzlePoint(0);
	%endpos = %obj.getMuzzleVector(0);
	serverPlay3D("frailsword_Swing_sound",%obj.getMuzzlePoint(0));

	for(%i = 0; %i <= %obj.getDataBlock().maxTools; %i++)
	if(%obj.tool[%i] $= %this.item.getID()) %itemslot = %i;
	
	%hit = containerRayCast(%startpos,vectorAdd(%startpos,VectorScale(%endpos,3)),$TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType,%obj);
	if(isObject(%hit))
	{
		%hitpos = posFromRaycast(%hit);
		%obj.frailswordhit++;

		if(%hit.getType() & $TypeMasks::PlayerObjectType)
		{
			if(minigameCanDamage(%obj,%hit) == 1)
			{
				if(%obj.frailswordhit < 3) %hit.Damage(%obj, %hit.getPosition(), 25, $DamageType::frailsword);
				else
				{
					%hit.mountimage("sm_stunImage",3);
					%hit.Damage(%obj, %hit.getPosition(), 50, $DamageType::barStool);
				}
				
				%hit.applyImpulse(%hit.getposition(),vectorAdd(vectorScale(%obj.getMuzzleVector(0),1000),"0 0 1000"));
			}
		}		

		if(%obj.frailswordhit < 3)
		{
			serverPlay3D("swordHit_sound",%hitpos);
			%p = new Projectile()
			{
				dataBlock = "swordProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();			
		}
		else
		{
			serverPlay3D("sword_break_sound",%hitpos);
			%p = new Projectile()
			{
				dataBlock = "frailswordProjectile";
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
			%obj.frailswordhit = 0;
		}
	}
}

function sm_frailswordImage::onUnmount(%this,%obj,%slot)
{    
    Parent::onUnmount(%this,%obj,%slot);
   // %obj.playAudio(1,"book_conceal_sound");
    %obj.playthread(2,"plant");
}

function sm_frailswordImage::onMount(%this,%obj,%slot)
{    
    Parent::onMount(%this,%obj,%slot);
    %obj.playthread(1,"armReady");
    %obj.playthread(2,"plant");
}