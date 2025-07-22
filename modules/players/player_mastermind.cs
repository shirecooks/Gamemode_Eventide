datablock PlayerData(PlayerMastermind : PlayerRenowned) 
{
	uiName = "Mastermind Player";

	hitprojectile = KillerRoughHitProjectile;
	hitobscureprojectile = "";
	meleetrailskin = "ragged";
	meleetrailoffset = "0.3 1.4 0.7"; 	
	meleetrailscale = "4 4 3";

	killerChaseLvl1Music = "musicData_Eventide_MastermindNear";
	killerChaseLvl2Music = "musicData_Eventide_MastermindChase";

	killeridlesound = "";
	killeridlesoundamount = 1;

	killerchasesound = "";
	killerchasesoundamount = 1;

	killermeleesound = "mastermind_melee";
	killermeleesoundamount = 5;	

	killermeleehitsound = "melee_tanto";
	killermeleehitsoundamount = 3;
	
    killernearsound = "mastermind_looking";
	killernearsoundamount = 5;

    killertauntsound = "mastermind_kill";
    killertauntsoundamount = 7;

	killerfoundvictimsound = "mastermind_foundvictim";
	killerfoundvictimsoundamount = 5;

    killerlostvictimsound = "mastermind_lostvictim";
	killerlostvictimsoundamount = 5;

    killerthreatenedsound = "mastermind_threatened";
	killerthreatenedsoundamount = 4;

    killerdesperatesound = "";
	killerdesperatesoundamount = 1;

    killerattackedsound = "mastermind_attacked";
	killerattackedsoundamount = 5;

    killerspawnsound = "mastermind_spawn";
    killerspawnsoundamount = 3;

    killerwinsound = "mastermind_win";
    killerwinsoundamount = 2;

    killerlosesound = "mastermind_lose";
    killerlosesoundamount = 2;
	
	killerlight = "NoFlarePLight";
	
	mastermindDash = true;
	mastermindDashZ = 0;
	mastermindDashCost = 100;
	mastermindDashDelay = 500;
	mastermindDashVel = 40;
	mastermindDashTime = 250;
	
	leftclickicon = "color_melee";
	rightclickicon = "color_slowmo";

	rechargeRate = 0.32;
	maxDamage = 1000; //1000
	maxTools = 0;
	maxWeapons = 0;
	maxForwardSpeed = 7.35;
	maxBackwardSpeed = 4.2;
	maxSideSpeed = 6.3;
	jumpForce = 0;

    gazeTickRate = 50;
};

function PlayerMastermind::onImpact(%this,%obj,%hit,%vec,%force)
{
	if(%obj.ismastermindDash && (%this.mastermindDashZ || mAbs(getWord(%vec,2)) < %this.minImpactSpeed)) 
	{
		return;
	}
	
	Parent::onImpact(%this,%obj,%hit,%vec,%force);
}

function PlayerMastermind::onTrigger(%this, %obj, %trig, %press)
{
	Parent::onTrigger(%this, %obj, %trig, %press);
		
	if(%press)
	{
		switch(%trig)
		{
			case 0: if(%obj.getEnergyLevel() >= 25)
					{
						%this.killerMelee(%obj,4);
						return;
					}
			case 4: if(!isObject(%obj.getObjectMount()))
					{
						%obj.mastermindDashStart();
					}					
		}
	}
}

function PlayerMastermind::onNewDatablock(%this,%obj)
{
	Parent::onNewDatablock(%this,%obj);
	%obj.mountImage("overcoatMountedImage",1);
	%obj.setScale("1 1 1");

    %obj.gazeTickRate = %this.gazeTickRate;
    %obj.MastermindGaze();
}

