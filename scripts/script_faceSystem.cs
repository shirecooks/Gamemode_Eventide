$Eventide_FacePacks["isGlobalFacePackArray"] = true; //All face packs will be stored in this array.
$Eventide_FaceDatas["isGlobalFaceDataArray"] = true; //All face data objects will be stored in this array.
$Eventide_FaceConfigs["isGlobalFaceConfigArray"] = true; //You get the idea.

//
// Class definitions.
//

// Face data.
///

function createFaceData(%faceFilePath, %faceName, %facePack)
{
    %faceData = new ScriptObject()
    {
        class = FaceData;
        isFaceData = true;
        faceFile = %faceFilePath;
        facePack = %facePack;
        faceName = %faceName;
        simpleName = %faceName;
        category = %facePack.category;
        subCategory = %facePack.subCategory;
    };
    %faceData.setName("faceData_" @ %faceName);

    $Eventide_FaceDatas[%facePack.category @ "_" @ %facePack.subCategory @ "_" @ %faceName] = %faceData;

    return %faceData;
}

function FaceData::setSimpleName(%obj, %name)
{
    %obj.simpleName = %name;
}

function FaceData::getSimpleName(%obj)
{
    return %obj.simpleName;
}

function FaceData::getFile(%obj)
{
    return %obj.faceFile;
}

// Face packs.
///

function createFacePack(%facePackPath, %faceFileCategory)
{
    %facePack = new ScriptObject()
    {
        class = FacePack;
        isFacePack = true;
        category = %faceFileCategory;
        subCategory = ""; //Will only be used for sub-face packs (Shown below.)
        faces["isFacePackArray"] = true; //All faces will be stored in this "faces" array.
    };
    %facePack.setName("facePack_" @ %faceFileCategory);

    %faceFilePattern = %facePackPath @ "/*.png";
    for(%faceFile = findFirstFile(%faceFilePattern); %faceFile !$= ""; %faceFile = findNextFile(%faceFilePattern))
    {
        //Ignore files outside the current directory. No better way to do this. :(
        if(filePath(%faceFile) !$= %facePackPath)
        {
            continue;
        }

        addExtraResource(%faceFile);
        %faceName = fileBase(%faceFile);
        %facePack.faces[%faceName] = createFaceData(%faceFile, %faceName, %facePack);

        //echo(" - Created face: \"" @ %faceName @ "\"");
    }

    $Eventide_FacePacks[%faceFileCategory] = %facePack;

    return %facePack;
}

function createSubFacePack(%subFaceFilePath, %subCategory, %facePack)
{
    //Create face packs that inherit from another face pack, overwriting existing faces with different ones.
    %subFacePack = cloneScriptObject(%facePack);
    %subFacePack.isSubFacePack = true;
    %subFacePack.parentFacePack = %facePack;
    %subFacePack.name = %subCategory;
    %subFacePack.subCategory = %subCategory;

    %subFaceFilePattern = %subFaceFilePath @ "/*.png";
    for(%subFaceFile = findFirstFile(%subFaceFilePattern); %subFaceFile !$= ""; %subFaceFile = findNextFile(%subFaceFilePattern))
    {
        //Ignore files outside the current directory.
        if(filePath(%subFaceFile) !$= %subFaceFilePath)
        {
            continue;
        }

        //Overwrite existing faces, write non-existing ones. Prevents face data duplication if a subface file matches a primary one.
        if(%subFacePack.faces[%faceName] $= "" || %subFacePack.faces[%faceName].faceFile !$= %subFaceFile)
        {
            %faceName = fileBase(%subFaceFile);
            %faceData = createFaceData(%subFaceFile, %faceName, %subFacePack);
            %subFacePack.faces[%faceName] = %faceData;

            //echo("\t - Created face: \"" @ %faceName @ "\"");
        }
    }

    $Eventide_FacePacks[%facePack.category, %subCategory] = %subFacePack;

    return %subFacePack;
}

function FacePack::getFaceData(%obj, %name)
{
    return %obj.faces[%name];
}

// Face configs.
///

function compileFaceDataName(%facePack, %name)
{
    return %facePack.category @ %facePack.subCategory @ %name;
}

