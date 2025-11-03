//
// Playing ambiant music tracks.
//

function GameConnection::playAmbiantMusic(%this, %musicDatablock, %volume, %category)
{
    if(%category $= "")
    {
        %category = "Ambiant";
    }

    if(%volume $= "")
    {
        %volume = 1.0;
    }

    %eventideMusicEmitter = %this.eventideMusicEmitter;
    if(isObject(%eventideMusicEmitter))
    {
        //Same type of music, skip.
        if(%eventideMusicEmitter.category $= %category)
        {
            return;
        }
        
        %eventideMusicEmitter.delete();
    }

    %eventideMusicEmitter = new AudioEmitter()
    {
        category = %category;
        position = "9e9 9e9 9e9";
        profile = %musicDatablock;
        volume = %volume;
        type = 10;
        useProfileDescription = false;
        is3D = false;
    };
    %this.eventideMusicEmitter = %eventideMusicEmitter;
    %eventideMusicEmitter.adjustObjectScopeToAll(false, %this);

    return %eventideMusicEmitter;
}
function Player::playAmbiantMusic(%obj, %musicDatablock, %volume, %category)
{
    %client = %obj.client;
    if(isObject(%client))
    {
        return %client.playAmbiantMusic(%musicDatablock, %volume, %category);
    }
}

//
// Playing a RANDOM ambiant music track.
//

function GameConnection::playRandomAmbiantTrack(%this)
{
    //Decide which category of ambiant music to play.
    //If enabled, do Robb's choice. If disabled, do Muna's choice.
    if($Pref::Eventide::PlayAltAmbiance)
    {
        %musicDatablockBase = "altAmbiance";
        %trackCount = 12;
    }
    else
    {
        %musicDatablockBase = "ambiance";
        %trackCount = 7;
    }

    //Never play the same ambiant track twice in a row.
    %lastAmbiantTrackChoice = %this.lastAmbiantTrackChoice;
    if(%lastAmbiantTrackChoice $= "")
    {
        %ambiantTrackChoice = getRandom(1, %trackCount);
    }
    else
    {
        if(%trackCount > 1)
        {
            if(%lastAmbiantTrackChoice == 1)
            {
                //Landed on the first choice last time.
                %ambiantTrackChoice = getRandom(2, %trackCount);
            }
            else if(%lastAmbiantTrackChoice == %trackCount)
            {
                //Landed on the last choice last time.
                %ambiantTrackChoice = getRandom(1, %trackCount - 1);
            }
            else
            {
                %ambiantTrackChoice = getRandom(1, %trackCount);
                if(%ambiantTrackChoice == %lastAmbiantTrackChoice)
                {
                    //Avoid the previously chose choice by randomly subtracting or adding one.
                    %ambiantTrackChoice += (getRandom(0, 1) ? -1 : 1);
                }
            }
        }
        else
        {
            %ambiantTrackChoice = 1;
        }
    }
    %this.lastAmbiantTrackChoice = %ambiantTrackChoice;

    //Compile the base and choice data to get a datablock.
    %musicDatablock = NameToID("musicData_" @ %musicDatablockBase @ %ambiantTrackChoice);

    //Create an AudioEmitter with the datablock, to play the music.
    return %this.playAmbiantMusic(%musicDatablock, 1.0, "Ambiant");
}
function Player::playRandomAmbiantTrack(%obj)
{
    %client = %obj.client;
    if(isObject(%client))
    {
        return %client.playRandomAmbiantTrack();
    }
}

//
// Is ambiant music playing?
//

function GameConnection::hasAmbiantMusic(%this)
{
    return %this.eventideMusicEmitter;
}
function Player::hasAmbiantMusic(%obj)
{
    %client = %obj.client;
    if(isObject(%client))
    {
        return %client.hasAmbiantMusic();
    }
    return 0;
}

//
// Stopping ambiant music playback.
//

function GameConnection::stopAmbiantMusic(%this)
{
    %eventideMusicEmitter = %this.eventideMusicEmitter;
    if(isObject(%eventideMusicEmitter))
    {
        %eventideMusicEmitter.delete();
    }
}
function Player::stopAmbiantMusic(%obj)
{
    %client = %obj.client;
    if(isObject(%client))
    {
        %client.stopAmbiantMusic();
    }
}

