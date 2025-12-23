datablock ItemData(ritualItem)
{
	class = "ritualItem";
	superClass = "";

	category = "Weapon";
	className = "Weapon";

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	canDrop = true;
	isRitualItem = true;
    ritualType = "Generic";
	maxRitualsOnCircle = 1;
	possibleOffset1 = "0 0 0";

	placeSound = "";
	placeSoundAmount = 0;

    emitterDatablock = "brickDeployExplosionEmitter";
};

function ritualItem::placeOnRitualCircle(%this, %obj, %circle)
{
	//The ritual circle can only accept 4 gems, for example.
	%ritualCount = %circle.ritualCount[%this.ritualType];
	if(%ritualCount >= %this.maxRitualsOnCircle)
	{
		return false;
	}

	//Were do we place the ritual relative to the circle?
	%offset = %this.possibleOffset[%ritualCount + 1];

	%circleTransform = %circle.getTransform();
	%circlePosition = posFromTransform(%circleTransform);
	%circleRotation = rotFromTransform(%circleTransform);

	//Move the ritual onto the circle, prevent it from being picked up, and stop any attention emitters if present.
	%newItemPosition = VectorAdd(%circlePosition, getWords(%offset, 0, 2));
	%newItemRotation = VectorAdd(%circleRotation, getWords(%offset, 3, 5));

	%obj.canPickup = false;
	%obj.setVelocity("0 0 0");
	%obj.setTransform(%newItemPosition SPC %newItemRotation);
	%obj.stopEmitter();

	//Play a sound when it's placed.
	if(%this.placeSound !$= "")
	{
		serverPlay3D(%this.placeSound @ getRandom(1, %this.placeSoundAmount) @ "_sound");
	}

	return true;
}