//
// Loading footstep materials.
//

$Eventide_FootstepMaterials["isGlobalFootstepMaterialArray"] = true; //All footstep packs will be stored in this array.
$Eventide_FootstepMaterials["index"] = ""; //Footsteps will be happening too often for us to use a ScriptObject. We'll instead be using a plain array.

function createFootstepMaterial(%footstepFileCategory, %footstepMaterialRGB)
{
	//Add the footstep pack RGB value to the index, to be iterated over later.
	$Eventide_FootstepsPacks["index"] = ($Eventide_FootstepsPacks["index"] $= "") ? (%footstepMaterialRGB) : ($Eventide_FootstepsPacks["index"] SPC %footstepMaterialRGB);

	$Eventide_FootstepsPacks[%footstepMaterialRGB] = %footstepFileCategory;
}

function parseFootstepMaterials(%startingDirectory)
{    
    %footstepMaterialPaths = getFileString(%startingDirectory @ "/*.etfsp");
    for(%i = 0; %i < getFieldCount(%footstepMaterialPaths); %i++)
    {
		%footstepMaterialFile = getField(%footstepMaterialPaths, %i);

		%fileObject = new FileObject();
		%footstepMaterialRGB = %fileObject.readLine();
		%fileObject.delete();

        %footstepMaterialPath = filePath(%footstepMaterialFile);
        %footstepMaterialFileName = fileBase(%footstepMaterialPath);

        echo("Parsing footstep material \"" @ %footstepMaterialFileName @ "\" from \"" @ %footstepMaterialPath @ "\"...");
        %footstepMaterial = createFootstepMaterial(%footstepMaterialFileName, %footstepMaterialRGB);
    }
}

//
// Managing footstep materials.
//

function findClosestFootstepMaterial(%rgb)
{
	%closestRGB = "0 0 0";
	%currentDistance = 0;

	//Determine which material has the closest RGB similarity to the provided RGB color.
	%materialIndex = $Eventide_FootstepMaterials["index"];
	for(%i = 0; %i < getWordCount(%materialIndex); %i++)
	{
		%targetRGB = getWord(%materialIndex, %i);
		%euclideanColorDistance = VectorDist(%rgb, %targetRGB);

		if(%euclideanColorDistance < %currentDistance)
		{
			%closestRGB = %targetRGB;
			%currentDistance = %euclideanColorDistance;
		}
	}

	return $Eventide_FootstepMaterials[%closestRGB];
}

package Script_Footsteps
{
	function fxDTSBrick::onPlant(%obj)
	{
		parent::onPlant(%obj);
		%obj.assumeMaterial();
	}

	function fxDTSBrick::onLoadPlant(%obj)
	{
		parent::onLoadPlant(%obj);
		%obj.assumeMaterial();
	}

	function paintProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
	{
		parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
		if(%col.getType() & $TypeMasks::FxBrickObjectType)
		{
			%obj.assumeMaterial();
		}
	}
};
if(isPackage(Script_Footsteps))
{
	deactivatePackage(Script_Footsteps);
}
activatePackage(Script_Footsteps);

//
// Material detection.
//

function fxDTSBrick::assumeMaterial(%obj)
{
	if(%obj.material !$= "")
	{
		return %obj.material;
	}

	%rgb = getColorIDTable(%obj.colorId);
    %obj.material = findClosestFootstepMaterial(%rgb);
	return %obj.material;
}

function WheeledVehicleData::assumeMaterial(%this, %obj)
{
	if(%this.numWheels > 0)
	{
		return "metal";
	}
	return "wood";
}

function SimObject::assumeMaterial(%this, %obj)
{
	return "tile";
}

//
// Surface detection.
//

//Calculates when the player would realistically take another footstep, based on human biomechanics.
function Armor::getNextFootstepTime(%this, %obj)
{
	%playerVelocity = %obj.getVelocity();
	if(%playerVelocity $= "0 0 0")
	{
		//Not moving, no need for footsteps.
		return -1;
	}

	%playerSpeedX = getWord(%playerVelocity, 0);
	%playerSpeedY = getWord(%playerVelocity, 1);
	%relevantSpeed = (%playerSpeedX > %playerSpeedY) ? %playerSpeedX : %playerSpeedY;

	//Based on the logistic growth equation. Solve for cadence to get steps per second.
	%e = 2.718281828459045;
	%maxStepsPerSecond = 3.5;
	%runTransitionPoint = 6.0;
	%cadenceRise = 0.35;
	return (%maxStepsPerSecond / (1.0 + mPow(%e, (-%k * (%relevantSpeed - %runTransitionPoint)))));
}

function Armor::getFootstepSound(%this, %obj)
{
	if(%obj.isMounted()) 
	{
		//If the player is in a vehicle, no footsteps.
		return -1;
	}

	%playerPosition = %obj.getPosition();
	%typemask = ($TypeMasks::FxBrickObjectType | $Typemasks::StaticObjectType | $TypeMasks::VehicleObjectType);

	%isOnGround = !containerBoxEmpty(%typemask, %playerPosition, 0.6, 0.6, 0.6);
	if(!%isOnGround) 
	{
		//If the player isn't touching the ground, they can't make a footstep sound.
		return -1;
	}

	%collider = containerRayCast(%playerPosition, VectorAdd(%playerPosition, "0.0 0.0 -0.1"), %typemask);
	%colliderType = %collider.getType();
	if(%colliderType & $TypeMasks::fxBrickObjectType || %colliderType & $TypeMasks::VehicleObjectType)
	{
		//For bricks, we only really have color to go on for assuming material.
		//For vehicles, we can assume metal if there's wheels, and perhaps wood otherwise (like a boat.)
		return %collider.assumeMaterial();
	}
}