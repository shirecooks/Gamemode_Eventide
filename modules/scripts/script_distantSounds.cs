//
// Support functions.
//

// Converts the angle to a 0-360 range and returns the "quadrant" (0-3) of the angle in the circle.
// 0 = 0-90, 1 = 90-180, 2 = 180-270, 3 = 270-360.
function mQuadrant(%angle)
{
    %angle = (%angle % 360) + (360 * (%angle < 0));
    return mFloor(%angle / 90) % 4;
}

//
// Creating AudioEmitters/playing a distant sound on a player.
//

function Player::playDistantSound(%player, %audioProfile)
{
    %audioEmitter = new AudioEmitter()
    {
        position = %player.getPosition();
        profile = %audioProfile;
        useProfileDescription = false;
		volume = 1;
		ReferenceDistance = "9e9";
		maxDistance = "9e9";
		isLooping = 0;
    };
    MissionCleanup.add(%audioEmitter);

    adjustObjectScopeToAll(%audioEmitter, false, %player.client); // Make sure only the target player hears it.

    // Get the millisecond length of the sound file, then divide it against 360 to make that the time needed to complete a rotation around the player.
    %soundFilePath = expandFilename(%audioEmitter.profile.fileName);
    %soundFileExtension = fileExt(%audioEmitter.profile.fileName);
    if(%soundFileExtension $= ".wav" && isFunction(getWavLength))
    {
        //BLPython module function.
        %soundLength = getWavLength(%soundFilePath);
    }
    else if(%soundFileExtension $= ".ogg" && isFunction(getOggLength))
    {
        //BLPython module function.
        %soundLength = getOggLength(%soundFilePath);
    }
    else
    {
        %soundLength = 8000; //Fallback, just say 8 seconds. Maybe too long, maybe too short.
    }
    
    %rotationSpeed = 360 / (%soundLength / 1000);
    %tickRate = 50;

    %distantSoundDataObject = new ScriptObject()
    {
        radius = "9e6"; // Impossible to run past an AudioEmitter this far away. Could be further, but higher values are bug-prone.
        rotationSpeed = %rotationSpeed;
        lastQuadrant = 0;
        player = %player;
        audioEmitter = %audioEmitter;
        soundLength = %soundLength;
    };
    MissionCleanup.add(%distantSoundDataObject);
    
    for(%i = %tickRate; %i <= %soundLength; %i += %tickRate)
    {
        %player.schedule(%i, "_distantSoundTick", %distantSoundDataObject);
    }
    // Delete this stuff once we're done with them.
    scheduleNoQuota((getSimTime() + %soundLength), %dsdo.audioEmitter, "delete");
    scheduleNoQuota((getSimTime() + %soundLength), %dsdo, "delete");

    return %distantSoundDataObject;
}

function Player::_distantSoundTick(%player, %dsdo)
{
    //If there's no data, we can't do this stuff. If the player is deleted, this function is stopped automatically.
    if(!%dsdo || !%player || !%dsdo.audioEmitter)
    {
        return;
    }

    //Use a sine wave to make the AudioEmitter rotate around the player.
    %angle = getSimTime() * %dsdo.rotationSpeed;

    //Simulate an echo by making the AudioEmitter skip every other quadrant.
    %quadrant = mQuadrant(%angle);
    if(%quadrant != %dsdo.lastQuadrant)
    {
        if((%dsdo.lastQuadrant == 0 && %quadrant == 1) || (%dsdo.lastQuadrant == 2 && %quadrant == 3))
        {
            %angle += 90; //Rotate the AudioEmitter 90 degrees, so that it skips a quadrant.
        }
        %dsdo.lastQuadrant = %quadrant;
    }

    //Calculate the new position based on the sine and cosine of the sine wave.
    %xOffset = (%dsdo.radius * mSin(%angle));
    %yOffset = (%dsdo.radius * mCos(%angle));

    %newPosition = VectorAdd(%player.getHackPosition(), %xOffset SPC %yOffset SPC "0");
    %dsdo.audioEmitter.setTransform(%newPosition);
}

//
// Deciding when distant sound effects should play.
//

function killerPlayDistantSound(%player, %category, %audioProfile, %cooldownAmount)
{
    return;
    %killers = getCurrentKillers();
    for(%i = 0; %i < %killers.getCount(); %i++)
    {
        %killer = %killers.getObject(%i).player;
        if(!isObject(%killer))
        {
            //No killer, no use.
            return;
        }

        %minimumDistance = 100; //The loudest sounds in Eventide (AudioDefault3d) reach 100 units. So, anything lower than this is not distant.
        if(VectorDist(%player.getPosition(), %killer.getPosition()) < %minimumDistance)
        {
            //Sound source isn't far enough away.
            return;
        }

        %cooldown = %killer.distantSoundData[%player.getID(), %category, "cooldown"];
        if(%cooldown > 0 && getSimTime() < %cooldown)
        {
            //The sound is under cooldown, don't play it.
            return;
        }

        //Check if the sound is already playing, and stop it first.
        %previousDsdo = %killer.distantSoundData[%player.getID(), %category];
        if(isObject(%previousDsdo))
        {
            %previousDsdo.audioEmitter.delete();
            %previousDsdo.delete();
        }

        //Check if a cooldown amount was provided, and if not, set it to 0.
        if(%cooldownAmount $= "" || %cooldownAmount < 0)
        {
            %cooldownAmount = 0;
        }

        //Finally, play the sound.
        %dsdo = %killer.playDistantSound(%audioProfile);
        %soundLength = (%dsdo.soundLength !$= "" ? %dsdo.soundLength : 8000);
        %killer.distantSoundData[%player.getID(), %category] = %dsdo;
        %killer.distantSoundData[%player.getID(), %category, "cooldown"] = (getSimTime() + %soundLength + %cooldownAmount); 
    }
}

