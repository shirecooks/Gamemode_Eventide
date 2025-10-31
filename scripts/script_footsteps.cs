//
// Loading footstep sounds.
//

$Eventide_FootstepPacks["isGlobalFootstepPackArray"] = true; //All footstep packs will be stored in this array.
$Eventide_FootstepsPacks["index"] = ""; //Footsteps will be happening too often for us to use a ScriptObject. We'll instead be using a plain array.

function createFootstepPack(%footstepFileCategory, %footstepPackRGB)
{
	//Add the footstep pack RGB value to the index, to be iterated over later.
	$Eventide_FootstepsPacks["index"] = ($Eventide_FootstepsPacks["index"] $= "") ? (%footstepPackRGB) : ($Eventide_FootstepsPacks["index"] SPC %footstepPackRGB);

	$Eventide_FootstepsPacks[%footstepPackRGB] = %footstepFileCategory;
	
}

function parseVoicePacks(%startingDirectory)
{    
    %footstepPackPaths = getFileString(%startingDirectory @ "/*.etfsp");
    for(%i = 0; %i < getFieldCount(%footstepPackPaths); %i++)
    {
		%footstepPackFile = getField(%footstepPackPaths, %i);

		%fileObject = new FileObject();
		%footstepPackRGB = %fileObject.readLine();
		%fileObject.delete();

        %footstepPackPath = filePath(%footstepPackFile);
        %footstepPackFileName = fileBase(%footstepPackPath);

        echo("Parsing footstep pack \"" @ %footstepPackFileName @ "\" from \"" @ %footstepPackPath @ "\"...");
        %footstepPack = createFootstepPack(%footstepPackFileName, %footstepPackRGB);
    }
}

//
// Miscellaneous functions.
//

//Ripped from Slayer.
function rgbToHex(%rgb)
{
	// use % to find remainder
	%r = getWord(%rgb,0);
	%g = getWord(%rgb,1);
	%b = getWord(%rgb,2);
	// in-case the rgb value isn't on the right scale
	%r = ( %r <= 1 ) ? %r * 255 : %r;
	%g = ( %g <= 1 ) ? %g * 255 : %g;
	%b = ( %b <= 1 ) ? %b * 255 : %b;
	// the hexidecimal numbers
	%a = "0123456789ABCDEF";

	%r = getSubStr(%a,(%r-(%r % 16))/16,1) @ getSubStr(%a,(%r % 16),1);
	%g = getSubStr(%a,(%g-(%g % 16))/16,1) @ getSubStr(%a,(%g % 16),1);
	%b = getSubStr(%a,(%b-(%b % 16))/16,1) @ getSubStr(%a,(%b % 16),1);

	return %r @ %g @ %b;
}

//
// Brick functions.
//

function fxDTSBrick::assumeMaterial(%obj)
{
    %rgb = getColorIDTable(%obj.colorID);
}

//
// Vehicle functions.
//

function WheeledVehicleData::assumeMaterial(%this, %obj)
{
	if(%this.numWheels > 0)
	{

	}
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
	%collider = containerRayCast(%playerPosition, VectorAdd(%playerPosition, -0.1), ($TypeMasks::fxBrickObjectType | $Typemasks::TerrainObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxPlaneObjectType));
	if(!%collider) 
	{
		//If the player isn't touching the ground, they can't make a footstep sound.
		return -1;
	}

	%colliderType = %collider.getType();
	if(%colliderType & $TypeMasks::fxBrickObjectType || %colliderType & $TypeMasks::VehicleObjectType)
	{
		//For bricks, we only really have color to go on for assuming material.
		//For vehicles, we can assume metal if there's wheels, and perhaps wood otherwise (like a boat.)
		return %collider.assumeMaterial();
	}
}