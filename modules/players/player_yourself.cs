//
// Datablock.
//

datablock PlayerData(PlayerYourself : PlayerKid) 
{
	uiName = "Yourself Player";

	killerChaseLvl1Music = "musicData_Eventide_YourselfNear";
	killerChaseLvl2Music = "musicData_Eventide_YourselfChase";

	killernearsound = "";
	killernearsoundamount = 1;

    killertauntsound = "";
    killertauntsoundamount = 1;

	killerfoundvictimsound = "";
	killerfoundvictimsoundamount = 1;

    killerlostvictimsound = "";
	killerlostvictimsoundamount = 1;

    killerattackedsound = "";
	killerattackedsoundamount = 1;

	killerpainsound = "";
	killerpainsoundamount = 1;

	killerwinsound = "";
    killerwinsoundamount = 1;

    killerlosesound = "";
    killerlosesoundamount = 1;
};

function PlayerYourself::onRemove(%this, %obj)
{
    if(isObject(%obj.incapsAchieved))
    {
        %obj.incapsAchieved.delete();
    }
    if(isObject(%obj.threatsReceived))
    {
        %obj.threatsReceived.delete();
    }
	if(isObject(%obj.yourselfTrapGroup))
    {
        %obj.yourselfTrapGroup.delete();
    }
    parent::onRemove(%this, %obj);
}

//
// Appearance and initialization.
//

function PlayerYourself::onNewDatablock(%this,%obj)
{
	//Face system functionality.
	%obj.createFaceConfig($Eventide_FacePacks["kid"]);
	%obj.faceConfig.setFaceAttribute("Attack", "length", 500);
	%obj.faceConfig.setFaceAttribute("Pain", "length", 1000);

	//Everything else.
	Parent::onNewDatablock(%this, %obj);
	%obj.setScale("1 1 1");
	%obj.mountImage("kidsHammerImage", $RightHandSlot);
	%obj.gazeTickRate = %this.gazeTickRate;
	%obj.isTeleportReady = false;
	%obj.KidGaze();
}

function PlayerYourself::EventideAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");
	%obj.unHideNode((%tempclient.chest 	? 	"femChest" : "chest"));	
	%obj.unHideNode((%tempclient.rhand 	? 	"rhook" : "rhand"));
	%obj.unHideNode((%tempclient.lhand 	? 	"lhook" : "lhand"));
	%obj.unHideNode((%tempclient.rarm 	? 	"rarmSlim" : "rarm"));
	%obj.unHideNode((%tempclient.larm 	? 	"larmSlim" : "larm"));
	%obj.unHideNode("headskin");
	
		//Packs
	if ($pack[%tempclient.pack] !$= "none")
	{
		%obj.unHideNode($pack[%tempclient.pack]);
		%obj.setNodeColor($pack[%tempclient.pack],%tempclient.packColor);
	}
	if ($secondPack[%tempclient.secondPack] !$= "none")
	{
		%obj.unHideNode($secondPack[%tempclient.secondPack]);
		%obj.setNodeColor($secondPack[%tempclient.secondPack],%tempclient.secondPackColor);
	}

	//Hats
	%hat = $HatMod::save::wornHat[%tempclient.bl_id];
	if(isHat(%hat))
	{
		//Hatmod support.
		%obj.mountHat(%hat);
	}
	else if (%tempclient.hat)
	{
		//Put on any default hats they may be wearing.
		%hatName = $hat[%tempclient.hat];
		%tempclient.hatString = %hatName;
		
		// Only check if it's the first hat
		if (%tempclient.hat == 1)
		{
			%newhat = (%tempclient.accent ? "helmet" : "hoodie1");
			%obj.unHideNode(%newhat);
			%obj.setNodeColor(%newhat,%tempclient.hatColor);
		}
		else
		{
			%obj.unHideNode(%hatName);
			%obj.setNodeColor(%hatName,%tempclient.hatColor);
		}			
	}
	
	//Legs
	if (%tempclient.hip) %obj.unHideNode("skirt");
	else
	{
		%obj.unHideNode("pants");
		%obj.unHideNode((%tempclient.rleg ? "rpeg" : "rshoe"));
		%obj.unHideNode((%tempclient.lleg ? "lpeg" : "lshoe"));
	}

	%obj.setHeadUp((%tempclient.pack+%tempclient.secondPack));

	//Set blood colors.
	if (%obj.bloody["lshoe"]) %obj.unHideNode("lshoe_blood");
	if (%obj.bloody["rshoe"]) %obj.unHideNode("rshoe_blood");
	if (%obj.bloody["lhand"]) %obj.unHideNode("lhand_blood");
	if (%obj.bloody["rhand"]) %obj.unHideNode("rhand_blood");
	if (%obj.bloody["chest_front"]) %obj.unHideNode((%tempclient.chest ? "fem" : "") @ "chest_blood_front");
	if (%obj.bloody["chest_back"]) %obj.unHideNode((%tempclient.chest ? "fem" : "") @ "chest_blood_back");
	
	// Set node colors
	%obj.setNodeColor("headskin",%tempclient.headColor);	
	%obj.setNodeColor("chest",%tempclient.chestColor);
	%obj.setNodeColor("femChest",%tempclient.chestColor);
	%obj.setNodeColor("pants",%tempclient.hipColor);
	%obj.setNodeColor("skirt",%tempclient.hipColor);	
	%obj.setNodeColor("rarm",%tempclient.rarmColor);
	%obj.setNodeColor("larm",%tempclient.larmColor);
	%obj.setNodeColor("rarmSlim",%tempclient.rarmColor);
	%obj.setNodeColor("larmSlim",%tempclient.larmColor);
	%obj.setNodeColor("rhand",%tempclient.rhandColor);
	%obj.setNodeColor("lhand",%tempclient.lhandColor);
	%obj.setNodeColor("rhook",%tempclient.rhandColor);
	%obj.setNodeColor("lhook",%tempclient.lhandColor);	
	%obj.setNodeColor("rshoe",%tempclient.rlegColor);
	%obj.setNodeColor("lshoe",%tempclient.llegColor);
	%obj.setNodeColor("rpeg",%tempclient.rlegColor);
	%obj.setNodeColor("lpeg",%tempclient.llegColor);
}