function createFaceConfig(%facePack)
{
    //For generic, default survivor faces.
    %faceConfig = new ScriptObject()
    {
        class = FaceConfig;
        isFaceConfig = true;
        category = %facePack.category;
        subCategory = %facePack.subCategory;
        previousFacePack = %facePack.getID();
        facePack = %facePack;
        currentFace = "";
        face["isFaceConfigArray"] = true;
        face["Pain", "length"] = 2000;
        animationQueue = New_QueueSO(255);
    };

    %faceConfigName = "faceConfig_" @ %faceConfig.getID();
    %faceConfig.setName(%faceConfigName);
    $Eventide_FaceConfigs[%faceConfigName] = %faceConfig;

    return %faceConfig;
}

function createEmptyFaceConfig(%facePack)
{
    //If you need something super custom. You must set all the expression fields manually.
    %faceConfig = new ScriptObject()
    {
        class = FaceConfig;
        isFaceConfig = true;
        category = %facePack.category;
        subCategory = %facePack.subCategory;
        facePack = %facePack;
        currentFace = "";
        face["isFaceConfigArray"] = true;
        animationQueue = New_QueueSO(255);
    };

    %faceConfigName = "faceConfig_" @ %faceConfig.getID();
    %faceConfig.setName(%faceConfigName);
    $Eventide_FaceConfigs[%faceConfigName] = %faceConfig;
    
    return %faceConfig;
}

function FaceConfig::getFacePack(%obj)
{
    if(%obj.facePack $= "")
    {
        return %obj.previousFacePack;
    }
    else
    {
        return %obj.facePack;
    }
}

function FaceConfig::setFacePack(%obj, %facePack)
{
    %obj.previousFacePack = %obj.getFacePack().getID();
    %obj.facePack = %facePack;
    %obj.category = %facePack.category;
    %obj.invalidateCache();

    //Hot-swap the current face for the one in the new face pack.
    %player = %obj.player;
    if(isObject(%player))
    {
        %player.faceConfigShowFaceTimed(%obj.currentFace.getSimpleName(), 0);
    }
}

function FaceConfig::cacheFace(%obj, %name)
{
    %facePack = %obj.getFacePack();
    %face = %facePack.getFaceData(%name);
    if(%face !$= "")
    {
        %face.setSimpleName(%name);
        %obj.face[%name] = %face;
        return;
    }
    else if(%facePack.getFaceData(%facePack.category @ %name) !$= "")
    {
        %faceData =  %facePack.getFaceData(%facePack.category @ %name);
    }
    else if(%facePack.getFaceData(compileFaceDataName(%facePack, %name)) !$= "")
    {
        %faceData = %facePack.getFaceData(compileFaceDataName(%facePack, %name));
    }
    else
    {
        //Fallback, no face found.
        return "smiley"; 
    }

    %faceData.setSimpleName(%name);
    %obj.face[%name] = %faceData;
    if(strstr(%obj.faceIndex, %name) == -1)
    {
        %obj.faceIndex = (%obj.faceIndex $= "") ? %name : %obj.faceIndex SPC %name;
    }
}

function FaceConfig::invalidateCache(%obj)
{
    //Clears all cached faces, forcing a reload on next access.
    for(%i = 0; %i < getWordCount(%obj.faceIndex); %i++)
    {
        %face = getWord(%obj.faceIndex, %i);
        %obj.face[%face] = "";
    }
    %obj.faceIndex = "";
}

function FaceConfig::getFace(%obj, %name)
{
    if(%obj.face[%name] $= "")
    {
        %result = %obj.cacheFace(%name);
        if(%result $= "smiley")
        {
            //No face found under that name.
            return "smiley";
        }
    }
    %obj.currentFace = %obj.face[%name];
    return fileBase(%obj.currentFace.getFile());
}

function FaceConfig::setFace(%obj, %name, %faceData)
{
    %obj.face[%name] = %faceData;
}

function FaceConfig::isFace(%obj, %name)
{
    if(%obj.face[%name] $= "")
    {
        //The face might be available but not cached, let's try that.
        %obj.cacheFace(%name);
    }
    return %obj.face[%name] !$= "";
}

function FaceConfig::getFaceAttribute(%obj, %name, %attribute)
{
    return %obj.face[%name, %attribute];
}

