//
// Datablocks.
//

datablock TSShapeConstructor(EventideplayerDts) 
{
	baseShape = "./models/eventideplayer.dts";
	sequence0 = "./models/default.dsq";
	sequence1 = "./models/default_melee.dsq";
};

datablock PlayerData(EventidePlayer : PlayerStandardArmor)
{
	shapeFile = EventideplayerDts.baseShape;
	uiName = "Eventide Player";

	uniformCompatible = true;
	isEventideModel = true;
	showEnergyBar = false;
	firstpersononly = false;
	isKiller = false;
	canJet = false;
	tunnelFOVIncrease = 20;

	useCustomPainEffects = true;
	jumpSound = "jumpSound";
	PainSound		= "";
	DeathSound		= "";

	rechargeRate = 0.375;
	maxTools = 3;
	maxWeapons = 3;
	jumpForce = 0;
	
	cameramaxdist = 2.25;
    cameratilt = 0.1;
	maxfreelookangle = 2.5;

	minimpactspeed = 15;
	groundImpactMinSpeed = 5;
	groundImpactShakeFreq = "4.0 4.0 4.0";
	groundImpactShakeAmp = "1.0 1.0 1.0";
	groundImpactShakeDuration = 0.8;
	groundImpactShakeFalloff = 15;
};

datablock PlayerData(EventidePlayerDowned : EventidePlayer)
{	
	maxForwardSpeed = 0;
   	maxBackwardSpeed = 0;
   	maxSideSpeed = 0;
   	maxForwardCrouchSpeed = 1.5;
   	maxBackwardCrouchSpeed = 1.5;
   	maxSideCrouchSpeed = 1.5;
   	jumpForce = 0;
	isDowned = true;
	uiName = "";
};

//
// Miscellaneous functions.
//

// function cloneScriptGroup(%targetObject)
// {
//     %targetObjectName = %targetObject.getName();
//     %targetObject.setName("targetScriptObject");

//     %cloneObject = new ScriptGroup(cloneScriptObject : targetScriptObject);

//     %targetObject.setName(%targetObjectName);
//     %cloneObject.setName("");

//     return %cloneObject;
// }

// //
// // Player classes.
// //

// // Templates, containers for pre-made groups of classes.
// ///

// function EventideClassGroupTemplates::onAdd(%this)
// {
// 	index["Classic"] = new ScriptGroup()
// 	{
// 		class = "EventideClassGroup";
// 		template = "Classic";

// 		new ScriptObject()
// 		{
// 			class = "EventidePlayerClass";
// 			title = "Mender";
// 			canStack = false;
// 			spawnMessage = "You acquired a soda and can run slightly faster!";
// 			items = new SimSet()
// 			{
// 				class = "EventideClassItem";

// 				//Can't store datablocks directly in a SimSet. How inconvenient.
// 				new ScriptObject()
// 				{
// 					itemData = (getRandom(0, 1) ? GauzeItem.getID() : ZombieMedpackItem.getID());
// 				};
// 			};
// 		};
// 	};
// }

// //Using an internal array to fetch templates is faster than iteration via a for or while loop.
// function EventideClassGroupTemplates::addTemplate(%this, %classGroup)
// {
// 	%this.index[%classGroup.name] = %classGroup;
// }

// function EventideClassGroupTemplates::getTemplate(%this, %classGroupName)
// {
// 	return %this.index[%classGroupName];
// }

// $Eventide_ClassGroupTemplates = new EventideClassGroupTemplate(Eventide_ClassGroupTemplates);

// // Container for classes.
// ///

// function EventideClassGroup::onAdd(%this)
// {
// 	//Nothing but a template was given, auto-fill the data from an existing template if possible.
// 	if(%this.template && %this.getCount() == 0)
// 	{
// 		%existingTemplate = $Eventide_ClassGroupTemplates.getTemplate(%this.template);
// 		if(%existingTemplate)
// 		{
// 			%this = cloneScriptGroup(%existingTemplate);
// 		}
// 	}
// }

// // Containers for class information.
// ///

