datablock PlayerData(PlayerGrabber : PlayerRenowned) 
{
	uiName = "Grabber Player";
	
	hitprojectile = KillerRoughHitProjectile;
	hitobscureprojectile = KillerMacheteClankProjectile;
	meleetrailskin = "ragged";	

	killerChaseLvl1Music = "musicData_Eventide_MaskedNear";
	killerChaseLvl2Music = "musicData_Eventide_MaskedChase";
	
	killermeleehitsound = "melee_tanto";
	killermeleehitsoundamount = 3;

	killermeleesound = "";
	killermeleesoundamount = 0;
	
	killerweaponsound = "grabber_weapon";
	killerweaponsoundamount = 5;	

	killeridlesound = "grabber_breathe";
	killeridlesoundamount = 1;

	killerchasesound = "grabber_breathe";
	killerchasesoundamount = 1;	
	
	killerlight = "NoFlareRLight";

	rightclickicon = "color_grab";
	leftclickicon = "color_melee";

	firstpersononly = false;
	rechargeRate = 0.5;
	maxDamage = 1043; //1200
	maxTools = 0;
	maxWeapons = 0;
	maxForwardSpeed = 6.55;
	maxBackwardSpeed = 3.74;
	maxSideSpeed = 5.61;
	cameramaxdist = 3;
	maxfreelookangle = 2.5;
	jumpForce = 0;
};

datablock PlayerData(PlayerGrabberNoJump : PlayerGrabber) 
{
	uiName = "";
	jumpForce = 0;
};

datablock ShapeBaseImageData(MaskedImage) 
{
	shapeFile = "Add-Ons/Gamemode_Eventide/modules/players/models/hallow.dts";
	mountPoint = $HeadSlot;
	offset = "0 -0.015 -0.41";
	eyeOffset = "0 0 -1000";
	rotation = eulerToMatrix("0 0 0");
	scale = "0.1 0.1 0.1";
	colorShiftColor = "0.55 0.32 0.04 1";
	emap = 0;
};

function PlayerGrabber::onNewDatablock(%this,%obj)
{
	Parent::onNewDatablock(%this,%obj);

	%obj.schedule(10,onKillerLoop);
	
	if(!isObject(%obj.client)) applyDefaultCharacterPrefs(%obj);
	else applyCharacterPrefs(%obj.client);
	%obj.schedule(1,setEnergyLevel,0);
	%obj.setScale("1.15 1.15 1.15");

	%obj.mountImage("meleeMacheteImage",0);
}

function PlayerGrabber::onPeggFootstep(%this,%obj)
{
	serverplay3d("grabber_walking" @ getRandom(1,5) @ "_sound", %obj.getHackPosition());
}

function PlayerGrabber::checkVictim(%this,%obj)
{
	if(!%obj.victim) 
	{
		%obj.playthread(3,"root");
	}
	%obj.setdatablock("PlayerGrabber");
}

function PlayerGrabber::onTrigger(%this,%obj,%triggerNum,%bool)
{	
	Parent::onTrigger(%this,%obj,%triggerNum,%bool);
	
	if(%bool && %obj.getState() !$= "Dead")
	{
		switch(%triggerNum)
		{
			case 0: if(%obj.getEnergyLevel() >= 25)
					{
						%this.killerMelee(%obj,4.5);
					}

			case 4: if(!isObject(%obj.victim))
					{
						if(!%obj.isCrouched() && %obj.getEnergyLevel() >= %this.maxEnergy && getWord(%obj.getVelocity(),2) == 0)
						{
							%obj.setdatablock("PlayerGrabberNoJump");					
							%obj.setVelocity(vectorscale(%obj.getForwardVector(),27));
							%obj.playaudio(3,"grabber_lunge_sound");
							%obj.playthread(3,"armReadyLeft");
							%this.schedule(500,checkVictim,%obj);
						}
					}
					else if(%obj.lastChokeTime < getSimTime() && isObject(%obj.victim))
					{					
						if(%obj.ChokeAmount < 4)
						{
							if(isObject(%obj.victim) && %obj.victim.getState() !$= "Dead") 
							{
								%obj.victim.damage(%obj, %obj.getmuzzlePoint(1), 16, $DamageType::Default);
							}

							%obj.lastChokeTime = getSimTime()+250;	
							%obj.playthread(0,"plant");
							%obj.ChokeAmount++;
						}
						else
						{
							%this.releaseVictim(%obj);
						}
					}

			default:
		}
	}
}

function PlayerGrabberNoJump::releaseVictim(%this,%obj)
{
	PlayerGrabber::releaseVictim(%this,%obj);
}

function PlayerGrabber::releaseVictim(%this,%obj)
{
	if(!isObject(%obj) || !isObject(%obj.victim))
	{
		return;
	}
	
	%obj.ChokeAmount = 0;
	%obj.victim.stunned = false;	
	%obj.setEnergyLevel(0);
	%obj.victim.unmount();
	%obj.victim.setarmthread("look");
	%obj.victim.playthread(0,"root");
	%obj.playthread(3,"leftrecoil");
	%obj.victim.setVelocity(vectorscale(vectorAdd(%obj.getEyeVector(),"0 0 0.005"),25));				
	%obj.victim.position = %obj.getHackPosition();

	switch$(%obj.victim.getClassName())
	{
		case "AIPlayer":	%obj.victim.startholeloop();
							%obj.victim.hRunAwayFromPlayer(%obj);
		case "Player":	%obj.victim.client.schedule(100,setControlObject,%obj.victim);
	}
	
	%this.setStun(%obj,true);
	%this.schedule(2000,setStun,%obj,false);
	
	%obj.victim.killer = 0;
	%obj.victim = 0;
}

