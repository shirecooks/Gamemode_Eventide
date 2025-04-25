datablock PlayerData(PlayerCaptain : PlayerRenowned) 
{
	uiName = "Sky Captain Player";
	isKiller = true;
	
	// Weapon: Combat Knife
	hitprojectile = KillerSharpHitProjectile;
	hitobscureprojectile = KillerMacheteClankProjectile;

	killerChaseLvl1Music = "musicData_Eventide_CaptainNear";
	killerChaseLvl2Music = "musicData_Eventide_CaptainChase";

	leftclickicon = "color_melee";
    rightclickicon = "";

	killeridlesound = "";
	killeridlesoundamount = 1;

    killerchasesound = "";
	killerchasesoundamount = 1;

    killermeleesound = "";
	killermeleesoundamount = 1;

    killernearsound = "captain_looking";
	killernearsoundamount = 4;

    killertauntsound = "captain_kill";
    killertauntsoundamount = 7;

	killerfoundvictimsound = "captain_foundvictim";
	killerfoundvictimsoundamount = 7;

    killerlostvictimsound = "captain_lostvictim";
	killerlostvictimsoundamount = 6;

    killerthreatenedsound = "captain_threatened";
	killerthreatenedsoundamount = 3;

    killerdesperatesound = "captain_desperate";
	killerdesperatesoundamount = 4;

    killerattackedsound = "captain_attacked";
	killerattackedsoundamount = 6;

    killerweaponchargedsound = "captain_rocketequip";
    killerweaponchargedsoundamount = 3;

    killerspawnsound = "captain_spawn";
    killerspawnsoundamount = 4;

    killerwinsound = "captain_win";
    killerwinsoundamount = 2;

    killerlosesound = "captain_lose";
    killerlosesoundamount = 2;
	
    killerweapon = "blackKnifeImage";
	killerweaponsound = "captain_weapon";
	killerweaponsoundamount = 1;
	
	killerlight = "NoFlareRLight";

	rechargeRate = 0.3;
	maxTools = 1;
	maxWeapons = 1;
	maxForwardSpeed = 7.35;
	maxBackwardSpeed = 7.35;
	maxSideSpeed = 7.35;
    airControl = 1.0;
	jumpForce = 0;

    //
    //Sky Captain can really fly! Just enough to clear a 5x height brick. 
    //The idea behind this is that Sky Captain can use the vertical mobility to climb on top of obstacles or access areas no other killer can.
    //Maps need to be changed to accomodate this, but it will help with stealth, and I believe it will add another layer of gameplay 
    //for both the killer and the survivors.
    //
    canJet = 1;
    minJetEnergy = 2;
	jetEnergyDrain = 2;

    //Crouching speed that's fast, but slow enough to not to chase people with.
    maxForwardCrouchSpeed = 5.88;
	maxBackwardCrouchSpeed = 5.88;
	maxSideCrouchSpeed = 5.88;

    nonStealthDamage = 25;
    gazeTickRate = 50;
    gazeMinimumTime = 3000;
    gazeMaximumTime = 6000;
};

//
// Appearance, initialization.
//

function PlayerCaptain::EventideAppearance(%this,%obj,%client)
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
    %obj.unhideNode("helmet");
    %obj.unHideNode("armor");
    %obj.unhideNode("visor");
    %obj.unhideNode("epauletsRankA");

	%bodyColor = "0 0 0 1";
    %secondaryBodyColor = "0.0514019 0.0514019 0.102804 1";
	%visorColor = "0.667 0.000 0.000 0.700";
	%insigniaColor = "0.388235 0 0.117647 1";

	%obj.setFaceName("smiley");
	%obj.setDecalName("AAA-None");
	%obj.setNodeColor("rarm", %secondaryBodyColor);
	%obj.setNodeColor("larm", %secondaryBodyColor);
	%obj.setNodeColor("chest", %secondaryBodyColor);
	%obj.setNodeColor("pants", %secondaryBodyColor);
	%obj.setNodeColor("rshoe", %bodyColor);
	%obj.setNodeColor("lshoe", %bodyColor);
	%obj.setNodeColor("rhand", %bodyColor);
	%obj.setNodeColor("lhand", %bodyColor);
	%obj.setNodeColor("headskin", %bodyColor);
    %obj.setNodeColor("helmet", %bodyColor);
    %obj.setNodeColor("armor", %bodyColor);
    %obj.setNodeColor("visor", %visorColor);
    %obj.setNodeColor("epauletsRankA", %insigniaColor);
}