function PlayerKidTrap::tick(%this, %obj)
{
	%killer = %obj.killer;

	if(!isObject(%obj))
	{
		//The tripmine no longer exists, stop.
		return;
	}
	else if(!isObject(%killer) || %killer.getState() $= "Dead")
	{
		//No killer? No point. Delete yourself now.
		%obj.delete();
		return;
	}
	else if(!%killer.isTeleportReady)
	{
		//If the Kid is not ready to teleport, don't bother checking if he can.
		%this.schedule(%obj.tickRate, "tick", %obj);
		return;
	}

	//First, check if a player is within range and the trap can be detonated.
	%currentPosition = %obj.getPosition();
    %maximumDistance = 1.5; //3 studs.
	%obstructions = ($TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticShapeObjectType);

    initContainerRadiusSearch(%currentPosition, %maximumDistance, $TypeMasks::PlayerObjectType);
    while(%foundPlayer = ContainerSearchNext())
	{
		%playerDatablock = %foundPlayer.getDatablock();
		if(%foundPlayer.isKiller | %playerDatablock.isKiller)
		{
			//If the found player is a killer (probably us) ignore them.
			continue;
		}
		else if(%playerDatablock.isDowned)
        {
            //Victim is downed, skip.
            continue;
        }
        else if(ContainerRayCast(%foundPlayer.getPosition(), %currentPosition, %obstructions))
        {
            //The trap and victim are phyiscally blocked, skip.
            continue;
        }

		//Play an audio que.
		serverPlay3D("kid_power" @ getRandom(1, 3) @ "_sound", %obj.getPosition());

		//The found player is a valid target, activate the trap.
		%killer.setTransform(%obj.getPosition());
		%killer.teleportEffect();
		%obj.delete();

		return;
	}

	%this.schedule(%obj.tickRate, "tick", %obj);
}

function PlayerKidTrap::onAdd(%this, %obj)
{
	%this.schedule(%obj.tickRate, "tick", %obj);
}

function PlayerKidTrap::onRemove(%this, %obj)
{
	if(isObject(%obj.trapEmitter))
	{
		%obj.trapEmitter.delete();
	}
}

