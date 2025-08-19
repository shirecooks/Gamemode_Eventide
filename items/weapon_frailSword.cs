//
// Particle and debris data.
//

datablock DebrisData(frailSwordDebris)
{
	elasticity = 0.5;
	gravModifier = 0.9;
	lifeTime = 5;
	maxSpinSpeed = 1000;
	numBounces = 2;
	fade = true;
	snapOnMaxBounce = false;
	staticOnMaxBounce = true;
	shapeFile = "./models/frailSword/frailSword.dts";
	velocity = 0;
};

datablock ExplosionData(frailSwordExplosion)
{
	debris = "frailSwordDebris";
	debrisNum = 1;
	debrisPhiMax = 360;
	debrisPhiMin = 180;
	debrisThetaMax = 180;
	debrisThetaMin = 0;
	debrisVelocity = 2;
};

datablock ProjectileData(frailSwordProjectile)
{
	explosion = "frailSwordExplosion";
};

//
// Item and image data.
//

AddDamageType("frailSword", '<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_frailSword> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_frailSword> %1', 0.75, 1);

datablock ItemData(frailSwordItem)
{
	category 			= "Weapon";
	className 			= "Weapon";

	shapeFile 			= "./models/frailSword/frailSword.dts";
	rotate 				= false;
	mass 				= 1;
	density 			= 0.2;
	elasticity 			= 0.2;
	friction 			= 0.6;
	emap 				= false;

	uiName 				= "Frail Sword";
	iconName 			= "./icons/icon_frailSword";
	doColorShift = true;
	colorShiftColor = "0.400 0.196 0 1.000";

	image 				= frailSwordImage;
	canDrop 			= true;
};

datablock ShapeBaseImageData(frailSwordImage)
{
	shapeFile 			= frailSwordItem.shapeFile;
	emap 				= false;

	mountPoint 			= 0;
	offset 				= "0 0 0";
	correctMuzzleVector = false;

	doColorShift = true;
	colorShiftColor = "0.400 0.196 0 1.000";
	className 			= "WeaponImage";

	item 				= frailSwordItem;
	armReady 			= true;
	melee				= true;
	
	stateName[0] 					= "Activate";
	stateSound[0]                    = "frailSwordDraw_sound";
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

//
// Sequence callbacks.
//

function frailSwordImage::onSwing(%this, %obj, %slot)
{	
	%obj.playThread(2, "shiftAway");
	//%obj.schedule(75,spawnKillerTrail,PlayerRenowned.meleetrailskin,"0.42 0.87 0.375","0 -90 0","3 2.7 1");
}
function frailSwordImage::onReady(%this, %obj, %slot)
{
	%obj.playThread(1, "armReady");
}

function frailSwordImage::onFire(%this, %obj, %slot)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") 
    {
        return;
    }

    //Play a swinging sound effect.
    serverPlay3D("frailSword_swing_sound", %obj.getMuzzlePoint(0));

    //Play a swinging animation.
	%obj.playThread(2, "shiftTo");

	%startpos = %obj.getMuzzlePoint(0);
	%endpos = %obj.getMuzzleVector(0);
	%hit = containerRayCast(%startpos, VectorAdd(%startpos, VectorScale(%endpos, 4)),$TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType, %obj);
	if(isObject(%hit))
	{
		%hitpos = posFromRaycast(%hit);
		%obj.frailSwordDamage[%obj.currTool] += 1;	

		if(%obj.frailSwordDamage[%obj.currTool] < 3)
		{
			serverPlay3D("swordHit_sound", %hitpos);
			%p = new Projectile()
			{
				dataBlock = "swordProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();

            //If a player was hit and we are in the same minigame, damage them and push them back.
            if((%hit.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj, %hit))
            {
                %hit.applyImpulse(%hit.getPosition(), VectorAdd(VectorScale(%obj.getMuzzleVector(0), 1000), "0 0 1000"));
                %hit.Damage(%obj, %hit.getPosition(), 25, $DamageType::frailSword);
            }
		}
		else
		{
			serverPlay3D("sword_break_sound", %hitpos);
			%p = new Projectile()
			{
				dataBlock = "frailSwordProjectile";
				initialPosition = %hitpos;
				sourceObject = %obj;
				client = %obj.client;
			};
			%p.explode();	

            //Remove the sword from the player's inventory, reset damage.
			%obj.removeItemFromInventory();
			%obj.frailSword = 0;

            //If a player was hit and we are in the same minigame, stun them and push them back.
            if((%hit.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj, %hit))
            {
                %hit.applyImpulse(%hit.getPosition(), VectorAdd(VectorScale(%obj.getMuzzleVector(0), 1000), "0 0 1000"));
                %hit.Damage(%obj, %hit.getPosition(), 50, $DamageType::frailSword);
                %hit.stun();
            }
		}
	}
}

function frailSwordImage::onUnmount(%this, %obj, %slot)
{    
    Parent::onUnmount(%this, %obj, %slot);
    %obj.playThread(2,"plant");
}

function frailSwordImage::onMount(%this, %obj, %slot)
{    
    Parent::onMount(%this, %obj, %slot);
    %obj.playThread(1, "armReady");
    %obj.playThread(2, "plant");
}