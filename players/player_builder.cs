//
// Core and appearance.
//

datablock PlayerData(PlayerBuilder : PlayerSurvivor)
{
    uiName = "Builder";

    brickRegenRate = 30000;
    bricksPerRegen = 3;
    maxBrickSupply = 9;
};
//Inherits functions from `PlayerSurvivor`.
PlayerBuilder.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerBuilder::getFacePack(%this, %obj)
{
	%client = %obj.client;
	%facePack = (isObject(%client) && %client.chest) ? "builderF" : "builderM";
	return %facePack;
}

function PlayerBuilder::eventideBodyParts(%this, %obj)
{
	%this.super("eventideBodyParts", %this, %obj);

    //Give the builder a hardhat.
    %obj.unhideNode("scoutHat");

    //Give their torso a custom decal.
	%obj.setDecalName("shopkeeper");
}

function PlayerBuilder::eventideBodyColors(%this, %obj)
{
    %this.super("eventideBodyColors", %this, %obj);

    //Color the hardhat yellow.
    %obj.setNodeColor("scoutHat", "1.0 0.98431372549 0.0 1.0");
}

//
// Brick supply regeneration mechanic.
function PlayerBuilder::onNewDatablock(%this, %obj)
{
    %this.super("onNewDatablock", %this, %obj);

    cancel(%obj.builderBrickSupplyRegenSchedule);
    %obj.builderBrickSupplyRegenSchedule = %this.schedule(1, "regenerateSupply", %obj);
}

function PlayerBuilder::regenerateSupply(%this, %obj)
{
    if(!isObject(%obj) || %obj.getState() $= "Dead" || %obj.Datablock.getID() != PlayerBuilder.getID())
    {
        return;
    }

    %builderBrickImage = builderBrickImage.getID();
    %maxBrickSupply = %this.maxBrickSupply;
    %brickRegenRate = %this.brickRegenRate;

    %mountedImage = %obj.getMountedImage(%builderBrickImage.mountPoint);
    if(%mountedImage !$= "" && %mountedImage == %builderBrickImage)
    {
        %invPosition = %obj.currTool;
    }
    else
    {
        %invPosition = %obj.hasItemInInventory(%builderBrickImage);
        if(%invPosition == -1)
        {
            return;
        }
    }

    //Increase the builder's available bricks to place.
    %supply = mClamp(%obj.getImageAttribute("supply", %invPosition) + %this.bricksPerRegen, 0, %maxBrickSupply);
    %obj.setImageAttribute("supply", %supply, %invPosition);

    //Raise the Builder's tool, if he has it equipped.
    %obj.setImageAmmo(%builderBrickImage.mountPoint, 0);

    //Play a sound, send a message telling the player they have new bricks to place.
    %client = %obj.client;
    if(%client)
    {
        %client.play2D("lego_click1_sound");
        %client.printFormatString("hint", "You gained some bricks! You now have" SPC %supply @ ".", %hintTime);
    }

    //We can't generate any more bricks at this time, don't schedule another redundant check.
    if(%supply >= %maxBrickSupply)
    {
        return;
    }

    //Make them generate more bricks in the future.
    %schedule = %this.schedule(%brickRegenRate, "regenerateSupply", %obj);
    cancel(%obj.builderBrickSupplyRegenSchedule);
    %obj.builderBrickSupplyRegenSchedule = %schedule;
    return %schedule;
}

//
// Builder's build tool, the meat.
//

//
// The projectile, ghost brick selection happens here.
datablock ProjectileData(builderBrickDeployProjectile : brickDeployProjectile)
{
	collideWithPlayers = 1;
};

function builderBrickDeployProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	if($Game::MissionCleaningUp)
	{
		return;
	}

    %client = %obj.client;
    %player = %client.player;
    if(!%player)
	{
		return;
	}

    %data = brick2x3Data;
    %angleID = getAngleIDFromPlayer(%player);
    %angle = %angleID + %data.orientationFix;
    if(%angle % 4 == 0)
    {
        %rot = "0 0 1 0";
    }
    else if(%angle % 4 == 1)
    {
        %rot = "0 0 -1" SPC (90 * $pi) / 180;
    }
    else if(%angle % 4 == 2)
    {
        %rot = "0 0 1" SPC (180 * $pi) / 180;
    }
    else if(%angle % 4 == 3)
    {
        %rot = "0 0 1" SPC (90 * $pi) / 180;
    }

    %tempBrick = %player.tempBrick;
	if(isObject(%tempBrick))
	{
		%posX = getWord(%pos, 0);
		%posY = getWord(%pos, 1);
		if(%data.brickSizeZ % 2 == 0)
		{
			%posZ = getWord(%pos, 2) + (%data.brickSizeZ / 2) * 0.2 + 0.05;
		}
		else
		{
			%posZ = (getWord(%pos, 2) + (%data.brickSizeZ / 2) * 0.2) - 0.05;
		}
		if(getWord(%normal, 2) < -0.9)
		{
			%posZ -= %data.brickSizeZ * 0.2;
		}

		%pos = %posX SPC %posY SPC %posZ;
		%tempBrick.setTransform(%pos SPC %rot);
	}
	else
	{
		%tempBrick = new fxDTSBrick()
		{
			dataBlock = %data;
			angleID = %angleID;
		};
        %client.brickGroup.add(%tempBrick);
        %player.tempBrick = %tempBrick;
        $Eventide_BuilderBrickGroup.add(%tempBrick);

		%posX = getWord(%pos, 0);
		%posY = getWord(%pos, 1);
		if(%data.brickSizeZ % 2 == 0)
		{
			%posZ = getWord(%pos, 2) + (%data.brickSizeZ / 2) * 0.2 + 0.05;
		}
		else
		{
			%posZ = (getWord(%pos, 2) + (%data.brickSizeZ / 2) * 0.2) - 0.05;
		}
		if(getWord(%normal, 2) < -0.9)
		{
			%posZ -= %data.brickSizeZ * 0.2;
		}

		%pos = %posX SPC %posY SPC %posZ;
		%tempBrick.setTransform(%pos SPC %rot);

        //Ripped from the game's default random brick color code.
        %randColor = getRandom(5);
        if(%randColor == 5)
        {
            %randColor += 2;
        }
        if(%randColor >= 2)
        {
            %randColor += 1;
        }
		%tempBrick.setColor(%randColor);
	}
}

//
// The ShapeBaseImageData itself, "ammo" detection happens here.
datablock ItemData(builderBrickItem)
{
    shapeFile = "base/data/shapes/brickWeapon.dts";

    uiName = "Builder's Tool";
    iconName = "base/client/ui/gglogo150";

	category = "Tools";
	className = "Weapon";

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = 1;

	doColorShift = 1;
	colorShiftColor = "0.647 0.647 0.647 1.000";

	image = builderBrickImage;
	canDrop = 1;
};

datablock ShapeBaseImageData(builderBrickImage : brickImage)
{
    item = builderBrickItem;
    doColorShift = builderBrickItem.doColorShift;
    colorShiftColor = builderBrickItem.colorShiftColor;

	Projectile = builderBrickDeployProjectile;
    StaticShape = staticBrick2x2;
	ghost = ghostBrick2x2;

    maxBuildHeight = 3;

    stateName[0] = "Activate";
    stateWaitForTimeout[0] = true;
    stateTimeOutValue[0] = 0.01;
	stateTransitionOnTimeout[0] = "SupplyCheck";

    //Check if we have any bricks to plant.
    stateName[1] = "SupplyCheck";
    stateScript[1] = "onSupplyCheck";
    stateAllowImageChange[1] = false;
    stateWaitForTimeout[1] = true;
    stateTimeOutValue[1] = 0.01;
    stateTransitionOnTimeout[1] = "SupplyRedirect";
    stateFire[1] = 0;
    stateEmitter[1] = "";

    //Redirect to another state based on what was set in the previous "CooldownCheck" state.
    stateName[2] = "SupplyRedirect";
    stateAllowImageChange[2] = false;
    stateTransitionOnAmmo[2] = "Cooldown";
    stateTransitionOnNoAmmo[2] = "Ready";

    //We have no bricks to plant, lower the item.
    stateName[3] = "Cooldown";
    stateScript[3] = "onCooldown";
    stateAllowImageChange[3] = true;
    ////The cooldown ended while the item was equipped, transition to the "Ready" state.
    stateTransitionOnNoAmmo[3] = "CooldownRevert";

    //The item is transitioning from "Cooldown" to "Ready", we need to raise the arm back up.
    stateName[4] = "CooldownRevert";
    stateScript[4] = "onCooldownRevert";
    stateAllowImageChange[4] = false;
    stateWaitForTimeout[4] = true;
    stateTimeOutValue[4] = 0.01;
    stateTransitionOnTimeout[4] = "Ready";

    //We are ready to plant a brick.
    stateName[5] = "Ready";
    stateScript[5] = "displaySupply";
	stateTransitionOnTriggerDown[5] = "Fire";
	stateAllowImageChange[5] = 1;

    //Attempt to spawn a ghost brick.
	stateName[6] = "Fire";
	stateScript[6] = "onFire";
	stateFire[6] = 1;
	stateAllowImageChange[6] = 1;
	stateTimeoutValue[6] = 0.25;
	stateTransitionOnTimeout[6] = "StopFire";
	stateEmitter[6] = brickTrailEmitter;
	stateEmitterTime[6] = 0.1;
	stateSequence[6] = "Fire";

	stateName[7] = "StopFire";
    stateScript[7] = "displaySupply";
	stateTransitionOnTriggerUp[7] = "SupplyCheck";
	stateAllowImageChange[7] = 1;
};

