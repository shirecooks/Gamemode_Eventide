datablock ItemData(ritualItem)
{
	class = "ritualItem";
	superClass = "";

	category = "Weapon";
	className = "Weapon";

	isRitualItem = true;
    ritualType = "Generic";
	maxRitualsOnCircle = 1;
	possibleOffset1 = "0 0 0";

    emitterDatablock = "brickDeployExplosionEmitter";
};

function ritualItem::placeOnRitualCircle(%this, %obj, %circle)
{
	//The ritual circle can only accept 4 gems.
	%ritualCount = %circle.ritualCount[%this.ritualType];
	if(%ritualCount >= %this.maxRitualsOnCircle)
	{
		return false;
	}

	%offset = %this.possibleOffset[%ritualCount + 1];

	%circleTransform = %circle.getTransform();
	%circlePosition = posFromTransform(%circleTransform);
	%circleRotation = rotFromTransform(%circleTransform);

	//Move the ritual onto the circle, prevent it from being picked up, and stop any attention emitters if present.
	%obj.canPickup = false;
	%obj.setVelocity("0 0 0");
	%obj.setTransform(VectorAdd(%circlePosition, %offset) SPC %circleRotation);
	%obj.stopEmitter();

	return true;
}