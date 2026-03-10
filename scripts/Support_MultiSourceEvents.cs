//----------------------------------------------------------------------
// Title:   Support_MultiSourceEvents
// Author:  Greek2me
// Version: 1
// Updated: December 30, 2014
//----------------------------------------------------------------------
// Create and call events that may be located on many bricks. An example
// is the onMiniGameReset event.
//----------------------------------------------------------------------
// Include this code in your own scripts as an *individual file*
// called "Support_MultiSourceEvents.cs". Do not modify this code.
//----------------------------------------------------------------------

if($Support_MultiSourceEvents::Version > 1 && !$Debug)
	return;
$Support_MultiSourceEvents::Version = 1;

//Processes an event which occurs on multiple sources, normally bricks. 
//@param	string inputEvent	The name of the input event.
//@param	GameConnection client	The client that activated the event.
//@param	ScriptObject minigame	Optional. Only bricks within this minigame will be processed.
function processMultiSourceInputEvent(%inputEvent, %client, %minigame)
{
	%group = "multiSourceEventGroup" @ inputEvent_GetInputEventIdx(%inputEvent);

	if(!isObject(%group))
		return;

	for(%i = 0; %i < %group.getCount(); %i ++)
	{
		%obj = %group.getObject(%i);

		if(!%minigame || %minigame == getMinigameFromObject(%obj))
		{
			$InputTarget_["Self"] = %obj;
			%obj.processInputEvent(%inputEvent, %client);
		}
	}
}

//Registers a multiple-source input event. Use this in place of registerInputEvent.
//@param	string class	The object class that this event is far.
//@param	string inputEvent	The name of the input event.
//@param	string targets	A tab-delimited list of targets. See the link below.
//@link	http://forum.blockland.us/index.php?topic=40631.0
function registerMultiSourceInputEvent(%class, %inputEvent, %targets, %adminOnly)
{
	registerInputEvent(%class, %inputEvent, %targets, %adminOnly);

	//for whatever reason, this doesn't take a class parameter
	%inputEventIdx = inputEvent_GetInputEventIdx(%inputEvent);
	
	if(%inputEventIdx != -1)
		$InputEvent_MultiSource[%class, %inputEventIdx] = true;
}

package Support_MultiSourceEvents
{
	function serverCmdAddEvent(%client, %enabled, %inputEventIdx, %delay, %targetIdx, %namedTargetNameIdx, %outputEventIdx, %par1, %par2, %par3, %par4)
	{
		%obj = %client.wrenchBrick;
		%class = %obj.getClassName();

		parent::serverCmdAddEvent(%client, %enabled, %inputEventIdx, %delay, %targetIdx, %namedTargetNameIdx, %outputEventIdx, %par1, %par2, %par3, %par4);

		echo("Adding event.");
		if($InputEvent_MultiSource[%class, %inputEventIdx])
		{
			%group = "multiSourceEventGroup" @ %inputEventIdx;
			if(!isObject(%group))
			{
				new SimSet(%group);
				missionCleanup.add(%group);
			}
			echo(%group);
			%group.add(%obj);
		}
	}

	function SimObject::clearEvents(%this)
	{
		%class = %this.getClassName();
		for(%i = 0; %i < %this.numEvents; %i ++)
		{
			%inputEventIdx = %this.eventInputIdx[%i];
			if(!%removed[%inputEventIdx] && $InputEvent_MultiSource[%class, %inputEventIdx])
			{
				%removed[%inputEventIdx] = true;
				%group = "multiSourceEventGroup" @ %inputEventIdx;
				%group.remove(%this);
			}
		}
		return parent::clearEvents(%this);
	}
};
activatePackage(Support_MultiSourceEvents);

//
// Robbinson Block addition: optimize the default MinigameSO::Reset function for less slowdowns and crashes.
//

//
// Tag the mainBrickGroup as it is created, so we can determine when new brick groups are created.
package MainBrickGroupMarker
{
	function SimGroup::add(%this, %obj)
	{
		parent::add(%this, %obj);
		if(%this.getName() $= "mainBrickGroup")
		{
			%this.isMainBrickGroup = true;
			deactivatePackage(MainBrickGroupMarker);
		}
	}
};
if(isPackage(MainBrickGroupMarker))
{
	deactivatePackage(MainBrickGroupMarker);
}
activatePackage(MainBrickGroupMarker);

