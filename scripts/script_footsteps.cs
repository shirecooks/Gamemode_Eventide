//
// Loading footstep materials.
//

$Pref::Eventide::FootstepSystemDebug = ($Pref::Eventide::FootstepSystemDebug $= "") ? false : $Pref::Eventide::FootstepSystemDebug; //Whether or not to print debug statements related to footstep material parsing and detection.
$Eventide_FootstepMaterials["isGlobalFootstepMaterialArray"] = true; //All footstep packs will be stored in this array.
$Eventide_FootstepMaterials["index"] = ""; //Footsteps will be happening too often for us to use a ScriptObject. We'll instead be using a plain array.

function createFootstepMaterial(%footstepMaterialFile, %footstepMaterialRGB)
{
	%material = strlwr(fileBase(%footstepMaterialFile));
	%cwd = filePath(%footstepMaterialFile);

	if($Pref::Eventide::FootstepSystemDebug)
	{
		echo("Parsing footstep material:" SPC %material SPC "from" SPC %cwd);
	}

	//Count the number of sounds available for a given material.
	%soundEffectCount = 0;
	%patterns = ".wav\t.ogg";
	for(%i = 0; %i < getFieldCount(%patterns); %i++)
	{
		%pattern = getField(%patterns, %i);
		for(%file = findFirstFile(%cwd @ "/*" @ %pattern); %file !$= ""; %file = findNextFile(%cwd @ "/*" @ %pattern))
		{
			if($Pref::Eventide::FootstepSystemDebug)
			{
				echo("\t- Added footstep sound:" SPC %file);
			}
			%soundEffectCount++;
		}
	}

	//Add the footstep pack RGB value to the index, to be iterated over later.
	$Eventide_FootstepMaterials["index"] = ($Eventide_FootstepMaterials["index"] $= "") ? (%footstepMaterialRGB) : ($Eventide_FootstepMaterials["index"] TAB %footstepMaterialRGB);
	
	if($Pref::Eventide::FootstepSystemDebug)
	{
		echo("Current index:" SPC $Eventide_FootstepMaterials["index"]);
	}

	$Eventide_FootstepMaterials[%material] = %footstepMaterialRGB;
	$Eventide_FootstepMaterials[%footstepMaterialRGB] = %material;
	$Eventide_FootstepMaterials[%footstepMaterialRGB, "count"] = %soundEffectCount;
}

function parseFootstepMaterials(%startingDirectory)
{    
    %footstepMaterialPaths = getFileString(%startingDirectory @ "/*.etmp");

	if($Pref::Eventide::FootstepSystemDebug)
	{
		echo("Footstep material file string:" SPC %footstepMaterialPaths);
	}

    for(%i = 0; %i < getFieldCount(%footstepMaterialPaths); %i++)
    {
		%footstepMaterialFile = getField(%footstepMaterialPaths, %i);

		%fileObject = new FileObject();
		%fileObject.openForRead(%footstepMaterialFile);
		%footstepMaterialRGB = %fileObject.readLine();
		%fileObject.delete();

        %footstepMaterialPath = filePath(%footstepMaterialFile);
        %footstepMaterialFileName = fileBase(%footstepMaterialPath);

		if($Pref::Eventide::FootstepSystemDebug)
		{
			echo("Parsing footstep material \"" @ %footstepMaterialFileName @ "\" from \"" @ %footstepMaterialPath @ "\"...");
		}

        %footstepMaterial = createFootstepMaterial(%footstepMaterialFile, %footstepMaterialRGB);
    }
}

//
// Managing footstep materials.
//

function findClosestFootstepMaterial(%rgb)
{
	%closestRGB = "0 0 0";
	%currentDistance = $maxInt; //Can only go down from here.

	//Determine which material has the closest RGB similarity to the provided RGB color.
	%materialIndex = $Eventide_FootstepMaterials["index"];
	for(%i = 0; %i < getFieldCount(%materialIndex); %i++)
	{
		%targetRGB = getField(%materialIndex, %i);
		%euclideanColorDistance = VectorDist(%rgb, %targetRGB);

		if(%euclideanColorDistance < %currentDistance)
		{
			%closestRGB = %targetRGB;
			%currentDistance = %euclideanColorDistance;
		}
	}

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

	//Based on the logistic growth equation. Solve for cadence to get steps per second.
	%e = 2.718281828459045;
	%maxStepsPerSecond = 4.5;
	%runTransitionPoint = 7.0;
	%cadenceRise = 0.35;

	%movementSpeed = VectorLen(%playerVelocity);
	if(%movementSpeed > 3 && %movementSpeed < 5)
	{
		//Prevent an awkward delay when accelerating up to normal speed. Return 2.2 steps per second.
		return (1000 / 2.2);
	}
	
	%stepsPerSecond = %maxStepsPerSecond / (1.0 + mPow(%e, (-%cadenceRise * (%movementSpeed - %runTransitionPoint))));
	%millisecondsPerStep = 1000 / %stepsPerSecond;
	return %millisecondsPerStep;
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

	%collider = containerRayCast(VectorAdd(%playerPosition, "0.0 0.0 0.1"), VectorAdd(%playerPosition, "0.0 0.0 -0.8"), %typemask);
	if(!%collider)
	{
		return;
	}

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
			%col.assumeMaterial();
		}
	}

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