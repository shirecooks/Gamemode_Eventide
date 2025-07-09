$Eventide_VoicePacks["isGlobalVoicePackArray"] = true; //All voice packs will be stored in this array.
$Eventide_VoiceConfigs["isGlobalVoiceConfigArray"] = true; //You get the idea.

//
// Class definitions.
//

// Voice packs.
///

function createVoicePack(%voicePackPath, %voiceFileCategory)
{
    %voicePack = new ScriptObject()
    {
        class = VoicePack;
        isVoicePack = true;
        category = %voiceFileCategory;
        voiceLines["isVoicePackArray"] = true; //All voice lines will be stored in this "voiceLines" array.
    };
    %voicePack.setName("voicePack_" @ %voiceFileCategory);

    %subVoicePackPaths = getFileString(%voicePackPath @ "/*.etsvp");
    for(%i = 0; %i < getFieldCount(%subVoicePackPaths); %i++)
    {
        %subVoicePack = getField(%subVoicePackPaths, %i);
        %subVoicePackPath = filePath(%subVoicePack);
        %voiceFileSubCategory = fileBase(%subVoicePack);

        %audioFiles = getFileString(%subVoicePackPath @ "/*.ogg") SPC getFileString(%subVoicePackPath @ "/*.wav");
        for(%j = 0; %j < getFieldCount(%audioFiles); %j++)
        {
            if(%voicePack.voiceLines[%voiceFileSubCategory] $= "")
            {
                %subCategoryCount = 1;
            }
            else
            {
                %subCategoryCount = %voicePack.voiceLines[%voiceFileSubCategory] + 1;
            }

            %voicePack.voiceLines[%voiceFileSubCategory] = %subCategoryCount;
        }
    }

    $Eventide_VoicePacks[%voiceFileCategory] = %voicePack;
    return %voicePack;
}

function VoicePack::getVoiceData(%obj, %name)
{
    return %obj.voiceLines[%name];
}

// Voice configs.
///

function createVoiceConfig(%voicePack)
{
    //If you need something super custom. You must set all the expression fields manually.
    %voiceConfig = new ScriptObject()
    {
        class = VoiceConfig;
        isVoiceConfig = true;
        category = %voicePack.category;
        voicePack = %voicePack;
        latestVoiceLine = "";
        voiceLines["isVoiceConfigArray"] = true;
        cooldownTime = 6000;
        soundSlot[0] = false; soundSlot[1] = false; soundSlot[2] = false; soundSlot[3] = false;
    };

    %voiceConfigName = "voiceConfig_" @ %voiceConfig.getID();
    %voiceConfig.setName(%voiceConfigName);
    $Eventide_VoiceConfigs[%voiceConfigName] = %voiceConfig;
    
    return %voiceConfig;
}

function VoiceConfig::getVoicePack(%obj)
{
    if(%obj.voicePack $= "")
    {
        return %obj.previousVoicePack;
    }
    else
    {
        return %obj.voicePack;
    }
}

function VoiceConfig::setVoicePack(%obj, %voicePack)
{
    %obj.previousVoicePack = %obj.voicePack.getID();
    %obj.voicePack = %voicePack;
}

function VoiceConfig::getLineAttribute(%obj, %subCategory, %attribute)
{
    return %obj.voiceLines[%subCategory, %attribute];
}

function VoiceConfig::setLineAttribute(%obj, %subCategory, %attribute, %value)
{
    %obj.voiceLines[%subCategory, %attribute] = %value;
}

//
// I assume the cooldown attribute will be used often enough to warrent dedicated helper functions.

function VoiceConfig::getLineCooldown(%obj, %subCategory)
{
    %customCooldownTime = %obj.getLineAttribute(%subCategory, "cooldown");
    return (%customCooldownTime !$= "") ? %customCooldownTime : %obj.cooldownTime;
}

function VoiceConfig::setLineCooldown(%obj, %subCategory, %value)
{
    %obj.setLineAttribute(%subCategory, "cooldown", %value);
}

