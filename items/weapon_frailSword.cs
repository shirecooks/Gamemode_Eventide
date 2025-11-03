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

datablock ShapeBaseImageData(frailSwordImage : eventideMeleeImage)
{
	class = "frailSwordImage";
    superClass = "eventideMeleeImage";

	shapeFile = frailSwordItem.shapeFile;
	item = frailSwordItem;

	doColorShift = frailSwordItem.doColorShift;
    colorShiftColor = frailSwordItem.colorShiftColor;

	hitProjectile = KillerSharpHitProjectile;
	hitObscureProjectile = KillerGenericSharpClankProjectile;

	meleeTrail = $Eventide_MeleeTrails["base.trail"];
	swingSound = "generic_lightSwing";
	swingSoundAmount = 5;
};
frailSwordImage.inheritFunctionsFromSuperClass();

//
// Sequence callbacks.
//

function frailSwordImage::onSwing(%this, %obj, %slot)
{	
	%this.super("onSwing", %this, %obj, %slot);

	%hits = %Obj.getImageAttribute("hits");
	%victim = %obj.getImageAttribute("lastPersonHit");
	if(%hits >= 3)
	{
		%muzzlePoint = %obj.getMuzzlePoint(0);

		//Play a breaking sound effect.
		serverPlay3D("sword_break_sound", %muzzlePoint);

		//Spawn some debris.
		new Projectile()
		{
			dataBlock = "frailSwordProjectile";
			initialPosition = %muzzlePoint;
		}.explode();

		//Remove the sword from the player's inventory, reset damage.
		%obj.removeItemFromInventory();

		//If a player was hit and we are in the same minigame, stun them and push them back.
		if(minigameCanDamage(%obj, %victim))
		{
			%victimPosition = %victim.getPosition();

			%victim.applyImpulse(%victimPosition, VectorAdd(VectorScale(%obj.getMuzzleVector(0), 1000), "0 0 1000"));
			%victim.Damage(%obj, %victimPosition, 50, $DamageType::frailSword);
			%victim.stun();
		}
	}
}