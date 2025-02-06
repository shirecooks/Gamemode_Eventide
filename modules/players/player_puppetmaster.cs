datablock TSShapeConstructor(PuppetMasterDTS) 
{
	baseShape = "./models/puppetmaster.dts";
	sequence0 = "./models/puppetmaster.dsq";
	sequence1 = "./models/puppetmaster_melee.dsq";
};

datablock PlayerData(PlayerPuppetMaster : PlayerRenowned) 
{
	uiName = "Puppet Master Player";
    shapeFile = PuppetMasterDTS.baseShape;
	
	hitprojectile = KillerSharpHitProjectile;
	hitobscureprojectile = KillerKatanaClankProjectile;	

	meleetrailskin = "base";
	meleetrailoffset = "0.3 1.4 0.7";
	meleetrailangle1 = "0 90 0";
	meleetrailangle2 = "0 -90 0";
	meleetrailscale = "4 4 2";

	killerlight = "NoFlareRLight";

	killerChaseLvl1Music = "musicData_Eventide_PuppetMasterNear";
	killerChaseLvl2Music = "musicData_Eventide_PuppetMasterChase";

	killeridlesound = "puppetmaster_idle";
	killeridlesoundamount = 3;

	killerchasesound = "puppetmaster_idle";
	killerchasesoundamount = 3;

	killermeleesound = "puppetmaster_melee";
	killermeleesoundamount = 3;	

	killermeleehitsound = "melee_tanto";
	killermeleehitsoundamount = 3;

	rightclickicon = "color_puppet";
	leftclickicon = "color_melee";
	
	showEnergyBar = true;
	renderFirstPerson = false;
	rechargeRate = 0.3;
	maxTools = 0;
	maxWeapons = 0;
	maxForwardSpeed = 6.25;
	maxBackwardSpeed = 3.57;
	maxSideSpeed = 5.36;

	cameramaxdist = 3;
	maxfreelookangle = 2.5;
	boundingBox = "4.8 4.8 10.1";
	crouchBoundingBox = "4.8 4.8 3.8";
	jumpForce = 0;
};

function PlayerPuppetMaster::onNewDatablock(%this,%obj)
{
    Parent::onNewDataBlock(%this,%obj);

	%obj.schedule(1,setEnergyLevel,0);
	%obj.setScale("1.15 1.15 1.15");
	%obj.mountImage("meleePuppetMasterDaggerImage",0);
	%obj.unHideNode("ALL");
}

function PlayerPuppetMaster::killerGUI(%this,%obj,%client)
{	
	%energylevel = %obj.getEnergyLevel();

	if(isObject(Eventide_MinigameGroup))
	for(%i = 0; %i < Eventide_MinigameGroup.getCount(); %i++)
    %obj.noSpawnPuppet = (isObject(%p = Eventide_MinigameGroup.getObject(%i)) && %p.getDataBlock().getName() $= "PuppetMasterPuppet" && %puppetcount++ >= 4) ? true : false;

	// Some dynamic varirables
	%leftclickstatus = (%obj.getEnergyLevel() >= %this.maxEnergy/4) ? "hi" : "lo";
	%rightclickstatus = (%obj.getEnergyLevel() >= %this.maxEnergy/2 && !%obj.noSpawnPuppet) ? "hi" : "lo";
	%leftclicktext = (%this.leftclickicon !$= "") ? "<just:left>\c6Left click" : "";
	%rightclicktext = (%this.rightclickicon !$= "") ? "<just:right>\c6Right click" : "";		

	// Regular icons
	%leftclickicon = (%this.leftclickicon !$= "") ? "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickicon @ ">" : "";
	%rightclickicon = (%this.rightclickicon !$= "") ? "<just:right><bitmap:" @ $iconspath @ %rightclickstatus @ %This.rightclickicon @ ">" : "";

	%client.bottomprint(%leftclicktext @ %rightclicktext @ "<br>" @ %leftclickicon @ %rightclickicon, 1);
}

