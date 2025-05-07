// Function that manages the behavior of the killer, handling its state, playing sounds, and scheduling future actions.
function Armor::onKillerLoop(%this, %obj)
{
    // Skip if invalid state
    if (!isObject(%obj) || %obj.getState() $= "Dead" || !isObject(getMinigamefromObject(%obj)))
	{
		return;
	}

	// Update UI and schedule next loop
    if (isObject(%obj.client)) 
	{
		%this.killerGUI(%obj, %obj.client);
	}

    // Handle killer container search and idle actions
	if(%obj.getDataBlock().isKiller)
    {			
		%this.killerContainerRadiusSearch(%obj);
		%this.playKillerLoopActions(%obj);
    }

	// Schedule next loop, prevent double scheduling by cancelling the previous loop schedule
    cancel(%obj.onKillerLoop);
    %obj.onKillerLoop = %this.schedule(500, onKillerLoop, %obj);
}

// Support function to handle victim's state changes during chase
function Armor::handleVictimChaseState(%this, %victim, %obj, %canSeeKiller, %victimDistance, %searchDistance, %isActiveChase)
{
    %victimDatablock = %victim.getDatablock();

    // The victim is being chased, this condition does some actions
	if(%isActiveChase)
    {
		// Start the tunnel vision effect
		if(isObject(%victim.client) && !%victim.tunnelvision && %victimDatablock.isEventideModel)
		{
			%this.TunnelVision(%victim,true);
		}

        if(%victimDistance < %searchDistance/2.5)
        {
			// Play a thread to have them talk, but it makes it look like they're nervous when the killer is near
            if(%victim.playerClass $= "" || %victim.playerClass.title !$= "Staller")
            {
               %victim.playthread(2, "talk"); 
            }
            
            if(%victimDistance < %searchDistance/4)
            {
				%viewNormal = vectorNormalize(vectorSub(%obj.getEyePoint(), %victim.getMuzzlePoint(2)));
                %dot = vectorDot(%victim.getEyeVector(), %viewNormal);

                // Handle panic sounds when victim sees killer
                if((%dot > 0.45) && %victim.lastChaseCall < getSimTime() && $Pref::Server::Eventide::victimScreamsEnabled && (%victim.playerClass $= "" || %victim.playerClass.title !$= "Staller"))
                {
                    %genderSound = (!%victim.client.chest) ? "male" : "female";
                    %genderSoundAmount = (!%victim.client.chest) ? 3 : 5;
                    %sound = %genderSound @ "_shock" @ getRandom(1, %genderSoundAmount) @ "_sound";
                    %victim.playaudio(0, %sound);        
                    %victim.lastChaseCall = getSimTime() + getRandom(1000, 5000);
                }
            }
            
            // Update victim's face to scared
            if(isObject(%victim.faceConfig))
            {
                if(%victim.faceConfig.subCategory $= "" && $Eventide_FacePacks[%victim.faceConfig.category, "Scared"] !$= "")
                {
                    %victim.createFaceConfig($Eventide_FacePacks[%victim.faceConfig.category, "Scared"]);
                }
                
                if(%victim.faceConfig.isFace("Scared"))
                {
                    %victim.faceConfig.dupeFaceSlot("Neutral", "Scared");                    
                }
            }
            
            // Handle victim's chase music
            if($Pref::Server::Eventide::chaseMusicEnabled && isObject(%victim.client))
            {
                if(%victim.chaseLevel != 2)
                {
                    %victim.client.SetChaseMusic(%obj.getDataBlock().killerChaseLvl2Music, true);
                }
                %victim.TimeSinceChased = getSimTime();
                cancel(%victim.client.StopChase);
                %victim.client.StopChase = %victim.client.schedule(6000, StopChase);
            }
			%victim.chaseLevel = 2; //Need this outside the music check for holebot testing support.
        }
    }
	// The victim is no longer being chased, just nearby
    else
    {
        %chaseEndGracePeriod = 4000;
        %victimChaseExpired = (%victim.TimeSinceChased + %chaseEndGracePeriod) < getSimTime();
        
        if(%victimChaseExpired)
        {
            %victim.playthread(2, "root");
            // Reset victim's face back to neutral when chase ends
            if(isObject(%victim.faceConfig) && %victim.faceConfig.face["Neutral"].faceName $= "Scared") 
            {                        
                %victim.faceConfig.resetFaceSlot("Neutral");                    
            }
            
            // Handle stepping down victim's music
            if($Pref::Server::Eventide::chaseMusicEnabled && isObject(%victim.client))
            {
				// Clear the tunnel vision effect
				if(%victim.tunnelvision)
				{
					%this.TunnelVision(%victim,false);
				}
				
				if(%victim.chaseLevel != 1)
                {
                    %victim.client.SetChaseMusic(%obj.getDataBlock().killerChaseLvl1Music, false);
                }
                cancel(%victim.client.StopChase);
                %victim.client.StopChase = %victim.client.schedule(6000, StopChase);
            }
			%victim.chaseLevel = 1; //Need this outside the music check for holebot testing support.
        }
    }
}

