datablock TSShapeConstructor(PuppetMasterPuppetDTS) 
{
	baseShape = "./models/puppet.dts";
	sequence0 = "./models/default.dsq";
};

datablock PlayerData(PuppetMasterPuppet : PlayerRenowned)
{
	uiName = "";
	shapeFile = PuppetMasterPuppetDTS.baseShape;

	killerHitProjectile = KillerHitProjectile;	
	killermeleesound = "puppetmasterpuppet_idle";
	killermeleesoundamount = 1;	
	killermeleehitsound = "melee_tanto";
	killermeleehitsoundamount = 3;

	rechargeRate = 0.35;
	runForce = 5616;
	maxForwardSpeed = 10.47;
	maxBackwardSpeed = 5.98;
	maxSideSpeed = 8.58;
	maxDamage = 50;
	showenergybar = true;
};

function PuppetMasterPuppet::onNewDatablock(%this,%obj)
{
	%obj.setScale("0.7 0.7 0.7");
	%obj.schedule(1,setEnergyLevel,100);
	%obj.mountImage("meleeKnifeImage",0);
	%this.EventideAppearance(%obj,ClientGroup.getObject(getRandom(0,ClientGroup.getCount()-1)));
}

function PuppetMasterPuppet::onTrigger(%this, %obj, %trig, %press) 
{
	Parent::onTrigger(%this, %obj, %trig, %press);
			
	switch(%trig)
	{
		case 0: %this.killerMelee(%obj,3.5);
		
		case 4: if(%obj.getEnergyLevel() == %this.maxEnergy)
				if(%press)
				{
					%obj.casttime = getSimTime();
					%obj.chargejumpsound = %obj.schedule(33,playaudio,1,"puppet_jumpCharge_sound");
				}
				else
				{
					cancel(%obj.chargejumpsound);
					
					if(%obj.casttime+350 < getSimTime())
					{
						%this.leap(%obj);
					}
				}
		default:
	}
}

function PuppetMasterPuppet::Leap(%this,%obj)
{
	if(!isObject(%obj)) return;
	
	%obj.setEnergyLevel(0);
	%obj.playthread(3,"rightrecoil");
	serverPlay3d("puppet_jump_sound", %obj.getEyePoint());
	%obj.setVelocity(vectorscale(vectorAdd(%obj.getEyeVector(),"0 0 0.75"),16));
}

function PuppetMasterPuppet::onImpact(%this, %obj, %col, %vec, %force)
{
	return;
}

function PuppetMasterPuppet::onDisabled(%this,%obj)
{
	Parent::onDisabled(%this,%obj);
	
	%obj.spawnExplosion("PlayerSootProjectile","1.5 1.5 1.5");
	%obj.sourceclient.setcontrolobject(%obj.source);
	%obj.source.mountimage("sm_stunImage",2);
	%obj.delete();
}

function PuppetMasterPuppet::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	
    %obj.setMoveSlowdown(0);
    %this.onBotLoop(%obj);    
}

