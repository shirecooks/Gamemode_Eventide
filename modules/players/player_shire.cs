datablock PlayerData(PlayerShire : PlayerRenowned) 
{
	uiName = "Shire Player";
	isKiller = true;
	
	// Weapon: Axe
	hitprojectile = KillerSharpHitProjectile;
	hitobscureprojectile = KillerAxeClankProjectile;
	
	meleetrailoffset = "0.6 1.4 0.7";

	rightclickicon = "color_blind";
	leftclickicon = "color_melee";

	killerChaseLvl1Music = "musicData_Eventide_HexNear";
	killerChaseLvl2Music = "musicData_Eventide_HexChase";

	killeridlesound = "";
	killeridlesoundamount = 1;

	killerchasesound = "";
	killerchasesoundamount = 1;
	
	killernearsound = "shire_looking";
	killernearsoundamount = 9;

    killertauntsound = "shire_kill";
    killertauntsoundamount = 2;

	killerfoundvictimsound = "shire_foundvictim";
	killerfoundvictimsoundamount = 3;

    killerlostvictimsound = "shire_lostvictim";
	killerlostvictimsoundamount = 2;

    killerthreatenedsound = "shire_threatened";
	killerthreatenedsoundamount = 2;

    killerattackedsound = "shire_attacked";
	killerattackedsoundamount = 2;

	killermeleesound = "shire_melee";
	killermeleesoundamount = 3;	
	
	killerweaponsound = "shire_weapon";
	killerweaponsoundamount = 5;

	killermeleehitsound = "melee_tanto";
	killermeleehitsoundamount = 3;
	
	killerlight = "NoFlarePLight";

	rechargeRate = 0.3;
	maxDamage = 696;
	maxForwardSpeed = 6.55;
	maxBackwardSpeed = 3.74;
	maxSideSpeed = 5.61;

	gazeTickRate = 50;
};

function DarknessProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal)
{
	Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal);
    if((%col.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj.sourceObject,%col) == 1) 
	{
		%col.mountImage("DarkBlindPlayerImage",3);
	}
}

function DarkBlindPlayerImage::onBlind(%this, %obj, %slot)
{
	%obj.ShireBlind++;
	%obj.setDamageFlash(1);	

	if(%obj.ShireBlind >= 3)
	{
		%obj.unmountImage(3);
		return;
	}
}

function DarkBlindPlayerImage::onMount(%this, %obj, %slot)
{
	%obj.playaudio(3,"shire_blind_sound");	
	%obj.shireZombify = true;
	parent::onMount(%this, %obj, %slot);
}

function DarkBlindPlayerImage::onunMount(%this, %obj, %slot)
{	
	parent::onunMount(%this, %obj, %slot);
	%obj.ShireBlind = 0;
}

function DarkBlindPlayerImage::killerGUI(%this,%obj,%client)
{	
	if (!isObject(%obj) || !isObject(%client))
	{
		return;
	}	

	// Some dynamic varirables
	%energylevel = %obj.getEnergyLevel();
	%leftclickstatus = (%energylevel >= %this.maxEnergy/4) ? "hi" : "lo";
	%rightclickstatus = (%energylevel >= %this.maxEnergy/2) ? "hi" : "lo";
	%leftclicktext = (%this.leftclickicon !$= "") ? "<just:left>\c6Left click" : "";
	%rightclicktext = (%this.rightclickicon !$= "") ? "<just:right>\c6Right click" : "";	

	// Regular icons
	%leftclickicon = (%this.leftclickicon !$= "") ? "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickicon @ ">" : "";
	%rightclickicon = (%this.rightclickicon !$= "") ? "<just:right><bitmap:" @ $iconspath @ %rightclickstatus @ %This.rightclickicon @ ">" : "";

	%client.bottomprint(%leftclicktext @ %rightclicktext @ "<br>" @ %leftclickicon @ %rightclickicon, 1);
}

//
// Voice-line handlers.
//

function PlayerShire::onKillerChaseStart(%this, %obj, %chasing)
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
    if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerShire::onKillerChase(%this, %obj, %chasing)
{
	if(!%chasing && !%obj.isInvisible)
    {
        //A victim is nearby but Shire can't see them yet. Say some quips.
        %soundType = %this.killernearsound;
        %soundAmount = %this.killernearsoundamount;
        if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + getRandom(15000, 25000))))
        {
            %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
            %obj.lastKillerSoundTime = getSimTime();
        }
    }
}