function FaceConfig::setFaceAttribute(%obj, %name, %attribute, %value)
{
    %obj.face[%name, %attribute] = %value;
}

function FaceConfig::dupeFaceSlot(%obj, %targetName, %sourceName)
{
    //Allows you to set the face of an expression as the face of another expression.
    %obj.face[%targetName] = %obj.face[%sourceName];
}

function FaceConfig::resetFaceSlot(%obj, %targetName)
{
    %facePack = %obj.getFacePack();
    %obj.face[%targetName] = %facePack.getFaceData(compileFaceDataName(%facePack, %targetName));
}

function FaceConfig::onRemove(%obj)
{
    %animationQueue = %obj.animationQueue;
    while((%animationSchedule = %animationQueue.pop()) != 0)
    {
        cancel(%animationSchedule);
    }
    %animationQueue.delete();
}

//
// Loading functions.
//

function getFacePackTree(%startingDirectory)
{
    %container = new ScriptObject() { _["index"] = ""; }; //Local arrays can't be returned from a function, so we must store it in a ScriptObject.

    //Get main face packs, store them all at an "index" position so they can be found later.
    %mainFacePackString = getFileString(%startingDirectory @ "/*.etfp");
    %container._["index"] = %mainFacePackString;
    
    for(%i = 0; %i < getFieldCount(%mainFacePackString); %i++)
    {
        %facePack = getField(%mainFacePackString, %i);
        %container._[%facePack] = getFileString(filePath(%facePack) @ "/*.etsp"); //Get sub-face packs, assign to the value of the parent face pack in the array.
    }

    return %container;
}

function parseFacePacks(%startingDirectory)
{    
    %facePackDictionary = getFacePackTree(%startingDirectory);
    for(%i = 0; %i < getFieldCount(%facePackDictionary._["index"]); %i++)
    {
        %facePackFile = getField(%facePackDictionary._["index"], %i);
        %facePackPath = filePath(%facePackFile);
        %facePackFileName = fileBase(%facePackFile);

        //echo("Parsing face pack \"" @ %facePackFileName @ "\" from \"" @ %facePackPath @ "\"...");

        %facePack = createFacePack(%facePackPath, %facePackFileName);

        //Check for subpacks.
        %subFaceFilePattern = %facePackPath @ "/*.etsp";
        for(%j = 0; %j < getWordCount(%facePackDictionary._[%facePackFile]); %j++)
        {
            %subFaceFile = getWord(%facePackDictionary._[%facePackFile], %j);
            %subFacePackPath = filePath(%subFaceFile);
            %subFacePackFileName = fileBase(%subFaceFile);

            //echo("\tParsing sub-face pack \"" @ %subFacePackFileName @ "\" from \"" @ %subFacePackPath @ "\"...");

            %subFacePack = createSubFacePack(%subFacePackPath, %subFacePackFileName, %facePack);
        }
    }

    %facePackDictionary.delete(); //Don't need it anymore, delete it to prevent a memory leak.
}

//
// Player functions/displaying faces.
//

function Player::faceConfigSanityCheck(%player)
{
    return isObject(%player) && isObject(%player.faceConfig) && %player.getState() !$= "Dead";
}

function Player::createFaceConfig(%player, %facePack)
{
    //This is basically the initilization function for the face system on a player.
    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }

    if(isObject(%player.faceConfig))
    {
        %player.faceConfig.delete();
    }

    %faceConfig = createFaceConfig(%facePack);
    %faceConfig.player = %player;
    %player.faceConfig = %faceConfig;

    %player.beginFaceConfigBlinkSchedule();
}

function Player::createSubfaceConfig(%player, %subFacePackName)
{
    //This is basically the initilization function for the face system on a player.
    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }

    //This can only work if the player already has a main face config present.
    if(!isObject(%player.faceConfig))
    {
        return;
    }
    
    //If the subcategory of the main face pack doesn't exist, we can't do anything.
    %subFacePack = $Eventide_FacePacks[%player.faceConfig.category, %subFacePackName];
    if(%subFacePack $= "")
    {
        return;
    }

    %currentFaceConfig = %player.faceConfig;
    %newFaceConfig = createFaceConfig(%subFacePack);
    %newFaceConfig.previousFacePack = %player.faceConfig.previousFacePack;
    %currentFaceConfig.delete();
    %player.faceConfig = %newFaceConfig;

    %player.beginFaceConfigBlinkSchedule();
}