function VoiceConfig::getVoiceLine(%obj, %subCategory, %noCooldown)
{
    %voiceLineCount = %obj.getVoicePack().getVoiceData(%subCategory);
    if(%voiceLineCount $= "")
    {
        return;
    }

    %currentTime = getSimTime();

    //Avoid spamming voice lines by preventing a subcategory from playing less than seconds after its last use.
    if(!%noCooldown)
    {
        %cooldownTime = %obj.getLineCooldown(%subCategory);
        %lastTimeVoiceLinePlayed = %obj.voiceLines[%subCategory, "lastPlayed"];
        if(%lastTimeVoiceLinePlayed !$= "" && (%currentTime - %lastTimeVoiceLinePlayed) < %cooldownTime)
        {
            return;
        }
    }

    //Never play the same voice line twice in a row.
    %lastVoiceLineChoice = %obj.voiceLines[%subCategory, "lastChoice"];
    if(%lastVoiceLineChoice $= "")
    {
        %voiceLineChoice = getRandom(1, %voiceLineCount);
    }
    else
    {
        if(%voiceLineCount > 1)
        {
            if(%lastVoiceLineChoice == 1)
            {
                //Landed on the first choice last time.
                %voiceLineChoice = getRandom(2, %voiceLineCount);
            }
            else if(%lastVoiceLineChoice == %voiceLineCount)
            {
                //Landed on the last choice last time.
                %voiceLineChoice = getRandom(1, %voiceLineCount - 1);
            }
            else
            {
                %voiceLineChoice = getRandom(1, %voiceLineCount);
                if(%voiceLineChoice == %lastVoiceLineChoice)
                {
                    //Avoid the previously chose choice by randomly subtracting or adding one.
                    %voiceLineChoice += (getRandom(0, 1) ? -1 : 1);
                }
            }
        }
        else
        {
            %voiceLineChoice = 1;
        }
    }

    %obj.voiceLines[%subCategory] = %voiceLineCount;
    %obj.voiceLines[%subCategory, "lastChoice"] = %voiceLineChoice;
    %obj.voiceLines[%subCategory, "lastPlayed"] = %currentTime;
    %obj.latestVoiceLine = %subCategory;
    
    return %obj.category @ %subCategory @ %voiceLineChoice @ "_sound";
}

//Plays a random sound within a category without being subject to slowdown or other limitations.
//Useful for things like melee swing sounds.
function VoiceConfig::getUnmanagedSound(%obj, %subCategory)
{
    %voiceLineCount = %obj.getVoicePack().getVoiceData(%subCategory);
    if(%voiceLineCount $= "")
    {
        return;
    }

    //Never play the same voice line twice in a row.
    %lastVoiceLineChoice = %obj.voiceLines[%subCategory, "lastChoice"];
    if(%lastVoiceLineChoice $= "")
    {
        %voiceLineChoice = getRandom(1, %voiceLineCount);
    }
    else
    {
        if(%voiceLineCount > 1)
        {
            if(%lastVoiceLineChoice == 1)
            {
                //Landed on the first choice last time.
                %voiceLineChoice = getRandom(2, %voiceLineCount);
            }
            else if(%lastVoiceLineChoice == %voiceLineCount)
            {
                //Landed on the last choice last time.
                %voiceLineChoice = getRandom(1, %voiceLineCount - 1);
            }
            else
            {
                %voiceLineChoice = getRandom(1, %voiceLineCount);
                if(%voiceLineChoice == %lastVoiceLineChoice)
                {
                    //Avoid the previously chose choice by randomly subtracting or adding one.
                    %voiceLineChoice += (getRandom(0, 1) ? -1 : 1);
                }
            }
        }
        else
        {
            %voiceLineChoice = 1;
        }
    }

    %obj.voiceLines[%subCategory] = %voiceLineCount;
    %obj.voiceLines[%subCategory, "lastChoice"] = %voiceLineChoice;
    
    return %obj.category @ %subCategory @ %voiceLineChoice @ "_sound";
}

function VoiceConfig::isVoiceLine(%obj, %subCategory)
{
    return (%obj.getVoicePack().getVoiceData(%subCategory) !$= "");
}

function VoiceConfig::resetSlotUse(%obj, %slotNumber)
{
    %obj.soundSlot[%slotNumber] = false;
}

//
// Loading functions.
//