package Eventide_distantSounds
{
    //
    // Distant footsteps sound.
    //
    function Armor::onPeggFootstep(%this, %obj)
    {
        parent::onPeggFootstep(%this, %obj);

        if(%obj.getDataBlock().isKiller)
        {
            return;
        }

        //Decide what sound to play.
        //TODO: This is hardcoded and must be changed every time you add a sound.
        //It would be cool to create another system like the face system, where this is automated.
        %distantSound = "";
        switch$(%obj.surface)
        {
            case "metal":
                %distantSound = "footsteps_metal" @ getRandom(1, 3) @ "_sound";
            case "sand":
                %distantSound = "footsteps_dirt" @ getRandom(1, 4) @ "_sound";
            case "dirt":
                %distantSound = "footsteps_dirt" @ getRandom(1, 4) @ "_sound";
            case "fabric":
                %distantSound = "footsteps_grass" @ getRandom(1, 4) @ "_sound";
            case "grass":
                %distantSound = "footsteps_grass" @ getRandom(1, 4) @ "_sound";
            case "basic":
                %distantSound = "footsteps_pavement" @ getRandom(1, 3) @ "_sound";
            case "stone":
                %distantSound = "footsteps_pavement" @ getRandom(1, 3) @ "_sound";
            case "wood":
                %distantSound = "footsteps_wood" @ getRandom(1, 3) @ "_sound";
            case "under water":
                %distantSound = "water_splash" @ getRandom(1, 4) @ "_sound";
            case "water":
                %distantSound = "water_splash" @ getRandom(1, 4) @ "_sound";
            case "snow":
                %distantSound = "footsteps_grass" @ getRandom(1, 4) @ "_sound";
            default:
                return;
        }

        %killers = getCurrentKillers();
        for(%i = 0; %i < %killers.getCount(); %i++)
        {
            %killer = %killers.getObject(%i).player;
            if(!isObject(%killer))
            {
                //No killer, no use.
                return;
            }

            killerPlayDistantSound(%obj, %obj.surface, %distantSound, 15000);
        }
    }

    //
    // Distant talking sounds.
    //

    function serverCmdMessageSent(%client, %message)
    {
        parent::serverCmdMessageSent(%client, %message);
        %player = %client.player;
        if(isObject(%player) && !%player.getDataBlock().isKiller)
        {
            %distantSound = (%client.chest ? "female" : "male") @ "_talk" @ getRandom(1, 8);
            killerPlayDistantSound(%player, "voiceTalking", %distantSound, 5000);
        }
    }

    function ChatMod_RadioMessage(%client, %message, %isTeamMessage)
    {
        Parent::ChatMod_RadioMessage(%client, %message, %isTeamMessage);
        %player = %client.player;
        if(isObject(%player) && !%player.getDataBlock().isKiller)
        {
            %distantSound = "radio_talk" @ getRandom(1, 16);
            killerPlayDistantSound(%player, "radioTalking", %distantSound, 5000);
        }
    }

    //
    // Distant item gathering sounds.
    //

    function ItemData::onPickup(%this, %obj, %user, %amount)
    {
        parent::onPickup(%this, %obj, %user, %amount);
        if(%user.getDataBlock().isKiller)
        {
            return;
        }
        if(%obj.canPickup && miniGameCanUse(%user, %obj))
        {
            %freeslot = false;
            for (%i = 0; %i < %user.getDataBlock().maxTools; %i++)
            {
                if(%user.tool[%i] == 0)
                {
                    %freeslot = true;
                    break;
                }
            }

            if(%freeslot)
            {
                %distantSound = "gather" @ getRandom(1, 3);
                killerPlayDistantSound(%user, "gathering", %distantSound, 5000);
            }
        }
    }

    //
    // Distant healing sounds.
    //

    function ZombieMedpackImage::onUse(%this, %obj, %slot)
    {
        parent::onUse(%this, %obj, %slot);
        if(!%obj.getDataBlock().isKiller && %obj.getDamageLevel() > 1.0)
        {
            %distantSound = "bandage" @ getRandom(1, 3);
            killerPlayDistantSound(%obj, "bandaging", %distantSound, 5000);
        }
    }

    function GauzeImage::onUse(%this, %obj, %slot)
    {
        parent::onUse(%this, %obj, %slot);
        if(!%obj.getDataBlock().isKiller && %obj.getDamageLevel() > 1.0)
        {
            %distantSound = "bandage" @ getRandom(1, 3);
            killerPlayDistantSound(%obj, "bandaging", %distantSound, 5000);
        }
    }
};
if(isPackage(Eventide_distantSounds))
{
    deactivatePackage(Eventide_distantSounds);
}
activatePackage(Eventide_distantSounds);