function Player::revertSubfaceConfig(%player)
{
    //This can only work if the player already has a main face config present.
    if(!isObject(%player.faceConfig))
    {
        return;
    }
    
    %player.createFaceConfig($Eventide_FacePacks[%player.faceConfig.category]);
}

function Player::hasSubfacePack(%player, %subFacePackName)
{
    return $Eventide_FacePacks[%player.faceConfig.category, %subFacePackName] !$= "";
}

function Player::createEmptyFaceConfig(%player, %facePack)
{
    //This is basically the initilization function for the face system on a player.
    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }

    if(isObject(%player.faceConfig))
    {
        %player.faceConfig.delete();
    }

    %player.faceConfig = createEmptyFaceConfig(%facePack);
}

function Player::faceConfigUnblink(%player)
{
    if(!%player.faceConfigSanityCheck())
    {
        return;
    }

    %faceConfig = %player.faceConfig;

    %player.setFaceName(%player.faceConfig.getFace("Neutral"));
    if(%faceConfig.subCategory $= "Scared")
    {
        if(%faceConfig.getFaceAttribute("Blink", "scaredOpenLength") $= "")
        {
            %faceConfig.setFaceAttribute("Blink", "scaredOpenLength", (5000 + getRandom(0, 2000)));
        }
        %blinkDelay = %faceConfig.getFaceAttribute("Blink", "scaredOpenLength"); 
    }
    else
    {
        if(%faceConfig.getFaceAttribute("Blink", "openLength") $= "")
        {
            %faceConfig.setFaceAttribute("Blink", "openLength", (3000 + getRandom(0, 750)));
        }
        %blinkDelay = %faceConfig.getFaceAttribute("Blink", "openLength"); 
    }

    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }
    
    %blinkSchedule = %player.schedule(%blinkDelay, "faceConfigBlink");
    %player.faceConfigBlinkSchedule = %blinkSchedule;
    %faceConfig.animationQueue.push(%blinkSchedule);
    return %blinkSchedule;
}

function Player::faceConfigBlink(%player)
{
    if(!%player.faceConfigSanityCheck())
    {
        return;
    }

    %faceConfig = %player.faceConfig;
    %player.setFaceName(%player.faceConfig.getFace("Blink"));

    if(%faceConfig.getFaceAttribute("Blink", "closedLength") $= "")
    {
        %faceConfig.setFaceAttribute("Blink", "closedLength", getRandom(100, 400)); //The average blink lasts between 0.1 to 0.4 seconds.
    }

    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }
    
    %blinkSchedule  = %player.schedule(%player.faceConfig.getFaceAttribute("Blink", "closedLength"), "faceConfigUnblink");
    %player.faceConfigBlinkSchedule = %blinkSchedule;
    %faceConfig.animationQueue.push(%blinkSchedule);
    return %blinkSchedule;
}

function Player::beginFaceConfigBlinkSchedule(%player)
{
    //Doesn't work if you try to set the face immediately after spawn, so we just have to settle for this small delay.
    if(!%player.faceConfigSanityCheck())
    {
        return;
    }
    %player.schedule(1, "faceConfigUnblink");
}

function Player::faceConfigShowFace(%player, %name)
{
    if(!%player.faceConfigSanityCheck())
    {
        return;
    }

    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }

    %faceConfig = %player.faceConfig;
    %player.setFaceName(%faceConfig.getFace(%name));

    %faceDisplayLength = %faceConfig.getFaceAttribute(%name, "length");
    if(%faceConfig.isFace("Blink"))
    {
        if(%faceDisplayLength $= "")
        {
            //Faces will have a default display time of 4 seconds.
            %blinkSchedule = %player.schedule(4000, "faceConfigUnblink");
            %player.faceConfigBlinkSchedule = %blinkSchedule;
            %faceConfig.animationQueue.push(%blinkSchedule);
            return %blinkSchedule;
        }
        else 
        {
            %blinkSchedule = %player.schedule(%faceDisplayLength, "faceConfigUnblink");
            %player.faceConfigBlinkSchedule = %blinkSchedule;
            %faceConfig.animationQueue.push(%blinkSchedule);
            return %blinkSchedule;
        }
    }
    else
    {
        if(%faceDisplayLength $= "")
        {
            %player.schedule(4000, "faceConfigShowFaceTimed", "Neutral", -1); 
        }
        else
        {
           %player.schedule(%faceDisplayLength, "faceConfigShowFaceTimed", "Neutral", -1); 
        }
        
    }
}

