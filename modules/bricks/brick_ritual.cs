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
	if(!isObject(%obj)) return;	
	
	if(%obj.ritualsPlaced < 10)
	{
		%this.DisplayText(%obj,"Rituals needed (drop here): " @ 10-%obj.ritualsPlaced, "0.8 0.1 0.75", "20");
	}
	else %this.DisplayText(%obj,"", "0 0 0", "0");

	if(MiniGameGroup.getCount())
	{
		for(%i = 0; %i < MiniGameGroup.getCount(); %i++)
		{
			if(isObject(%minigame = MiniGameGroup.getObject(i)) && strstr(strlwr(%minigame.title), "eventide") != -1)
			{
				%eventideminigame = %minigame;
				break;
			}
		}						
	}
	else %eventideminigame = 0;

	if(%obj.ritualsPlaced < 10 && isObject(%eventideminigame))
	{
		initContainerRadiusSearch(%obj.getPosition(), 2.5, $TypeMasks::ItemObjectType | $TypeMasks::PlayerObjectType);		
		while(%scan = containerSearchNext())
		{
			%itemimage = %scan.getdatablock().image;		

			// Make sure the item is above the ritual's position
			if(!%itemimage.isRitual || getWord(%scan.getPosition(),2) < getWord(%obj.getPosition(),2)) continue;

			if(!isObject(Eventide_MinigameGroup)) missionCleanUp.add(new SimGroup(Eventide_MinigameGroup));
			if(!isObject(Eventide_MinigameRitualGroup)) Eventide_MinigameGroup.add(new SimGroup(Eventide_MinigameRitualGroup));

			if(%itemimage.isGemRitual)
			{
				if(%obj.gemcount <= 4 && !isObject(%obj.gemshape[%obj.gemcount+1]))
				{	
					%obj.gemcount++;
					%obj.gemshape[%obj.gemcount] = new Item() 
					{ 
						datablock = %scan.getdatablock(); 
						isRitual = true;
					};

					%obj.gemshape[%obj.gemcount].schedule(33,playaudio,3,"gem_place_sound");
					%obj.gemshape[%obj.gemcount].canPickup = false;
					%obj.gemshape[%obj.gemcount].settransform(vectoradd(%obj.gettransform(),%obj.ritualshape.getdatablock().gempos[%obj.gemcount] SPC getWords(%obj.gettransform,3,6)));
					%interactiveshape = %obj.gemshape[%obj.gemcount];
					%interactiveshape.setnodecolor("ALL",%itemimage.colorShiftColor);
					Eventide_MinigameRitualGroup.add(%interactiveshape);
				}
				else continue;			
			}
			else switch$(%itemimage.staticShape)
			{
				case "brickCandleStaticShape":  if(%obj.candlecount <= 4 && !isObject(%obj.candleshape[%obj.candlecount+1]))											
												{
													%obj.candlecount++;										
													%obj.candleshape[%obj.candlecount] = new StaticShape() { datablock = %itemimage.staticShape; };
													%obj.candleshape[%obj.candlecount].settransform(vectoradd(%obj.gettransform(),%obj.ritualshape.getdatablock().candlePos[%obj.candlecount] SPC getWords(%obj.gettransform,3,6)));
													%interactiveshape = %obj.candleshape[%obj.candlecount];
													Eventide_MinigameRitualGroup.add(%interactiveshape);
												}
												else continue;

				case "brickBookStaticShape":	if(isObject(%obj.bookshape)) continue;

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
												Eventide_MinigameRitualGroup.add(%interactiveshape);


				case "brickdaggerStaticShape":	if(isObject(%obj.daggershape)) continue;

												%obj.daggershape = new StaticShape() { datablock = %itemimage.staticShape; };
												%transformdelta = %obj.ritualshape.getdatablock().daggerPos SPC "0 90 0";
												%obj.daggershape.settransform(vectoradd(%obj.gettransform(),%transformdelta));
												%interactiveshape = %obj.daggershape;
												%interactiveshape.setnodecolor("ALL",%itemimage.colorShiftColor);
												Eventide_MinigameRitualGroup.add(%interactiveshape);													
			}

			%obj.ritualsPlaced++;
			%scan.delete();

			if(%obj.ritualsPlaced >= 10)
			{
				serverPlay3D("generator_explode_sound",%obj.getPosition());
				%eventideminigame.playSound("round_start_sound");
				%obj.setEmitter("LaserEmitterA");				
				%obj.spawnExplosion("horseRayProjectile","2 2 2");				

				for(%i = 0; %i < ClientGroup.getCount(); %i++)
				{
					%client = ClientGroup.getObject(%i);

					// Set the music to hurry					
					if(isObject(%client.EventideMusicEmitter))
					{
						%client.SetChaseMusic("musicData_eventide_hurry",false);
					}
					
					// Call the killer's onAllRitualsPlaced function
					if(isObject(%client.player) && %client.player.getDatablock().isKiller)
					{
						%client.player.getDatablock().onAllRitualsPlaced(%client.player);
					}
				}
			}

			if(isObject($EventideEventCaller))
			{
				$InputTarget_["Self"] = $EventideEventCaller;
				$InputTarget_["MiniGame"] = getMiniGameFromObject($EventideEventCaller.getGroup().client);
				$EventideEventCaller.processInputEvent("onRitualPlaced", $EventideEventCaller.getGroup().client);

				if(%obj.ritualsPlaced >= 10)
				$EventideEventCaller.processInputEvent("onAllRitualsPlaced",$EventideEventCaller.getGroup().client);
			}			
		}
	}

	// Cancel the ritual check to prevent duplicate calls
	cancel(%obj.ritualCheck);
	%obj.ritualCheck = %this.schedule(500,ritualCheck,%obj);
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
	%obj.setColor(17);

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
	
	if(isObject(%brick.interactiveshape)) %brick.interactiveshape.delete();
	if(isObject(%obj.ritualshape)) %obj.ritualshape.delete();
	if(isObject(%obj.bookshape)) %obj.bookshape.delete();
	if(isObject(%obj.daggershape)) %obj.daggershape.delete();
	for(%candlecount = 1; %candlecount <= 4; %candlecount++) if(isObject(%obj.candleshape[%candlecount])) %obj.candleshape[%candlecount].delete();
	for(%gemcount = 1; %gemcount <= 4; %gemcount++) if(isObject(%obj.gemshape[%gemcount])) %obj.gemshape[%gemcount].delete();
}

function brickEventideRitual::onDeath(%this, %obj)
{	
	Parent::onRemove(%this,%obj);
}