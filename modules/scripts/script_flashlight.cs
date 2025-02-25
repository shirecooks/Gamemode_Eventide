$Eventide_FlashlightLength = 2;
$Eventide_FlashlightStepSize = 5;
$Eventide_FlashlightRate = 50;

//
// Support functions.
//

//https://blockdoc.block.land/VectorRotate
//Angle in radians.
function VectorRotate(%vec, %axis, %angle)
{
    if (vectorLen(%axis) != 1)
    {
        %axis = vectorNormalize(%axis);
    }

    %proj = vectorScale(%axis, vectorDot(%vec, %axis));
    %ortho = vectorSub(%vec, %proj);
    %w = vectorCross(%axis, %ortho);
    %cos = mCos(%angle);
    %sin = mSin(%angle);
    %x1 = %cos / vectorLen(%ortho);
    %x2 = %sin / vectorLen(%w);
    %rotOrtho = vectorScale(vectorAdd(vectorScale(%ortho, %x1), vectorScale(%w, %x2)), vectorLen(%ortho));
    return vectorAdd(%rotOrtho, %proj);
}

//
// Datablocks.
//

datablock FxLightData(PlayerFlashlight : PlayerLight) 
{
	uiName = "";
	flareOn = 0;

	radius = 12;
	brightness = 2;
};

datablock FxLightData(PlayerGreenFlashlight : PlayerFlashlight) 
{
	color = "0 1 0 1";
};

datablock fxLightData(PlayerBlockedFlashlight)
{
	LightOn = false;

	flareOn = true;
	flarebitmap = "base/data/shapes/blank.png";
	ConstantSize = 1;
    ConstantSizeOn = true;
    FadeTime = 0;

	LinkFlare = false;
	blendMode = 1;
	flareColor = "1 0 0 1";

	AnimOffsets = true;
	startOffset = "0 0 0";
	endOffset = "0 0 0";
};

datablock ShapeBaseImageData(FlashlightImage) 
{
	shapeFile = "Add-Ons/Gamemode_Eventide/modules/misc/models/flashlight.dts";
	hasLight = false;

	emap = true;
	offset = "0 0 0";
	mountPoint = $LeftHandSlot;

	doColorShift = true;
	colorShiftColor = "0.3 0.3 0.35 1";
};

//
// Functions and logic.
//

function Player::flashlightTick(%obj) 
{
	if(isEventPending(%obj.flashlightTick))
	{
		cancel(%obj.flashlightTick);
	}

	//Don't update the flashlight if the player is dead.
	if(%obj.getState() $= "Dead" || !isObject(%obj.light))
	{
		%obj.deleteFlashlightBeam();
		return;
	}

	//Fire a raycast, limited to half the visible distance. If it hits something, stop there. If not, stop at the end of the raycast.
	%range = 32; //64 studs.
	%start = %obj.getMuzzlePoint($LeftHandSlot);
	%eyeVector = VectorNormalize(%obj.getEyeVector());
	%end = VectorAdd(%start, VectorScale(%eyeVector, %range));

	%mask = ($TypeMasks::StaticShapeObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::FxBrickAlwaysObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::TerrainObjectType);
	%raycast = containerRayCast(%start, %end, %mask, %obj);

	if(%raycast $= "0") 
	{
		%endPosition = %end;
	}
	else 
	{
		//Elevate the light off the surface it hit just a little bit. Otherwise, it clips into the ground and does not shine.
		%endPosition = VectorAdd(posFromRaycast(%raycast), normalFromRaycast(%raycast));
	}

	%flashlightVector = VectorNormalize(VectorSub(%endPosition, %start));

	//The "official" player light will be the first step in the flashlight beam, so set it to the first position.
	%lightDatablock = %obj.light.getDataBlock();

	//To ensure the beam is visually smooth, make sure the beam nodes are always 3 units above the ground or higher. Any lower, and the light begins to clip into the ground.
	%initialLightPosition = VectorAdd(%start, VectorScale(%flashlightVector, $Eventide_FlashlightStepSize));
	%initialLightZValue = getWord(%initialLightPosition, 2);
	%initialLightPosition = setWord(%initialLightPosition, 2, mClampF(%initialLightZValue, 3.0, %initialLightZValue));

	%obj.light.setTransform(%initialLightPosition);
	%obj.light.reset();

	//Recycle previously used flashlight beam nodes indiscriminately.
	%unusedLightObjects = new SimSet();
	for(%i = 0; %i < %obj.flashlightBeamGroup.getCount(); %i++)
	{
		%unusedLightObjects.add(%obj.flashlightBeamGroup.getObject(%i));
	}

	//Create a trail of lights, to make the beam of the flashlight smooth.
	for(%i = 1; %i <= %obj.flashlightBeamGroup.numIterations; %i++)
	{
		%lightsThisStep = mPow(2, %i);

		//Have it cone out based on the number of beam nodes this step.
		for(%j = 1; %j <= %lightsThisStep; %j++)
		{
			%lightObject = %unusedLightObjects.getObject(0);

			%initialLightVector = VectorSub(%initialLightPosition, %start);
			%beamStepCenter = VectorAdd(%start, VectorScale(%initialLightVector, (%i + 1)));

			//Rotate the point left if on an even iteration, or right if on an odd iteration.
			%rotationFactor = %j % 2 == 0 ? mDegToRad($Eventide_FlashlightStepSize) : -mDegToRad($Eventide_FlashlightStepSize);
			%translatedPoint = VectorSub(%beamStepCenter, %initialLightPosition);
			%rotatedPoint = VectorRotate(%translatedPoint, "0 0 1", %rotationFactor);
			%finalPoint = VectorAdd(%rotatedPoint, %initialLightPosition);

			%raycast = containerRayCast(%finalPoint, %start, %mask, %obj);
			if(%raycast $= "0")
			{
				if(%lightObject.getDataBlock() !$= %lightDatablock)
				{
					%lightObject.setDataBlock(%lightDatablock);
				}
				%lightObject.setTransform(%finalPoint);
				%lightObject.reset();
			}
			else
			{
				if(%lightObject.getDataBlock() !$= PlayerBlockedFlashlight)
				{
					%lightObject.setDataBlock(PlayerBlockedFlashlight);
				}
			}

			%unusedLightObjects.remove(%lightObject);
		}
	}
	%unusedLightObjects.delete();

	//Schedule another flashlight update, soon.
	%obj.flashlightTick = %obj.schedule($Eventide_FlashlightRate, "flashlightTick");
}

