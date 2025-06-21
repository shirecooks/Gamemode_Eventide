datablock PlayerData(PlayerBlockhead666 : PlayerRenowned) 
{
	uiName = "Blockhead666 Player";

	hitprojectile = KillerRoughHitProjectile;
	hitobscureprojectile = "";
	meleetrailskin = "glitch";
	meleetrailoffset = "0.3 1.4 0.7"; 	
	meleetrailscale = "4 4 3";

	killerChaseLvl1Music = "musicData_Eventide_BlockheadNear";
	killerChaseLvl2Music = "musicData_Eventide_BlockheadChase";

	killeridlesound = "";
	killeridlesoundamount = 1;

	killerchasesound = "";
	killerchasesoundamount = 1;

	killermeleesound = "";
	killermeleesoundamount = 1;	
	
	killerweapon = "corruptedswordImage";
	killerweaponsound = "blockhead_weapon";
	killerweaponsoundamount = 1;

	killermeleehitsound = "blockhead_weaponhit";
	killermeleehitsoundamount = 1;
	
    killernearsound = "";
	killernearsoundamount = 1;

    killertauntsound = "";
    killertauntsoundamount = 1;

	killerfoundvictimsound = "";
	killerfoundvictimsoundamount = 1;

    killerlostvictimsound = "";
	killerlostvictimsoundamount = 1;

    killerthreatenedsound = "";
	killerthreatenedsoundamount = 1;

    killerdesperatesound = "";
	killerdesperatesoundamount = 1;

    killerattackedsound = "";
	killerattackedsoundamount = 1;

    killerspawnsound = "";
    killerspawnsoundamount = 1;

    killerwinsound = "";
    killerwinsoundamount = 1;

    killerlosesound = "";
    killerlosesoundamount = 1;
	
	killerlight = "NoFlareRLight";
	
	leftclickicon = "color_melee";
	rightclickicon = "color_spike";

	isKiller = true;
	rechargeRate = 0.35;
	maxDamage = 666;
	maxTools = 1;
	maxWeapons = 1;
	maxForwardSpeed = 7.55;
	maxBackwardSpeed = 4.35;
	maxSideSpeed = 6.5;
	jumpForce = 0;

    gazeTickRate = 50;
};