function parseVoicePacks(%startingDirectory)
{    
    %voicePackPaths = getFileString(%startingDirectory @ "/*.etvp");
    for(%i = 0; %i < getFieldCount(%voicePackPaths); %i++)
    {
        %voicePackPath = filePath(getField(%voicePackPaths, %i));
        %voicePackFileName = fileBase(%voicePackPath);

        echo("Parsing voice pack \"" @ %voicePackFileName @ "\" from \"" @ %voicePackPath @ "\"...");
        %voicePack = createVoicePack(%voicePackPath, %voicePackFileName);
    }
}

//
// Player functions/playing voice lines.
//

function Player::createVoiceConfig(%obj, %voicePack)
{
    if(isObject(%obj.voiceConfig))
    {
        %obj.voiceConfig.delete();
    }
    %obj.voiceConfig = createVoiceConfig(%voicePack);
}

//Plays a random sound within a category without being subject to slowdown or other limitations.
//Useful for things like melee swing sounds.
function Player::playUnmanagedSound(%obj, %subCategory, %slot)
{
    //If the player does not have a voice config, we can't do this.
    %voiceConfig = %obj.voiceConfig;
    if(!isObject(%voiceConfig))
    {
        return;
    }

    if(%slot $= "")
    {
        %slot = 0;
    }

    %voiceLine = %voiceConfig.getUnmanagedSound(%subCategory);
    if(isObject(%voiceLine))
    {
        %obj.playAudio(%soundSlot, %voiceLine);
    }
}

//Similar to unmanaged sounds, but specifically plays on the first slot. That way, voice lines do not stack and instead override each other.
function Player::playVoiceLine(%obj, %subCategory, %noCooldown)
{
    %voiceConfig = %obj.voiceConfig;
    if(!isObject(%voiceConfig))
    {
        return;
    }
    
    //Search the voice config for the requested voice line category, and if it exists, return a random line.
    %voiceLine = %voiceConfig.getVoiceLine(%subCategory, %noCooldown);
    if(isObject(%voiceLine))
    {
        %obj.playAudio(0, %voiceLine);
    }
}

function Player::playManagedSound(%obj, %subCategory, %noCooldown)
{
    //If the player does not have a voice config, we can't do this.
    %voiceConfig = %obj.voiceConfig;
    if(!isObject(%voiceConfig))
    {
        return;
    }

    //Players can only have 4 sounds attached to them at a given time. Find an empty slot, ignoring the first slot which is reserved for voice lines.
    for(%i = 1; %i <= 3; %i++)
    {
        if(%voiceConfig.soundSlot[%i] == false)
        {
            %soundSlot = %i;
            break;
        }
    }
    //If we couldn't find an empty slot but %noCooldown is true, it's probably important. Override the latest attached audio.
    //Otherwise, just rage quit.
    if(%soundSlot $= "")
    {
        if(%noCooldown)
        {
            %soundSlot = 3;
        }
        else
        {
            return;
        }
    }

    //Search the voice config for the requested voice line category, and if it exists, return a random line.
    %voiceLine = %voiceConfig.getVoiceLine(%subCategory, %noCooldown);
    if(isObject(%voiceLine))
    {
        %obj.playAudio(%soundSlot, %voiceLine);
    }

    //Currently, the system cannot detect the length of an audio track, but let's just assume it's the length of the cooldown for now.
    //Mark a used audio slot back to unused after some time passes.
    %voiceConfig.schedule(%voiceConfig.getLineCooldown(%subCategory), "resetSlotUse", %soundSlot);
}

//
// Package to clean up VoiceConfigs.
//

package Gamemode_Eventide_VoiceSystem
{
    function EventidePlayer::onRemove(%this, %player)
    {
        //In case the minigame resets, in which case onDisabled is not called.
        if(isObject(%player.voiceConfig))
        {
            %player.voiceConfig.delete();
        }
        Parent::onRemove(%this, %player);
    }

    function destroyServer()
    {
        //These are ScriptObjects, which the garbage collector will never automatically delete, so we need to do it manually.
        deleteVariables("$Eventide_*");
        Parent::destroyServer();
    }
};
activatePackage(Gamemode_Eventide_VoiceSystem);