datablock ParticleData(mastermind_trailParticle)
{
	textureName				= "./models/blockhead";
	lifetimeMS				= 500;
	lifetimeVarianceMS		= 0;
	dragCoefficient			= 0.0;
	windCoefficient			= 0.0;
	gravityCoefficient		= 0.0;
	inheritedVelFactor		= 0.0;
	constantAcceleration	= 0.0;
	spinRandomMin			= 0.0;
	spinRandomMax			= 0.0;
	colors[0]				= "0 0 0 0.4";
	colors[1]				= "0 0 0 0.1";
	colors[2]				= "0 0 0 0";
	sizes[0]				= 2.6;
	sizes[1]				= 2.6;
	sizes[2]				= 2.6;
	times[0]				= 0;
	times[1]				= 0.5;
	times[2]				= 1.0;
	useInvAlpha				= true;
};
datablock ParticleEmitterData(mastermind_trailEmitter)
{
	uiName				= "";
	particles			= "mastermind_trailParticle";
	ejectionPeriodMS	= 10;
	periodVarianceMS	= 0;
	ejectionVelocity	= 0.0;
	velocityVariance	= 0.0;
	ejectionOffset		= 0.0;
	thetaMin			= 0.0;
	thetaMax			= 0.0;
	phiReferenceVel		= 0.0;
	phiVariance			= 0.0;
};
datablock ShapeBaseImageData(mastermind_trailImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;
	
	mountPoint = $BackSlot;

	offset = "0 -0.6 -0.5";
	eyeOffset = "0 0 -9999";
	
	stateName[0] 					= "Idle";
	stateTransitionOnTimeout[0] 	= "Idle2";
	stateTimeoutValue[0]	 		= 1000;
	stateEmitter[0]					= mastermind_trailEmitter;
	stateEmitterTime[0]				= 1000;
	stateEmitterNode[0]				= "muzzleNode";	
	
	stateName[1] 					= "Idle2";
	stateTransitionOnTimeout[1] 	= "Idle";
	stateTimeoutValue[1]	 		= 1000;
	stateEmitter[1]					= mastermind_trailEmitter;
	stateEmitterTime[1]				= 1000;
	stateEmitterNode[1]				= "muzzleNode";	
};
function rgbGradient(%step, %c1, %c2)
{
	%r1 = getWord(%c1, 0);
	%g1 = getWord(%c1, 1);
	%b1 = getWord(%c1, 2);

	%r2 = getWord(%c2, 0);
	%g2 = getWord(%c2, 1);
	%b2 = getWord(%c2, 2);

	%r3 = %r1 + %step * (%r2 - %r1);
	%g3 = %g1 + %step * (%g2 - %g1);
	%b3 = %b1 + %step * (%b2 - %b1);

	return %r3 SPC %g3 SPC %b3;
}
function player::mastermindDashStart(%pl)
{
	%db = %pl.getDatablock();
	if(!%pl.ismastermindDash)
	{
		if((%time = getSimTime()-%pl.lastmastermindDash) > %db.mastermindDashDelay)
		{
			if((%energy = %pl.getEnergyLevel()) >= (%cost = %db.mastermindDashCost))
			{
				if(isObject(%cl = %pl.client))
				{
					%pl.setDamageFlash(0.1);
					%pl.mastermindFov = %cl.getControlCameraFov();
				}
				cancel(%pl.mastermindFizzleSched);
				%pl.lastmastermindDash = getSimTime();
				%pl.mastermindDashSound = %pl.mastermindCancelDash = %pl.mastermindHasWastedEnergy = false;
				%vec = vectorScale((%db.mastermindDashZ ? %pl.getEyeVector() : %pl.getForwardVector()),%db.mastermindDashVel);
				%pl.ismastermindDash = 1;
				%pl.mastermindDash(%vec,0);
				%pl.setEnergyLevel(%pl.mastermindEnergy = (%energy-%cost-((%db.rechargeRate*32)*(0.00065*%db.mastermindDashDelay))));
				%pl.playaudio(3,"mastermind_teleport_sound");
				%pl.playaudio(2,"mastermind_melee1");
				%pl.mountImage(mastermind_trailImage,3);
				%pl.playThread(0,plant);
			}
			else if(getSimTime()-%pl.lastmastermindDash > 260) %error = 1;
		}
		else %error = 2;
	}
	else %error = 0;
	if(%error)
	{
		if(getSimTime()-%pl.lastmastermindInvalid > 400)
		{
			%pl.lastmastermindInvalid = getSimTime();
			%time = (getSimTime()-%pl.lastmastermindDash);
			if(%error == 2)
			{
				if(!%pl.mastermindHasWastedEnergy && %time > %db.mastermindDashDelay-(%db.mastermindDashDelay*0.8))
				{
					%pl.setEnergyLevel(%pl.mastermindEnergy = (%pl.getEnergyLevel()-(%db.mastermindDashCost*0.5)));
					%pl.mastermindHasWastedEnergy = 1;
				}
			}
			else if(%error == 1)
			{
				%pl.mastermindFizzleSched = %pl.schedule(100,unMountImage,3);
				%pl.playThread(0,plant);
				%pl.playThread(3,undo);
				if(isObject(%cl = %pl.client)) %pl.playaudio(3,"mastermind_invalid_sound");
			}
		}
	}
}
function player::mastermindDash(%pl,%vec,%tick)
{
	%db = %pl.getDatablock();
	%forward = vectorScale((%db.mastermindDashZ ? %pl.getEyeVector() : %pl.getForwardVector()),0.8);
	%pos = vectorAdd(%pl.getPosition(),"0 0 0.2");
	%hit = containerRayCast(%pos,vectorAdd(%pos,%forward),$TypeMasks::FxBrickObjectType | $TypeMasks::StaticObjectType | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType,%pl);
	if(%hit || %pl.getState() $= "Dead")
		%pl.mastermindCancelDash = 1;
	%vel = %pl.getVelocity();
	%x = getWord(%vel,0);
	%y = getWord(%vel,1);
	%z = getWord(%vel,2);
	if(%pl.mastermindCancelDash && %tick > 0) %pl.unMountImage(3);
	
	%time = getSimTime()-%pl.lastmastermindDash;
	if(!%pl.mastermindDashSound && (%time > (%max = %db.mastermindDashTime)-100 || %pl.mastermindCancelDash && %tick > 4))
	{
		//%pl.schedule(1,playAudio,2,mastermind_airRush @ getRandom(1,2) @ Sound);
		//%pl.mastermindDashSound = 1;
	}
	if(%time > %max || (%pl.mastermindCancelDash && %tick > 4))
	{		
		if(isObject(%cl = %pl.client))
		{
			%cl.setControlCameraFov(%pl.mastermindFov);
			commandToClient(%cl,'setVignette',$EnvGuiServer::VignetteMultiply,$EnvGuiServer::VignetteColor);
		}
		%pl.playThread(0,jump);
		%pl.unMountImage(3);
		%pl.setTempSpeed(0.375);
		%pl.setEnergyLevel(20);
		%pl.schedule(1500,setTempSpeed,1);
		%pl.ismastermindDash = 0;
		%pl.setVelocity(%x/4 SPC %y/4 SPC (%db.mastermindDashZ ? %z/4 : %z));
		return;
	}
	else
	{
		if(isObject(%cl = %pl.client))
		{
			%cl.setControlCameraFov(%pl.mastermindFov-((%prcnt = ((1+mCos($PI*(%time/%max)))/2))*(%pl.mastermindFov/5)));
			%alpha = mClampF(%prcnt*0.6,0,1);
			%vigAlpha = mClampF(getWord($EnvGuiServer::VignetteColor,3),0,1);
			%fAlpha = mClampF(%alpha,(%cont ? mClampF(%vigAlpha,0,0.2) : %vigAlpha),1);
			commandToClient(%cl,'setVignette',$EnvGuiServer::VignetteMultiply,rgbGradient(1-%alpha,"0.4 0 0",$EnvGuiServer::VignetteColor) SPC %fAlpha);
		}
	}
	%pl.setEnergyLevel(%pl.mastermindEnergy);
	if(!%pl.mastermindCancelDash) %pl.setVelocity(vectorAdd(%x/5 SPC %y/5 SPC (%db.mastermindDashZ ? %z/5 : %z),%vec));
	%pl.mastermindDashSched = %pl.schedule(32,mastermindDash,%vec,%tick++);
}

