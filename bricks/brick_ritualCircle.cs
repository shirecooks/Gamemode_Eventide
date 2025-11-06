//
// Storage of ritual circles, for minigame reset.
//

$Eventide_RitualCircles = new SimGroup();

//
// The brick itself.
//

datablock fxDTSBrickData(brickEventideRitualCircle : brick16x16fData)
{
	uiName = "Ritual Shape";
	category = "Special";
	subCategory = "Eventide";
    iconName = "Add-Ons/Gamemode_Eventide/bricks/models/ritualCircle/icon_ritual";

	lightDatablock = "ritualLight";
	emitterDatablock = "pongTrailEmitter";
	completionEmitterDatablock = "LaserEmitterA";
	textDisplacement = "0 0 0.25";
	ritualsNeeded = 10;
};

datablock StaticShapeData(brickEventideRitualCircleStaticShape)
{
	isInvincible = true;
	shapeFile = "./models/ritualCircle/ritualCircle.dts";

	gemPos1 = "2.28 2.875 0.1";
	gemPos2 = "-2.28 2.875 0.1";
	gemPos3 = "2.1 -2.725 0.1";
	gemPos4 = "-2.1 -2.725 0.1";
	candlePos1 = "0 -3.5 0.375";
	candlePos2 = "0 3.5 0.375";
	candlePos3 = "-3.25 1.1 0.375";
	candlePos4 = "3.25 1.1 0.375";
	bookPos = "0 0.65 0.1";
	daggerPos = "0 -0.65 0.1";
};

datablock fxLightData(ritualLight)
{
	uiName = "Ritual Light";
	LightOn = true;
	radius = 10;
	brightness = 5;
	color = "1 0.1 0.9";
	FlareOn			= false;
	FlareTP			= false;
	Flarebitmap		= "";
	FlareColor		= "1 1 1";
	ConstantSizeOn	= false;
	ConstantSize	= 1;
	NearSize		= 1;
	FarSize			= 0.5;
	NearDistance	= 10.0;
	FarDistance		= 30.0;
	FadeTime		= 0.1;
};

datablock StaticShapeData(BrickTextEmptyShape)
{
    shapefile = "base/data/shapes/empty.dts";
};

//
// Progress display and logic.
//

function brickEventideRitualCircle::onObjectCollision(%this, %obj, %col)
{
	%this.schedule(1, checkForItems, %obj);
	return true;
}

function brickEventideRitualCircle::checkForItems(%this, %obj)
{
	//The Object Collision DLL incorrectly reports dropped items as random objects.
	//Triggers don't detect items, and PhysicalZones have no callbacks.
	//No other choice that to just run a container search to determine if it was an actual item that collided with the ritual circle.
	%brickX = (%this.brickSizeX / 2);
	%brickY = (%this.brickSizeY / 2);
	%brickZ = (%this.brickSizeZ / 2);

	%item = containerFindFirst($TypeMasks::ItemObjectType, %obj.position, %brickX, %brickY, %brickZ);
	if(!%item || %item.Datablock $= "" || %item.Datablock.ritualType $= "" || %item.isPlaced)
	{
		return;
	}

	%this.placeRitual(%obj, %item);
}

function brickEventideRitualCircle::displayText(%this, %obj, %text, %color, %distance)
{
	//Default color.
	%color = (%color !$= "") ? %color : "0.8 0.1 0.75";

	//Default distance.
	%distance = (%distance !$= "") ? %distance : 20;

	%textShape = %obj.textShape;
    %textShape.setShapeName(%text);
	%textShape.setShapeNameColor(%color);        
	%textShape.setShapeNameDistance(%distance);
	%textShape.currentText = %text;
}

function brickEventideRitualCircle::displayProgress(%this, %obj)
{
	%this.displayText(%obj, "Rituals needed (drop here):" SPC (%this.ritualsNeeded - %obj.ritualCollection.getCount()));
}

function brickEventideRitualCircle::placeRitual(%this, %obj, %item)
{
	//Let the item itself dictate how it will be placed on the ritual circle.
	//If the item returns 0, it does not want to be placed. Don't.
	%itemDatablock = %item.getDatablock();
	%shouldContinue = %itemDatablock.placeOnRitualCircle(%item, %obj);
	if(!%shouldContinue)
	{
		return;
	}

	//Store the objects for later deletion, and count how many there are.
	%obj.ritualCollection.add(%item);
	%ritualsCollected = %obj.ritualCollection.getCount();
	%ritualsNeeded = %this.ritualsNeeded;

	%obj.ritualCount[%itemDatablock.ritualType]++;
	%item.isPlaced = true;
	%this.displayProgress(%obj);

	//Process input events.
	$InputTarget_["Self"] = %obj;
	$InputTarget_["MiniGame"] = getMiniGameFromObject(%obj);
	%obj.processInputEvent("onRitualPlaced");

	//If enough rituals have been placed, run the below function.
	if(%ritualsCollected >= %ritualsNeeded)
	{
		%this.onAllRitualsPlaced(%obj);
	}

	%brickPosition = %obj.getPosition();

	//Spawn some particles.
	for(%m = 0; %m < getRandom(4, 8); %m++) 
	{
		%obj.spawnExplosion("horseRayProjectile", "0.25 0.25 0.25");					
	}

	//Play a sound effect, that pitches up for each additional ritual item.
	%oldTimescale = getTimescale();
	%percentagePitch = (%ritualsCollected / %ritualsNeeded);
	setTimescale(%percentagePitch);
	serverPlay3D("ritual_place_sound", %brickPosition);
	serverPlay3D("puzzle_chime_sound", %brickPosition);
	setTimescale(%oldTimescale);
}