//
// Accessory music playback - used for status effects and such.
//

function GameConnection::playAccessoryMusic(%this, %musicDatablock, %volume, %category)
{
    if(%category $= "")
    {
        %category = "Default";
    }

    if(%volume $= "")
    {
        %volume = 1.0;
    }

    %accessoryMusicBag = %this.accessoryMusicBag;
    if(!isObject(%accessoryMusicBag))
    {
        %accessoryMusicBag = new SimGroup();
        %this.accessoryMusicBag = %accessoryMusicBag;
    }

    %accessoryMusicEmitter = new AudioEmitter()
    {
        category = %category;
        position = "9e9 9e9 9e9";
        profile = %musicDatablock;
        volume = %volume;
        type = 10;
        useProfileDescription = false;
        is3D = false;
    };
    %accessoryMusicBag.add(%accessoryMusicEmitter);
    %accessoryMusicEmitter.adjustObjectScopeToAll(false, %this);

    return %accessoryMusicEmitter;
}
function Player::playAccessoryMusic(%obj, %musicDatablock, %volume, %category)
{
    %client = %obj.client;
    if(isObject(%client))
    {
        return %client.playAccessoryMusic(%musicDatablock, %volume, %category);
    }
}

function GameConnection::stopAccessoryMusic(%this, %category, %datablock)
{
    %accessoryMusicBag = %this.accessoryMusicBag;
    if(!isObject(%accessoryMusicBag))
    {
        return;
    }

    if(%datablock $= "" && %category $= "")
    {
        %accessoryMusicBag.clear();
        return;
    }

    for(%i = 0; %i < %accessoryMusicBag.getCount(); %i++)
    {
        %accessoryMusicEmitter = %accessoryMusicBag.getObject(%i);
        if(%accessoryMusicEmitter.profile.getID() $= %datablock || %accessoryMusicEmitter.category $= %category)
        {
            %accessoryMusicEmitter.delete();
        }
    }
}
function Player::stopAccessoryMusic(%obj, %category, %datablock)
{
    %client = %obj.client;
    if(isObject(%client))
    {
        %client.stopAccessoryMusic(%category, %datablock);
    }
}

//
// Is accessory music playing?
//

function GameConnection::hasAccessoryMusic(%this)
{
    return %this.accessoryMusicBag;
}
function Player::hasAccessoryMusic(%obj)
{
    %client = %obj.client;
    if(isObject(%client))
    {
        return %client.hasAccessoryMusic();
    }
    return 0;
}

//
// Package to play an ambient music track on player spawn, and stop it upon minigame end/anything else.
//

package Gamemode_Eventide_AmbiantMusic
{
    function PlayerEventide::onRemove(%this, %obj)
    {
        Parent::onRemove(%this, %obj);

        %client = %obj.client;
        if(isObject(%client))
        {
            %eventideMusicEmitter = %client.eventideMusicEmitter;
            %accessoryMusicBag = %client.accessoryMusicBag;

            if(isObject(%eventideMusicEmitter))
            {
                %eventideMusicEmitter.delete();
            }
            if(isObject(%accessoryMusicBag))
            {
                %accessoryMusicBag.clear();
                %accessoryMusicBag.delete();
            }
        }
    }
    function PlayerEventide::onNewDatablock(%this, %obj)
    {
        Parent::onNewDatablock(%this, %obj);

        //Play an ambiant track.
        %client = %obj.client;
        if(isObject(%client) && !%obj.hasAmbiantMusic())
        {
            %client.playRandomAmbiantTrack();
        }
    }
    function Armor::onNewDataBlock(%this, %obj)
    {
        if(!%this.isEventideClass)
        {
            if(%obj.hasAmbiantMusic())
            {
                %obj.stopAmbiantMusic();
            }
            if(%obj.hasAccessoryMusic())
            {
                %obj.stopAccessoryMusic();
            }
        }
    }
};
if(isPackage(Gamemode_Eventide_AmbiantMusic))
{
    deactivatePackage(Gamemode_Eventide_AmbiantMusic);
}
activatePackage(Gamemode_Eventide_AmbiantMusic);