// function EventidePlayerClass::onAdd(%this)
// {
// 	if(!%this.items)
// 	{
// 		%this.items = new ScriptGroup();
// 	}
// 	else
// 	{
// 		//Cycle through the item set to make sure they're all valid. Delete any items that are invalid.
// 		for(%i = 0; %i < %this.items.getCount(); %i++)
// 		{
// 			%itemContainer = %this.getObject(%i);
// 			if(!%itemContainer.itemData || %itemContainer.itemData.getClassName() !$= "ItemData")
// 			{
// 				%itemContainer.delete();
// 			}
// 		}
// 	}
	
// 	if(%this.canStack $= "")
// 	{
// 		%this.canStack = false;
// 	}
	
// 	if(!%this.title)
// 	{
// 		%this.title = "Specialist";
// 	}
// }

// $Eventide_PlayerClasses = new ScriptGroup(Eventide_PlayerClasses)
// {
// 	class = "EventideClassGroup";
// 	template = "default";
// };

//
// Everything else.
//

function EventidePlayer::pulsingScreen(%this,%obj)
{
	// If any of these are met, do not continue
	if ((!isObject(%obj) || %obj.getclassname() !$= "Player" || %obj.getState() $= "Dead") || %obj.getDamageLevel() < 25) return;

	if (isObject(%obj.client)) %obj.client.play2D("survivor_heartbeat_sound");
	%obj.setdamageflash(0.125);

	// Prevent multiple schedules
	cancel(%obj.pulsingScreenSched);
	%obj.pulsingScreenSched = %this.schedule(850,pulsingScreen,%obj);
}

function EventidePlayer::assignClass(%this,%obj,%class)
{
	if (!isObject(%obj) || !isObject(%obj.client) || %class $= "") return;

	commandToClient(%obj.client,'PlayGui_CreateToolHud',(%class $= "hoarder") ? 5 : %this.maxTools);

	%formatString = "<font:impact:40><color:FFFF00>";
	%firstString = "You acquired a";

	switch$(%class)
	{
		case "mender":  %healitem = (getRandom(1)) ? GauzeItem.getID() : ZombieMedpackItem.getID();
						%obj.tool[0] = %healitem;
         				messageClient(%obj.client,'MsgItemPickup','',0,%healitem);
						%obj.client.centerprint(%formatString @ "Class: Mender <br>" @ %firstString SPC "medical item and can revive survivors faster!",4);

		case "runner": 	%obj.setTempSpeed(); // Reset the player's speed to the new class default
						%obj.tool[0] = SodaItem.getID();
         				messageClient(%obj.client,'MsgItemPickup','',0,SodaItem.getID());
						%obj.client.centerprint(%formatString @ "Class: Runner <br>" @ %firstString SPC "soda and can run slightly faster!",4);

		case "hoarder": %obj.hoarderToolCount = 5;
						%obj.tool[0] = DCamera.getID();
         				messageClient(%obj.client,'MsgItemPickup','',0,DCamera.getID());
						%obj.client.centerprint(%formatString @ "Class: Hoarder <br>" @ %firstString SPC "camera and have 5 slots!",4);

		case "fighter":	%obj.pseudoHealth = 75;
						%obj.tool[0] = sm_poolCueItem.getID();
         				messageClient(%obj.client,'MsgItemPickup','',0,sm_poolCueItem.getID());
						%obj.client.centerprint(%formatString @ "Class: Fighter <br>" @ %firstString SPC "pool cue, can shove further and can take 1 hit before getting damaged!",4);

		case "tinkerer": %obj.tool[0] = MonkeyWrench.getID();
         				 messageClient(%obj.client,'MsgItemPickup','',0,MonkeyWrench.getID());
						 %obj.tool[1] = StunGun.getID();
         				 messageClient(%obj.client,'MsgItemPickup','',1,StunGun.getID());
						 %obj.client.centerprint(%formatString @ "Class: Tinkerer <br>" @ %firstString SPC "monkey wrench, stungun, use the wrench to repair generators faster!",4);
	}
	
}

function EventidePlayer::onNewDatablock(%this,%obj)
{
	Parent::onNewDatablock(%this,%obj);

	//Play ambiant music.
	%client = %obj.client;
	if(isObject(%client))
	{
		%client.playAmbiance();
	}

	%obj.schedule(33,setEnergyLevel,0);
	%obj.setScale("1 1 1");
}

