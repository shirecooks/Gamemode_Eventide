package Support_VehicleCallbacks
{
    function serverCmdNextSeat(%client)
    {
        %player = %client.player;
        %playerExists = isObject(%player);
        
        if(%playerExists)
        {
            %player.isSwitchingSeat = true;
        }

        parent::serverCmdNextSeat(%client);

        if(%playerExists)
        {
            %player.isSwitchingSeat = false;
        }
    }

    function serverCmdPrevSeat(%client)
    {
        %player = %client.player;
        %playerExists = isObject(%player);

        if(%playerExists)
        {
            %player.isSwitchingSeat = true;
        }

        parent::serverCmdPrevSeat(%client);

        if(%playerExists)
        {
            %player.isSwitchingSeat = false;
        }
    }

    function WheeledVehicleData::onCollision(%this, %obj, %col, %vec, %speed)
    {
        if (%obj.getDamageState() $= "Dead")
        {
            return;
        }
        if (%col.getDamagePercent() >= 1)
        {
            return;
        }
        %runOver = 0;
        if (isObject(%obj.client))
        {
            if (%col.client == %obj.client)
            {
                return;
            }
        }
        %canUse = 0;
        if (isObject(%obj.spawnBrick))
        {
            %vehicleOwner = findClientByBL_ID(%obj.spawnBrick.getGroup().bl_id);
        }
        else
        {
            %vehicleOwner = 0;
        }
        if (isObject(%vehicleOwner))
        {
            if (getTrustLevel(%col, %obj) >= $TrustLevel::RideVehicle)
            {
                %canUse = 1;
            }
        }
        else
        {
            %canUse = 1;
        }
        if (miniGameCanUse(%col, %obj) == 1)
        {
            %canUse = 1;
        }
        if (miniGameCanUse(%col, %obj) == 0)
        {
            %canUse = 0;
        }
        if (miniGameCanDamage(%col, %obj) == 1)
        {
            %canDamage = 1;
        }
        else
        {
            %canDamage = 0;
        }
        %minSpeed = mClampF(%this.minRunOverSpeed, $Game::DefaultMinRunOverSpeed, 999);
        if (!isObject(%obj.getControllingObject()))
        {
            %minSpeed += 2;
        }
        %relativeSpeed = VectorLen(VectorSub(%obj.getVelocity(), %col.getVelocity()));
        if (%col.getDataBlock().canRide && %this.rideAble && %this.nummountpoints > 0)
        {
            if (getSimTime() - %col.lastMountTime > $Game::MinMountTime)
            {
                %colZpos = getWord(%col.getPosition(), 2);
                %objZpos = getWord(%obj.getPosition(), 2);
                if (%colZpos > %objZpos + 0.2)
                {
                    if (%canUse)
                    {
                        for (%i = 0; %i < %this.nummountpoints; %i++)
                        {
                            %blockingObj = %obj.getMountNodeObject(%i);
                            if (isObject(%blockingObj))
                            {
                                %blockingObjData = %blockingObj.Datablock;
                                if (!%blockingObjData.rideAble)
                                {
                                    continue;
                                }
                                if (%blockingObj.getMountedObject(0))
                                {
                                    continue;
                                }
                                //Edit to add two checks for undrivable vehicles.
                                %blockingObj.mountObject(%col, 0);
                                if(%blockingObj.getControllingClient() == 0 && !%blockingObj.passengerOnly && !%blockingObjData.passengerOnly)
                                {
                                    %col.setControlObject(%blockingObj);
                                }
                            }
                            else
                            {
                                %obj.mountObject(%col, %i);
                                if (%i == 0)
                                {
                                    //Edit to add two checks for undrivable vehicles.
                                    if(%obj.getControllingClient() == 0 && !%obj.passengerOnly && !%this.passengerOnly)
                                    {
                                        %col.setControlObject(%obj);
                                    }
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        %ownerName = %obj.spawnBrick.getGroup().name;
                        %msg = %ownerName @ " does not trust you enough to do that";
                        if ($lastError == $LastError::Trust)
                        {
                            %msg = %ownerName @ " does not trust you enough to ride.";
                        }
                        else if ($lastError == $LastError::MiniGameDifferent)
                        {
                            if (isObject(%col.client.miniGame))
                            {
                                %msg = "This vehicle is not part of the mini-game.";
                            }
                            else
                            {
                                %msg = "This vehicle is part of a mini-game.";
                            }
                        }
                        else if ($lastError == $LastError::MiniGameNotYours)
                        {
                            %msg = "You do not own this vehicle.";
                        }
                        else if ($lastError == $LastError::NotInMiniGame)
                        {
                            %msg = "This vehicle is not part of the mini-game.";
                        }
                        commandToClient(%col.client, 'CenterPrint', %msg, 1);
                        %runOver = 1;
                    }
                }
                else
                {
                    %runOver = 1;
                }
            }
        }
        else
        {
            %runOver = 1;
        }
        if (%canDamage)
        {
            if (%runOver)
            {
                if (%col.getType() & $TypeMasks::PlayerObjectType)
                {
                    %vehicleSpeed = VectorLen(%obj.getVelocity());
                    if (%vehicleSpeed > %minSpeed)
                    {
                        %damageScale = %this.runOverDamageScale;
                        if (%damageScale $= "")
                        {
                            %damageScale = $Game::DefaultRunOverDamageScale;
                        }
                        %damageType = %this.damageType;
                        if (%damageType $= "")
                        {
                            %damageType = $DamageType::Vehicle;
                        }
                        %damageAmt = %vehicleSpeed * %damageScale;
                        %col.Damage(%obj, %pos, %damageAmt, %damageType);
                    }
                }
            }
            %pushScale = %this.runOverPushScale;
            if (%pushScale $= "")
            {
                %pushScale = $Game::DefaultRunOverPushScale;
            }
            %pushVec = %obj.getVelocity();
            %pushVec = VectorScale(%pushVec, %pushScale);
            %col.setVelocity(%pushVec);
        }
    }

    function Armor::onCollision(%this, %obj, %col, %vec, %speed)
    {
        if (%obj.getState() $= "Dead")
        {
            return;
        }
        if (%col.getDamagePercent() >= 1)
        {
            return;
        }
        %colClassName = %col.getClassName();
        if (%colClassName $= "Item")
        {
            %client = %obj.client;
            %colData = %col.getDataBlock();
            for (%i = 0; %i < %this.maxTools; %i++)
            {
                if (%obj.tool[%i] == %colData)
                {
                    return;
                }
            }
            %obj.pickup(%col);
        }
        else if (%colClassName $= "Player" || %colClassName $= "AIPlayer")
        {
            if (%col.getDataBlock().canRide && %this.rideAble && %this.nummountpoints > 0)
            {
                if (getSimTime() - %col.lastMountTime <= $Game::MinMountTime)
                {
                    return;
                }
                %colZpos = getWord(%col.getPosition(), 2);
                %objZpos = getWord(%obj.getPosition(), 2);
                if (%colZpos <= %objZpos + 0.2)
                {
                    return;
                }
                %canUse = 0;
                if (isObject(%obj.spawnBrick))
                {
                    %vehicleOwner = findClientByBL_ID(%obj.spawnBrick.getGroup().bl_id);
                }
                if (isObject(%vehicleOwner))
                {
                    if (getTrustLevel(%col, %obj) >= $TrustLevel::RideVehicle)
                    {
                        %canUse = 1;
                    }
                }
                else
                {
                    %canUse = 1;
                }
                if (miniGameCanUse(%col, %obj) == 1)
                {
                    %canUse = 1;
                }
                if (miniGameCanUse(%col, %obj) == 0)
                {
                    %canUse = 0;
                }
                if (!%canUse)
                {
                    if (!isObject(%obj.spawnBrick))
                    {
                        return;
                    }
                    %ownerName = %obj.spawnBrick.getGroup().name;
                    %msg = %ownerName @ " does not trust you enough to do that";
                    if ($lastError == $LastError::Trust)
                    {
                        %msg = %ownerName @ " does not trust you enough to ride.";
                    }
                    else if ($lastError == $LastError::MiniGameDifferent)
                    {
                        if (isObject(%col.client.miniGame))
                        {
                            %msg = "This vehicle is not part of the mini-game.";
                        }
                        else
                        {
                            %msg = "This vehicle is part of a mini-game.";
                        }
                    }
                    else if ($lastError == $LastError::MiniGameNotYours)
                    {
                        %msg = "You do not own this vehicle.";
                    }
                    else if ($lastError == $LastError::NotInMiniGame)
                    {
                        %msg = "This vehicle is not part of the mini-game.";
                    }
                    commandToClient(%col.client, 'CenterPrint', %msg, 1);
                    return;
                }
                for (%i = 0; %i < %this.nummountpoints; %i++)
                {
                    if (%this.mountNode[%i] $= "")
                    {
                        %mountNode = %i;
                    }
                    else
                    {
                        %mountNode = %this.mountNode[%i];
                    }
                    %blockingObj = %obj.getMountNodeObject(%mountNode);
                    if (isObject(%blockingObj))
                    {
                        %blockingObjData = %blockingObj.Datablock;
                        if (!%blockingObjData.rideAble)
                        {
                            continue;
                        }
                        if (%blockingObj.getMountedObject(0))
                        {
                            continue;
                        }
                        %blockingObj.mountObject(%col, 0);
                        //Edit to add two checks for undrivable vehicles.
                        if (%blockingObj.getControllingClient() == 0 && !%blockingObj.passengerOnly && !%blockingObjData.passengerOnly)
                        {
                            %col.setControlObject(%blockingObj);
                        }
                        %col.setTransform("0 0 0 0 0 1 0");
                        %col.setActionThread(root, 0);
                    }
                    else
                    {
                        %obj.mountObject(%col, %mountNode);
                        %col.setActionThread(root, 0);
                        if (%i == 0)
                        {
                            if (%obj.isHoleBot)
                            {
                                if (%obj.controlOnMount)
                                {
                                    %col.setControlObject(%obj);
                                }
                            }
                            else if (%obj.getControllingClient() == 0 && !%obj.passengerOnly && !%this.passengerOnly)
                            {
                                //Edit to add two checks for undrivable vehicles.
                                %col.setControlObject(%obj);
                            }
                            if (isObject(%obj.spawnBrick))
                            {
                                %obj.lastControllingClient = %col;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }

    function Armor::onMount(%this, %obj, %vehicle, %node)
    {
        %vehicleDatablock = %vehicle.Datablock;

        //Edit to add two checks for undrivable vehicles.
        if(%node == 0 && !%vehicle.passengerOnly && !%vehicleDatablock.passengerOnly)
        {
            if(%vehicle.isHoleBot)
            {
                if(%vehicle.controlOnMount)
                {
                    %obj.setControlObject(%vehicle);
                    %vehicle.lastDrivingClient = %obj.client;
                }
            }
            else if(%vehicle.getControllingClient() == 0)
            {
                %obj.setControlObject(%vehicle);
                %vehicle.lastDrivingClient = %obj.client;
            }
        }
        else
        {
            %obj.setControlObject(%obj);
        }

        %obj.setTransform("0 0 0 0 0 1 0");
        %obj.playThread(0, %vehicleDatablock.mountThread[%node]);

        ServerPlay3D(playerMountSound, %obj.getPosition());

        if(%vehicleDatablock.lookUpLimit !$= "")
        {
            %obj.setLookLimits(%vehicleDatablock.lookUpLimit, %vehicleDatablock.lookDownLimit);
        }

        //Bookkeeping for the `onSwitchSeat` callback.
        if(%obj.isSwitchingSeat)
        {
            %vehicleDatablock.onSwitchSeat(%obj, %vehicle, %node);
        }
        else
        {
            %vehicleDatablock.onEnter(%vehicle, %obj, %node);
        }
    }

    function Armor::onUnMount(%this, %obj, %vehicle, %node)
    {
        parent::onUnMount(%this, %obj, %vehicle, %node);
        if(isObject(%vehicle) && !%obj.isSwitchingSeat)
        {
            %vehicle.Datablock.onLeave(%vehicle, %obj, %node);
        }
    }

    function VehicleData::onEnter(%this, %obj, %node)
    {

    }

    function VehicleData::onLeave(%this, %obj, %node)
    {

    }

    function VehicleData::onSwitchSeat(%this, %obj, %node)
    {

    }

    //
    // Some players/bots are mountable, so we need functions for them too.
    function Armor::onEnter(%this, %obj, %node)
    {

    }

    function Armor::onLeave(%this, %obj, %node)
    {

    }

    function Armor::onSwitchSeat(%this, %obj, %node)
    {

    }

};
activatePackage(Support_VehicleCallbacks);

function Vehicle::getEmptyMountPoint(%this)
{
    %numMountPoints = %this.Datablock.numMountPoints;
    if(%this.getMountedObjectCount() >= %numMountPoints)
    {
        //There are no available seats.
        return -1;
    }

    //A seat is available, but we need to find it.
    for(%i = 0; %i < %numMountPoints; %i++)
	{
        %mountedObject = %this.getMountNodeObject(%i);
		if(!isObject(%mountedObject))
        {
            return %i;
        }
	}
}

function Vehicle::setVehiclePowered(%obj, %bool)
{
	for(%i = 0; %i < %obj.Datablock.numWheels; %i++)
	{
		%obj.setWheelPowered(%i, %bool);
        %obj.setWheelSteering(%i, %bool);
	}
	%obj.poweredTime = getSimTime();
}