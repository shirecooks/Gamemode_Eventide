datablock PlayerData(PlayerRenowned : EventidePlayer) 
{
	uiName = "Renowned Player";	
	
	// Weapon: Katana
	hitprojectile = KillerSharpHitProjectile;
	hitobscureprojectile = KillerKatanaClankProjectile;	
	meleetrailskin = "base";
	meleetrailoffset = "0.3 1.4 0.7"; 	
	meleetrailangle1 = "0 90 0";
	meleetrailangle2 = "0 -90 0";
	meleetrailangle3 = "0 0 0";
	meleetrailangle4 = "0 180 0";
	meleetrailscale = "4 4 2";

	rechargeRate = 0.26;
	maxDamage = 9999;
	maxTools = 0;
	maxWeapons = 0;
	maxForwardSpeed = 7.32;
	maxBackwardSpeed = 4.18;
	maxSideSpeed = 6.27;

	useCustomPainEffects = true;
	jumpSound = "";
	PainSound		= "";
	DeathSound		= "";
	
	renderFirstPerson = false;
	firstpersononly = true;
	showenergybar = true;
	jumpForce = 0;

	rightclickicon = "color_headache";
	leftclickicon = "color_melee";
	rightclickspecialicon = "";
	leftclickspecialicon = "";

	isKiller = true;
	killerraisearms = false;
	killerlight = "NoFlareYLight";	

	killerChaseLvl1Music = "musicData_Eventide_RenownedNear";
	killerChaseLvl2Music = "musicData_Eventide_RenownedChase";

	killeridlesound = "renowned_idle";
	killeridlesoundamount = 11;

	killerchasesound = "renowned_chase";
	killerchasesoundamount = 18;

	killermeleesound = "renowned_melee";
	killermeleesoundamount = 4;
	
	killerweaponsound = "renowned_weapon";
	killerweaponsoundamount = 5;	

	killermeleehitsound = "melee_tanto";
	killermeleehitsoundamount = 3;
};