function EventidePlayer::onImpact(%this, %obj, %col, %vec, %force)
{	
	%zvector = getWord(%vec,2);	

	// Apply a multiplier to the impact force based on the vertical component of the impact vector, it starts by checking the highest impact force to the lowest
	%force = (%zvector > %this.minImpactSpeed + 20)	? %force * 2.5 :
	(%zvector > %this.minImpactSpeed + 5) ? %force * 1.5 :
	(%zvector > %this.minImpactSpeed) ? %force * 0.5 : %force;	
	
	Parent::onImpact(%this, %obj, %col, %vec, mCeil(%force));

	if (%obj.getState() $= "Dead") return;
	if (%zvector > %this.minImpactSpeed) %obj.playthread(3,"plant");
}

function EventidePlayer::onActivate(%this,%obj)
{
	%triggerTime = getSimTime();
	%obj.setEnergyLevel(%obj.getEnergyLevel()-4);

	// Reset the delay if the player waits long enough, 3.5 seconds
	if (%triggerTime - %obj.staminaTime > 3500) 
	{
		%obj.staminaCount = 0;
		%obj.staminaTime = 0;
	}
	else
	{
		%obj.staminaCount += 0.25;
		%obj.staminaTime = (%triggerTime+200)+(10*%obj.staminaCount);

		// Show the vignette when the player is is exhausted
		if (%obj.staminaCount >= 5) 
		{
			// Only enable the tunnel vision once to prevent the player from seeing the vignette multiple times 
			if (%obj.staminaCount == 5) %this.tunnelVision(%obj,true);

			// Reset the stamina after 4 seconds, this is reset if the player continues to activate this function
			cancel(%obj.resetStamina);
			%obj.resetStamina = %this.schedule(4000,resetStamina,%obj);
		}		
	}

	// When the player is possessed by the renowned, perform some actions
	if (%obj.isPossessed) 
	{		
		%obj.playthread(2,"activate2");
		%obj.AntiPossession = mClampF(%obj.AntiPossession+1, 0, 15);
		%this.CounterPrint(%obj,%obj.client,%obj.AntiPossession/2,"Left click to regain control!");		

		if (%obj.AntiPossession >= 15)
		{
			if (isObject(%obj.Possesser))
			{
				%obj.Possesser.client.Camera.setMode("Corpse", %obj.Possesser);
				%obj.Possesser.client.setControlObject(%obj.Possesser.client.camera);
				%obj.Possesser.client.centerprint("<font:Impact:30>\c3Your victim broke free!",2);

				cancel(%obj.Possesser.returnObserveSchedule);
				%obj.Possesser.returnObserveSchedule = %obj.Possesser.schedule(4000,ClearRenownedEffect);
				
				%obj.Possesser.playthread(2,"undo");
				%obj.Possesser.playthread(3,"activate2");
				%obj.Possesser.mountImage("RenownedPossessedImage",3);
				%obj.Possesser.playaudio(3,"renowned_melee" @ getRandom(0,2) @ "_sound");
			}

			%obj.client.centerprint("<color:FFFFFF><font:Impact:40>You broke free!",1);
			%obj.ClearRenownedEffect();			
		}
	}
}

function EventidePlayer::resetStamina(%this,%obj)
{
	if (!isObject(%obj) || %obj.getState() $= "Dead") return;

	%obj.staminaCount = 0;
	%obj.staminaTime = 0;
	%this.tunnelVision(%obj,false);
}