function PlayerShire::onKillerChaseEnd(%this, %obj)
{
	//If Sky Captain doesn't get any kills during a chase, play a voice line marking his dismay.
    if(%obj.incapsAchieved.getCount() == 0 && !%obj.isInvisible)
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

function PlayerShire::onIncapacitateVictim(%this, %obj, %victim, %killed)
{
    parent::onIncapacitateVictim(%this, %obj, %victim, %killed);

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
}

function PlayerShire::onExitStun(%this, %obj)
{
	%soundType = %this.killerattackedsound;
    %soundAmount = %this.killerattackedsoundamount;
    if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerShire::onTrigger(%this, %obj, %trig, %press) 
{
	Parent::onTrigger(%this, %obj, %trig, %press);
		
	switch(%trig)
	{
		case 0: if(%obj.getEnergyLevel() >= 25 && %press)
		{
			%this.killerMelee(%obj,4.5);
			%obj.faceConfigShowFace("Attack");
			return;
		}
		
		case 4: if(%obj.getEnergyLevel() >= %this.maxEnergy/1)
				if(%press)
				{
					%obj.mountImage(GlowFaceImage, 1);					
					%obj.casttime = getSimTime();
					%obj.channelcasthand = %obj.schedule(250, setNodeColor, lHand, "0.5 0.33 0.68 1");
					%obj.channelcasthandimage = %obj.schedule(250,mountImage,DarkCastImage,2);
				}
				else
				{
					%obj.unmountImage(1);
					%obj.unmountImage(2);
					cancel(%obj.channelcasthand);
					cancel(%obj.channelcasthandimage);
					%obj.setNodeColor(lHand, "1 1 1 1");
			
					if(%obj.casttime+250 < getSimTime())
					{
						%obj.setEnergyLevel(%obj.getEnergyLevel()-%this.maxEnergy/1);
						%obj.playthread(2,"leftrecoil");
						serverPlay3d("shire_cast_sound", %obj.getEyePoint());
			
						%p = new projectile()
						{
							dataBlock = "DarknessProjectile";
							initialVelocity = vectorScale(%obj.getEyeVector(),200);
							initialPosition = vectorAdd(%obj.getEyePoint(),"0 0 0.45");
							sourceObject = %obj;
							client = %obj.client;
						};
						MissionCleanup.add(%p);						
					}		
				}
		default:
	}	
}

function PlayerShire::onPeggFootstep(%this,%obj)
{
	serverplay3d("shire_walking" @ getRandom(1,6) @ "_sound", %obj.getHackPosition());
}

function PlayerShire::onNewDatablock(%this, %obj)
{
	//Face system functionality.
	%obj.createFaceConfig($Eventide_FacePacks["shire"]);
	%obj.faceConfig.setFaceAttribute("Attack", "length", 500);
	%obj.faceConfig.setFaceAttribute("Pain", "length", 1000);
	%obj.faceConfig.setFaceAttribute("Blink", "length", 100);

	//Everything else.
	Parent::onNewDatablock(%this,%obj);	

	%obj.setScale("1.15 1.15 1.15");
	%obj.mountImage("meleeAxeImage",0);
	%obj.mountImage("newhoodieimage",3,2,addTaggedString("darkpurple"));

	//Start the gaze loop.
	%obj.gazeTickRate = %this.gazeTickRate;
	%obj.ShireGaze();
}

function PlayerShire::onRemove(%this, %obj)
{
    if(isObject(%obj.incapsAchieved))
    {
        %obj.incapsAchieved.delete();
    }
    if(isObject(%obj.threatsReceived))
    {
        %obj.threatsReceived.delete();
    }
    parent::onRemove(%this, %obj);
}

function PlayerShire::EventideAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");	
	%obj.unhideNode("pants");
	%obj.unhideNode("headskin");
	%obj.unhideNode("larm");
	%obj.unhideNode("rarm");
	%obj.unhideNode("rshoe");
	%obj.unhideNode("lshoe");
	%obj.unhideNode("lhand");
	%obj.unhideNode("rhand");
	%obj.unhideNode("femchest");

	%hoodieColor = "0.22 0.11 0.3 1";
	%pantsColor = "0.075 0.075 0.075 1";
	%skinColor = "1 1 1 1";
	
	%obj.setDecalName("robe");
	%obj.setNodeColor("rarm",%hoodieColor);
	%obj.setNodeColor("larm",%hoodieColor);
	%obj.setNodeColor("femchest",%hoodieColor);
	%obj.setNodeColor("pants",%pantsColor);
	%obj.setNodeColor("rshoe",%pantsColor);
	%obj.setNodeColor("lshoe",%pantsColor);
	%obj.setNodeColor("rhand",%skinColor);
	%obj.setNodeColor("lhand",%skinColor);
	%obj.setNodeColor("headskin",%skinColor);
	%obj.unHideNode("hoodie2");
	%obj.setNodeColor("hoodie2",%hoodieColor);

}

function Player::ShireGaze(%obj)
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

		//They have an item equipped and it's a weapon, have Shire react to it.
		if(%victimEquippedItem.isWeapon || %victimEquippedItem.className $= "WeaponImage")
		{
			%soundType = %killerDatablock.killerthreatenedsound;
			%soundAmount = %killerDatablock.killerthreatenedsoundamount;
			if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
			{
				%obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
				%obj.lastKillerSoundTime = getSimTime();
				%obj.threatsReceived.add(%foundPlayer); //Ensure Shire does not acknowledge any further weapons. Less annoying.
			}
		}

		continue;
	}

	%obj.schedule(%obj.gazeTickRate, ShireGaze);
}

function PlayerShire::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);

	if(%obj.getState() !$= "Dead")
	{
		%obj.playaudio(0,"shire_pain" @ getRandom(1,5) @ "_sound");
		%obj.faceConfigShowFace("Pain");
	}
}