function PuppetMasterPuppet::onBotLoop(%this, %obj)
{
    // Early return if bot is invalid or dead
    if(!isObject(%obj) || %obj.getState() $= "Dead") return;
    
    %obj.BotLoopSched = %this.schedule(500, onBotLoop, %obj);    
    %target = %obj.target;
    %currentTime = getSimTime();

    %obj.setMoveSpeed(0.75);
    
    // Target search logic
    if(!%target && %obj.lastSearchTime < %currentTime)
    {
        %obj.lastSearchTime = %currentTime + 1500; //Search every 1.5 seconds
        
        initContainerRadiusSearch(%obj.getPosition(), 30, $TypeMasks::PlayerObjectType);
        while (%target = containerSearchNext())
        {
            // Skip invalid conditions
            if (%target == %obj || %target.getDataBlock().isDowned)
            {
                continue;
            }

            // Split condition check to avoid long line
            if (%target.getDataBlock().isKiller || %target.getDataBlock().classname $= "PlayerData")
            {
                continue;
            }

            // Obstruction check
            %typemasks = $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType;
            %obscure = containerRayCast(%obj.getEyePoint(),vectorAdd(%target.getPosition(), "0 0 1.9"),%typemasks,%obj);

            // Valid line of sight
            %line = vectorNormalize(vectorSub(%target.getPosition(), %obj.getPosition()));
            %dot = vectorDot(%obj.getEyeVector(), %line);
            
            // Valid target if no obstruction and is visible
            if(!isObject(%obscure) && %dot > 0.5)
            {
                %obj.target = %target;
                %target = %obj.target;
                break;
            }
            
        }
    }
    
    // Target validation
    if(%target)
    {
        if(!isObject(%target) || %target.getState() $= "Dead" || %target.getDataBlock().isKiller || %target.getDataBlock().getName() $= "EventidePlayerDowned")
        {
            // Clear invalid target
            %obj.target = 0;
            %obj.cannotSeeTarget = 0;
            %obj.setMoveY(0);
            %obj.setMoveX(0);
        }
        else
        {
            // Line of sight check
            %targetpos = %target.getPosition();
            %playerpos = %obj.getPosition();
            %typemasks = $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::FxBrickObjectType;

            // Calculate the dot product to determine if the target is within the visible range
            %targetDirection = vectorNormalize(vectorSub(%targetPos, %playerPos));
            %dotProduct = vectorDot(%obj.getEyeVector(), %targetDirection);

            // Perform a raycast to check if there are objects blocking the line of sight
            %adjustedTargetPos = vectorAdd(%targetPos, "0 0 1.9"); // Adjust target height if necessary
            %obstruction = containerRayCast(%obj.getEyePoint(), %adjustedTargetPos, %typeMasks, %obj);

            // Check visibility conditions: no obstruction, within field of view, and within distance
            %distanceToTarget = vectorDist(%playerPos, %targetPos);

            if (!isObject(%obstruction) && %dotProduct > 0.5 && %distanceToTarget < 50) %obj.cannotSeeTarget = 0; // Target visible            
            else %obj.cannotSeeTarget++;// Target not visible
            
            if(%obj.cannotSeeTarget >= 15)
            {
                %obj.target = 0;
                %obj.cannotSeeTarget = 0;
                %obj.clearMoveX();
                %obj.clearMoveY();
            }
        }
    }
    
    // Combat behavior
    if(isObject(%target))
    {
        %distance = VectorDist(%obj.getPosition(), %target.getPosition());
        
        // Close combat handling
        if(%distance < 10)
        {
            if(!%obj.raiseArms)
            {
                %obj.playThread(1, "ArmReadyBoth");
                %obj.raiseArms = true;
            }
            
            if(%distance < 5) 
			{
				%obj.playThread(2, "activate2");

				if(getRandom(1,10) == 1 && %obj.getEnergyLevel() == %this.maxEnergy)
				{
					%this.leap(%obj);
				}			
			}
                
            if(%distance < 2)
            {
                // Use ontrigger for killer melee
		%this.onTrigger(%obj,0,1);
                
                %obj.playAudio(3, "melee_tanto" @ getRandom(1, 3) @ "_sound");
                cancel(%obj.BotLoopSched);
                %obj.playThread(3, "activate2");
                %obj.setMoveX(0);
                %obj.BotLoopSched = %this.schedule(2000, onBotLoop, %obj);
            }
        }
        else if(%obj.raiseArms)
        {
            %obj.playThread(1, "root");
            %obj.raiseArms = false;
        }
        
        // Movement update
        if(%obj.lastTargetTime < %currentTime)
        {
            if($Pref::Server::Eventide::killerSoundsEnabled)
            {
                %obj.playAudio(0, "puppet_jumpCharge_sound");
            }
            
            %obj.lastTargetTime = %currentTime + 3500;
            %obj.setMoveY(1);
            
            if(getRandom(1, 3) == 1)
                %obj.setMoveX(getRandom(-100, 100) * 0.01);
            else
                %obj.setMoveX(0);
                
            %obj.setHeadAngle(0);
            %obj.setMoveObject(%target);
            %obj.setAimObject(%target);
        }
    }
    // Idle behavior
    else if(%obj.lastIdleTime < %currentTime)
    {
        %obj.lastIdleTime = %currentTime + 4000;
        
        if(%obj.raiseArms)
        {
            %obj.playThread(1, "root");
            %obj.raiseArms = false;
        }
        
        if($Pref::Server::Eventide::killerSoundsEnabled)
        {
            %obj.playAudio(0, "puppetmasterpuppet_idle1_sound");
        }
        
        switch(getRandom(1, 4))
        {
            case 1:
                %obj.maxYawSpeed = getRandom(3, 8);
                %obj.maxPitchSpeed = getRandom(3, 8);
                
                %xPos = getRandom(1, 5);
                if(getRandom(0, 1))
                    %xPos *= -1;
                    
                %yPos = getRandom(1, 5);
                if(getRandom(0, 1))
                    %yPos *= -1;
                    
                %obj.setAimLocation(vectorAdd(%obj.getEyePoint(), %xPos SPC %yPos SPC getRandom(1, -1)));
                
                if(getRandom(1, 4) == 1)
                {
                    %obj.setHeadAngleSpeed(0.25);
                    %obj.setHeadAngle(getRandom(-90, 90));
                }
                else
                    %obj.setHeadAngle(0);
                    
            case 4:
                %obj.maxYawSpeed = getRandom(3, 10);
                %obj.maxPitchSpeed = getRandom(3, 10);
                
                %speed = getRandom(50, 100) * 0.01;
                if(getRandom(1, 5) == 1)
                    %speed *= -1;
                %obj.setMoveY(%speed);
                
                %xPos = getRandom(1, 5);
                if(getRandom(0, 1))
                    %xPos *= -1;
                    
                %yPos = getRandom(1, 5);
                if(getRandom(0, 1))
                    %yPos *= -1;
                    
                %obj.setAimLocation(vectorAdd(%obj.getEyePoint(), %xPos SPC %yPos SPC 0));
                
            default:
                %obj.clearMoveY();
                %obj.clearMoveX();
        }
    }
}