function EventidePlayer::Shove(%this,%obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead") return;

	%triggerTime = getSimTime();

	if (%triggerTime - %obj.staminaTime > 3500)//Reset the delay if the player waits long enough, 3.5 seconds
	{
		%obj.staminaCount = 0;
		%obj.staminaTime = 0;
	}

	if (%obj.staminaTime < %triggerTime && %obj.getEnergyLevel() >= %this.maxEnergy/4)//Shoving
	{
		%obj.setEnergyLevel(%obj.getEnergyLevel()-20);
		%obj.staminaCount++;
		%obj.staminaTime = (%triggerTime+400)+(40*%obj.staminaCount);
		%soundpitch = getRandom(50,125);

		if (%obj.staminaCount >= 5)
		{
			cancel(%obj.resetStamina);
			%soundpitch = getRandom(50,80);
			%obj.resetStamina = %this.schedule(4000,resetStamina,%obj);

			if (%obj.staminaCount == 5)
			{							
				%this.tunnelVision(%obj,true);
				%obj.resetStamina = %this.schedule(4000,resetStamina,%obj);
			}								
		}
		
		%obj.playthread(2,"activate2");
		$oldTimescale = getTimescale();
		setTimescale((%soundpitch*0.01) * $oldTimescale);
		serverPlay3D("melee_swing" @ getRandom(1,2) @ "_sound",%obj.getHackPosition());
		setTimescale($oldTimescale);
		
		%pos = %obj.getEyePoint();
		%eyeVec = %obj.getEyeVector();
		%radius = 0.25;		
		%mask = $TypeMasks::PlayerObjectType;

		initContainerRadiusSearch(%pos,%radius,%mask);
		while (%hit = containerSearchNext())
		{
			%obscure = containerRayCast(%obj.getEyePoint(),%hit.getHackPosition(),$TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType, %obj);
			%dot = vectorDot(%obj.getEyeVector(),vectorNormalize(vectorSub(%hit.getHackPosition(),%obj.getHackPosition())));

			if (%hit == %obj || %hit.getState() $= "Dead" || isObject(%obscure) || %dot < 0.5)
			{
				continue;
			}

			serverPlay3D("melee_shove_sound",%hit.getHackPosition());
			%hit.playThread(2,"jump");

			if (!%obj.shoveForce)
			{
				%obj.shoveForce = 1;
			}

			%shoveForce = (%hit.getDatablock().getName() $= "PuppetMasterPuppet") ? 2 : %obj.shoveForce;
			%exhausted = (%obj.staminaCount >= 5) ? 2 : 1;
						
			%forwardimpulse = (((%obj.survivorclass $= "fighter") ? 2.5 : 1.25) / %exhausted) * %shoveForce;
			%zimpulse = (((%obj.survivorclass $= "fighter") ? 5 : 2.5) / %exhausted) * %shoveForce;
			%hit.setVelocity(VectorAdd(VectorScale(%obj.getEyeVector(),%forwardimpulse),"0 0 " @ %zimpulse));
		}												
	}
}

function EventidePlayer::onTrigger(%this, %obj, %trig, %press) 
{
	Parent::onTrigger(%this, %obj, %trig, %press);
	
	if (%press)
	{
		switch(%trig)
		{
			case 0:	if (isObject(%obj.getMountedImage(0))) return;
			
					%eyePoint = %obj.getEyePoint();
					%endPoint = vectoradd(%obj.getEyePoint(),vectorscale(%obj.getEyeVector(),5*getWord(%obj.getScale(),2)));
					%masks = $TypeMasks::FxBrickObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType;
			
					%ray = containerRayCast(%eyePoint, %endPoint,%masks,%obj);
					if (isObject(%ray) && (%ray.getType() & $TypeMasks::PlayerObjectType) && %ray.getdataBlock().isDowned && !%ray.isBeingSaved)
					{
						%obj.setTempSpeed(0.25);
						%obj.playthread(2,"armReadyRight");
						%obj.isSaving = %ray;
						%ray.isBeingSaved = true;						
						%this.reviveDowned(%obj,%ray,%press);
					}
					else
					{
						%obj.setTempSpeed();
					}

			case 4: if (%obj.isSkinwalker)
					{
						PlayerSkinwalker.Transform(%obj);
					}
					else
					{
						%this.Shove(%obj);
					}
		}
	}
	else if (isObject(%obj.isSaving))
	{
		%this.reviveDowned(%obj,%obj.isSaving,false);
	}
}

function EventidePlayer::CounterPrint(%this,%obj, %client, %amount, %message)
{
    if (!isobject(%client)) 
	{
		return;
	}

    %addsymbol = "";
    %symbol = "|";

    // Generate the repeated symbols
    for (%i = 0; %i < %amount; %i++) 
	{
		%addsymbol = %addsymbol @ %symbol;
	}

    // Display the custom message
    %client.centerPrint("<font:impact:30>\c3" @ %message @ " <br>\c2" @ %addsymbol, 1);
}

