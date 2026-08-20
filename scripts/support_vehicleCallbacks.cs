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

    function Armor::onMount(%this, %obj, %vehicle, %node)
    {
        parent::onMount(%this, %obj, %vehicle, %node);
        if(%obj.isSwitchingSeat)
        {
            %vehicle.Datablock.onSwitchSeat(%obj, %vehicle, %node);
        }
        else
        {
            %vehicle.Datablock.onEnter(%vehicle, %obj, %node);
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
};
activatePackage(Support_VehicleCallbacks);

function Vehicle::getEmptyMountPoint(%this)
{
    %numMountPoints = %this.Datablock.numMountPoints;
    for(%i = 0; %i < %numMountPoints; %i++)
	{
        %mountedObject = %this.getMountNodeObject(%i);
		if(!isObject(%mountedObject))
        {
            return %i;
        }
	}
    return -1;
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