function Player::createTrap(%obj, %pos)
{
	%trap = new StaticShape()
	{
		datablock = PlayerKidTrap;
		killer = %obj;
		timePlaced = getSimTime();
		tickRate = PlayerKidTrap.tickRate;
	};
	%trap.setTransform(%pos);

	%trap.trapEmitter = new ParticleEmitterNode()
	{
		datablock = EighthEmitterNode;
		emitter = KidBinaryEmitter0;
	};
	%trap.trapEmitter.setTransform(%trap.getTransform());

	if(!isObject(%obj.kidTrapGroup))
	{
		%obj.kidTrapGroup = new ScriptGroup();
	}
	%obj.kidTrapGroup.add(%trap);

	serverPlay3D("kid_trapspawn" @ getRandom(1, 3) @ "_sound", %trap.getPosition()); //Play a sound where the trap is created...

	%client = %obj.client;
	if(isObject(%client))
	{
		%client.playSound("kid_trapcue_sound"); //Play a killer-only audio cue letting him know the trap landed.
	}
}

function PlayerKidTrapProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity)
{
	parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
	
	%killer = %obj.sourceObject;
	if(isObject(%killer) && %killer.getState() !$= "Dead")
	{
		%killer.createTrap(%pos);
	}
}

function PlayerYourself::onTrigger(%this, %obj, %trig, %press) 
{		
	Parent::onTrigger(%this, %obj, %trig, %press);

	if(%press)
	{
		if(!%trig && %obj.getEnergyLevel() >= 25 && !%obj.isPreparingTrap)
		{
			//Melee attack.
			%this.killerMelee(%obj, 4.5);
			%obj.faceConfigShowFace("Attack");
		}
		else if(%trig == 4 && %obj.getEnergyLevel() == %this.maxEnergy)
		{
			if(!isObject(%obj.kidTrapGroup))
			{
				%obj.kidTrapGroup = new ScriptGroup();
			}

			if(%obj.kidTrapGroup.getCount() == %this.maxTraps)
			{
				%client = %obj.client;
				if(isObject(%client))
				{
					%client.centerPrint("\c6You cannot set more than" SPC %this.maxTraps SPC "traps.", 5);
				}
			}
			else
			{
				//Arm a glitch wave in the killer's left hand. When space is unpressed, it will be thrown to lay a trap.
				%obj.isPreparingTrap = true;
				%obj.playThread(1, armReadyLeft);
				%obj.mountImage(KidBinaryImage0, KidBinaryImage0.mountPoint);
				%obj.playAudio(1, "kid_powerready_sound");
			}
		}
	}
	else
	{
		if(%trig == 4 && %obj.isPreparingTrap)
		{
			%obj.isPreparingTrap = false;
			%obj.setEnergyLevel(0);
			if(isObject(%obj.getMountedImage($LeftHandSlot)))
			{
				%obj.unmountImage($LeftHandSlot);
			}
			%obj.playThread(1, root);

			//Throw a projectile from the killer's left hand. When it hits something, a trap will be placed there.
			%obj.playAudio(1, "kid_trapthrow_sound");
			new Projectile()
			{
				dataBlock = PlayerKidTrapProjectile;
				originPoint = %obj.getPosition();
				initialPosition = %obj.getMuzzlePoint($LeftHandSlot);
				initialVelocity = VectorScale(%obj.getMuzzleVector($LeftHandSlot), PlayerKidTrapProjectile.muzzleVelocity);
				sourceObject = %obj;
			};
		}
	}
}