function PlayerGrabber::setStun(%this,%obj,%bool)
{
	if(!isObject(%obj) || !isObject(%client = %obj.client))
	{
		return;
	}

	%obj.stunned = %bool;

	switch(%bool)
	{
		case true: 	%obj.client.setControlObject(%obj.client.camera);
					%obj.client.camera.setMode("Corpse",%obj);

		case false:	%obj.client.setControlObject(%obj);
					%obj.client.camera.setMode("Observer",%obj);
	}
	
}


function PlayerGrabber::EventideAppearance(%this,%obj,%client)
{
	%funcclient = (%obj.isSkinwalker && isObject(%obj.victimreplicatedclient)) ? %obj.victimreplicatedclient : %client;
	
	%obj.hideNode("ALL");
	%obj.unHideNode("chest");	
	%obj.unHideNode("rhand");
	%obj.unHideNode("lhand");
	%obj.unHideNode((%funcclient.rarm ? "rarmSlim" : "rarm"));
	%obj.unHideNode((%funcclient.larm ? "larmSlim" : "larm"));
	%obj.unHideNode("headskin");

	if($pack[%funcclient.pack] !$= "none")
	{
		%obj.unHideNode($pack[%funcclient.pack]);
		%obj.setNodeColor($pack[%funcclient.pack],%funcclient.packColor);
	}
	if($secondPack[%funcclient.secondPack] !$= "none")
	{
		%obj.unHideNode($secondPack[%funcclient.secondPack]);
		%obj.setNodeColor($secondPack[%funcclient.secondPack],%funcclient.secondPackColor);
	}	
		
	%obj.unHideNode("pants");
	%obj.unHideNode("rshoe");
	%obj.unHideNode("lshoe");	

	%obj.setHeadUp((%funcclient.pack+%funcclient.secondPack > 0));

	if(%obj.bloody["lshoe"]) %obj.unHideNode("lshoe_blood");
	if(%obj.bloody["rshoe"]) %obj.unHideNode("rshoe_blood");
	if(%obj.bloody["lhand"]) %obj.unHideNode("lhand_blood");
	if(%obj.bloody["rhand"]) %obj.unHideNode("rhand_blood");
	if(%obj.bloody["chest_front"]) %obj.unHideNode((%funcclient.chest ? "fem" : "") @ "chest_blood_front");
	if(%obj.bloody["chest_back"]) %obj.unHideNode((%funcclient.chest ? "fem" : "") @ "chest_blood_back");
	%obj.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("lhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("rhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_back", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_back", "0.7 0 0 1");	

	%obj.setDecalName("classicshirt");
	%shirtColor = "0.28 0.21 0.12 1";
	%pantsColor = "0.075 0.075 0.075 1";
	%skinColor = "0.83 0.73 0.66 1";
	%obj.HideNode($secondPack[%client.secondPack]);
	%obj.hideNode($pack[%client.pack]);
	%obj.HideNode("visor");
	%obj.mountImage("MaskedImage",2);
	%obj.setNodeColor("headskin",%skinColor);
	%obj.setNodeColor("Rhand",%skinColor);
	%obj.setNodeColor("Lhand",%skinColor);
	%obj.setNodeColor((%client.rarm ? "rarmSlim" : "rarm"),%shirtColor);
	%obj.setNodeColor((%client.larm ? "larmSlim" : "larm"),%shirtColor);
	%obj.setNodeColor("chest",%shirtColor);
	%obj.setNodeColor("pants",%pantsColor);
	%obj.setNodeColor("rshoe",%pantsColor);
	%obj.setNodeColor("lshoe",%pantsColor);
	//Set blood colors.
	%obj.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("lhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("rhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_back", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("femchest_blood_back", "0.7 0 0 1");
//	%obj.unHideNode("jasonmask");
//	%obj.setNodeColor("jasonmask","1 1 1 1");
	%obj.setHeadUp(0);
}

function PlayerGrabberNoJump::onCollision(%this,%obj,%col,%vec,%speed)
{
	Parent::onCollision(%this,%obj,%col,%vec,%speed);

	// Do not continue if any of these conditions are met.
	if(isObject(%obj.victim) || !(%col.getType() & $TypeMasks::PlayerObjectType) || !minigameCanDamage(%obj,%col) || %col.getdataBlock().isDowned)
	{
		return;
	}
			
	if(isObject(%obj.client))
	{
		ServerCmdUnUseTool(%obj.client);
	}
	
	%obj.victim = %col;
	%col.killer = %obj;
	%col.stunned = true;
	%obj.mountObject(%col,8);
	%col.setarmthread("activate2");
	PlayerGrabber.schedule(4500,"releaseVictim",%obj);

	switch$(%col.getClassName())
	{
		case "AIPlayer": %col.stopholeloop();
		case "Player": 	%col.client.camera.setOrbitMode(%col, %col.getTransform(), 0, 5, 0, 1);
						%col.client.setControlObject(%col.client.camera);
	}	
}

function PlayerGrabberNoJump::onNewDatablock(%this,%obj)
{
	PlayerGrabber::onNewDatablock(%this,%obj);
}

function PlayerGrabberNoJump::checkVictim(%this,%obj)
{
	PlayerGrabber::checkVictim(%this,%obj);
	%obj.setdatablock("PlayerGrabber");
}

function PlayerGrabberNoJump::onTrigger(%this,%obj,%triggerNum,%bool)
{	
	PlayerGrabber::onTrigger(%this,%obj,%triggerNum,%bool);
}

function PlayerGrabberNoJump::EventideAppearance(%this,%obj,%client)
{
	PlayerGrabber::EventideAppearance(%this,%obj,%client);
}