function EventidePlayer::reviveDowned(%this,%obj,%victim,%bool)
{
	if(!isObject(%obj) || !isObject(%victim) || %obj.getState() $= "Dead" || %victim.getState() $= "Dead")
	{
		return;
	}
	
	if (%bool && vectorDist(%obj.getPosition(),%victim.getPosition()) < 3)
	{	
		// The victim will be saved after 4 ticks if the player is still holding left click
		if (%obj.reviveDownedCounter <= 5)
		{			
			%obj.reviveDownedCounter++;
			%this.CounterPrint(%obj,%obj.client,%obj.reviveDownedCounter,"Get up!");
			%this.CounterPrint(%obj,%victim.client,%obj.reviveDownedCounter,"Get up!");			

			// The mender class will save the victim faster
			cancel(%obj.reviveDownedSched);
			%obj.reviveDownedSched = %this.schedule((%obj.survivorClass $= "mender") ? 300 : 750,reviveDowned,%obj,%victim,%bool);
			return;
		}
		else
		{
			%obj.setTempSpeed(); // Reset the player's speed
			%obj.reviveDownedCounter = 0;			
			%stringformat = "<font:impact:30>\c3";

			// Message the savior
			if (isObject(%obj.client))
			{
				%obj.client.centerprint(%stringformat @ "You revived" SPC %victim.client.name,1);
			}
			
			// Message the saved victim and apply the effects
			if (isObject(%victim.client)) 
			{
				%victim.client.centerprint(%stringformat @ "You were revived by" SPC %obj.client.name,1);			
				%victim.getdataBlock().pulsingScreen(%victim);
			}

			// Clear the billboard
			$Eventide::BillboardMounts.clearAVBillboards(%victim,"Downed");
			%victim.setHealth(%victim.getdatablock().maxDamage/1.3333);
			%victim.pseudoHealth = (%victim.survivorclass $= "fighter") ? 75 : 0;
			%victim.setDatablock("EventidePlayer");
			%victim.playthread(0,"root");				
			return;
		}					
	}
	else
	{
		cancel(%obj.reviveDownedSched);
		%obj.isSaving = false;
		%victim.isBeingSaved = false;
		%obj.reviveDownedCounter = 0;
		%obj.playthread(2,"root");
		return;
	}	
}

function EventidePlayer::EventideAppearance(%this,%obj,%client)
{
	if (!isObject(%obj) || !isObject(%client)) return;

	// Use the victim client if the player is a skinwalker and they killed someone
	%tempclient = (%obj.isSkinwalker && isObject(%obj.victimreplicatedclient)) ? %obj.victimreplicatedclient : %client;
	
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

	//Face system functionality: prevent face from being overwritten by an avatar update.
	if (isObject(%obj.faceConfig))
	{
		%neededFacePack = (%obj.client.chest ? $Eventide_FacePacks["female"] : $Eventide_FacePacks["male"]);
		if (%obj.faceConfig.getFacePack() !$= %neededFacePack) {		
			//If the player updated their avatar, give them a new face pack to reflect it.
			%obj.createFaceConfig(%neededFacePack);
		}
		%obj.faceConfigShowFace((%obj.faceConfig.currentFace !$= "") ? %obj.faceConfig.currentFace : "");
	}
	else %obj.setFaceName(%tempclient.faceName); // Use the default player face if the player doesn't have a face system
	
	%obj.setDecalName(%tempclient.decalName);

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

	// Set blood colors
	%obj.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("lhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("rhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_back", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_back", "0.7 0 0 1");
}

function EventidePlayerDowned::EventideAppearance(%this,%obj,%funcclient)
{
	EventidePlayer::EventideAppearance(%this,%obj,%funcclient);
}

function EventidePlayer::tunnelVision(%this,%obj,%bool)
{
	if (!isObject(%obj) || !isObject(%obj.client) || %obj.getState() $= "Dead") {
		return;
	}

	// Start the tunnel vision effect
	if (%bool) 
	{		
		%obj.tunnelVision = mClampF(%obj.tunnelVision + 0.1, 0, 1);
		commandToClient(%obj.client, 'SetVignette', true, "0 0 0" SPC %obj.tunnelVision);

		if (%obj.tunnelVision >= 1) {
			return;
		}
	}
	else if (!%obj.chaseLevel)
	{
		// Reverse the tunnel vision effect first
		if (%obj.tunnelVision)
		{
			%obj.tunnelVision = mClampF(%obj.tunnelVision - 0.1, 0, 1);
		    commandToClient(%obj.client, 'SetVignette', true, "0 0 0" SPC %obj.tunnelVision);
		}
		// Then reset the tunnel vision effect
		else
		{
			commandToClient(%obj.client, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor);		
			return;
		}
	}

	%obj.tunnelVisionsched = %this.schedule(50, tunnelVision, %obj, %bool);	
}

