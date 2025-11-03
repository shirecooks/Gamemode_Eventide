//
// Loading footstep materials.
//

$Eventide_FootstepMaterials["isGlobalFootstepMaterialArray"] = true; //All footstep packs will be stored in this array.
$Eventide_FootstepMaterials["index"] = ""; //Footsteps will be happening too often for us to use a ScriptObject. We'll instead be using a plain array.

function createFootstepMaterial(%footstepMaterialFile, %footstepMaterialRGB)
{
	%material = fileBase(%footstepMaterialFile);
	%cwd = filePath(%footstepMaterialFile);

	//Count the number of sounds available for a given material.
	%soundEffectCount = 0;
	%patterns = ".wav\t.ogg";
	for(%i = 0; %i < getFieldCount(%patterns); %i++)
	{
		%pattern = getField(%patterns, %i);
		for(%file = findFirstFile(%cwd @ "/*" @ %pattern); %file !$= ""; %file = findNextFile(%cwd @ "/*" @ %pattern))
		{
			%soundEffectCount++;
		}
	}

	//Add the footstep pack RGB value to the index, to be iterated over later.
	$Eventide_FootstepMaterials["index"] = ($Eventide_FootstepMaterials["index"] $= "") ? (%footstepMaterialRGB) : ($Eventide_FootstepMaterials["index"] SPC %footstepMaterialRGB);
	
	$Eventide_FootstepMaterials[%material] = %footstepMaterialRGB;
	$Eventide_FootstepMaterials[%footstepMaterialRGB] = %material;
	$Eventide_FootstepMaterials[%footstepMaterialRGB, "count"] = %soundEffectCount;
}

function parseFootstepMaterials(%startingDirectory)
{    
    %footstepMaterialPaths = getFileString(%startingDirectory @ "/*.etmp");
	echo("Footstep material file string:" SPC %footstepMaterialPaths);
    for(%i = 0; %i < getFieldCount(%footstepMaterialPaths); %i++)
    {
		%footstepMaterialFile = getField(%footstepMaterialPaths, %i);

		%fileObject = new FileObject();
		%footstepMaterialRGB = %fileObject.readLine();
		%fileObject.delete();

        %footstepMaterialPath = filePath(%footstepMaterialFile);
        %footstepMaterialFileName = fileBase(%footstepMaterialPath);

        echo("Parsing footstep material \"" @ %footstepMaterialFileName @ "\" from \"" @ %footstepMaterialPath @ "\"...");
        %footstepMaterial = createFootstepMaterial(%footstepMaterialFile, %footstepMaterialRGB);
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
	talk("Index:" SPC %materialIndex);
	for(%i = 0; %i < getWordCount(%materialIndex); %i++)
	{
		%targetRGB = getWord(%materialIndex, %i);
		talk("Candidate RGB:" SPC %targetRGB);
		%euclideanColorDistance = VectorDist(%rgb, %targetRGB);

		if(%euclideanColorDistance < %currentDistance)
		{
			%closestRGB = %targetRGB;
			%currentDistance = %euclideanColorDistance;
		}
	}

	talk("Closest RGB value found:" SPC %closestRGB);

	return $Eventide_FootstepMaterials[%closestRGB];
}

function getFootstepSoundFromMaterial(%material)
{
	%rgb = $Eventide_FootstepMaterials[%material];
	if(%rgb $= "")
	{
		//No sound effect found.
		return -1;
	}

	%soundEffectCount = $Eventide_FootstepMaterials[%rgb, "count"];
	return "fs" @ %material @ getRandom(1, %soundEffectCount) @ "_sound";
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
	return 1000 * (%maxStepsPerSecond / (1.0 + mPow(%e, (-%k * (%relevantSpeed - %runTransitionPoint)))));
}

function Armor::getFootstepSound(%this, %obj)
{
	if(%obj.isMounted()) 
	{
		//If the player is in a vehicle, no footsteps.
		return -1;
	}

	if(%obj.getWaterCoverage() >= 0.4)
	{
		//The player is swimming, get some splash sounds.
		return getFootstepSoundFromMaterial("water");
	}

	%playerPosition = %obj.getPosition();
	%typemask = ($TypeMasks::FxBrickObjectType | $Typemasks::StaticObjectType | $TypeMasks::VehicleObjectType);

	%isOnGround = !containerBoxEmpty(%typemask, %playerPosition, 0.6, 0.6, 0.6);
	if(!%isOnGround) 
	{
		//If the player isn't touching the ground, they can't make a footstep sound.
		return -1;
	}

	%collider = containerRayCast(VectorAdd(%playerPosition, "0.0 0.0 0.1"), VectorAdd(%playerPosition, "0.0 0.0 -0.1"), %typemask);
	%colliderType = %collider.getType();
	if(!(%colliderType & $TypeMasks::FxBrickObjectType) && !(%colliderType & $TypeMasks::VehicleObjectType))
	{
		return;
	}

	//For bricks, we only really have color to go on for assuming material.
	//For vehicles, we can assume metal if there's wheels, and perhaps wood otherwise (like a boat.)
	//For everything else, just assume tile.
	%material = %collider.assumeMaterial();
	return getFootstepSoundFromMaterial(%material);
}

//
// Loop and initialization.
//

function Armor::footstepTick(%this, %obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	%playerDatablock = %obj.getDatablock();
	%nextFootstepTime = %playerDatablock.getNextFootstepTime(%obj);

	if(%nextFootstepTime != -1)
	{
		%soundDatablock = %playerDatablock.getFootstepSound(%obj);
		if(%soundDatablock != -1)
		{
			ServerPlay3D(%soundDatablock, %obj.getPosition());
		}
	}
	else
	{
		%nextFootstepTime = 33;
	}

	cancel(%obj.footstepSchedule);
	%obj.footstepSchedule = %this.schedule(%nextFootstepTime, "footstepTick", %obj);
}

package Script_Footsteps
{
	function Armor::onAdd(%this, %obj)
	{
		parent::onAdd(%this, %obj);
		cancel(%obj.footstepSchedule);
		%this.footstepTick(%obj);
	}

	function Armor::onDisabled(%this, %obj)
	{
		parent::onDisabled(%this, %obj);
		cancel(%obj.footstepSchedule);
	}

	function Armor::onRemove(%this, %obj)
	{
		parent::onRemove(%this, %obj);
		cancel(%obj.footstepSchedule);
	}
};
if(isPackage(Script_Footsteps))
{
	deactivatePackage(Script_Footsteps);
}
activatePackage(Script_Footsteps);