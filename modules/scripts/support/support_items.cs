package Eventide_Items
{
	function Armor::onCollision(%this, %obj, %col, %vec, %speed)
	{
		// Return to the parent function if any of these conditions are true
		if(!isObject(getMinigameFromObject(%obj)) || !%obj.getDataBlock().isEventideModel || %obj.getState() $= "Dead" || %col.getClassName() !$= "Item") 
		{
			Parent::onCollision(%this, %obj, %col, %vec, %speed);
			return;
		}
					
		%obj.pickup(%col);
	}

	function ItemData::onPickup(%this, %obj, %user, %amount)
	{
		if(!%obj.getDataBlock().isEventideModel && !isObject(getMinigameFromObject(%obj))) 
		{
			Parent::onPickup(%this, %obj, %user, %amount);
			return;
		}

		if (!%obj.canPickup || !isObject(%client = %user.client) || getSimTime() - %client.lastF8Time < 5000)
		{
			return;
		}		

		%inventoryToolCount = (%user.hoarderToolCount) ? %user.hoarderToolCount : %user.getDataBlock().maxTools;
		%canUse = miniGameCanUse(%user, %obj) ? 1 : 0;

		if (!%canUse)
		{
			if (isObject(%obj.spawnBrick)) %ownerName = %obj.spawnBrick.getGroup().name;
			%msg = %ownerName @ " does not trust you enough to use this item.";
	
			if ($lastError == $LastError::Trust) 
			%msg = %ownerName @ " does not trust you enough to use this item.";

			else if ($lastError == $LastError::MiniGameDifferent) 
			%msg = isObject(%client.miniGame) ? "This item is not part of the mini-game." : "This item is part of a mini-game.";

			else if ($lastError == $LastError::MiniGameNotYours) 
			%msg = "You do not own this item.";

			else if ($lastError == $LastError::NotInMiniGame) 
			%msg = "This item is not part of the mini-game.";
	
			commandToClient(%client, 'CenterPrint', %msg, 1);
			return;
		}

		%freeslot = -1;
		for (%i = 0; %i < %inventoryToolCount; %i++)		
		if (%user.tool[%i] == 0)
		{
			%freeslot = %i;
			break;
		}
		
		if (%freeslot != -1)
		{
			if (%obj.isStatic()) %obj.Respawn();			
			else %obj.delete();
			
			%user.tool[%freeslot] = %this;
			messageClient(%client, 'MsgItemPickup', '', %freeslot, %this.getId());
			
			return 1;
		}
	}	
	
	function Player::Pickup(%obj,%item)
	{		
		if (!%obj.getDataBlock().isEventideModel) 
		{
			return Parent::Pickup(%obj,%item);
		}

		// Skinwalker players and killers can't pick up items, or if the item can't be picked up anyway
		if (%obj.isSkinwalker || %obj.getDataBlock().isKiller || !%item.canPickup)
		{
			return;
		}

		// Hoarder class support
		%inventoryToolCount = (%obj.hoarderToolCount) ? %obj.hoarderToolCount : %obj.getDataBlock().maxTools;

		// Check if the player already has the item
		for (%i = 0; %i < %inventoryToolCount; %i++)
		if (%item.getDataBlock() == %obj.tool[%i])
		{
			if(!%item.getDataBlock().image.isRitual)
			{
				return;
			}			
		}

		// Check for an available slot in the inventory
		for (%i = 0; %i < %inventoryToolCount; %i++)
		if (!isObject(%obj.tool[%i])) 
		{
			%obj.tool[%i] = %item.getDataBlock();
			messageClient(%obj.client, 'MsgItemPickup', '', %i, %item.getDataBlock());
			if(isObject(%item.spawnBrick)) %item.spawnBrick.setEmitter();
			%item.delete();
			return;
		}

		// Return if no slots are available (inventory is full)
		return;
	}
	
	function ItemData::onAdd(%this, %obj)	
	{
		Parent::onAdd(%this,%obj);

		if(!isObject(Eventide_MinigameGroup)) 
		{
			missionCleanUp.add(new SimGroup(Eventide_MinigameGroup));
		}
		Eventide_MinigameGroup.add(%obj);
	}

	function Item::schedulePop(%obj)
	{		
		// Do not continue if there is a minigame going on, the item should not disappear
		if(MiniGameGroup.getCount() || (isObject(Slayer_MiniGameHandlerSG) && Slayer_MiniGameHandlerSG.getCount()))
		{
			return;
		}
		Parent::schedulePop(%obj);				
	}
};

// In case the package is already activated, deactivate it first before reactivating it
if(isPackage(Eventide_Items)) deactivatePackage(Eventide_Items);
activatePackage(Eventide_Items);