//
// Sequence callbacks.
function builderBrickImage::onSupplyCheck(%this, %obj)
{
    %supply = %obj.getImageAttribute("supply");
    if(%supply $= "")
    {
        %obj.setImageAttribute("supply", 0);
        %supply = 0;
    }

    //Rest the item's animation.
    fixArmReady(%obj);

    //If the player does not have any bricks to build with, put them on "cooldown".
    if(%supply <= 0)
    {
        %obj.setImageAmmo(%this.mountPoint, 1);
    }
    else
    {
        %obj.setImageAmmo(%this.mountPoint, 0);
    }
}

function builderBrickImage::onCooldown(%this, %obj)
{
    //Lower the item, it cannot be used.
    %obj.playThread(1, root);

    //Clear the ghost brick, if it exists.
    %tempBrick = %obj.tempBrick;
    if(isObject(%tempBrick))
    {
        %tempBrick.delete();
    }

    %client = %obj.client;
    if(%client)
    {
        %hintStyle = %this.getHintStyle();
        %hintTime = getTextStyle(%hintStyle).displayTime;
        %milliseconds = (%hintStyle !$= "") ? msFromS(%hintTime) : 6000;

        %lastItemUseTime = %obj.lastUseTime[%this.getName()];
        %currentTime = getSimTime();

        if((%lastItemUseTime + %milliseconds) < %currentTime)
        {
            %client.printFormatString("hint", "You don't have any bricks to put down!", %hintTime);
        }
    }
}

function builderBrickImage::onCooldownRevert(%this, %obj)
{
    //Raise the item back up, it's ready.
    fixArmReady(%obj);
}

function builderBrickImage::displaySupply(%this, %obj)
{
    %client = %obj.client;
    if(%client)
    {
        %client.printFormatString("hint", "Bricks left:" SPC %obj.getImageAttribute("supply"), 4);
    }
}

//
// Mount callbacks.
function builderBrickImage::onMount(%this, %obj, %slot)
{
    %tempBrick = %obj.tempBrick;
    if(isObject(%tempBrick))
    {
        %tempBrick.delete();
    }
    
    Parent::onMount(%this, %obj, %slot);
}

function builderBrickImage::onUnMount(%this, %obj, %slot)
{
    Parent::onUnMount(%this, %obj, %slot);

    %tempBrick = %obj.tempBrick;
    if(isObject(%tempBrick))
    {
        %tempBrick.delete();
    }
}

//
// Custom brick planting code for Builder's tool.
//

//
// Data structure for storing Builder's bricks.
$Eventide_BuilderBrickGroup = new SimGroup(BrickGroup_Builder);
$Eventide_BuilderBrickGroup.client = 0;
$Eventide_BuilderBrickGroup.name = "\c1BL_ID: 888888\c0";
$Eventide_BuilderBrickGroup.bl_id = 888888;
mainBrickGroup.add($Eventide_BuilderBrickGroup);