function Playermastermind::onDisabled(%this,%obj)
{
	Parent::onDisabled(%this,%obj);

	if(%obj.ismastermindDash && isObject(%client = %obj.client))
	{
		%client.setControlCameraFov(%obj.mastermindFov);
		commandToClient(%client,'setVignette',$EnvGuiServer::VignetteMultiply,$EnvGuiServer::VignetteColor);
	}
		
}

function PlayerMastermind::onRemove(%this, %obj)
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

function PlayerMastermind::EventideAppearance(%this,%obj,%client)
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
	%obj.unhideNode("chest");

	%shirtColor = "0.05 0.05 0.05 1";
	%pantsColor = "0.05 0.05 0.05 1";
	%skinColor = "0.83 0.73 0.66 1";
	%shoesColor = "0.05 0.05 0.05 1";

	%obj.setFaceName("smiley");
	%obj.setDecalName("brickadiashirt25");
	%obj.setNodeColor("rarm",%shirtColor);
	%obj.setNodeColor("larm",%shirtColor);
	%obj.setNodeColor("chest",%shirtColor);
	%obj.setNodeColor("pants",%pantsColor);
	%obj.setNodeColor("rshoe",%shoesColor);
	%obj.setNodeColor("lshoe",%shoesColor);
	%obj.setNodeColor("rhand",%skinColor);
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

function PlayerMastermind::onKillerChaseStart(%this, %obj, %chasing)
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

function PlayerMastermind::onKillerChase(%this, %obj, %chasing)
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

function PlayerMastermind::onKillerChaseEnd(%this, %obj)
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

function PlayerMastermind::onExitStun(%this, %obj)
{
	%soundType = %this.killerattackedsound;
    %soundAmount = %this.killerattackedsoundamount;
    if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerMastermind::onAllRitualsPlaced(%this, %obj)
{
    %soundType = %this.killerdesperatesound;
    %soundAmount = %this.killerdesperatesoundamount;
    if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerMastermind::onRoundEnd(%this, %obj, %won)
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

function PlayerMastermind::onIncapacitateVictim(%this, %obj, %victim, %killed)
{
	//Play a voice-line taunting the victim.
    %soundType = %this.killertauntsound;
    %soundAmount = %this.killertauntsoundamount;
    if(%soundType !$= "") 
    {
		%obj.setEnergyLevel(100);
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

function Player::MastermindGaze(%obj)
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

    %obj.schedule(%obj.gazeTickRate, MastermindGaze);
}

function PlayerMastermind::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);
	if(%obj.getState() !$= "Dead")
	{
		%obj.playaudio(0,"mastermind_pain" @ getRandom(1, 3) @ "_sound");
	}
}