package Eventide_Yourself
{
	function serverCmdLight(%client)
    {
		%player = %client.player;
		%playerDatablock = %player.getDataBlock();
        if(isObject(%player) && %playerDatablock.getName() $= "PlayerYourself")
		{
			%player.isTeleportReady = !%player.isTeleportReady; //Toggle the variable.
			%playerDatablock.killerGUI(%player, %client); //Update the GUI immediately.
			
			//Display a message to the Kid, telling him if his trap is on or off.
			if(isObject(%player.kidTrapGroup))
			{
				%trapCount = %player.kidTrapGroup.getCount();
				if(%player.isTeleportReady)
				{
					%client.playSound("kid_trapson_sound"); //Play a killer-only audio que that the traps are on.
					switch(%trapCount)
					{
						case 0:
							%client.centerPrint("\c6Your traps will be set to \c2ON\c6.", 5);
						case 1:
							%client.centerPrint("\c6Your trap is set to \c2ON\c6.", 5);
						default:
							%client.centerPrint("\c6Your traps are set to \c2ON\c6.", 5);
					}
				}
				else
				{
					%client.playSound("kid_trapsoff_sound"); //Play a killer-only audio que that the traps are off.
					switch(%trapCount)
					{
						case 0:
							%client.centerPrint("\c6Your traps will be set to \c0OFF\c6.", 5);
						case 1:
							%client.centerPrint("\c6Your trap is set to \c0OFF\c6.", 5);
						default:
							%client.centerPrint("\c6Your traps are set to \c0OFF\c6.", 5);
					}
				}
			}
		}
		else
		{
			parent::serverCmdLight(%client);
		}
    }
};
if(isPackage(Eventide_Yourself))
{
	deactivatePackage(Eventide_yourself);
}
activatePackage(Eventide_Yourself);

//
// Voice-line handlers.
//

function PlayerYourself::onKillerChaseStart(%this, %obj, %chasing)
{
    //Mark kills for the below end-of-chase voice line.
    if(!isObject(%obj.incapsAchieved))
    {
        %obj.incapsAchieved = new SimSet();
    }
    if(!isObject(%obj.threatsReceived))
    {
        %obj.threatsReceived = new SimSet();
    }

    %soundType = %this.killerfoundvictimsound;
    %soundAmount = %this.killerfoundvictimsoundamount;
    if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 10000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerYourself::onKillerChase(%this, %obj, %chasing)
{
	if(!%chasing)
    {
        //A victim is nearby but the Kid can't see them yet. Say some quips.
        %soundType = %this.killernearsound;
        %soundAmount = %this.killernearsoundamount;
        if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + getRandom(15000, 25000))))
        {
            %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
            %obj.lastKillerSoundTime = getSimTime();
        }
    }
}

function PlayerYourself::onKillerChaseEnd(%this, %obj)
{
	//If Postal Dude doesn't get any kills during a chase, play a voice line marking his dismay.
    if(%obj.incapsAchieved.getCount() == 0)
    {
        %soundType = %this.killerlostvictimsound;
        %soundAmount = %this.killerlostvictimsoundamount;
        if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + getRandom(5000, 15000))))
        {
            %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
            %obj.lastKillerSoundTime = getSimTime();
        }
    }

    //Need to clear the list. Deleting it is simple and safe.
    %obj.incapsAchieved.delete();
}

function PlayerYourself::onExitStun(%this, %obj)
{
	%soundType = %this.killerattackedsound;
    %soundAmount = %this.killerattackedsoundamount;
    if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerYourself::onIncapacitateVictim(%this, %obj, %victim, %killed)
{
	//Play a voice-line taunting the victim.
    %soundType = %this.killertauntsound;
    %soundAmount = %this.killertauntsoundamount;
    if(%soundType !$= "") 
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }

    //Mark the kill on a temporary SimSet. Used for a voice-line mechanic in `onKillerChaseEnd`.
    if(!isObject(%obj.incapsAchieved))
    {
        %obj.incapsAchieved = new SimSet();
    }
    if(isObject(%victim.client))
    {
        %obj.incapsAchieved.add(%victim.client);
    }
    else
    {
        //Add in a dummy object for holebot support.
        %obj.incapsAchieved.add(
            new ScriptObject() 
            {
                player = %victim;
                name = %victim.getClassName();
            }
        );
    }

	//Generate fake ban messages for flavor.
	if(%killed && isObject(%obj.client) && isObject(%victim.client))
	{
		%victimClient = %victim.client;
 
		%possibleBanMessages = "get a job\nepoic fail\nbased ban\nfuck eventide\nlol\nfukkin die\nanotha chungusite\nlmao\nbannd O_O\nsup bruh\n71.215.225.225\ndis you? ABUR2-566X-6HMR-WEFA\nyou are banned from posting and sending personal messages on this forum.";
		%choosenBanMessage = getRecord(%possibleBanMessages, getRandom(0, (getRecordCount(%possibleBanMessages) - 1))); //Get a random ban message from the string above.

		//Fake ban message.
		MessageAll('MsgAdminForce', '\c3%1\c2 permanently banned \c3%2\c2 (ID: %3) - \c2"%4"', %obj.client.name, %victimClient.name, %victimClient.getBLID(), %choosenBanMessage);
		
		//Fake ban sound.
		for(%i = 0; %i < ClientGroup.getCount(); %i++)
		{
			%client = ClientGroup.getObject(%i);
			%client.playSound("kid_powerready_sound");//AdminSound); //Default sound datablock.
		}
		
		%victim.delete();
	}
}