function PlayerBlockhead666::onTrigger(%this, %obj, %trig, %press) 
{		
	Parent::onTrigger(%this, %obj, %trig, %press);
	
	switch(%trig)
	{
		case 0: if(%obj.getEnergyLevel() >= 25 && %press)
		{
			%this.killerMelee(%obj,4);
			%obj.faceConfigShowFace("Attack");
			return;
		}
		
		case 4: if(%obj.getEnergyLevel() >= %this.maxEnergy/1)
				if(%press)
				{				
					%obj.casttime = getSimTime();
					%obj.channelcasthandimage = %obj.schedule(750,mountImage,GlitchAmbientImage,2);
					serverPlay3d("GlitchSpikeCharge_sound", %obj.getEyePoint());
					%obj.setTempSpeed(0.4);
					%obj.playthread(2,"armReadyLeft");
				}
				else
				{
					%obj.unmountImage(2);
					%obj.setTempSpeed(1);
					cancel(%obj.channelcasthandimage);
					%obj.playthread(2,"root");
			
					if(%obj.casttime+750 < getSimTime())
					{
						%obj.setEnergyLevel(%obj.getEnergyLevel()-%this.maxEnergy/1);
						%obj.playthread(2,"leftrecoil");
						%obj.setTempSpeed(1);
						serverPlay3d("GlitchSpikeThrow_sound", %obj.getEyePoint());
			
						%p = new projectile()
						{
							dataBlock = "GlitchProjectile";
							initialVelocity = vectorScale(%obj.getEyeVector(),50);
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

function PlayerBlockhead666::onPeggFootstep(%this,%obj)
{
	serverplay3d("blockhead_walking" @ getRandom(1,6) @ "_sound", %obj.getHackPosition());
}

function PlayerBlockhead666::onNewDatablock(%this,%obj)
{
		//Face system functionality.
	%obj.createFaceConfig($Eventide_FacePacks["blockhead"]);
	%obj.faceConfig.setFaceAttribute("Pain", "length", 1000);
	%obj.faceConfig.setFaceAttribute("Blink", "length", 500);
	
	Parent::onNewDatablock(%this,%obj);
	%obj.mountImage(%this.killerweapon, 0);
	%obj.mountImage("Blockhead666Image", 1);
	%obj.setScale("1 1 1");

    %obj.gazeTickRate = %this.gazeTickRate;
    %obj.Blockhead666Gaze();
}

function PlayerBlockhead666::onRemove(%this, %obj)
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

function PlayerBlockhead666::EventideAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");	
	%obj.unhideNode("pants");
	%obj.unhideNode("headskin");
	%obj.unhideNode("larm");
	%obj.unhideNode("rarmslim");
	%obj.unhideNode("rarm");
	%obj.unhideNode("rshoe");
	%obj.unhideNode("lshoe");
	%obj.unhideNode("lhand");
	%obj.unhideNode("rhand");
	%obj.unhideNode("rhook");
	%obj.unhideNode("chest");

	%shirtColor = "1 1 1 1";
	%pantsColor = "0.2 0 0.8 1";
	%sleevesColor = "0.95 0 0 1";
	%skinColor = "1 0.88 0.61 1";
	%glitchAColor = "1 0.88 0.61 0.4";
	%glitchBColor = "0.95 0 0 0.5";
	%glitchCColor = "0.2 0 0.8 0.5";

	%obj.setDecalName("none");
	%obj.setNodeColor("rarm",%glitchBColor);
	%obj.setNodeColor("larm",%sleevesColor);
	%obj.setNodeColor("rarmslim",%sleevesColor);
	%obj.setNodeColor("chest",%shirtColor);
	%obj.setNodeColor("pants",%pantsColor);
	%obj.setNodeColor("rshoe",%glitchCColor);
	%obj.setNodeColor("lshoe",%pantsColor);
	%obj.setNodeColor("rhand",%glitchAColor);
	%obj.setNodeColor("rhook",%glitchAColor);
	%obj.setNodeColor("lhand",%skinColor);
	%obj.setNodeColor("headskin",%skinColor);
	
	//Set blood colors.
	%obj.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("lhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("rhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_back", "0.7 0 0 1");
}

//
// Voice-line handlers.
//

function PlayerBlockhead666::onKillerChaseStart(%this, %obj, %chasing)
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

function PlayerBlockhead666::onKillerChase(%this, %obj, %chasing)
{
	if(!%chasing)
    {
        //A victim is nearby but Postal Dude can't see them yet. Say some quips.
        %soundType = %this.killernearsound;
        %soundAmount = %this.killernearsoundamount;
        if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + getRandom(15000, 25000))))
        {
            %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
            %obj.lastKillerSoundTime = getSimTime();
        }
    }
}

function PlayerBlockhead666::onKillerChaseEnd(%this, %obj)
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

function PlayerBlockhead666::onExitStun(%this, %obj)
{
	%soundType = %this.killerattackedsound;
    %soundAmount = %this.killerattackedsoundamount;
    if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerBlockhead666::onAllRitualsPlaced(%this, %obj)
{
    %soundType = %this.killerdesperatesound;
    %soundAmount = %this.killerdesperatesoundamount;
    if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerBlockhead666::onRoundEnd(%this, %obj, %won)
{
    //Plays a taunt if Postal Dude wins, or despair if he loses.
    if(%won)
    {
        return;
    }

    %soundType = %won ? %this.killerwinsound : %this.killerlosesound;
    %soundAmount = %won ? %this.killerwinsoundamount : %this.killerlosesoundamount;
    if(%soundType !$= "")
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerBlockhead666::onIncapacitateVictim(%this, %obj, %victim, %killed)
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
}

function Player::Blockhead666Gaze(%obj)
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

		//They have an item equipped and it's a weapon, have Postal Dude react to it.
		if(%victimEquippedItem.isWeapon || %victimEquippedItem.className $= "WeaponImage")
		{
			%soundType = %killerDatablock.killerthreatenedsound;
			%soundAmount = %killerDatablock.killerthreatenedsoundamount;
			if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
			{
				%obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
				%obj.lastKillerSoundTime = getSimTime();
				%obj.threatsReceived.add(%foundPlayer); //Ensure Postal Dude does not acknowledge any further weapons. Less annoying.
			}
		}

		continue;
	}

    %obj.schedule(%obj.gazeTickRate, Blockhead666Gaze);
}

function PlayerBlockhead666::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);
	if(%obj.getState() !$= "Dead")
	{
		%obj.playaudio(0,"blockhead_pain" @ getRandom(1, 3) @ "_sound");
		%obj.faceConfigShowFace("Pain");
	}
}