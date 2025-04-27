datablock fxDTSBrickData (brickEventideRitual : brick16x16fData)
{
	uiName = "Ritual Shape";
	Category = "Special";
	Subcategory = "Eventide";
    iconName = "Add-Ons/Gamemode_Eventide/modules/bricks/icons/icon_ritual";
};

datablock StaticShapeData(brickEventideRitualStaticShape)
{
	isInvincible = true;
	shapeFile = "./models/rittualstatic.dts";

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

datablock fxLightData(RitualLight)
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

registerInputEvent("fxDTSBrick","onRitualPlaced","Self fxDTSBrick" TAB "MiniGame MiniGame");
registerInputEvent("fxDTSBrick","onAllRitualsPlaced","Self fxDTSBrick" TAB "MiniGame MiniGame");

function brickEventideRitual::DisplayText(%this, %obj, %name, %color, %distance, %client)
{
    %player = %client.player;

    if (isObject(%obj.textShape))
    {
        %obj.textShape.setShapeName(%name);
        %obj.textShape.setShapeNameColor(%color);
        %obj.textShape.setShapeNameDistance(%distance);
        %obj.bricktext = %name;
    }
    else
    {
        %obj.textShape = new StaticShape()
        {
            datablock = BrickTextEmptyShape;
            position = vectorAdd(%obj.getPosition(), "0 0" SPC %obj.getDatablock().brickSizeZ / 9 + "0.166");
            scale = "0.1 0.1 0.1";
        };

        %obj.textShape.setShapeName(%name);
        %obj.textShape.setShapeNameColor(%color);        
        %obj.textShape.setShapeNameDistance(%distance);
        %obj.bricktext = %name;
    }
}

function brickEventideRitual::ritualCheck(%this,%obj)
{
	%minigame = getMiniGameFromObject(%obj);
	if(!isObject(%obj) || !isObject(%minigame))
	{
		return;
	}

	if(!%obj.resetEffects)
	{
		%obj.ritualshape.playaudio(3,"eventide_ritual_ambience_loop_sound");
		%obj.setEmitter("pongTrailEmitter");
		%obj.resetEffects = true;
	}
	
	%this.DisplayText(%obj,"Rituals needed (drop here): " @ 10-%obj.ritualsPlaced,"0.8 0.1 0.75", "20");

	if(%obj.ritualsPlaced < 10 && isObject(%minigame))
	{
		initContainerRadiusSearch(%obj.getPosition(),3,$TypeMasks::ItemObjectType);		
		while(%scan = containerSearchNext())
		{
			%itemimage = %scan.getdatablock().image;		

			// Make sure the item is above the ritual's position
			if(!%itemimage.isRitual || getWord(%scan.getPosition(),2) < getWord(%obj.getPosition(),2)) continue;

			if(!isObject(Eventide_MinigameGroup)) 
			{
				missionCleanUp.add(new SimGroup(Eventide_MinigameGroup));
			}
			if(!isObject(Eventide_MinigameRitualGroup)) 
			{
				Eventide_MinigameGroup.add(new SimGroup(Eventide_MinigameRitualGroup));
			}

			// Check if the item is a gem to create the gem shape			
			if(%itemimage.isGemRitual)
			{
				if(%obj.gemcount < 4 && !isObject(%obj.gemshape[%obj.gemcount+1]))
				{	
					%obj.gemcount++;
					%obj.gemshape[%obj.gemcount] = new Item() 
					{ 
						datablock = %scan.getdatablock(); 
						isRitual = true;
					};

					%obj.gemshape[%obj.gemcount].schedule(33,playaudio,3,"gem_place_sound");					
					%obj.gemshape[%obj.gemcount].settransform(vectoradd(%obj.gettransform(),%obj.ritualshape.getdatablock().gempos[%obj.gemcount] SPC getWords(%obj.gettransform,3,6)));
					%obj.gemshape[%obj.gemcount].canPickup = false;
					%interactiveshape = %obj.gemshape[%obj.gemcount];
					%interactiveshape.setnodecolor("ALL",%itemimage.colorShiftColor);
					Eventide_MinigameRitualGroup.add(%obj.gemshape[%obj.gemcount]);
				}
				else continue;			
			}
			// Or check if the item is a candle, book or dagger to create the shape
			else switch$(%itemimage.staticShape)
			{
				case "brickCandleStaticShape":  if(%obj.candlecount < 4 && !isObject(%obj.candleshape[%obj.candlecount+1]))											
												{
													%obj.candlecount++;										
													%obj.candleshape[%obj.candlecount] = new StaticShape() { datablock = %itemimage.staticShape; };
													%obj.candleshape[%obj.candlecount].settransform(vectoradd(%obj.gettransform(),%obj.ritualshape.getdatablock().candlePos[%obj.candlecount] SPC getWords(%obj.gettransform,3,6)));
													%interactiveshape = %obj.candleshape[%obj.candlecount];
													Eventide_MinigameRitualGroup.add(%obj.candleshape[%obj.candlecount]);
												}
												else continue;

				case "brickBookStaticShape":	if(isObject(%obj.bookshape))
												{
													continue;
												}

												%obj.bookshape = new Item() 
												{ 
													datablock = %scan.getDataBlock();
													isRitual = true;
												};

												%obj.bookShape.schedule(33,playaudio,3,"book_place_sound");
												%obj.bookshape.canPickup = false;
												%transformdelta = %obj.ritualshape.getdatablock().bookPos SPC getWords(%obj.gettransform,3,6);
												%obj.bookshape.settransform(vectoradd(%obj.gettransform(),%transformdelta));
												%interactiveshape = %obj.bookshape;
												Eventide_MinigameRitualGroup.add(%obj.bookshape);


				case "brickdaggerStaticShape":	if(isObject(%obj.daggershape))
												{
													continue;
												}

												%obj.daggershape = new StaticShape() 
												{ 
													datablock = %itemimage.staticShape; 
												};

												%transformdelta = %obj.ritualshape.getdatablock().daggerPos SPC "0 90 0";
												%obj.daggershape.settransform(vectoradd(%obj.gettransform(),%transformdelta));
												%interactiveshape = %obj.daggershape;
												%interactiveshape.setnodecolor("ALL",%itemimage.colorShiftColor);
												Eventide_MinigameRitualGroup.add(%obj.daggershape);													
			}

			%obj.ritualsPlaced++;
			%scan.delete();

			// Spawn some particles
			for (%m = 0; %m < getRandom(4,8); %m++) 
			{
				%obj.spawnExplosion("horseRayProjectile","0.25 0.25 0.25");					
			}

			// Play some sounds when the rituals are placed, with a hacky workaround to get the pitch to increase
			$oldTimescale = getTimescale();
  			setTimescale(mClampF(0.25+(%obj.ritualsPlaced/10),0.25,2));
  			serverPlay3D("ritual_place_sound",%obj.getPosition());
  			serverPlay3D("puzzlechime_sound",%obj.getPosition());
  			setTimescale($oldTimescale);			

			if(%obj.ritualsPlaced >= 10)
			{
				// Do some effects and play some sounds
				%obj.ritualshape.playaudio(3,"eventide_complete_ritual_ambience_loop_sound");
				serverPlay3D("ritual_explosion_sound",%obj.getPosition());
				%obj.setEmitter("LaserEmitterA");			

				for (%p = 0; %p < getRandom(2,4); %p++) 
				{
					%obj.spawnExplosion("horseRayProjectile","2 2 2");					
				}

				%minigame.playSound("round_start_sound");

				for(%i = 0; %i < %minigame.numMembers; %i++)
				{
					// Set the music to hurry					
					if(isObject(%client = %minigame.member[%i]))
					{
						%client.SetChaseMusic("musicData_escape" SPC getRandom(1, 3), false);

						// Call the killer's onAllRitualsPlaced function
						if(isObject(%client.player) && %client.player.getDatablock().isKiller)
						{
							%client.player.getDatablock().onAllRitualsPlaced(%client.player);
						}						
					}									
				}
			}

			if(isObject($EventideEventCaller))
			{
				$InputTarget_["Self"] = $EventideEventCaller;
				$InputTarget_["MiniGame"] = getMiniGameFromObject($EventideEventCaller.getGroup().client);
				$EventideEventCaller.processInputEvent("onRitualPlaced", $EventideEventCaller.getGroup().client);

				if(%obj.ritualsPlaced >= 10)
				{
					$EventideEventCaller.processInputEvent("onAllRitualsPlaced",$EventideEventCaller.getGroup().client);
					%this.DisplayText(%obj,"", "20");

					// End the loop, we don't want to call this function again, the minigame will restart it anyway
					return;
				}				
			}			
		}
	}

	// Cancel the ritual check to prevent duplicate calls
	cancel(%obj.ritualCheck);
	%obj.ritualCheck = %this.schedule(250,ritualCheck,%obj);
}

function brickEventideRitual::onPlant(%this, %obj)
{		
	Parent::onPlant(%this,%obj);

	%this.ritualCheck(%obj);
	$EventideEventCaller = %obj;
	$EventideRitualBrick = %obj;

	%obj.setRendering(0);
	%obj.setColliding(0);
	%obj.setRaycasting(1);
	%obj.setColor(15);

	%obj.ritualshape = new StaticShape()
	{
		datablock = "brickEventideRitualStaticShape";
		spawnbrick = %obj;
	};

	%obj.light = new fxlight()
	{
		dataBlock = "RitualLight";
		enable = true;
	};
	%obj.light.settransform(vectoradd(%obj.gettransform(),"0 0 0.25") SPC getwords(%obj.gettransform(),3,6));
	%obj.isLightOn = true;	

	%obj.ritualshape.position = %obj.getposition();
}

function brickEventideRitual::onloadPlant(%this, %obj) 
{ 
	brickEventideRitual::onPlant(%this, %obj);
}

function brickEventideRitual::onRemove(%this, %obj)
{	
	Parent::onRemove(%this,%obj);

	if(isObject(%obj.ritualshape))
	{
		%obj.ritualshape.delete();
	}
	
	if(isObject(Eventide_MinigameRitualGroup))
	{
		Eventide_MinigameRitualGroup.delete();
	}	
}

function brickEventideRitual::onDeath(%this, %obj)
{	
	Parent::onRemove(%this,%obj);
}