function EventidePlayer::dropAllTools(%this,%obj)
{
	if(!isObject(%obj) || !isObject(getMinigamefromObject(%obj))) return;

	%obj.unmountimage(0);
	
	// Check if the player is a hoarder class for the tool count	
	%inventoryToolCount = (%obj.hoarderToolCount) ? %obj.hoarderToolCount : %obj.getDataBlock().maxTools;
	for (%i = 0; %i < %inventoryToolCount; %i++) if (isObject(%obj.tool[%i]))
	{
		// Clear the player's tool
		if(isObject(%obj.client))
		{					
			%obj.tool[%i] = 0;
			messageClient(%obj.client, 'MsgItemPickup', '', %i, 0);
		}
		
		// Spawn the item
		%item = new Item()
		{
			dataBlock = %obj.tool[%i];
			position = %obj.getHackPosition();	
			BL_ID = %funcclient.BL_ID;
			minigame = %minigame;
		};
		
		// Set the item's velocity
		%item.setVelocity(vectorAdd(%obj.getVelocity(),getRandom(-4,4) SPC getRandom(-4,4) SPC getRandom(4,8)));
		
		if (!isObject(Eventide_MinigameGroup)) 
		{
			missionCleanUp.add(new SimGroup(Eventide_MinigameGroup));
		}

		Eventide_MinigameGroup.add(%item); // Add the item to the minigame group for cleanup when the minigame ends or restarts		
	}
}

function EventidePlayer::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType)
{
	//Some killers have projectile weapons, and this allows you to call the `onIncapacitateVictim` on them if they use one.
	if(isObject(%sourceObject))
	{	
		%sourceDatablock = %sourceObject.getDataBlock();
		if(%sourceDatablock.getClassName() $= "ProjectileData")
		{
			%sourceDatablock = %sourceObject.sourceObject.getDatablock();
			%killerSourceObject = %sourceObject.sourceObject;
		}
		else
		{
			%killerSourceObject = %sourceObject;
		}
	}	

	// Dont let the player die if they havent been downed yet
	if(%obj.getState() !$= "Dead" && %damage+%obj.getdamageLevel() >= %this.maxDamage) //%damage < mFloor(%this.maxDamage/1.33) //This is commented out to prevent homing rockets from being fatal.
    {   
		if(!%obj.wasDowned)
		{
			// Work in progress billboard for downed players, still not working :(
			$Eventide::BillboardMounts.AVBillboard(%obj,"downedAVBillboard","Downed");

			// Reset the player's health, and set the player to be downed
			%obj.wasDowned = true; // They have been downed once, they wont be able to go down again until they are healed
			%obj.setHealth(%this.maxDamage);
			%obj.setDatablock("EventidePlayerDowned");

			//New Sky Captain functionality. Execute a function on the killer after they down somebody.
			if(%sourceDatablock.isKiller)
			{
				%sourceDatablock.onIncapacitateVictim(%killerSourceObject, %obj, false);
			}

			//Check if the killer is the only one remaining.
			%minigame = getMinigamefromObject(%obj);
			if(isObject(%minigame))
			{
				// Notify everyone that the player is downed
				%minigame.playSound("outofbounds_sound");
				%minigame.checkDownedSurvivors();
			}

			// Return here, or else the player will die after this condition is met
			return;
		}
		else
		{
			//Execute the same function, but with the %killed parameter set to true. Not used as of 1/2/2015, but I wanted to future-proof.
			if(%sourceDatablock.isKiller)
			{
				%sourceDatablock.onIncapacitateVictim(%killerSourceObject, %obj, true);
			}

			$Eventide::BillboardMounts.clearAVBillboards(%obj,"Downed");
		}
    }

	// Continue the damage
    Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType);

	//Check if the killer is the only one remaining.
	%minigame = getMinigamefromObject(%obj);
	if(isObject(%minigame))
	{
		// Notify everyone that the player is downed
		%minigame.checkDownedSurvivors();
	}

	// Face system functionality: play a pained facial expression when the player is hurt, and switch to hurt facial expression afterward 
	// if enough damage has been received.
	if (isObject(%obj.faceConfig))
	{
		if (%obj.getDamagePercent() > 0.33 && $Eventide_FacePacks[%obj.faceConfig.category, "Hurt"] !$= "") 
		{
			%obj.createFaceConfig($Eventide_FacePacks[%obj.faceConfig.category, "Hurt"]);
		}

		if (%obj.faceConfig.isFace("Pain")) 
		{
			%obj.schedule(33, "faceConfigShowFace", "Pain"); //This needs to be delayed for whatever reason. Blinking doesn't start otherwise.
		}		
	}

	// Pseudo health for the fighter class, gives the player a temporary health boost until they are hurt again
	// Not sure why just not using %obj.pseudohealth as a condition wouldnt work, so check if it is greater than 0
	if (%obj.pseudoHealth > 0)
	{
		%obj.pseudoHealth -= %damage;
		%obj.addhealth(%this.maxDamage);
		%obj.mountimage("HealImage",3);
		%obj.setwhiteout(0.1);
		
		if (isObject(%obj.client)) 
		{
			%obj.client.play2D("printfiresound");
		}
	}
		
	if (%damage) 
	{
		%genderSound = (!%obj.client.chest) ? "male" : "female";
		%genderSoundAmount = (!%obj.client.chest) ? 3 : 6;
		%sound = %genderSound @ "_pain" @ getRandom(1, %genderSoundAmount) @ "_sound";

		// Condition for the skinwalker
		if (%obj.isSkinwalker)
		{
			// The disguise is about to be broken, now that the player has been hurt
			if (getRandom(1,4) == 1) 
			{
				%sound = "skinwalker_pain_sound";	
				if (!isObject(%obj.victim) && !isEventPending(%obj.monsterTransformschedule)) 
				{ 
					PlayerSkinwalker.monsterTransform(%obj,true);
				}
			}

			%obj.setHealth(%this.maxDamage);
		}

		if (%obj.getState() !$= "Dead" && %obj.lastDamageCall < getSimTime())
		{
			%obj.playaudio(0,%sound);
			%obj.lastDamageCall = getSimTime() + getRandom(250,750);
		}
	}
}