//
// Logic for spawn sets.
function VehicleSpawnSet::reset(%this)
{
	%vehicleSpawnCount = %this.getCount();
	for(%i = 0; %i < %vehicleSpawnCount; %i++)
	{
		%this.getObject(%i).spawnVehicle(0);
	}
}

function ItemSpawnSet::reset(%this)
{
	%itemSpawnCount = %this.getCount();
	for(%i = 0; %i < %itemSpawnCount; %i++)
	{
		%this.getObject(%i).Item.fadeIn(0);
	}
}

function SimGroup::initSpawnSets(%this)
{
	%this.isBrickGroup = true;

	if(%this.vehicleSpawnSet $= "")
	{
		%this.vehicleSpawnSet = new SimSet()
		{
			class = VehicleSpawnSet;
		};
	}
	if(%this.itemSpawnSet $= "")
	{
		%this.itemSpawnSet = new SimSet()
		{
			class = ItemSpawnSet;
		};
	}
}

//Stub function to prevent console errors.
function SimGroup::onRemove(%this)
{
	
}

//
// Package for loading bricks, adding or removing bricks from spawn sets.
// Also, using the spawn sets to expedite minigame resets.
package Script_MinigameAntiLag
{
	function SimGroup::add(%this, %obj)
	{
		parent::add(%this, %obj);
		if(%this.isMainBrickGroup)
		{
			//A new brick group is being created, add spawn sets to it for tracking vehicles and items.
			%obj.initSpawnSets();
		}
	}

	function SimGroup::onRemove(%this)
	{
		if(%this.isBrickGroup)
		{
			//A brick group is being deleted, take down the spawn sets with it.
			%itemSet = %this.itemSpawnSet;
			if(%itemSet !$= "")
			{
				%itemSet.delete();
			}

			%vehicleSet = %this.vehicleSpawnSet;
			if(%vehicleSet !$= "")
			{
				%vehicleSet.delete();
			}
		}
		
		parent::onRemove(%this);
	}

	function fxDTSBrickData::addBrickToSpawnSet(%this, %obj)
	{
		if(!isObject(%obj))
		{
			return;
		}

		%brickGroup = %obj.getGroup();
		switch$(%this.brickType)
		{
			case $BRICK_TYPE::VEHICLESPAWN:
				%brickGroup.vehicleSpawnSet.add(%obj);
			default:
				return;
		}
	}

	function fxDTSBrickData::onAdd(%this, %obj)
	{
		parent::onAdd(%this, %obj);

		if(!%obj.isPlanted || %this.brickType != $BRICK_TYPE::VEHICLESPAWN)
		{
			return;
		}

		//Bricks aren't added to a brickgroup until after the current frame completes, so we need to wait.
		%this.schedule(1, addBrickToSpawnSet, %obj);
	}

	function fxDTSBrickData::onRemove(%this, %obj)
	{
		if(%obj.isPlanted && %this.brickType == $BRICK_TYPE::VEHICLESPAWN)
		{
			%obj.getGroup().vehicleSpawnSet.remove(%obj);
		}

		parent::onRemove(%this, %obj);
	}

	function fxDTSBrick::setItem(%obj, %data, %client)
	{
		parent::setItem(%obj, %data, %client);

		%brickGroup = %obj.getGroup();
		%itemSet = %brickGroup.itemSpawnSet;

		%hasItemSpawn = (%data != 0);
		%isInSet = %itemSet.isMember(%obj);
		if(!%hasItemSpawn && %isInSet)
		{
			%brickGroup.itemSpawnSet.remove(%obj);
		}
		else if(!%isInSet)
		{
			%brickGroup.itemSpawnSet.add(%obj);
		}
	}

	function MiniGameSO::Reset(%obj, %client)
	{
		if(%client $= "")
		{
			return;
		}
		if(%client > 0)
		{
			if(%client.miniGame != %obj)
			{
				return;
			}
		}
		%currTime = getSimTime();
		if(%obj.lastResetTime + 5000 > %currTime)
		{
			return;
		}
		%numMinigameMembers = %obj.numMembers;
		%obj.lastResetTime = %currTime;
		cancel(%obj.timeLimitSchedule);
		%obj.timeLimitSchedule = 0;
		cancel(%obj.resetSchedule);
		%obj.resetSchedule = 0;
		%mask = $TypeMasks::PlayerObjectType | $TypeMasks::ProjectileObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::CorpseObjectType;
		if($Server::LAN)
		{
			%quotaObject = GlobalQuota;
			if(isObject(%quotaObject))
			{
				cancelQuotaSchedules(%quotaObject);
				%quotaObject.killObjects(%mask);
			}

			processMultiSourceInputEvent("onMiniGameReset", %client, %obj);
			%brickGroupCount = mainBrickGroup.getCount();
			for(%i = 0; %i < %brickGroupCount; %i++)
			{
				%brickGroup = mainBrickGroup.getObject(%i);
				ItemSpawnSet::reset(%brickGroup.itemSpawnSet);
				VehicleSpawnSet::reset(%brickGroup.vehicleSpawnSet);
			}
		}
		else
		{
			if(%obj.UseAllPlayersBricks)
			{
				for(%i = 0; %i < %numMinigameMembers; %i++)
				{
					%cl = %obj.member[%i];

					%brickGroup = %cl.brickGroup;
					%quotaObject = %brickGroup.QuotaObject;
					if(isObject(%quotaObject))
					{
						cancelQuotaSchedules(%quotaObject);
						%quotaObject.killObjects(%mask);
					}

					processMultiSourceInputEvent("onMiniGameReset", %cl, %obj);
					ItemSpawnSet::reset(%brickGroup.itemSpawnSet);
					VehicleSpawnSet::reset(%brickGroup.vehicleSpawnSet);
				}
			}
			else if(isObject(%owner = %obj.owner))
			{
				%brickGroup = %owner.brickGroup;
				%quotaObject = %brickGroup.QuotaObject;
				if(isObject(%quotaObject))
				{
					cancelQuotaSchedules(%quotaObject);
					%quotaObject.killObjects(%mask);
				}

				processMultiSourceInputEvent("onMiniGameReset", %owner, %obj);
				ItemSpawnSet::reset(%brickGroup.itemSpawnSet);
				VehicleSpawnSet::reset(%brickGroup.vehicleSpawnSet);
			}
			if(%obj == $DefaultMiniGame)
			{
				%brickGroup = BrickGroup_888888;
				%quotaObject = %brickGroup.QuotaObject;
				if(isObject(%quotaObject))
				{
					cancelQuotaSchedules(%quotaObject);
					%quotaObject.killObjects(%mask);
				}

				processMultiSourceInputEvent("onMiniGameReset", "", %obj);
				ItemSpawnSet::reset(%brickGroup.itemSpawnSet);
				VehicleSpawnSet::reset(%brickGroup.vehicleSpawnSet);
			}
		}

		for(%i = 0; %i < %numMinigameMembers; %i++)
		{
			%cl = %obj.member[%i];
			%cl.setScore(0);
			if($Pref::Server::ClearEventsOnMinigameChange)
			{
				%cl.ClearEventSchedules();
			}
			//%cl.resetVehicles();
			%mask = $TypeMasks::PlayerObjectType | $TypeMasks::ProjectileObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::CorpseObjectType;
			%cl.ClearEventObjects(%mask);
			%cl.InstantRespawn();
			if(isObject(%client))
			{
				commandToClient(%cl, 'CenterPrint', "\c3" @ %client.getPlayerName() @ "\c5 reset the mini-game", 1);
			}
			else
			{
				commandToClient(%cl, 'CenterPrint', "\c5Mini-game reset", 1);
			}
		}
		if(%obj.TimeLimit > 0)
		{
			%obj.timeLimitTick(1);
		}
	}
};
if(isPackage(Script_MinigameAntiLag))
{
	deactivatePackage(Script_MinigameAntiLag);
}
activatePackage(Script_MinigameAntiLag);

//
// Any leftover logic needed.
unRegisterInputEvent("fxDTSBrick", "onMiniGameReset");
registerMultiSourceInputEvent(FxDTSBrick, "onMiniGameReset", "Self fxDTSBrick" TAB "MiniGame MiniGame", 1);