function brickEventideRitualCircle::onAllRitualsPlaced(%this, %obj)
{
	//Play the appropriate sounds.
	%obj.ritualCircle.playAudio(3, "ritual_complete_sound");
	serverPlay3D("ritual_explosion_sound", %obj.getPosition());

	//Visual FX, you know the drill.
	%obj.setEmitter(%this.completionEmitterDatablock);			
	for (%p = 0; %p < getRandom(2,4); %p++) 
	{
		%obj.spawnExplosion("horseRayProjectile","2 2 2");					
	}

	//If we're in a minigame, let's make some noise.
	%minigame = getMiniGameFromObject(%obj);
	if(isObject(%minigame))
	{
		%minigame.onAllRitualsPlaced();
	}

	//Process input events.
	$InputTarget_["Self"] = %obj;
	$InputTarget_["MiniGame"] = %minigame;
	%obj.processInputEvent("onAllRitualsPlaced");
}

//
// Loading and unloading.
//

function brickEventideRitualCircle::onPlant(%this, %obj)
{		
	Parent::onPlant(%this, %obj);

	%brickZ = (%this.brickSizeZ / 2);

	%brickTransform = %obj.getTransform();
	%brickPosition = VectorSub(posFromTransform(%brickTransform), "0 0" SPC %brickZ); //getTransform returns the center of the brick, not the bottom.
	%brickRotation = rotFromTransform(%brickTransform);

	//Hide the actual brick, to show off the static shape.
	%obj.setRendering(0);
	%obj.setColliding(0);
	%obj.setRaycasting(1);
	%obj.setColor(15);

	//Storage of dropped ritual items.
	%obj.ritualCollection = new SimGroup();

	//Static shape above the main one, for holding text.
	%textShape = new StaticShape()
	{
		datablock = BrickTextEmptyShape;
		position = VectorAdd(%obj.getPosition(), "0 0" SPC (%this.brickSizeZ / 9 + "0.166"));
		scale = "0.1 0.1 0.1";
	};
	%obj.textShape = %textShape;
	%this.displayProgress(%obj);

	//The ritual circle model.
	%ritualCircle = new StaticShape()
	{
		datablock = brickEventideRitualCircleStaticShape;
		spawnbrick = %obj;
	};
	%ritualCircle.setTransform(%brickTransform);
	%obj.ritualCircle = %ritualCircle;

	//Illuminate the ritual circle with the given light.
	%light = new fxLight()
	{
		datablock = %this.lightDatablock;
		enable = true;
	};
	%light.setTransform(VectorAdd(%brickPosition, %this.textDisplacement) SPC %brickRotation);
	%obj.light = %light;

	//Play an ambient sound loop.
	%ritualCircle.playAudio(3, "ritual_hum_sound");
}

function brickEventideRitualCircle::onLoadPlant(%this, %obj) 
{ 
	%this.onPlant(%obj);
}

function brickEventideRitualCircle::onRemove(%this, %obj)
{	
	if(%obj.isPlanted())
	{
		//Blockland treats ghost bricks the same as normal bricks. This stuff below won't exist if the brick isn't planted.
		//%obj.textShape.delete(); Not needed apparently?
		%obj.ritualCircle.delete();
		%obj.ritualCollection.delete();
	}

	return parent::onRemove(%this, %obj);
}

//
// Minigame functionality.
//

function MinigameSO::onAllRitualsPlaced(%obj)
{
	
}

function brickEventideRitualCircle::reset(%this, %obj)
{
	//Clear any held items.
	%obj.ritualCollection.delete();
	%obj.ritualCollection = new SimGroup();

	//Reset the progress counter.
	%obj.displayProgress();
}

package Brick_RitualCircle
{
	function MinigameSO::Reset(%obj, %client)
	{
		parent::Reset(%obj, %client);
		
		//Reset each ritual circle back to it's default state.
		for(%i = 0; %i < $Eventide_RitualCircles.getCount(); %i++)
		{
			%ritualCircle = $Eventide_RitualCircles[%i];
			%ritualCircle.Datablock.reset(%ritualCircle, %i);
		}
	}
};
if(isPackage(Brick_RitualCircle))
{
	deactivatePackage(Brick_RitualCircle);
}
activatePackage(Brick_RitualCircle);