function PlayerYourself::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);

	if(%obj.getState() !$= "Dead") 
	{
		%obj.faceConfigShowFace("Pain");
		%soundType = %this.killerpainsound;
		%soundAmount = %this.killerpainsoundamount;
		if(%soundType !$= "")
		{
			%obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
			%obj.lastKillerSoundTime = getSimTime();
		}
	}
}

function PlayerYourself::onDisabled(%this, %obj)
{
	parent::onDisabled(%this, %obj);
	
	%soundType = %this.killerlosesound;
	%soundAmount = %this.killerlosesoundamount;
	if(%soundType !$= "")
	{
		%obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
		%obj.lastKillerSoundTime = getSimTime();
	}
}

function Player::KidGaze(%obj)
{
    if(!isObject(%obj) || %obj.isDisabled())
    {
        return;
    }

    %currentPosition = %obj.getPosition();
    %maximumDistance = $EnvGuiServer::VisibleDistance;

    initContainerRadiusSearch(%currentPosition, %maximumDistance, $TypeMasks::PlayerObjectType);
    while(%foundPlayer = ContainerSearchNext())
    {
        %killerPosition = %obj.getEyePoint();
        %killerDatablock = %obj.getDataBlock();
        %victimPosition = %foundPlayer.getEyePoint();
        %victimDatablock = %foundPlayer.getDataBlock();
        %obstructions = ($TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::StaticShapeObjectType);

        if(%foundPlayer.isKiller || %victimDatablock.isKiller)
        {
            //We found ourselves, skip. Future-proofing in case multiple-killer setups become a thing.
            continue;
        }
        else if(%victimDatablock.isDowned)
        {
            //Victim is downed, skip.
            continue;
        }
        else if(ContainerRayCast(%victimPosition, %killerPosition, %obstructions))
        {
            //The killer and victim are phyiscally blocked, skip.
            continue;
        }
        else if(!%obj.isChasing || %foundPlayer.chaseLevel != 2)
        {
			//The victim is not being chased, they are irrelevant here. Skip.
			continue;
		}

		//The victim does not have any items equipped, don't bother with this.
		%victimEquippedItem = %foundPlayer.getMountedImage($RightHandSlot);
		if(!%victimEquippedItem)
		{
			continue;
		}

		//Nowhere better to put this: if the player has a weapon, have Shire play a voice line acknowledging it.
		%alreadyThreatenedKiller = false;
		for(%i = 0; %i < %obj.threatsReceived.getCount(); %i++)
		{
			if(%obj.threatsReceived.getObject(%i).getId() == %foundPlayer.getId())
			{
				%alreadyThreatenedKiller = true;
				break;
			}
		}
		if(%alreadyThreatenedKiller)
		{
			//They already threatened us, skip playing any more voice lines.
			continue;
		}

		//They have an item equipped and it's a weapon, have the Kid react to it.
		if(%victimEquippedItem.isWeapon || %victimEquippedItem.className $= "WeaponImage")
		{
			%soundType = %killerDatablock.killerthreatenedsound;
			%soundAmount = %killerDatablock.killerthreatenedsoundamount;
			if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
			{
				%obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
				%obj.lastKillerSoundTime = getSimTime();
				%obj.threatsReceived.add(%foundPlayer); //Ensure the Kid does not acknowledge any further weapons. Less annoying.
			}
		}

		continue;
	}

    %obj.schedule(%obj.gazeTickRate, KidGaze);
}