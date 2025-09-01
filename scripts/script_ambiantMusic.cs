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

        // //If this music is already playing, skip.
        // if(%eventideMusicEmitter.profile == %musicDatablock)
        // {
        //     return;
        // }
        
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
        return %client.eventideMusicEmitter;
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
        %eventideMusicEmitter = %client.eventideMusicEmitter;
        if(isObject(%eventideMusicEmitter))
        {
            %client.eventideMusicEmitter.delete();
        }
    }
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
        %eventideMusicEmitter = %client.eventideMusicEmitter;
        if(isObject(%client) && isObject(%eventideMusicEmitter))
        {
            %eventideMusicEmitter.delete();
        }
    }
    function PlayerEventide::onNewDatablock(%this, %obj)
    {
        Parent::onNewDatablock(%this, %obj);

        //Play an ambiant track.
        %client = %obj.client;
        if(isObject(%client))
        {
            %client.playRandomAmbiantTrack();
        }
    }
    function Armor::onNewDataBlock(%this, %obj)
    {
        if(!%this.isEventideClass && %obj.hasAmbiantMusic())
        {
            %obj.stopAmbiantMusic();
        }
    }
};
if(isPackage(Gamemode_Eventide_AmbiantMusic))
{
    deactivatePackage(Gamemode_Eventide_AmbiantMusic);
}
activatePackage(Gamemode_Eventide_AmbiantMusic);