// Support function to manage killer's chase state
function Armor::handleKillerChaseState(%this, %obj, %chasingVictims, %isActiveChase)
{
    if(%isActiveChase)
    {
        if(!%obj.isChasing)
        {
            %this.onKillerChaseStart(%obj);
            %obj.isChasing = true;
        }
        %obj.TimeSinceChased = getSimTime();
        %this.onKillerChase(%obj, true);
        
        // Handle killer's chase music
        if($Pref::Server::Eventide::chaseMusicEnabled && isObject(%obj.client) && %chasingVictims)
        {    
            if(%obj.chaseLevel != 2)
            {
                %obj.client.SetChaseMusic(%obj.getDataBlock().killerChaseLvl2Music, false);
            }
            cancel(%obj.client.StopChase);
            %obj.client.StopChase = %obj.client.schedule(6000, StopChase);
        }
		%obj.chaseLevel = 2;
    }
    else
    {
        %chaseEndGracePeriod = 4000;
        %killerChaseExpired = (%obj.TimeSinceChased + %chaseEndGracePeriod) < getSimTime();
        
        if(%killerChaseExpired)
        {
            if(%obj.isChasing)
            {
                %this.onKillerChaseEnd(%obj);
                %obj.isChasing = false;
            }
            %this.onKillerChase(%obj, false);
            
            // Handle stepping down killer's music
            if($Pref::Server::Eventide::chaseMusicEnabled && isObject(%obj.client) && !%chasingVictims)
            {
                if(%obj.chaseLevel != 1)
                {
                    %obj.client.SetChaseMusic(%obj.getDataBlock().killerChaseLvl1Music, false);
                }
                cancel(%obj.client.StopChase);
                %obj.client.StopChase = %obj.client.schedule(6000, StopChase);
            }
			%obj.chaseLevel = 1;
        }
    }
}

// Main function refactored to use support functions
function Armor::killerContainerRadiusSearch(%this, %obj)
{
    //Don't do any of this stuff if the killer is invisible.
    if(%obj.isInvisible)
    {
        return;
    }

    %chasingVictims = 0;
    %searchDistance = 40; //80 studs, should be a decent balance for both small and large maps.
    initContainerRadiusSearch(%obj.getMuzzlePoint(0), %searchDistance, $TypeMasks::PlayerObjectType);

    while(%victim = containerSearchNext())
    {
        // Skip invalid conditions
        %victimDatablock = %victim.getDataBlock();
        if(!isObject(getMinigamefromObject(%victim)) || %victimDatablock.isKiller || %victimDatablock.isDowned || %victim.getState() $= "Dead" || %victim.isInvisible) 
        {
            continue;
        }
        
        %typemasks = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType;
        %dot = vectorDot(%obj.getEyeVector(), vectorNormalize(vectorSub(%victim.getMuzzlePoint(2), %obj.getEyePoint())));
        %canSeeVictim = !isObject(containerRayCast(%obj.getEyePoint(), %victim.getMuzzlePoint(2), %typemasks, %obj));
        %victimDistance = containerSearchCurrDist();

        %isActiveChase = %dot > 0.45 && %canSeeVictim;
        if(%isActiveChase)
        {
            %chasingVictims++;
        }

        // Update states for both killer and victim
        %this.handleKillerChaseState(%obj, %chasingVictims, %isActiveChase);
        %this.handleVictimChaseState(%victim, %obj, %canSeeVictim, %victimDistance, %searchDistance, %isActiveChase);
    }
}

function Armor::playKillerLoopActions(%this,%obj)
{	
	// Handle raising the arms
	if (%this.killerRaiseArms && %obj.isChasing != %obj.raiseArms) 
	{    				
		%obj.playThread(1, %obj.isChasing ? "armReadyBoth" : "root");
		%obj.raiseArms = %obj.isChasing;
	}
	
	// Handle killer sounds
	if ($Pref::Server::Eventide::killerSoundsEnabled && !%obj.isCrouched() && %obj.lastKillerSoundTime + getRandom(7000, 10000) < getSimTime())
	{                       
		if (!%obj.isInvisible) 
		{
			// Determine if chasing or idle sounds should be played
			%soundType = %obj.isChasing ? %this.killerChaseSound : %this.killerIdleSound;
			%soundAmount = %obj.isChasing ? %this.killerChaseSoundAmount : %this.killerIdleSoundAmount;

			if(%soundType !$= "") 
			{
				%obj.playAudio(0, %soundType @ getRandom(1, %soundAmount) @ "_sound");
				%obj.playThread(3, "plant");
				%obj.lastKillerSoundTime = getSimTime();
			}
		}
	}
}

function Armor::killerGUI(%this,%obj,%client)
{	
	if (!isObject(%obj) || !isObject(%client))
	{
		return;
	}	

	// Some dynamic varirables
	%energylevel = %obj.getEnergyLevel();
	%leftclickstatus = (%energylevel >= 25) ? "hi" : "lo";
	%rightclickstatus = (%energylevel == %this.maxEnergy) ? "hi" : "lo";
	%leftclicktext = (%this.leftclickicon !$= "") ? "<just:left>\c6Left click" : "";
	%rightclicktext = (%this.rightclickicon !$= "") ? "<just:right>\c6Right click" : "";	

	// Regular icons
	%leftclickicon = (%this.leftclickicon !$= "") ? "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickicon @ ">" : "";
	%rightclickicon = (%this.rightclickicon !$= "") ? "<just:right><bitmap:" @ $iconspath @ %rightclickstatus @ %This.rightclickicon @ ">" : "";

	%client.bottomprint(%leftclicktext @ %rightclicktext @ "<br>" @ %leftclickicon @ %rightclickicon, 1);
}