function EventidePlayerDowned::onNewDataBlock(%this,%obj)
{
	Parent::onNewDataBlock(%this,%obj);
	%this.DownLoop(%obj);
}

function EventidePlayerDowned::DownLoop(%this,%obj)
{ 
	// Do not continue if the player is dead, invalid, or not the downed datablock anymore
	if (!isobject(%obj) || %obj.getstate() $= "Dead" || %obj.getDataBlock() != %this) 
	{
		return;
	}

	// Force the player to sit if they are not already, and and they are not crouched
	if (!%obj.isCrouched())
	{
		%obj.setActionThread("sit",1);
	}

	// Update victim's face
	if(isObject(%obj.faceConfig))
	{
		if(%obj.faceConfig.subCategory $= "" && $Eventide_FacePacks[%obj.faceConfig.category, "Hurt"] !$= "")
		{
			%obj.createFaceConfig($Eventide_FacePacks[%obj.faceConfig.category, "Hurt"]);
		}
		
		if(%obj.faceConfig.isFace("Scared"))
		{
			%obj.faceConfig.dupeFaceSlot("Neutral", "Scared");                    	
		}					
	}

	// If the player is not being saved, then continue the down loop
	if (!%obj.isBeingSaved && %obj.lastDownLoop < getSimTime())
	{
		// As the player loses health, the loop gets faster
		%obj.lastDownLoop = getSimTime() + mClampF((100-%obj.getDamageLevel()) * 15,200,1100);

		if (isObject(%obj.client)) 
		{
			%heartbeatvariant = (%obj.getDamageLevel() >= %this.maxDamage/2) ? 2 : 1;
			%obj.client.play2D("survivor_heartbeat" @ %heartbeatvariant @ "_sound");
		}

		// Start the blood drip effect
		%obj.startDrippingBlood(1000);
		
		%obj.addHealth(-1);
		%pulse = 0.1 + ((%obj.getDamageLevel() / 100) * 0.75);
		%obj.setDamageFlash(%pulse);

		// Scream every 5-10 seconds
		if ($Pref::Server::Eventide::victimScreamsEnabled && %obj.lastDownCall+getRandom(5000,10000) < getSimTime())
		{
			%obj.lastDownCall = getSimTime();			

			%genderSound = (!%obj.client.chest) ? "male" : "female";
			%genderSoundAmount = (!%obj.client.chest) ? 3 : 5;
			%sound = %genderSound @ "_shock" @ getRandom(1, %genderSoundAmount) @ "_sound";

			%obj.playaudio(0,%sound);
			%obj.playthread(3,"plant");
		}
	}

	// Keep the loop going unless the first condition is met
	cancel(%obj.downloop);
	%obj.downloop = %this.schedule(100,DownLoop,%obj);
}