function Player::faceConfigShowFaceTimed(%player, %name, %time)
{
    if(!%player.faceConfigSanityCheck())
    {
        return;
    }

    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }

    %faceConfig = %player.faceConfig;
    %player.setFaceName(%faceConfig.getFace(%name));
    
    if(%time != -1)
    {
        if(%faceConfig.isFace("Blink"))
        {
            %blinkSchedule = %player.schedule(%time, "faceConfigUnblink");
            %player.faceConfigBlinkSchedule = %blinkSchedule;
            %faceConfig.animationQueue.push(%blinkSchedule);
            return %blinkSchedule;
        }
        else
        {
            %player.schedule(%time, "faceConfigShowFaceTimed", "Neutral", -1);
        }
    }
}

function Player::faceConfigTalkAnimation(%player, %message)
{
    if(!%player.faceConfigSanityCheck())
    {
        return;
    }

    %faceConfig = %player.faceConfig;

    %simTime = getSimTime();
    if(%player.finishTalkingTime != 0 && (%simTime < %player.finishTalkingTime))
    {
        %milisecondTimeIndex = (%player.finishTalkingTime - %simTime);
    }
    else
    {
        %milisecondTimeIndex = 0;
    }

    for(%i = 0; %i < strlen(%message); %i++)
    {
        //For every letter in the message...
        %currentLetter = strlwr(getSubStr(%message, %i, 1));
        %speakingTime = 50; //By default, 50 miliseconds to pronounce each letter. Matches the default talking animation.

        if(%currentLetter $= "u" || %currentLetter $= "o" || %currentLetter $= "r" || %currentLetter $= "w" || %currentLetter $= "q")
        {
            %faceName  = "Oh";
        }
        else if(%currentLetter $= "e" || %currentLetter $= "c" || %currentLetter $= "s" || %currentLetter $= "n" || %currentLetter $= "k" || %currentLetter $= "z" || %currentLetter $= "j" || %currentLetter $= "s" || %currentLetter $= "t" || %currentLetter $= "f" || %currentLetter $= "v" || %currentLetter $= "h" || %currentLetter $= "g" || %currentLetter $= "x" || %currentLetter $= "y")
        {
            %faceName = "Tooth";
        }
        else if(%currentLetter $= "m" || %currentLetter $= "p" || %currentLetter $= "b")
        {
            %faceName = "Smirk"; //Relaxed lips.
        }
        else
        {
            %faceName = "Smiley";
        }

        if(%faceConfig.isFace(%faceName))
        {
            %talkAnimation = %player.schedule(%milisecondTimeIndex, "faceConfigShowFaceTimed", %faceName, %speakingTime); //Parsed lips.
            %faceConfig.animationQueue.push(%talkAnimation);
        }  

        %milisecondTimeIndex += %speakingTime;
    }

    %player.finishTalkingTime = getSimTime() + %milisecondTimeIndex;
}

//
// Emotes, spawning, clean-up.
//

