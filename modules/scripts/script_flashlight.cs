$Eventide_FlashlightBeamSteps = 8;
$Eventide_FlashlightRate = 50;

datablock FxLightData(PlayerFlashlight : PlayerLight) 
{
	uiName = "";
	flareOn = 0;

	radius = 16;
	brightness = 3;
};

datablock FxLightData(PlayerGreenFlashlight : PlayerFlashlight) 
{
	color = "0 1 0 1";
};

datablock ShapeBaseImageData(FlashlightImage) 
{
	shapeFile = "Add-Ons/Gamemode_Eventide/modules/misc/models/flashlight.dts";
	hasLight = true;

	emap = true;
	offset = "0 0 0";
	mountPoint = 1;

	doColorShift = true;
	colorShiftColor = "0.3 0.3 0.35 1";

	lightType = "ConstantLight";
	lightColor = "1 1 1 1";
	lightTime = "1000";
	lightRadius = "10";
};

function Player::flashlightTick(%obj) 
{
	if(isEventPending(%obj.flashlightTick))
	{
		cancel(%obj.flashlightTick);
	}

	//Don't update the flashlight if the player is dead.
	if(%obj.getState() $= "Dead" || !isObject(%obj.light))
	{
		return;
	}

	//Fire a raycast, limited to half the visible distance. If it hits something, stop there. If not, stop at the end of the raycast.
	%range = 32; //64 studs.
	%start = %obj.getMuzzlePoint($LeftHandSlot);
	%vector = %obj.getEyeVector();
	%end = VectorAdd(%start, VectorScale(%obj.getEyeVector(), %range));

	%mask = ($TypeMasks::StaticShapeObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::TerrainObjectType);
	%ray = containerRayCast(%start, %end, %mask, %obj);

	if(%ray $= "0") 
	{
		%pos = %end;
	}
	else 
	{
		//Elevate the light off the ground just a little bit. Otherwise, it clips into the ground and does not shine.
		%normal = VectorNormalize(normalFromRaycast(%ray));
		%pos = VectorAdd(posFromRaycast(%ray), %normal);
	}

	//Set the position of the player light based on the end position of the raycast.
	%obj.light.setTransform(%pos);
	%obj.light.reset();

	//Create a trail of light, to make the beam of the flashlight smooth.
	%flashlightPath = VectorSub(%pos, %start);
	for(%i = 0; %i < %obj.flashlightBeam["steps"]; %i++)
	{
		if(!isObject(%obj.flashlightBeam[%i]))
		{
			%obj.flashlightBeam[%i] = new FxLight() 
			{
				datablock = %obj.light.getDataBlock();

				iconSize = 1;	
				enable = 1;
			};
		}

		%beamStepLocation = VectorAdd(%start, VectorScale(%flashlightPath, (%i + 1) / $Eventide_FlashlightBeamSteps));
		%obj.flashlightBeam[%i].setTransform(%beamStepLocation);
		%obj.flashlightBeam[%i].reset();
	}

	//Schedule another flashlight update, soon.
	%obj.flashlightTick = %obj.schedule($Eventide_FlashlightRate, "flashlightTick");
}

function Player::deleteFlashlightBeam(%obj)
{
	for(%i = 0; %i < %obj.flashlightBeam["steps"]; %i++)
	{
		if(isObject(%obj.flashlightBeam[%i]))
		{
			%obj.flashlightBeam[%i].delete();
		}
	}
}

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

			%player.flashlightBeam["steps"] = $Eventide_FlashlightBeamSteps;
			%flashlightDatablock = %player.greenLight ? PlayerGreenFlashlight : PlayerFlashlight;
			%player.light = new FxLight() 
			{
				datablock = %flashlightDatablock;
				player = %player;

				iconSize = 1;
				enable = 1;
			};
			%player.light.setTransform(%player.getTransform());

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