function EventidePlayer::onDisabled(%this,%obj)
{
	EventidePlayerDowned::onDisabled(%this,%obj);
}

function EventidePlayer::onRemove(%this, %obj)
{
	EventidePlayerDowned::onRemove(%this, %obj);
}

function EventidePlayerDowned::onDisabled(%this,%obj)
{	
	Parent::onDisabled(%this,%obj);

	//TODO: Quick-fix for corpses standing up on death
	%obj.playThread(1, "Death1");

	// Remove all mounted images and stop all animation threads
	for (%j = 0; %j < 4; %j++)
	{
		%obj.unmountimage(%j);
		%obj.playThread(%j,"root");
	}

	// Remove the downed billboard
	$Eventide::BillboardMounts.clearAVBillboards(%obj,"Downed");

	%genderSound = (!%obj.client.chest) ? "male" : "female";
	%genderSoundAmount = (!%obj.client.chest) ? 4 : 2;
	%sound = %genderSound @ "_death" @ getRandom(1, %genderSoundAmount) @ "_sound";	
	%obj.playaudio(0,%sound);	

	// Let the killer know that a survivor has been killed
	%killers = getCurrentKillers();
	for(%i = 0; %i < %killers.getCount(); %i++)
	{
		%currentKillerClient = %killers.getObject(%i);
		if(isObject(%currentKillerClient))
		{
			%currentKillerClient.PlaySkullFrames();
			%currentKillerClient.play2D("elimination_sound");
		}
	}

	// Only do this if the client exists
	if (isObject(%obj.client))
	{
		%nClient = (isObject(%obj.ghostclient)) ? %obj.ghostclient : %obj.client;
		commandToClient(%nClient, 'SetVignette', $EnvGuiServer::VignetteMultiply, $EnvGuiServer::VignetteColor);
		EventidePlayer.dropAllTools(%obj);

		// Clear the tunnel vision effect
		if(%obj.tunnelvision)
		{
			EventidePlayer.TunnelVision(%obj,false);
		}
		
		if (isObject(%minigame = getMinigamefromObject(%obj)))
		{		
			if (%obj.markedforRenderDeath) 
			{
				%minigame.playSound("render_kill_sound");
				%obj.spawnExplosion("PlayerSootProjectile","1.5 1.5 1.5");
				%obj.schedule(33,delete);
			}

			if (%obj.shireZombify)
			{
				%bot = new AIPlayer()
				{
					dataBlock = "ShireZombieBot";
					minigame = %obj.ghostClient.minigame;
					ghostclient = %obj.ghostclient;
				};				

				if (!isObject(Eventide_MinigameGroup)) missionCleanup.add(new SimGroup(Eventide_MinigameGroup));
				Eventide_MinigameGroup.add(%bot);
				%bot.setTransform(%obj.getTransform());
				%obj.spawnExplosion("PlayerSootProjectile","1.5 1.5 1.5");
				%obj.schedule(33,delete);
			}			
		}
	}	
}

function EventidePlayerDowned::onRemove(%this, %obj)
{
	Parent::onRemove(%this, %obj);
	
	// Remove the downed billboard if it still exists
	$Eventide::BillboardMounts.clearAVBillboards(%obj,"Downed");

	// If there is one remaining survivor, then call the minigame's onLastSurvivor function
	if(isObject(%minigame = getMinigameFromObject(%obj)) && isObject(%obj.client) && %obj.client.getRemainingTeamMembers() == 1)
	{
		%minigame.onLastSurvivor();
	}	
}