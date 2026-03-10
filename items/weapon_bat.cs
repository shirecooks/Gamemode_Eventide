//
// Item and image data.
//

AddDamageType("bat", '<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_bat> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_bat> %1', 0.75, 1);

datablock ItemData(batItem)
{
	category 			= "Weapon";
	className 			= "Weapon";

	shapeFile 			= "./models/bat/bat.dts";
	rotate 				= false;
	mass 				= 1;
	density 			= 0.2;
	elasticity 			= 0.2;
	friction 			= 0.6;
	emap 				= false;

	uiName 				= "Bat";
	iconName 			= "./icons/icon_bat";

	image 				= batImage;
	canDrop 			= true;
};

datablock ShapeBaseImageData(batImage : eventideCooldownMeleeImage)
{
	shapeFile = batItem.shapeFile;
	item = batItem;

	doColorShift = batItem.doColorShift;
    colorShiftColor = batItem.colorShiftColor;

	hitProjectile = KillerSharpHitProjectile;
	hitObscureProjectile = KillerGenericSharpClankProjectile;

	meleeTrail = $Eventide_MeleeTrails["base.trail"];
	swingSound = "generic_heavySwing";
	swingSoundAmount = 2;

	cooldown = 36000;
};
batImage.inheritFunctionsFromSuperClass("eventideMeleeImage").implementCooldownCallbacks();

function batImage::getHintMessage(%this, %obj)
{
	return "Clobber someone mean to make them see stars.";
}

//
// Sequence callbacks.
//

function batImage::onSwing(%this, %obj)
{	
	%victims = %this.super("onSwing", %this, %obj);
    if(!%victims)
    {
        //We didn't hit anyone, bail out.
        return 0;
    }

    //For each victim, apply damage and stun, as well as fling them away.
    %victimCount = getWordCount(%victims);
    for(%i = 0; %i < %victimCount; %i++)
    {
        %victim = getWord(%victims, %i);

        //Apply damage, then fling and stun the victim.
        %victimPosition = %victim.getPosition();
        %victim.applyImpulse(%victimPosition, VectorAdd(VectorScale(%obj.getMuzzleVector(%this.mountPoint), 1000), "0 0 1000"));
        %victim.Damage(%obj, %victimPosition, 50, $DamageType::bat);
        %victim.stun();
    }

    //We hit someone, so start the cooldown.
    %obj.weaponCooldown(%obj.slot, "The bat is cracked, and won't be fixed for " @ sFromMs(%this.cooldown) @ " seconds.", "Your bat is fixed!", 6);

    return %victims;
}