$Eventide_FacePacks["isGlobalFacePackArray"] = true; //All face packs will be stored in this array.
$Eventide_FaceDatas["isGlobalFaceDataArray"] = true; //All face data objects will be stored in this array.
$Eventide_FaceConfigs["isGlobalFaceConfigArray"] = true; //You get the idea.

//
// Miscellaneous functions.
//

function cloneScriptObject(%targetObject)
{
    %targetObjectName = %targetObject.getName();
    %targetObject.setName("targetScriptObject");

    %cloneObject = new ScriptObject(cloneScriptObject : targetScriptObject);

    %targetObject.setName(%targetObjectName);
    %cloneObject.setName("");

    return %cloneObject;
}

function getFileString(%pattern)
{
    %fileString = "";
    for(%file = findFirstFile(%pattern); %file !$= ""; %file = findNextFile(%pattern))
    {
        if(%fileString $= "")
        {
            %fileString = %file;
        }
        else
        {
            %fileString = %fileString SPC %file;
        }
    }

    return %fileString;
}

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
        category = %facePack.category;
        subCategory = %facePack.subCategory;
    };
    %faceData.setName("faceData_" @ %faceName);

    $Eventide_FaceDatas[%facePack.category @ "_" @ %facePack.subCategory @ "_" @ %faceName] = %faceData;

    return %faceData;
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

        echo(" - Created face: \"" @ %faceName @ "\"");
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

            echo(" - Created face: \"" @ %faceName @ "\"");
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
        face["Blink"] = %facePack.getFaceData();
        face["Neutral"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Neutral"));
        face["Oh"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Oh"));
        face["Scared"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Scared"));
        face["Smiley"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Smiley"));
        face["Smirk"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Smirk"));
        face["SmirkSquint"] = %facePack.getFaceData(compileFaceDataName(%facePack, "SmirkSquint"));
        face["Tooth"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Tooth"));

        face["Pain"] = %facePack.getFaceData(%facePack.category @ "Pain");
        face["Pain", "length"] = 2000;
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
    %obj.previousFacePack = %obj.facePack.getID();
    %obj.facePack = %facePack;
}

function FaceConfig::cacheFace(%obj, %name)
{
    %facePack = %obj.getFacePack();
    %face = %facePack.getFaceData(%name);
    if(%face !$= "")
    {
        %obj.face[%name] = %face;
    }
    else if(%facePack.getFaceData(%facePack.category @ %name) !$= "")
    {
        %obj.face[%name] = %facePack.getFaceData(%facePack.category @ %name);
    }
    else if(%facePack.getFaceData(compileFaceDataName(%facePack, %name)) !$= "")
    {
        %obj.face[%name] = %facePack.getFaceData(compileFaceDataName(%facePack, %name));
    }
    else
    {
        //Fallback, no face found.
        return "smiley"; 
    }
}

function FaceConfig::getFace(%obj, %name)
{
    if(%obj.face[%name] $= "")
    {
        echo("Cached face name" SPC %name SPC "|" SPC $obj.getID());
        %result = %obj.cacheFace(%name);
        if(%result $= "smiley")
        {
            //No face found under that name.
            return "smiley";
        }
    }
    %obj.currentFace = fileBase(%obj.face[%name].getFile());
    return %obj.currentFace;
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

//
// Loading functions.
//

function getFacePackTree(%startingDirectory)
{
    %container = new ScriptObject() { _["index"] = ""; }; //Local arrays can't be returned from a function, so we must store it in a ScriptObject.

    //Get main face packs, store them all at an "index" position so they can be found later.
    %mainFacePackString = getFileString(%startingDirectory @ "/*.etfp");
    %container._["index"] = %mainFacePackString;
    
    for(%i = 0; %i < getWordCount(%mainFacePackString); %i++)
    {
        %facePack = getWord(%mainFacePackString, %i);
        %container._[%facePack] = getFileString(filePath(%facePack) @ "/*.etsp"); //Get sub-face packs, assign to the value of the parent face pack in the array.
    }

    return %container;
}

function parseFacePacks(%startingDirectory)
{    
    %facePackDictionary = getFacePackTree(%startingDirectory);
    for(%i = 0; %i < getWordCount(%facePackDictionary._["index"]); %i++)
    {
        %facePackFile = getWord(%facePackDictionary._["index"], %i);
        %facePackPath = filePath(%facePackFile);
        %facePackFileName = fileBase(%facePackFile);

        echo("Parsing face pack \"" @ %facePackFileName @ "\" from \"" @ %facePackPath @ "\"...");

        %facePack = createFacePack(%facePackPath, %facePackFileName);

        //Check for subpacks.
        %subFaceFilePattern = %facePackPath @ "/*.etsp";
        for(%j = 0; %j < getWordCount(%facePackDictionary._[%facePackFile]); %j++)
        {
            %subFaceFile = getWord(%facePackDictionary._[%facePackFile], %j);
            %subFacePackPath = filePath(%subFaceFile);
            %subFacePackFileName = fileBase(%subFaceFile);

            echo("Parsing sub-face pack \"" @ %subFacePackFileName @ "\" from \"" @ %subFacePackPath @ "\"...");

            %subFacePack = createSubFacePack(%subFacePackPath, %subFacePackFileName, %facePack);
        }
    }

    %facePackDictionary.delete(); //Don't need it anymore, delete it to prevent a memory leak.
}

//
// Player functions/displaying faces.
//

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

    %player.faceConfig = createFaceConfig(%facePack);
    %player.beginFaceConfigBlinkSchedule();
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
    if(!isObject(%player) || %player.getDamagePercent() == 1)
    {
        return;
    }

    %player.setFaceName(%player.faceConfig.getFace("Neutral"));
    if(%player.faceConfig.subCategory $= "Scared")
    {
        if(%player.faceConfig.getFaceAttribute("Blink", "scaredOpenLength") $= "")
        {
            %player.faceConfig.setFaceAttribute("Blink", "scaredOpenLength", (5000 + getRandom(0, 2000)));
        }
        %blinkDelay = %player.faceConfig.getFaceAttribute("Blink", "scaredOpenLength"); 
    }
    else
    {
        if(%player.faceConfig.getFaceAttribute("Blink", "openLength") $= "")
        {
            %player.faceConfig.setFaceAttribute("Blink", "openLength", (3000 + getRandom(0, 750)));
        }
        %blinkDelay = %player.faceConfig.getFaceAttribute("Blink", "openLength"); 
    }

    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }
    %player.faceConfigBlinkSchedule = %player.schedule(%blinkDelay, "faceConfigBlink");
    return %player.faceConfigBlinkSchedule;
}

function Player::faceConfigBlink(%player)
{
    if(!isObject(%player) || %player.getDamagePercent() == 1)
    {
        return;
    }

    %customBlink = %player.faceConfig.getFaceAttribute(%player.faceConfig.currentFace, "blinkFace");
    if(%customBlink !$= "")
    {
        %player.setFaceName(%player.faceConfig.getFace(%customBlink));
    }
    else
    {
        %player.setFaceName(%player.faceConfig.getFace("Blink"));
    }

    if(%player.faceConfig.getFaceAttribute("Blink", "closedLength") $= "")
    {
        %player.faceConfig.setFaceAttribute("Blink", "closedLength", getRandom(100, 400)); //The average blink lasts between 0.1 to 0.4 seconds.
    }

    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }
    
    %player.faceConfigBlinkSchedule = %player.schedule(%player.faceConfig.getFaceAttribute("Blink", "closedLength"), "faceConfigUnblink");
    return %player.faceConfigBlinkSchedule;
}

function Player::beginFaceConfigBlinkSchedule(%player)
{
    //Doesn't work if you try to set the face immediately after spawn, so we just have to settle for this small delay.
    %player.schedule(1, "faceConfigUnblink");
}

function Player::faceConfigShowFace(%player, %name)
{
    if(!isObject(%player) || %player.getDamagePercent() == 1)
    {
        return;
    }

    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }

    %player.setFaceName(%player.faceConfig.getFace(%name));

    %faceDisplayLength = %player.faceConfig.getFaceAttribute(%name, "length");
    if(%player.faceConfig.isFace("Blink"))
    {
        if(%faceDisplayLength $= "")
        {
            //Faces will have a default display time of 4 seconds.
            %player.faceConfigBlinkSchedule = %player.schedule(4000, "faceConfigUnblink");
            return %player.faceConfigBlinkSchedule;
        }
        else 
        {
            %player.faceConfigBlinkSchedule = %player.schedule(%faceDisplayLength, "faceConfigUnblink");
            return %player.faceConfigBlinkSchedule;
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
    if(!isObject(%player) || %player.getDamagePercent() == 1)
    {
        return;
    }

    if(isEventPending(%player.faceConfigBlinkSchedule))
    {
        cancel(%player.faceConfigBlinkSchedule);
    }

    %player.setFaceName(%player.faceConfig.getFace(%name));
    
    if(%time != -1)
    {
        if(%player.faceConfig.isFace("Blink"))
        {
            %player.faceConfigBlinkSchedule = %player.schedule(%time, "faceConfigUnblink");
            return %player.faceConfigBlinkSchedule;
        }
        else
        {
            %player.schedule(%time, "faceConfigShowFaceTimed", "Neutral", -1);
        }
    }
}

function Player::faceConfigShowDeathFace(%player)
{
    if(!isObject(%player) || !isObject(%player.faceConfig))
    {
        return;
    }

    if(%player.faceConfig.isFace("Death"))
    {
        %player.setFaceName(%player.faceConfig.getFace("Death"));
    }
    else if(%player.faceConfig.isFace("Blink"))
    {
        %player.setFaceName(%player.faceConfig.getFace("Blink"));
    }
}

function Player::faceConfigTalkAnimation(%player, %message)
{
    if(!isObject(%player) || !isObject(%player.faceConfig))
    {
        return;
    }

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
        %speakingTime = 50; //By default, 50 miliseconds to pronounce each letter. Changes for commas, periods, colons and semicolons.

        if(%currentLetter $= "u" || %currentLetter $= "o" || %currentLetter $= "r" || %currentLetter $= "w" || %currentLetter $= "q")
        {
            if(%player.faceConfig.isFace("Oh"))
            {
                %player.schedule(%milisecondTimeIndex, "faceConfigShowFaceTimed", "Oh", %speakingTime); //Parsed lips.
            }
        }
        else if(%currentLetter $= "e" || %currentLetter $= "c" || %currentLetter $= "s" || %currentLetter $= "n" || %currentLetter $= "k" || %currentLetter $= "z" || %currentLetter $= "j" || %currentLetter $= "s" || %currentLetter $= "t" || %currentLetter $= "f" || %currentLetter $= "v" || %currentLetter $= "h" || %currentLetter $= "g" || %currentLetter $= "x" || %currentLetter $= "y")
        {
            if(%player.faceConfig.isFace("Tooth"))
            {
                %player.schedule(%milisecondTimeIndex, "faceConfigShowFaceTimed", "Tooth", %speakingTime); //Closed teeth.
            }
        }
        else if(%currentLetter $= "m" || %currentLetter $= "p" || %currentLetter $= "b")
        {
            if(%player.faceConfig.isFace("Smirk"))
            {
                %player.schedule(%milisecondTimeIndex, "faceConfigShowFaceTimed", "Smirk", %speakingTime); //Relaxed lips.
            }
        }
        else if(%currentLetter $= "." || %currentLetter $= "?")
        {
            %speakingTime = 350;
            %player.schedule(%milisecondTimeIndex, "faceConfigShowFaceTimed", "Neutral", %speakingTime); //Not talking.
        }
        else if(%currentLetter $= "," || %currentLetter $= ":" || %currentLetter $= ";")
        {
            %speakingTime = 200;
            %player.schedule(%milisecondTimeIndex, "faceConfigShowFaceTimed", "Neutral", %speakingTime); //Not talking.
        }
        else
        {
            if(%player.faceConfig.isFace("Smiley"))
            {
                %player.schedule(%milisecondTimeIndex, "faceConfigShowFaceTimed", "Smiley", %speakingTime); //Open mouth.
            }
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
    function EventidePlayer::onNewDataBlock(%this, %player)
    {
        parent::onNewDataBlock(%this, %player);
        if(isObject(%player.client) && !%player.getDataBlock().isKiller)
        {
            if(isObject(%player.victimreplicatedclient))
            {
                //Skinwalker support: Need to pull the face appearance from the victim's client, not the skinwalker's.
                %client = %player.victimreplicatedclient;
            }
            else
            {
                %client = %player.client;
            }

            //Curvy chest = female, blocky chest = male.
            %player.createFaceConfig((%client.chest ? $Eventide_FacePacks["female"] : $Eventide_FacePacks["male"]));
        }
    }
    function EventidePlayer::onDisabled(%this, %player, %state)
    {
        //When a player dies, end all facial expression and close their eyes.
        if(isObject(%player.client))
        {
            if(isEventPending(%player.faceConfigBlinkSchedule))
            {
                cancel(%player.faceConfigBlinkSchedule);
            }

            if(%player.faceConfig.isFace("Blink"))
            {
                //Again, playing the face immediately just doesn't work, so we add a little delay. Consider it an aesthetic transition period.
                %player.faceConfigShowDeathFace();
            }

            if(isObject(%player.faceConfig))
            {
                %player.faceConfig.delete();
            }
        }
        parent::onDisabled(%this, %player, %state);
    }
    function EventidePlayer::onRemove(%this, %player)
    {
        //In case the minigame resets, in which case onDisabled is not called.
        parent::onRemove(%this, %player);
        //This shouldn't be needed, but just in case...
        if(isObject(%player.faceConfig))
        {
            %player.faceConfig.delete();
        }
    }

    function Player::emote(%player, %emote)
    {
        //Play a facial expression when the player emotes.
        parent::emote(%player, %emote);
        if(isObject(%player.faceConfig))
        {
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
            if(!%player.faceConfig.isFace(%choice))
            {
                %choice = "Neutral";
            }
            %player.faceConfigShowFace(%choice);
        }
    }

    function serverCmdMessageSent(%client, %message)
    {
        //Make's a player's mouth move when they speak.
        parent::serverCmdMessageSent(%client, %message);
        %player = %client.player;
        if(isObject(%player) && isObject(%player.faceConfig) && %player.faceConfig.isFace("Smiley"))
        {
            %player.faceConfigTalkAnimation(%message);
        }
    }
    function serverCmdTeamMessageSent(%client, %message)
    {
        //Make's a player's mouth move when they speak.
        parent::serverCmdTeamMessageSent(%client, %message);
        %player = %client.player;
        if(isObject(%player) && isObject(%player.faceConfig) && %player.faceConfig.isFace("Smiley"))
        {
            %player.faceConfigTalkAnimation(%message);
        }
    }

    function Player::addHealth(%obj, %amount)
    {
        //If hurt, change their face pack. If no longer hurt, change it back to normal.
        parent::addHealth(%obj, %amount);
        if(isObject(%obj.faceConfig))
        {
            if(%obj.getDamagePercent() < 0.33 && %obj.faceConfig.subCategory $= "Hurt")
            {
                %obj.createFaceConfig($Eventide_FacePacks[%obj.faceConfig.category]);
            }
        }
    }
    function Player::setHealth(%obj, %amount)
    {
        //If hurt, change their face pack. If no longer hurt, change it back to normal.
        parent::setHealth(%obj, %amount);
        if(isObject(%obj.faceConfig))
        {
            if(%obj.getDamagePercent() > 0.33 && %obj.faceConfig.subCategory !$= "Hurt" && $Eventide_FacePacks[%obj.faceConfig.category, "Hurt"] !$= "")
            {
                %obj.createFaceConfig($Eventide_FacePacks[%obj.faceConfig.category, "Hurt"]);
            }
            else if(%obj.faceConfig.subCategory $= "Hurt")
            {
                %obj.createFaceConfig($Eventide_FacePacks[%obj.faceConfig.category]);
            }
        }
    }
    function Player::setDamageLevel(%obj, %amount)
    {
        parent::setDamageLevel(%obj, %amount);
        if(isObject(%obj.faceConfig))
        {
            if(%obj.getDamagePercent() > 0.33 && %obj.faceConfig.subCategory !$= "Hurt" && $Eventide_FacePacks[%obj.faceConfig.category, "Hurt"] !$= "")
            {
                %obj.createFaceConfig($Eventide_FacePacks[%obj.faceConfig.category, "Hurt"]);
            }
            else if(%obj.faceConfig.subCategory $= "Hurt")
            {
                %obj.createFaceConfig($Eventide_FacePacks[%obj.faceConfig.category]);
            }
        }
    }

    function destroyServer()
    {
        //These are ScriptObjects, which the garbage collector will never automatically delete, so we need to do it manually.
        deleteVariables("$Eventide_*");
        parent::destroyServer();
    }
};
activatePackage(Gamemode_Eventide_FaceSystem);