function PlayerCaptain::onNewDatablock(%this, %obj)
{
	Parent::onNewDatablock(%this,%obj);
	%obj.mountImage(%this.killerweapon, $LeftHandSlot);
	%obj.setScale("1 1 1");

    //Start up the Gaze mechanic.
    %obj.gazeTickRate = %this.gazeTickRate;
    %obj.gazeFullyCharged = %this.gazeFullyCharged;
    %obj.SCMissleCount = 0;
    %obj.trackingStatus = "\c0OFFLINE";
    %obj.lastTrackingTime = 0;
    %obj.justIncapped = false;
    %obj.incapsAchieved = new SimSet();
    %obj.threatsReceived = new SimSet();
    %obj.SkyCaptainGaze();

    //Spawn-in voice-line.
    %soundType = %this.killerspawnsound;
    %soundAmount = %this.killerspawnsoundamount;
    if(%soundType !$= "")
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }

    //Remind Sky Captain that he can stay stealthy by crouching, and benefits from doing so.
    %obj.client.schedule(33, "centerprint", "<font:Consolas:18>\n\n\n\n\n\n\n\n\c6Crouch for enhanced stealth capability.\n\c6You are much stronger while going unnoticed.\n\c6Use your jets to access unusual places and vantage points.", 10);
}

function PlayerCaptain::onRemove(%this, %obj)
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

//
// Custom HUD.
//

function PlayerCaptain::killerGUI(%this, %obj, %client)
{	
    if(!isObject(%obj) || !isObject(%client))
    {
        return;
    }

    if(isObject(%obj.trackingCandidate))
    {
        if(isObject(%obj.trackingCandidate.client))
        {
            %trackingCandidateName = %obj.trackingCandidate.client.name;
        }
        else
        {
            %trackingCandidateName = %obj.trackingCandidate.getClassName();
        }
    }

    %ammoLabelColorCode = "\c6";
    if(%obj.SCMissleCount < 1)
    {
        %ammoDisplayColorCode = "\c0";
    }
    else
    {
        if(%obj.justIncapped)
        {
            %ammoDisplayColorCode = "\c2";
            %ammoLabelColorCode = "\c2";
            %obj.justIncapped = false;
        }
        else
        {
            %ammoDisplayColorCode = "\c3";
        }
    }
    %ammoDisplay = %ammoDisplayColorCode @ %obj.SCMissleCount;

    if(%obj.isTracking)
    {
        %trackingDisplay = "\c3CALIBRATING";
    }
    else if(%obj.trackingCandidate)
    {
        %trackingDisplay = "\c2ONLINE | " @ %trackingCandidateName;
    }
    else
    {
        %trackingDisplay = "\c0OFFLINE";
    }

	// Some dynamic varirables
	%energylevel = %obj.getEnergyLevel();
	%leftclickstatus = (%energylevel >= 25 && !isObject(%obj.getMountedImage($RightHandSlot))) ? "hi" : "lo";
	%rightclickstatus = (%energylevel == %this.maxEnergy) ? "hi" : "lo";
	%leftclicktext = (%this.leftclickicon !$= "") ? "<just:left>\c9[ \c6Left click \c9]" : "";
	%trackingStatus = "<just:right>\c9[ \c6Tracking system: " @ %trackingDisplay @ " \c9]";	

	// Regular icons
	%leftclickicon = (%this.leftclickicon !$= "") ? "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickicon @ ">" : "";
	%ammoCounter = (%obj.SCMissleCount !$= "") ? "<just:right>\c9[ " @ %ammoLabelColorCode @ "Ammunition: " @ %ammoDisplay @ " \c9]" : "<just:right>\c9[---]";

	%client.bottomprint("<font:Consolas:24>" @ %leftclicktext @ %trackingStatus @ "<br>" @ %leftclickicon @ %ammoCounter, 1);
}

//
// Voice-line handlers.
//

function PlayerCaptain::onKillerChaseStart(%this, %obj, %chasing)
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

