//a number specifying how many always visible billboards are loaded per client
$Example::ClientAVBillboardCount = 12;
// Example always visible billboard light
datablock fxLightData(downedAVBillboard : DefaultAVBillboard)
{
	flarebitmap = "add-ons/gamemode_eventide/modules/misc/icons/icon_downed.png";
	uiName = "downedAVBillboard";

	ConstantSize = 5;
	flareColor = "1 1 1 1";
};

// returns a new MountGroup object with starting datablocks or false if failed
function MountGroup_Create(%db,%num,%slot)
{
	if(%slot < 0)
	{
		// Slot has to be greater than 0
		return false;
	}

	if(%num <= 0)
	{
		// The required number of mounts is less than 1. Doesn't make any sense to continue.
		return false;
	}

	if(!isFunction(%db.ClassName,"make"))
	{
		// The mount datablock doesn't have a make function
		return false;
	}

	// Create our group and add the number of new mounts to it
	%o = new ScriptGroup(){class = "MountGroup";slot = %slot;};

	for(%i = 0; %i < %num; %i++)
	{
		%mount = %db.make();
		%mount.setScopeAlways();
		%o.add(%mount);
	}	

	return %o;
}

// Mounts a mount from the mountgroup to this player. Returns true if successful
function MountGroup::Mount(%o,%player)
{
	// Loops through the group looking for any unmounted mounts
	%mount = "";
	%count = %o.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%mount = %o.getObject(%i);
		if(!%mount.getObjectMount())
		{
			// %mount is not mounted. Break
			break;
		}
	}

	// See if the loop completed without breaking.
	if(%i >= %count)
	{
		// None are availible
		return false;
	}

	// MountObject will return if it is successful
	return %player.mountObject(%mount,%o.slot);
}

// Add a Always Visible Billboard to the player using this mount group
function MountGroup::AVBillboard(%o,%player,%light,%tag)
{
	// See if the playe already has a mount
	%mount = %player.getMountNodeObject(%o.slot);
	if(!%mount)
	{
		if(!%o.mount(%player))
		{
			// Mounting a new mount failed return
			return false;
		}
	}
 	else if(%mount.getGroup() != %o)
	{
		// The mount is not part of our group return
		// Mostly a sanity check
		return false;
	}

	// Loop through all of the clients and add the billboard for them
	%group = ClientGroup;
	%count = %group.getCount();
	%mount = %player.getMountNodeObject(%o.slot);
	for(%i = 0; %i < %count; %i++)
	{
		%player = %group.getObject(%i).player;
		if(isObject(%player) && %player.getDataBlock().Hunter) //skip if hunter
		{
			continue;
		}
		%avGroup = %group.getObject(%i).AVBillboardGroup;
		// Appending the object id to the tag so future clears only effect their own group
		%bb = BillboardMount_AddAVBillboard(%mount, %avGroup, %light, %o @ "_" @ %tag);
	}
}

// Clear Always Visible Billboards with this tag within the mount group
function MountGroup::clearAVBillboards(%o,%player,%tag)
{
	// See if the playe already has a mount
	%mount = %player.getMountNodeObject(%o.slot);
	if(!%mount)
	{
		if(!%o.mount(%player))
		{
			// Mounting a new mount failed return
			return false;
		}
	}
 	else if(%mount.getGroup() != %o)
	{
		// The mount is not part of our group return
		// Mostly a sanity check
		return false;
	}

	// Loop through all of the clients and clear the billboard for them
	%group = ClientGroup;
	%count = %group.getCount();
	%mount = %player.getMountNodeObject(%o.slot);
	for(%i = 0; %i < %count; %i++)
	{
		%group.getObject(%i).AVBillboardGroup.Clear(%o @ "_" @ %tag);
	}
}

// Torque callback called when the object is deleted
function MountGroup::onRemove(%o)
{
	// Deletes all of the mounts in the group
	%o.DeleteAll();
}

// Package to create always visible billboard groups
package MountGroup_Billboards
{
	function GameConnection::onClientEnterGame(%this)
	{
		%this.AVBillboardGroup = AVBillboardGroup_Make();
		%this.AVBillboardGroup.schedule(1000, load, %this, $Example::ClientAVBillboardCount);

		return parent::onClientEnterGame(%this);
	}
	function GameConnection::onClientLeaveGame(%this)
	{
		if(isObject(%this.AVBillboardGroup))
			%this.AVBillboardGroup.delete();

		return parent::onClientLeaveGame(%this);
	}
};
activatePackage(MountGroup_Billboards);

$Eventide::BillboardMounts = MountGroup_Create(OverheadBillboardMount, 12, 5);