//
// Server command for clearing Builder's bricks.
function serverCmdClearBuilderBricks(%client)
{
    if(!%client.isAdmin && !%client.isSuperAdmin)
    {
        messageClient(%client, '', 'You must be an admin to use this command.');
		return;
    }

    %currentTime = getSimTime();

	if($Game::MissionCleaningUp)
	{
		messageClient(%client, '', 'Can\'t clear bricks during mission clean up');
		return;
	}
	if(%currentTime - %client.lastClearBricksTime < 5000)
	{
		return;
	}

	%brickGroup = $Eventide_BuilderBrickGroup;
	if(!isObject(%brickGroup))
	{
		return;
	}
	if(%brickGroup.getCount() <= 0)
	{
		return;
	}

	%client.lastClearBricksTime = %currentTime;

	MessageAll('MsgClearBricks', '\c3%1\c2 cleared all Builder bricks', %client.getPlayerName());
	%brickGroup.ChainDeleteAll();
}

//
// Package for clearing bricks, damaging bricks, and allowing them to plant.
package Player_Builder
{
    function ServerCmdPlantBrick(%client)
    {
        if($Game::MissionCleaningUp)
        {
            return 0;
        }

        %obj = %client.Player;
        if(!isObject(%obj))
        {
            return 0;
        }
        
        %mountedImage = %obj.getMountedImage(builderBrickImage.mountPoint);
        if(%mountedImage $= "" || %mountedImage != builderBrickImage.getID())
        {
            return Parent::ServerCmdPlantBrick(%client);
        }

        //Play the brick-planting animation.
        %obj.playThread(3, plant);

        //No brick supply? We can't plant no matter what.
        %supply = %obj.getImageAttribute("supply");
        if(%supply <= 0)
        {
            return 0;
        }

        %tempBrick = %obj.tempBrick;
        if(!isObject(%tempBrick))
        {
            return 0;
        }

        //Respect default brick count and building speed limits.
        if(getBrickCount() >= getBrickLimit())
        {
            messageClient(%client, 'MsgPlantError_Limit');
            return 0;
        }

        if(!%client.isAdmin && !%client.isSuperAdmin)
        {
            if($Server::MaxBricksPerSecond > 0)
            {
                %currTime = getSimTime();
                if (%client.bpsTime + 1000 < %currTime)
                {
                    %client.bpsCount = 0;
                    %client.bpsTime = %currTime;
                }
                if (%client.bpsCount >= $Server::MaxBricksPerSecond)
                {
                    return 0;
                }
            }
        }

        %tempBrickTrans = %tempBrick.getTransform();
        %tempBrickPos = posFromTransform(%tempBrickTrans, 0, 2);
        %brickData = %tempBrick.Datablock;

        //Respect default brick distance limits.
        %brickRadius = (mMax(%brickData.brickSizeX, %brickData.brickSizeY) * 0.5) / 2;
        $Pref::Server::TooFarDistance = mClampF($Pref::Server::TooFarDistance, 20, 99999);
        if(VectorDist(%tempBrickPos, %obj.position) > $Pref::Server::TooFarDistance + %brickRadius)
        {
            messageClient(%client, 'MsgPlantError_TooFar');
            return 0;
        }

        %plantBrick = new fxDTSBrick()
        {
            dataBlock = %brickData;
            position = %tempBrickTrans;

            stackBL_ID = %client.bl_id;
            client = %client;

            dontCollideAfterTrust = 0;
            isPlanted = 1;
            isBuilderBrick = 1;
        };
        %plantBrick.setTransform(%tempBrickTrans);
        %plantBrick.setColor(%tempBrick.getColorID());
        %plantBrick.setPrint(%tempBrick.getPrintID());

        %plantErrorCode = %plantBrick.plant();
        %plantBrick.trustCheckFinished();

        //Limit the height a Builder can build, to minimize out-of-bounds exploits.
        %bricksBelow = 0;
        %maxBuildHeight = (builderBrickImage.maxBuildHeight - 1);
        %downBrick = %plantBrick.getDownBrick(0);
        while(%downBrick != 0 && %downBrick.isBuilderBrick)
        {
            %bricksBelow++;

            if(%bricksBelow > %maxBuildHeight)
            {
                %plantErrorCode = 6;
                break;
            }

            %downBrick = %downBrick.getDownBrick(0);
        }

        //In case the player is quick enough to outrun the state machine of Builder's Tool, we also have this check.
        %supply = %obj.getImageAttribute("supply");
        if(%supply <= 0)
        {
            %plantErrorCode = -1;

            if(isObject(%tempBrick))
            {
                %tempBrick.delete();
            }
        }

        //Run through all the default plant errors, with the sixth added to limit the builder's build height.
        if(%plantErrorCode == 0)
        {
            //No errors, proceed with planting the brick.
            %client.brickGroup.add(%plantBrick);
            $Eventide_BuilderBrickGroup.add(%plantBrick);
            %client.undoStack.push(%plantBrick TAB "PLANT");
            %client.bpsCount++;

            ServerPlay3D(brickPlantSound, %tempBrickTrans);

            //Since we know the player is using this tool, we can just update their supply and UI right away.
            %obj.decrementImageAttribute("supply", 1);
            builderBrickImage.displaySupply(%obj);

            //In case the planter is a Builder, and they were sitting at max stored bricks, restart the regen process.
            %playerBuilderDatablock = PlayerBuilder.getID();
            if(%obj.Datablock.getID() == %playerBuilderDatablock)
            {
                %maxBrickSupply = %playerBuilderDatablock.maxBrickSupply;

                if(!isEventPending(%obj.builderBrickSupplyRegenSchedule))
                {
                    %obj.builderBrickSupplyRegenSchedule = %playerBuilderDatablock.schedule(%playerBuilderDatablock.brickRegenRate, "regenerateSupply", %obj);
                }

                if((%supply - 1) == 0)
                {
                    if(isObject(%tempBrick))
                    {
                        %tempBrick.delete();
                    }
                }
            }

            //Randomize the color of the next brick. Ripped from the game's default random brick color code.
            if(isObject(%tempBrick))
            {
                %randColor = getRandom(5);
                if(%randColor == 5)
                {
                    %randColor += 2;
                }
                if(%randColor >= 2)
                {
                    %randColor += 1;
                }
                %obj.tempBrick.setColor(%randColor);
            }
        }
        else if(%plantErrorCode == 1)
        {
            messageClient(%client, 'MsgPlantError_Overlap');
        }
        else if(%plantErrorCode == 2)
        {
            messageClient(%client, 'MsgPlantError_Float');
        }
        else if(%plantErrorCode == 3)
        {
            messageClient(%client, 'MsgPlantError_Stuck');
        }
        else if(%plantErrorCode == 4)
        {
            messageClient(%client, 'MsgPlantError_Unstable');
        }
        else if(%plantErrorCode == 5)
        {
            messageClient(%client, 'MsgPlantError_Buried');
        }
        else if(%plantErrorCode == 6)
        {
            messageClient(%client, 'MsgPlantError_TooFar');
        }
        else
        {
            messageClient(%client, 'MsgPlantError_Forbidden');
        }

        if(%plantErrorCode != 0)
        {
            %plantBrick.delete();
        }

        //We planted a brick, and it's valid. Render it.
        if(getBrickCount() <= 100 && getRayTracerProgress() <= -1 && getRayTracerProgress() < 0 && $Server::LAN == 0 && doesAllowConnections())
        {
            startRaytracer();
        }

        return %plantBrick;
    }

    function minigameCanDamage(%client, %victimObject)
    {
        if((%victimObject.getType() & $TypeMasks::FxBrickAlwaysObjectType) && %victimObject.isBuilderBrick && minigameCanDamage(%client, %victimObject.client))
        {
            return 1;
        }
        return Parent::minigameCanDamage(%client, %victimObject);
    }

    function fxDTSBrick::canExplode(%obj, %maxVolume, %maxFloatingVolume)
    {
        if(%obj.isBuilderBrick)
        {
            return 1;
        }
        return Parent::canExplode(%obj, %maxVolume, %maxFloatingVolume);
    }

    function fxDTSBrick::onBlownUp(%obj, %client, %player)
    {
        Parent::onBlownUp(%obj, %client, %player);
        if(%obj.isBuilderBrick && minigameCanDamage(%player, %obj.client))
        {
            %obj.isBroken = true;
            %position = %obj.position;

            serverPlay3D("brick_break1_sound", %position);
            transmitBrickExplosion(VectorSub(%position, "0 0 1"), 14, 1, 30000, %obj);
            %obj.schedule(30000, "delete");
        }
    }

    function MiniGameSO::endGame(%obj)
    {
        Parent::endGame(%obj);

        //Clear any of Builder's bricks after a minigame restart.
        $Eventide_BuilderBrickGroup.ChainDeleteAll();
    }

    function MinigameSO::Reset(%this, %client)
	{
		parent::Reset(%this, %client);
        
        $Eventide_BuilderBrickGroup.ChainDeleteAll();
	}
};
if(isPackage(Player_Builder))
{
    deactivatePackage(Player_Builder);
}
activatePackage(Player_Builder);