package Gamemode_Eventide_FaceSystem
{
    function Armor::onDisabled(%this, %player, %state)
    {
        //When a player dies, end all facial expression and close their eyes.
        %faceConfig = %player.faceConfig;
        if(isObject(%faceConfig))
        {
            %deathFace = (%faceConfig.isFace("Death")) ? %faceConfig.getFace("Death") : %faceConfig.getFace("Blink");
            %faceConfig.delete();
            %player.setFaceName(%deathFace);
        }

        Parent::onDisabled(%this, %player, %state);
    }
    
    function Armor::onRemove(%this, %player)
    {
        //In case the minigame resets, in which case onDisabled is not called.
        %faceConfig = %player.faceConfig;
        if(isObject(%faceConfig))
        {
            %faceConfig.delete();
        }

        Parent::onRemove(%this, %player);
    }

    function Player::emote(%player, %emote)
    {
        //Play a facial expression when the player emotes.
        Parent::emote(%player, %emote);

        %faceConfig = %player.faceConfig;
        if(!isObject(%faceConfig))
        {
            return;
        }

        %choice = "Neutral";
        switch$(%emote.getName())
        {
            case "LoveImage": //Love.
                %choice = "Smirk";
            case "AlarmProjectile": //Alarm.
                %choice = "Tooth";
            case "WtfImage": //Confusion.
                %choice = "Oh";
            case "HateImage": //Hate
                %choice = "Blink";
            case "winStarProjectile": //Win - plays at the end of a Slayer round, or when a player finds a Treasure Chest.
                %choice = "SmirkSquint";
            default:
                return;
        }

        //In case the face is not present, this is better than getting the default smiley.
        if(!%faceConfig.isFace(%choice))
        {
            %choice = "Neutral";
        }
        %player.faceConfigShowFace(%choice);
    }

    function serverCmdMessageSent(%client, %message)
    {
        //Make's a player's mouth move when they speak.
        Parent::serverCmdMessageSent(%client, %message);

        %player = %client.player;
        %faceConfig = %player.faceConfig;
        if(isObject(%player) && isObject(%faceConfig) && %faceConfig.isFace("Smiley"))
        {
            %player.faceConfigTalkAnimation(%message);
        }
    }
    function serverCmdTeamMessageSent(%client, %message)
    {
        //Make's a player's mouth move when they speak.
        Parent::serverCmdTeamMessageSent(%client, %message);

        %player = %client.player;
        %faceConfig = %player.faceConfig;
        if(isObject(%player) && isObject(%faceConfig) && %faceConfig.isFace("Smiley"))
        {
            %player.faceConfigTalkAnimation(%message);
        }
    }

    function Player::addHealth(%obj, %amount)
    {
        //If hurt, change their face pack. If no longer hurt, change it back to normal.
        Parent::addHealth(%obj, %amount);

        if(%amount < 0 && isObject(%obj.faceConfig) && !%obj.getState() $= "Dead")
        {
            if(%obj.getDamageLevel() > 33 && %obj.faceConfig.subCategory !$= "Hurt" && %obj.hasSubfacePack("Hurt"))
            {
                %obj.createSubfaceConfig("Hurt");
            }
        }
        else if(%amount > 0 && isObject(%obj.faceConfig))
        {
            if(%obj.getDamageLevel() < 33 && %obj.faceConfig.subCategory $= "Hurt")
            {
                %obj.revertSubfaceConfig();
            }
        }
    }
    function Player::setHealth(%obj, %amount)
    {
        //If hurt, change their face pack. If no longer hurt, change it back to normal.
        Parent::setHealth(%obj, %amount);

        if(%amount > 0 && isObject(%obj.faceConfig))
        {
            if(%obj.getDamageLevel() > 33 && %obj.faceConfig.subCategory !$= "Hurt" && %obj.hasSubfacePack("Hurt"))
            {
                %obj.createSubfaceConfig("Hurt");
            }
            else if(%obj.faceConfig.subCategory $= "Hurt")
            {
                %obj.revertSubfaceConfig();
            }
        }
    }
    function Player::setDamageLevel(%obj, %amount)
    {
        Parent::setDamageLevel(%obj, %amount);

        if(isObject(%obj.faceConfig) && !%obj.getState() $= "Dead")
        {
            if(%obj.getDamageLevel() > 33 && %obj.faceConfig.subCategory !$= "Hurt" && %obj.hasSubfacePack("Hurt"))
            {
                %obj.createSubfaceConfig("Hurt");
            }
            else if(%obj.faceConfig.subCategory $= "Hurt")
            {
                %obj.revertSubfaceConfig();
            }
        }
    }

    function destroyServer()
    {
        //These are ScriptObjects, which the garbage collector will never automatically delete, so we need to do it manually.
        deleteVariables("$Eventide_*");

        Parent::destroyServer();
    }
};
activatePackage(Gamemode_Eventide_FaceSystem);