function PuppetMasterPuppet::EventideAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");
	%obj.unHideNode((%client.chest ? "femChestpuppet" : "chestpuppet"));	
	%obj.unHideNode((%client.rhand ? "rhook" : "rhandpuppet"));
	%obj.unHideNode((%client.lhand ? "lhook" : "lhandpuppet"));
	%obj.unHideNode((%client.rarm ? "rarmSlim" : "rarm"));
	%obj.unHideNode((%client.larm ? "larmSlim" : "larm"));
	%obj.unHideNode("headpuppet");
	%obj.unHideNode("buttoneyes");

	if($pack[%client.pack] !$= "none")
	{
		%obj.unHideNode($pack[%client.pack]);
		%obj.setNodeColor($pack[%client.pack],%client.packColor);
	}
	if($secondPack[%client.secondPack] !$= "none")
	{
		%obj.unHideNode($secondPack[%client.secondPack]);
		%obj.setNodeColor($secondPack[%client.secondPack],%client.secondPackColor);
	}
	if($hat[%client.hat] !$= "none")
	{
		%obj.unHideNode($hat[%client.hat]);
		%obj.setNodeColor($hat[%client.hat],%client.hatColor);
	}
	if(%client.hip)
	{
		%obj.unHideNode("skirthip");
		%obj.unHideNode("skirttrimleft");
		%obj.unHideNode("skirttrimright");
	}
	else
	{
		%obj.unHideNode("pantspuppet");
		%obj.unHideNode((%client.rleg ? "rpeg" : "rshoe"));
		%obj.unHideNode((%client.lleg ? "lpeg" : "lshoe"));
	}

	%obj.setHeadUp(0);
	if(%client.pack+%client.secondPack > 0) %obj.setHeadUp(1);
	if($hat[%client.hat] $= "Helmet")
	{
		if(%client.accent == 1 && $accent[4] !$= "none")
		{
			%obj.unHideNode($accent[4]);
			%obj.setNodeColor($accent[4],%client.accentColor);
		}
	}
	else if($accent[%client.accent] !$= "none" && strpos($accentsAllowed[$hat[%client.hat]],strlwr($accent[%client.accent])) != -1)
	{
		%obj.unHideNode($accent[%client.accent]);
		%obj.setNodeColor($accent[%client.accent],%client.accentColor);
	}

	%obj.setFaceName(%client.faceName);
	%obj.setDecalName(%client.decalName);

	%obj.setNodeColor("headpuppet",%client.headColor);	
	%obj.setNodeColor("chestpuppet",%client.chestColor);
	%obj.setNodeColor("femChestpuppet",%client.chestColor);
	%obj.setNodeColor("pantspuppet",%client.hipColor);
	%obj.setNodeColor("skirthip",%client.hipColor);	
	%obj.setNodeColor("rarm",%client.rarmColor);
	%obj.setNodeColor("larm",%client.larmColor);
	%obj.setNodeColor("rarmSlim",%client.rarmColor);
	%obj.setNodeColor("larmSlim",%client.larmColor);
	%obj.setNodeColor("rhandpuppet",%client.rhandColor);
	%obj.setNodeColor("lhandpuppet",%client.lhandColor);
	%obj.setNodeColor("rhook",%client.rhandColor);
	%obj.setNodeColor("lhook",%client.lhandColor);	
	%obj.setNodeColor("rshoe",%client.rlegColor);
	%obj.setNodeColor("lshoe",%client.llegColor);
	%obj.setNodeColor("rpeg",%client.rlegColor);
	%obj.setNodeColor("lpeg",%client.llegColor);
	%obj.setNodeColor("skirttrimright",%client.rlegColor);
	%obj.setNodeColor("skirttrimleft",%client.llegColor);
	%obj.setNodeColor("buttoneyes","0.1 0.1 0.1 1");
}