function Player::createFlashlightBeam(%obj)
{
	if(isObject(%obj.flashlightBeamGroup))
	{
		%obj.flashlightBeamGroup.delete();
	}

	%obj.flashlightBeamGroup = new SimGroup();
	%obj.flashlightBeamGroup.numIterations = 0;
	
	%totalLights = 0;
	for(%i = 1; %i < $Eventide_FlashlightLength; %i++)
	{
		%totalLights += mPow(2, %i);
		%obj.flashlightBeamGroup.numIterations += 1;
	}

	//We figured out how much, now lets create all the needed light and add them to the flashlight group.
	%lightDatablock = %obj.light.getDataBlock();
	for(%i = 0; %i < %totalLights; %i++)
	{
		%flashlightNode = new FxLight() 
		{
			datablock = %lightDatablock;

			iconSize = 1;	
			enable = 1;
		};
		%flashlightNode.setTransform(%obj.getTransform());
		%flashlightNode.reset();
		%obj.flashlightBeamGroup.add(%flashlightNode);
	}
}

function Player::deleteFlashlightBeam(%obj)
{
	if(isObject(%obj.flashlightBeamGroup))
	{
		%obj.flashlightBeamGroup.delete();
	}
}

//
// Package, default flashlight functionality override.
//

function pushServerPackageToBack(%package) 
{
	//Make sure a package certain package on a function gets called last. In this case, the flashlight override.
	for(%i = getNumActivePackages() - 1; %i >= $numClientPackages; %i--) 
	{
		%current = getActivePackage(%i);
		if(%current !$= %package) 
		{
			%stack = ltrim(%stack SPC %current);
			deactivatePackage(%current);
		}
	}

	if(%stack !$= "") 
	{
		for(%i = getWordCount(%stack) - 1; %i >= 0; %i--) 
		{
			activatePackage(getWord(%stack, %i));
		}
	}
}

package Eventide_Flashlight 
{
	function serverCmdLight(%client) 
	{
		%player = %client.player;

		if(!isObject(%player) || %player.getState() $= "Dead" || %player.getDatablock().isKiller) 
		{
			parent::serverCmdLight(%client);
			return;
		}

		if(getSimTime() - %player.lastLightTime < 250) 
		{
			return;
		}

		%player.lastLightTime = getSimTime();

		if(isObject(%player.light)) 
		{
			//Turn the flashlight off.

			%player.light.delete();
			%player.deleteFlashlightBeam();

			serverPlay3D("flashlight_off_sound", %player.getHackPosition());

			if(%player.getMountedImage($LeftHandSlot) == nameToID("FlashlightImage")) 
			{
				%player.unMountImage($LeftHandSlot);
			}
		}
		else 
		{
			//Turn the flashlight on.

			%flashlightDatablock = %player.greenLight ? PlayerGreenFlashlight : PlayerFlashlight;
			%player.light = new FxLight() 
			{
				datablock = %flashlightDatablock;
				player = %player;

				iconSize = 1;
				enable = 1;
			};
			%player.light.setTransform(%player.getTransform());
			%player.createFlashlightBeam();

			serverPlay3D("flashlight_on_sound", %player.getHackPosition());

			//Place a flashlight model in the player's left hand, if it's empty.
			if(!isObject(%player.getMountedImage($LeftHandSlot)))
			{
				%player.mountImage(flashLightImage, $LeftHandSlot);
			}

			if(!isEventPending(%player.flashlightTick)) 
			{
				%player.flashlightTick();
			}
		}
	}

	function serverCmdGreenLight(%client, %checkValue)
	{
		%player = %client.player;
		if(isObject(%player))
		{
			%player.greenLight = true;
		}
		serverCmdLight(%client);
		%player.greenLight = false;
	}

	function Player::unmountImage(%this, %slot) 
	{
		parent::unmountImage(%this, %slot);

		if(%slot == $LeftHandSlot && isObject(%this.light)) 
		{
			%this.mountImage(FlashlightImage, 1);
		}
	}

	function Armor::onRemove(%this, %obj)
	{
		//Delete the flashlight beam when the player dies, respawns, or disconnects.
		if(isEventPending(%obj.flashlightTick))
		{
			cancel(%obj.flashlightTIck);
		}
		%obj.deleteFlashlightBeam();
		parent::onRemove(%this, %obj);
	}
};

if(isPackage("Eventide_Flashlight"))
{
	deactivatePackage("Eventide_Flashlight");
}
activatePackage("Eventide_Flashlight");
pushServerPackageToBack("Eventide_Flashlight");