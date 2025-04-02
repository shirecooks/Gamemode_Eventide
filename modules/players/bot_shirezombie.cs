datablock PlayerData(ShireZombieBot : EventidePlayer)
{
	uiName = "";
	isKiller = true;
    uiName = "";
    maxDamage = 50;
    thirdpersonlonly = 1;
	maxForwardSpeed = 5.95;
	maxBackwardSpeed = 3.4;
	maxSideSpeed = 5.1;
	thirdPersonOnly = true;
};

function ShireZombieBot::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType)
{
    Parent::Damage(%this,%obj,%sourceObject,%position,%damage,%damageType);
    %obj.kill();
}

function ShireZombieBot::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	
    %obj.setMoveSlowdown(0);
	%this.applyAppearance(%obj,%obj.ghostclient);
    %this.onBotLoop(%obj);
    %obj.mountImage("GlowFaceZombieImage",0);
	%obj.mountImage("ZombieBodyImage",1);    
	%obj.schedule(33,playAudio,2,"hex_ghostSpawn_sound");
}

function ShireZombieBot::applyAppearance(%this,%obj)
{
    Armor::EventideAppearance(%this,%obj,%obj.ghostclient);

    %ghostClient = %obj.ghostclient;
	%zskin = "0 0 0 1";
    %headColor = %zskin;
    %obj.setFaceName(%faceName);
	%obj.HideNode("visor");
	%obj.HideNode("lpeg");
	%obj.HideNode("rpeg");
	%obj.unHideNode("lshoe");
	%obj.unHideNode("rshoe");
	%obj.setNodeColor("ALL",%headColor);
    %obj.setFaceName("hexZombie");
	%obj.setDecalName("none"); 	
}

function ShireZombieBot::onBotLoop(%this, %obj)
{
    // Early return if bot is invalid or dead
    if(!isObject(%obj) || %obj.getState() $= "Dead") return;
    
    %obj.BotLoopSched = %this.schedule(500, onBotLoop, %obj);    
    %target = %obj.target;
    %currentTime = getSimTime();
    
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
        %distance = VectorDist(%obj.getPosition(), %target.getPosition());

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

        if (!isObject(%obstruction) && %dotProduct > 0.5 && %distance < 50)
        {
            %obj.cannotSeeTarget = 0; // Target visible
        }
        else 
        {
            %obj.cannotSeeTarget++;// Target not visible
        }
        
        if(%obj.cannotSeeTarget >= 10)
        {
            %obj.target = 0;
            %obj.cannotSeeTarget = 0;
            %obj.clearMoveX();
            %obj.clearMoveY();
        }
    }

    // Combat behavior
    if(isObject(%target))
    {        
        // Close combat handling
        if(%distance < 10)
        {
            if(!%obj.raiseArms)
            {
                %obj.playThread(1, "ArmReadyBoth");
                %obj.raiseArms = true;
            }
            
            if(%distance < 5) %obj.playThread(2, "activate2");
                
            if(%distance < 2)
            {
                %target.damage(%obj, %target.getWorldBoxCenter(), 20, $DamageType::Default);
                %target.setTempSpeed(0.5);
                %target.schedule(1000, setTempSpeed, 1);
                %target.playThread(3, "plant");
                
                %obj.playAudio(3, "skullwolf_hit" @ getRandom(1, 3) @ "_sound");
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
                %obj.playAudio(0, "zombie_chase" @ getRandom(0, 10) @ "_sound");
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
            %obj.playAudio(0, "zombie_idle" @ getRandom(0, 4) @ "_sound");
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

function ShireZombieBot::onDisabled(%this,%obj)
{
	Parent::onDisabled(%this,%obj);
    %obj.delete();
}

function ShireZombieBot::onRemove(%this,%obj)
{    
    Parent::onRemove(%this,%obj);
    %obj.spawnExplosion("PlayerSootProjectile","1.5 1.5 1.5");
}