function PlayerPuppetMaster::onTrigger(%this,%obj,%triggerNum,%bool)
{
	Parent::onTrigger(%this,%obj,%triggerNum,%bool);
	
	if(%bool) switch(%triggerNum)
	{
		case 0:	if(%obj.getEnergyLevel() >= %this.maxEnergy/4) return %this.killerMelee(%obj,4.25);
				
		case 4: if(%obj.getEnergyLevel() >= %this.maxEnergy/2)
				{
					if(!isObject(Eventide_MinigameGroup)) MissionCleanup.add(new SimGroup(Eventide_MinigameGroup));

					//Do not continue if there are already 4 puppets in the group
					for(%i = 0; %i < Eventide_MinigameGroup.getCount(); %i++)
					if(isObject(%p = Eventide_MinigameGroup.getObject(%i)) && %p.getDataBlock().getName() $= "PuppetMasterPuppet" && %puppetcount++ >= 10)
					{
						%obj.noSpawnPuppet = true;
						return;
					}
					else %obj.noSpawnPuppet = false;
					
					
					%puppet = new AIPlayer()
					{
						dataBlock = "PuppetMasterPuppet";
						source = %obj;
						sourceclient = %obj.client;
						minigame = getMiniGameFromObject(%obj);
					};

					%obj.mountObject(%puppet,8);
					Eventide_MinigameGroup.add(%puppet);
					%puppet.unmount();
					%obj.playthread(3,"leftrecoil");

					%puppet.setVelocity(vectorscale(%obj.getEyeVector(),25));
					%puppet.position = %obj.getmuzzlePoint(1);
					%puppet.schedule(2000,setActionThread,sit,1);
					%obj.client.centerprint("<font:impact:30>\c2Press your light key to control the puppet!" ,3);
					%obj.setEnergyLevel(%obj.getEnergyLevel() - 50);					
				}
	}	
}

function PlayerPuppetMaster::onPeggFootstep(%this,%obj)
{
	serverplay3d("puppetmaster_walking" @ getRandom(1,4) @ "_sound", %obj.getHackPosition());
	if(getRandom(1,5) == 5) %obj.spawnExplosion("singleBoneProjectile","1 1 1");
}

function PlayerPuppetMaster::onDamage(%this,%obj,%delta)
{
	Parent::onDamage(%this,%obj,%delta);
	%obj.playaudio(0,"puppetmaster_pain" @ getRandom(1,3) @ "_sound");
}

function PlayerPuppetMaster::onDisabled(%this,%obj,%state)
{
	Parent::onDisabled(%this,%obj,%state);
}

function PlayerPuppetMaster::onRemove(%this,%obj)
{
	Parent::onRemove(%this,%obj);
}

function PlayerPuppetMaster::EventideAppearance(%this,%obj,%client)
{
	%obj.unHideNode("ALL");	
	%bonecolor = "1 1 1 1";
	%obj.setNodeColor("head",%bonecolor);
	%obj.setNodeColor("rarmslim",%bonecolor);
	%obj.setNodeColor("larmslim",%bonecolor);
	%obj.setNodeColor("lshoe",%bonecolor);
	%obj.setNodeColor("rshoe",%bonecolor);
	%obj.setNodeColor("lhand",%bonecolor);
	%obj.setNodeColor("chest",%bonecolor);
	%obj.setNodeColor("pants",%bonecolor);
	%obj.setNodeColor("rhand",%bonecolor);
	%obj.setHeadUp(0);
}

package Eventide_PuppetMasterBotToggle
{
	function serverCmdLight(%client)
	{
		if(!isObject(%client.player)) return;

		// Ensure the player is valid and is the Puppet Master
	    if (%client.player.getDataBlock().getName() $= "PlayerPuppetMaster" && isObject(Eventide_MinigameGroup))
	    {
			if(isObject(%client.player.getMountedImage(2)) && %client.player.getMountedImage(2).getName() $= "sm_stunImage")
			return;

	        // Populate the temporary puppet list
	        for (%i = 0; %i < Eventide_MinigameGroup.getCount(); %i++)	        
			if (isObject(%puppet = Eventide_MinigameGroup.getObject(%i)) && %puppet.getDataBlock().getName() $= "PuppetMasterPuppet")
			%puppetList[%puppetCount++] = %puppet;	
	        	        
			if (%client.player.puppetIndex <= %puppetCount)
	        {
	            %currentPuppet = %puppetList[%client.player.puppetIndex];
				%client.getControlObject().schedule(1500, setActionThread, sit, 1);
				%client.setControlObject(%currentPuppet);
				%client.player.puppetIndex++;
	        }
			else
			{				
				%client.player.puppetIndex = 1;				
				%client.getControlObject().schedule(1500, setActionThread, sit, 1);
				%client.setControlObject(%client.player);
			}

			return;
	    }
		else if(%client.player.getdataBlock().isKiller) return;

		Parent::serverCmdLight(%client);		
	}
};
if(isPackage("Eventide_PuppetMasterBotToggle")) deactivatePackage("Eventide_PuppetMasterBotToggle");
activatePackage("Eventide_PuppetMasterBotToggle");