function PlayerCaptain::onKillerChase(%this, %obj, %chasing)
{
	if(!%chasing && !%obj.isInvisible)
    {
        //A victim is nearby but Sky Captain can't see them yet. Say some quips.
        %soundType = %this.killernearsound;
        %soundAmount = %this.killernearsoundamount;
        if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + getRandom(15000, 25000))))
        {
            %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
            %obj.lastKillerSoundTime = getSimTime();
        }
    }
}

function PlayerCaptain::onKillerChaseEnd(%this, %obj)
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

function PlayerCaptain::onExitStun(%this, %obj)
{
    //"You DARE..."
	%soundType = %this.killerattackedsound;
    %soundAmount = %this.killerattackedsoundamount;
    if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerCaptain::onAllRitualsPlaced(%this, %obj)
{
    //"Will NOTHING stop you!?"
    %soundType = %this.killerdesperatesound;
    %soundAmount = %this.killerdesperatesoundamount;
    if(%soundType !$= "")// && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

function PlayerCaptain::onRoundEnd(%this, %obj, %won)
{
    //Plays a taunt if Sky Captain wins, or despair if he loses.
    %soundType = %won ? %this.killerwinsound : %this.killerlosesound;
    %soundAmount = %won ? %this.killerwinsoundamount : %this.killerlosesoundamount;
    if(%soundType !$= "")
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

//
// Stealth, jetting, and "snowball" mechanics.
//

//Partially corrects the "dive" mechanic of players who jet and crouch simutaneously, which is hardcoded into the engine and cannot be disabled.
//Torque jank!!!!
function Player::crouchDiveCorrectionLoop(%obj)
{
    %playerDatablock = %obj.getDataBlock();
    if(!%obj.isJetting || !%obj.isCrouching || %obj.getEnergyLevel() < %playerDatablock.minJetEnergy)
    {
        return;
    }

    %tickRate = 33;
    %currentVelocity = %obj.getVelocity();

    %verticalDeltaPerMillisecond = -0.0183537578583; //%forwardDeltaPerMillisecond = 0.02030721092224121;
    %normalVerticalVelocityPerMillisecond = 0.01497669219970703; //%verticalDistancePerTick = (%verticalDeltaPerMillisecond * %tickRate);
    //This formula only nullifies the dive velocity, and does not give Sky Captain normal upward jetting force.
    //So, we need to give it a numerical boost to get (roughly) of the normal jetting force back.
    %boost = 3; 

    %xyReduction = VectorScale(VectorNormalize(%currentVelocity), %playerDatablock.maxForwardCrouchSpeed);
    %zInverse = mAbs(%verticalDeltaPerMillisecond) + (%normalVerticalVelocityPerMillisecond * %tickRate) + %boost;

    %newVelocity = getWords(%xyReduction, 0, 1) SPC %zInverse;

    %obj.setVelocity(%newVelocity);
    %obj.crouchDiveCorrection = %obj.schedule(%tickRate, "crouchDiveCorrectionLoop");
}

function PlayerCaptain::onTrigger(%this, %obj, %trig, %press)
{
	Parent::onTrigger(%this, %obj, %trig, %press);

    //When Sky Captain crouches, make him "invisible". No music.
    //Also reduced voice lines, but that'll be handled seperately.
    if(%trig == 3 && %press)
    {
        //"isCrouched" would be ideal here, but it turns it is broken.
        %obj.isCrouching = true;
        %obj.isInvisible = true;

        //Make Sky Captain partially invisible.
        %obj.startFade(0, 0, true);
        %obj.setNodeColor("ALL", "0.05 0.05 0.05 0.25");
    }
    else if(%trig == 3 && !%press)
    {
        %obj.isCrouching = false;
        %obj.isInvisible = false;

        //Disables Sky Captain's invisibility.
        %obj.startFade(0, 0, false);
        %obj.setNodeColor("ALL", "0.05 0.05 0.05 1");
        %obj.getDataBlock().EventideAppearance(%obj, %obj.client);
    }

    //Add jetting sounds for Sky Captain, if he has enough charge.
    if(%trig == 4 && %press && %obj.getEnergyLevel() > %this.minJetEnergy)
    {
        %obj.isJetting = true;
        %obj.playAudio(3, "jet_loop_sound");
    }
    else if(%trig == 4 && !%press)
    {
        %obj.isJetting = false;
        %obj.playAudio(3, "jet_end_sound");
    }

    //Blockland's engine has a built-in feature that causes jetting players to dive downwards if they crouch. This loop solution aims to remedy that.
    if(%obj.isJetting && %obj.isCrouching && !isEventPending(%obj.crouchDiveCorrection))
    {
        %obj.crouchDiveCorrectionLoop();
    }
	
    //Slight edit to make it so Sky Captain can't melee with an item (homing rocket launcher) out.
	if(%press && !%trig && %obj.getEnergyLevel() >= 25 && !isObject(%obj.getMountedImage($RightHandSlot)))
	{
		%this.killerMelee(%obj, 4);
	}
}

function PlayerCaptain::onIncapacitateVictim(%this, %obj, %victim, %killed)
{
    parent::onIncapacitateVictim(%this, %obj, %victim, %killed);
    //Reward Sky Captain for his kill/down with a homing rocket.
    if(%obj.SCMissleCount $= "")
    {
        %obj.SCMissleCount = 1;
    }
    else
    {
        %obj.SCMissleCount++;
    }

    //If the victim was our tracking candidate, clear them so we can get another one.
    if(isObject(%obj.trackingCandidate) && %obj.trackingCandidate.getID() == %victim.getID())
    {
        %obj.trackingCandidate = "";
    }

    //Update the killer's GUI immediately.
    %this.killerGUI(%obj, %obj.client);

    //Flag to make Sky Captain's GUI flash green when he get's a kill. Not put before the forced GUI update, since the schedule loop
    //Would make the indicator appear inconsistently.
    %obj.justIncapped = true;

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

    //Play a voice-line taunting the victim.
    %soundType = %this.killertauntsound;
    %soundAmount = %this.killertauntsoundamount;
    if(%soundType !$= "") 
    {
        %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
        %obj.lastKillerSoundTime = getSimTime();
    }
}

//
// Largely copied and pasted from script_killers. I needed to make a change so Sky Captain can melee when crouched, and instantly
// incaps unaware players. Also, reset gaze timer if he attacks before it is finished.
function PlayerCaptain::killerMelee(%this, %obj, %radius)
{
	if(%obj.getState() $= "Dead" || %obj.getEnergyLevel() < %this.maxEnergy/8 || %obj.lastMeleeTime+1250 > getSimTime()) 
	{
		return;
	}
		
	%obj.lastMeleeTime = getSimTime();
	%meleeAnim = (%this.shapeFile $= EventideplayerDts.baseShape) ? getRandom(1,4) : getRandom(1,2);
	%hackPos = %obj.getHackPosition();
	%obj.setEnergyLevel(%obj.getEnergyLevel()-%this.maxEnergy/6);	
	%obj.playthread(2,"melee" @ %meleeAnim);							

	if(%this.meleetrailskin !$= "") 
	{
		%meleetrailangle = %this.meleetrailangle[%meleeAnim];
		%obj.spawnKillerTrail(%this.meleetrailskin,%this.meleetrailoffset,%meleetrailangle,%this.meleetrailscale);		
	}	

	if(%this.killerMeleesound !$= "") 
	{
		serverPlay3D(%this.killerMeleesound @ getRandom(1,%this.killerMeleesoundamount) @ "_sound",%hackPos);
	}
	
	if(%this.killerWeaponSound !$= "") 
	{
		serverPlay3D(%this.killerWeaponSound @ getRandom(1,%this.killerWeaponSoundamount) @ "_sound",%hackPos);
	}	

	initContainerRadiusSearch(%obj.getMuzzlePoint(0), %radius, $TypeMasks::PlayerObjectType);		
	while(%hit = containerSearchNext())
	{
		if(%hit == %obj || %hit == %obj.effectbot || VectorDist(%obj.getPosition(),%hit.getPosition()) > %radius || %hit.stunned) 
		{
			continue;
		}

		%typemasks = $TypeMasks::VehicleObjectType | $TypeMasks::FxBrickObjectType;
		%obscure = containerRayCast(%obj.getEyePoint(),%hit.getPosition(),%typemasks, %obj);
		%dot = vectorDot(%obj.getEyeVector(),vectorNormalize(vectorSub(%hit.getHackPosition(),%obj.getPosition())));				

		if(isObject(%obscure) && %this.hitobscureprojectile !$= "")
		{								
			%c = new Projectile()
			{
				dataBlock = %this.hitobscureprojectile;
				initialPosition = posfromraycast(%obscure);
				sourceObject = %obj;
				client = %obj.client;
			};
			
			MissionCleanup.add(%c);
			%c.explode();
			return;
		}

		if(%dot < 0.4)
		{
			continue;
		}

		if((%hit.getType() && $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj, %hit))								
		{	
			if(%hit.getdataBlock().isDowned) 
			{
				continue;
			}
			
			if(%this.killerMeleehitsound !$= "")
			{
				%obj.stopaudio(3);
				%obj.playaudio(3,%this.killerMeleehitsound @ getRandom(1,%this.killerMeleehitsoundamount) @ "_sound");		
			}

			if(%this.hitprojectile !$= "")
			{
				%effect = new Projectile()
				{
					dataBlock = %this.hitprojectile;
					initialPosition = %hit.getHackPosition();
					initialVelocity = vectorNormalize(vectorSub(%hit.getHackPosition(), %obj.getEyePoint()));
					scale = %obj.getScale();
					sourceObject = %obj;
				};
				
				MissionCleanup.add(%effect);
				%effect.explode();
			}

			
			%hit.setvelocity(vectorscale(VectorNormalize(vectorAdd(%obj.getForwardVector(),"0" SPC "0" SPC "0.15")),15));

            if(isObject(%obj.trackingCandidate) && (%obj.trackingCandidate.getId() == %hit.getId()))
            {
                //The victim is unaware and has been gazed at for over the threshold, insta-down/kill.
                %hit.damage(%obj, %hit.getHackPosition(), %hit.getDataBlock().maxDamage, $DamageType::Default);
            }
            else
            {
                //Sky Captain is weaker, he does half the damage of other killers in a straight-up confrontation.
                %hit.damage(%obj, %hit.getHackPosition(), %this.nonStealthDamage*getWord(%obj.getScale(), 2), $DamageType::Default);	
                %hit.timeGazedUpon = 0;
            }								
			
			%obj.setTempSpeed(0.3);	
			%obj.schedule(2500,setTempSpeed, 1);
		}			
	}	
}

//
// Make Sky Captain put away his knife when the homing rocket launcher is out. Also, another voice line.
if(isPackage(Gamemode_Eventide_Player_Captain))
{
    deactivatePackage(Gamemode_Eventide_Player_Captain);
}
package Gamemode_Eventide_Player_Captain
{
    function ServerCmdUseTool(%client, %slot)
    {
        %player = %client.player;
        %playerDatablock = %player.getDataBlock();
        if(!isObject(%player) || %playerDatablock.getName() !$= "PlayerCaptain")
        {
            parent::ServerCmdUseTool(%client, %slot);
            return;
        }

        //Get rid of the knife.
        %player.unmountImage($LeftHandSlot);

        //Gotta get rid of the knife before calling this, or both arms get raised.
        parent::ServerCmdUseTool(%client, %slot);

        %tool = %player.tool[%slot];
        if(%tool > 0 && %tool.image.getId() == homingRocketLauncherImage.getId())
        {
            //"Say hello to my little friend..."
            %soundType = %playerDatablock.killerweaponchargedsound;
            %soundAmount = %playerDatablock.killerweaponchargedsoundamount;
            if(%soundType !$= "" && !%player.hasIntroducedWeapon && %player.SCMissleCount > 0) 
            {
                %player.hasIntroducedWeapon = true;
                %player.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
                %player.lastKillerSoundTime = getSimTime();
            }
        }

        //Set the rocket launcher's ammo to the Player object's missle count.
        %player.setImageAmmo(%tool.image.mountPoint, mClamp(%player.SCMissleCount, 0, 999));
        
        //Update the GUI immediately, need to make the left-click icon colored again.
        %playerDatablock.killerGUI(%client.player, %client);
    }
    function ServerCmdUnUseTool(%client)
    {
        parent::ServerCmdUnUseTool(%client);
        %player = %client.player;
        if(!isObject(%player) || %player.getDataBlock().getName() !$= "PlayerCaptain")
        {
            return;
        }
        %playerDatablock = %player.getDataBlock();

        //Re-equip the knife.
        %player.mountImage(%playerDatablock.killerweapon, $LeftHandSlot);

        //Update the GUI immediately, need to make the left-click icon greyscale.
        %playerDatablock.killerGUI(%player, %client);
    }
};
activatePackage(Gamemode_Eventide_Player_Captain);

//
// Gaze mechanic.
//

function PlayerCaptain::setTrackingTarget(%this, %obj, %trackingTarget)
{
    if(%obj.lastTrackingTime > getSimTime())
    {
        return;
    }

    %obj.trackingCandidate = %trackingTarget;

    %killerClient = %obj.client;
    %killerDatablock = %obj.getDataBlock();

    //Play a tune to notify Sky Captain that homing rockets are available.
    %killerClient.playSound("captain_trackingfinished_sound");

    //Display a mini-tutorial message informing the player that homing rockets are available.
    %killerClient.centerprint("<font:Consolas:18>\n\n\n\n\n\n\n\n\c6Homing rockets are now available.\n\c6Open your inventory to select the SC Rocket Launcher.\n\c6Gain ammo for it by getting downs or kills with your knife.", 10);

    //Update the GUI immediately.
    %killerDatablock.killerGUI(%obj, %killerClient);

    %trackingUpdateDelay = 1000; //1 second.
    %obj.lastTrackingTime = getSimTime() + %trackingUpdateDelay;
}

function PlayerCaptain::clearTrackingTarget(%this, %obj)
{
    if(isObject(%obj.trackingCandidate))
    {
        %obj.trackingCandidate.timeGazedUpon = 0;
        %obj.trackingCandidate = "";
        %this.killerGUI(%obj, %obj.client);

        //Play an error sound effect on Sky Captain to let him and his target know his calibration progress was reset.
        //Only if there's a tracking candidate tracked, to prevent spam.
        %obj.playAudio(1, "captain_trackingreset_sound");
    }
}

function Player::SkyCaptainGaze(%obj)
{
    if(!isObject(%obj) || %obj.isDisabled())
    {
        return;
    }

    %foundTrackingTarget = false;
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
        else if(isObject(getWord(ContainerRayCast(%victimPosition, %killerPosition, %obstructions), 0)))
        {
            //The killer and victim are phyiscally blocked, skip.
            continue;
        }
        else if(%obj.isChasing && %foundPlayer.chaseLevel == 2)
        {
            //Victim is someone we're in a chase with, no stealth here. Skip.
            %killerDatablock.clearTrackingTarget(%obj);

            //The victim does not have any items equipped, don't bother with this.
            %victimEquippedItem = %foundPlayer.getMountedImage($RightHandSlot);
            if(!%victimEquippedItem)
            {
                continue;
            }

            //Nowhere better to put this: if the player has a weapon, have Sky Captain play a voice line acknowledging it.
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

            //They have an item equipped and it's a weapon, have Sky Captain react to it.
            if(isObject(%victimEquippedItem) && (%victimEquippedItem.isWeapon || %victimEquippedItem.className $= "WeaponImage"))
            {
                //"You think that will help you!?"
                %soundType = %killerDatablock.killerthreatenedsound;
                %soundAmount = %killerDatablock.killerthreatenedsoundamount;
                if(%soundType !$= "" && (getSimTime() > (%obj.lastKillerSoundTime + 5000)))
                {
                    %obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
                    %obj.lastKillerSoundTime = getSimTime();
                    %obj.threatsReceived.add(%foundPlayer); //Ensure Sky Captain does not acknowledge any further weapons. Less annoying.
                }
            }
            continue;
        }

        //Is the victim looking at us?
        %victimEyeVector = VectorScale(%foundPlayer.getEyeVector(), %maximumDistance);
        %victimEyeVectorEnd = VectorAdd(%victimPosition, %victimEyeVector);
        %victimLookingAt = ContainerRayCast(%victimPosition, %victimEyeVectorEnd, ($TypeMasks::PlayerObjectType | %obstructions), %foundPlayer);
        %foundObject = getWord(%victimLookingAt, 0);

        if(%foundObject $= %obj.getId())
        {
            //The victim is looking at the killer, reset their gaze and disable tracking.
            %killerDatablock.clearTrackingTarget(%obj);
        }
        else
        {
            //If the victim isn't looking at us, are we looking at them?
            //First, check if we already have them as a tracking target. Then, these math is pointless.
            if(isObject(%obj.trackingCandidate) && %obj.trackingCandidate.getId() == %foundPlayer.getId())
            {
                continue;
            }

            %killerEyeVector = VectorScale(%obj.getEyeVector(), %maximumDistance);
            %killerEyeVectorEnd = VectorAdd(%currentPosition, %killerEyeVector);
            %killerLookingAt = ContainerRayCast(%currentPosition, %killerEyeVectorEnd, ($TypeMasks::PlayerObjectType | %obstructions), %obj);
            %foundObject = getWord(%killerLookingAt, 0);

            if(%foundObject $= %foundPlayer.getId())
            {
                //We're looking at them.

                //Signal to the player via the GUI that they are currently tracking a player.
                %foundTrackingTarget = true;

                //16 TU or below is the produces the minimum tracking time, and 64 TU or above produces the maximum tracking time.
                %victimKillerDistance = mClamp(VectorDist(%currentPosition, %victimPosition), 16, 64); //1 TU = 2 studs.
                %victimLowTrackingDistance = 16;
                %victimHighTrackingDistance = 64;

                %victimLowTrackingTime = %killerDataBlock.gazeMinimumTime;
                %victimHighTrackingTime = %killerDataBlock.gazeMaximumTime;

                %victimTrackingScale = (%victimKillerDistance - %victimLowTrackingDistance) / (%victimHighTrackingDistance - %victimLowTrackingDistance);
                %trackingThreshold = mCeil(%victimLowTrackingTime + ((%victimHighTrackingTime - %victimLowTrackingTime) * %victimTrackingScale));
                
                if(%foundPlayer.timeGazedUpon $= "")
                {
                    %foundPlayer.timeGazedUpon = %obj.gazeTickRate;
                }
                else
                {
                    %foundPlayer.timeGazedUpon += %obj.gazeTickRate;
                }

                //Play a little beep to Sky Captain every 10 ticks (1 second), to let him know the tracking is working.
                if((%foundPlayer.timeGazedUpon % 1000) == 0)
                {
                    %obj.client.playSound("captain_trackingbeep_sound");
                }

                if(%foundPlayer.timeGazedUpon >= %trackingThreshold)
                {
                    //We've tracked the victim for long enough, set them as our tracking candidate for homing rockets.
                    %killerDatablock.setTrackingTarget(%obj, %foundPlayer);
                }
                else
                {
                    //Tell Sky Captain to keep looking, while remaining undetected.
                    %obj.client.centerprint("<font:Consolas:18>\n\n\n\n\n\n\n\n\c6Remain undetected.\n\c6After enough calibrating, your tracking system will become available.\n\c6This unlocks instant downs or kills with your knife, and homing rockets.", 10);
                }
            }
        }
    }

    if(%foundTrackingTarget)
    {
        %obj.isTracking = true;
    }
    else
    {
        %obj.isTracking = false;
    }

    %obj.schedule(%obj.gazeTickRate, SkyCaptainGaze);
}

//
// Old testing functions for determining the velocity produced by diving while jetting.
//

// function Player::startVelocity(%obj)
// {
//     $startVelocity = %obj.getVelocity();
//     echo("Start Velocity:" SPC $startVelocity);
// }


// function Player::endVelocity(%obj)
// {
//     $endVelocity = %obj.getVelocity();
//     echo("End Velocity:" SPC $endVelocity);
// }

// function Player::DiveVelocityTest(%obj)
// {
//     crouch(0);
//     jet(0);

//     %obj.setTransform("0 0 50 0 0 0 0");
//     %obj.setVelocity("0 0 0");

//     jet(1);
//     %obj.schedule(1000, "startVelocity");

//     scheduleNoQuota(1001, 0, "crouch", 1);
//     %obj.schedule(2001, "endVelocity");
// }