function PlayerRenowned::onNewDatablock(%this,%obj)
{
	//Face system functionality.
	%obj.createEmptyFaceConfig($Eventide_FacePacks["renowned"]);
	%facePack = %obj.faceConfig.getFacePack();
	%obj.faceConfig.face["Neutral"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Neutral"));
	%obj.faceConfig.setFaceAttribute("Neutral", "length", -1);

	%obj.faceConfig.face["Attack"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Attack"));
	%obj.faceConfig.setFaceAttribute("Attack", "length", 500);

	%obj.faceConfig.face["Pain"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Pain"));
	%obj.faceConfig.setFaceAttribute("Pain", "length", 1000);
	
	//Everything Else
	Parent::onNewDatablock(%this,%obj);
	
	%obj.mountImage("meleeTantoImage",0);
	%obj.schedule(1, setEnergyLevel, 0);
	%obj.setScale("1.05 1.05 1.05");
	%obj.mountImage("renownedeyesimage",2);
}

function PlayerRenowned::killerGUI(%this,%obj,%client)
{	
	%energylevel = %obj.getEnergyLevel();

	// Some dynamic varirables
	%leftclickstatus = (%obj.getEnergyLevel() >= 25) ? "hi" : "lo";
	%rightclickstatus = (%obj.getEnergyLevel() == %this.maxEnergy && %obj.gazingPlayer) ? "hi" : "lo";
	%leftclicktext = (%this.leftclickicon !$= "") ? "<just:left>\c6Left click" : "";
	%rightclicktext = (%this.rightclickicon !$= "") ? "<just:right>\c6Right click" : "";		

	// Regular icons
	%leftclickicon = (%this.leftclickicon !$= "") ? "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickicon @ ">" : "";
	%rightclickicon = (%this.rightclickicon !$= "") ? "<just:right><bitmap:" @ $iconspath @ %rightclickstatus @ %This.rightclickicon @ ">" : "";

	%client.bottomprint(%leftclicktext @ %rightclicktext @ "<br>" @ %leftclickicon @ %rightclickicon, 1);
}

function PlayerRenowned::EventideAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");
	%obj.unHideNode("chest");	
	%obj.unHideNode("rhand");
	%obj.unHideNode("lhand");
	%obj.unHideNode("rarm");
	%obj.unHideNode("larm");
	%obj.unHideNode("headskin");
	%obj.unHideNode("pants");
	%obj.unHideNode("rshoe");
	%obj.unHideNode("lshoe");	

	//Set blood colors.
	%obj.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("lhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("rhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_back", "0.7 0 0 1");
	
	if(isObject(%obj.faceConfig))
	{
		%obj.faceConfigShowFaceTimed("Neutral", -1);
	}
	%obj.setDecalName("renowneddecal");
	
	%skinColor = "0.83 0.73 0.66 1";
	%pantsColor = "0.075 0.075 0.075 1";
	%shirtColor = "0.541 0.698 0.553 1";

	%obj.setNodeColor("headskin",%skinColor);
	%obj.setNodeColor("chest",%shirtColor);
	%obj.setNodeColor("Rhand",%skinColor);
	%obj.setNodeColor("Lhand",%skinColor);
	%obj.setNodeColor("lshoe",%pantsColor);
	%obj.setNodeColor("rshoe",%pantsColor);
	%obj.setNodeColor("pants",%pantsColor);
	%obj.setNodeColor("rarm",%shirtColor);
	%obj.setNodeColor("larm",%shirtColor);
	%obj.hideNode($hat[%client.hat]);
	%obj.HideNode($accent[%client.accent]);
	%obj.HideNode($secondPack[%client.secondPack]);
	%obj.hideNode($pack[%client.pack]);
	%obj.HideNode("visor");
	%obj.HideNode("rpeg");
	%obj.HideNode("lpeg");
	%obj.mountImage("renownedeyesimage",2);

	%obj.unHideNode("renownedeyes");
}

function PlayerRenowned::onTrigger(%this, %obj, %trig, %press) 
{
	Parent::onTrigger(%this, %obj, %trig, %press);
	
	switch(%trig)
	{
		case 0: if(%press && %obj.getEnergyLevel() >= 25) return; %this.killerMelee(%obj,4);

		case 4: if(%obj.getEnergyLevel() == %this.maxEnergy)
				{
					if(%press)
					{
						%obj.playthread(2,"armReadyLeft");
						%obj.casttime = getSimTime();
						%obj.channelcasthand = %obj.schedule(500, setNodeColor, lHand, "0.8 0.8 0.5 1");
						%obj.channelcasthandimage = %obj.schedule(500,mountImage,"RenownedCastImage",2);
					}
					else
					{
						%obj.unmountImage(2);
						cancel(%obj.channelcasthand);
						cancel(%obj.channelcasthandimage);
						%this.EventideAppearance(%obj,%obj.client);

						if(%obj.casttime+500 < getSimTime())
						{								
							%start = %obj.getEyePoint();
							%end = VectorAdd(%start,VectorScale(%obj.getEyeVector(),getWord(%obj.getScale(),2)*40));
							%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::ItemObjectType;
							%search = containerRayCast (%start, %end, %mask, %obj);

							if(isObject(%search) && minigameCanDamage(%obj,%search))
							{
								%obj.client.setControlObject(%search);
								%obj.returnObserveSchedule = %obj.schedule(4000,ClearRenownedEffect);

								if(isObject(%search.client))
								{
									%search.client.centerprint("<color:FFFFFF><font:Impact:40>You are being controlled, press E to break free!",2);
								}
								
								%search.Possesser = %obj;
								%search.isPossessed = true;
								%search.mountImage("RenownedPossessedImage",3);
								%search.schedule(4000,ClearRenownedEffect);

								%obj.PossessedPlayer = %search;
								%obj.setEnergyLevel(0);
								%obj.playthread(2,"leftrecoil");
								%this.PossessDistanceCheck(%obj);								
							}
							else %obj.setEnergyLevel(%obj.getEnergyLevel()-50);
						}		
					}
				}
				else if(%press && %obj.getEnergyLevel() < 20) %obj.playthread(0,"undo");
	}	
}

function PlayerRenowned::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);
	if(%obj.getState() !$= "Dead")
	{
		%obj.playaudio(0,"renowned_pain" @ getRandom(1, 4) @ "_sound");
		%obj.faceConfigShowFace("Pain");
	}
}

function PlayerRenowned::PossessDistanceCheck(%this,%obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead" || !isObject(%obj.PossessedPlayer)) return;

	// Clear the effect when the victim is close enough to the killer
	if(VectorDist(%obj.getPosition(),%obj.PossessedPlayer.getPosition()) < 3)
	{
		%obj.ClearRenownedEffect();
		%obj.PossessedPlayer.ClearRenownedEffect();
		cancel(%obj.returnObserveSchedule);
		return;
	}
	
	%obj.PossessDistanceCheckSched = %this.schedule(100,PossessDistanceCheck,%obj);
}

function Player::ClearRenownedEffect(%obj)
{
	if(!isObject(%obj) || !(%obj.getType() & $TypeMasks::PlayerObjectType)) return;
	
	%obj.Possesser.PossessedPlayer = "";
	%obj.AntiPossession = "";
	%obj.Possesser = "";
	%obj.isPossessed = "";
	%obj.unMountImage(3);
	serverPlay3d("renowned_spellBreak_sound", %obj.getPosition());

	switch$(%obj.getclassname())
	{
		case "Player": 	%obj.client.setControlObject(%obj);
						%obj.client.camera.setMode("Observer");
		case "AIPlayer": %obj.setControlObject(%obj);
	}
}

package Eventide_RenownedSpectate
{
	function serverCmdLight(%client)
	{
		if(!isObject(%client.player)) return;
		
		// Ensure the player is valid and is the Puppet Master
	    if (%client.player.getDataBlock().getName() $= "PlayerRenowned" && isObject(Eventide_MinigameGroup))
	    {			
			if(isObject(%client.player.getMountedImage(3)) && %client.player.getMountedImage(3).getName() $= "sm_stunImage")
			return;			

	        // Populate the survivor list
	        for (%i = 0; %i < ClientGroup.getCount(); %i++)
			{			
				if (isObject(%survivor = ClientGroup.getObject(%i)))
				{
					if(%survivor == %client)
					{
						continue;
					}

					if(isObject(getMiniGameFromObject(%survivor)) && isObject(%survivor.player) && !%survivor.getDataBlock().isKiller)
					{
						%survivorList[%survivorCount++] = %survivor.player;
					}					
				}
			}
	        	        
			if (%client.player.survivorSpecIndex <= %survivorCount)
	        {
	            %currentSurvivor = %survivorList[%client.player.survivorSpecIndex];
				%client.setControlObject(%currentSurvivor.client.camera);    
				%client.player.survivorSpecIndex++;
	        }
			else
			{				
				%client.player.survivorSpecIndex = 1;								
				%client.setControlObject(%client.player);
			}

			return;
	    }
		else if(%client.player.getdataBlock().isKiller) return;

		Parent::serverCmdLight(%client);		
	}
};
if(isPackage("Eventide_RenownedSpectate")) deactivatePackage("Eventide_RenownedSpectate");
activatePackage("Eventide_RenownedSpectate");
