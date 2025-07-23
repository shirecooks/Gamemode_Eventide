function GameConnection::playAmbiantMusic(%this, %musicDatablock, %priority, %volume, %override)
{
    if(%priority $= "")
    {
        %priority = 0;
    }

    if(%volume $= "")
    {
        %volume = 1.0;
    }

    %eventideMusicEmitter = %this.eventideMusicEmitter;
    if(isObject(%eventideMusicEmitter))
    {
        //The music currently playing is more important than the music to be played.
        //For example, round end tension music has higher priority than chase music.
        //If that is the case, do nothing.
        if(%eventideMusicEmitter.priority > %priority && !%override)
        {
            return;
        }
        
        %eventideMusicEmitter.delete();
    }

    %eventideMusicEmitter = new AudioEmitter()
    {
        priority = %priority;
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
function Player::playAmbiantMusic(%obj, %musicDatablock, %priority, %volume, %override)
{
    %client = %obj.client;
    if(isObject(%client))
    {
        return %client.playAmbiantMusic(%musicDatablock, %priority, %volume, %override);
    }
}

function GameConnection::playRandomAmbiantTrack(%this, %override)
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
    return %this.playAmbiantMusic(%musicDatablock, 0, 1.0, %override);
}
function Player::playRandomAmbiantTrack(%obj, %override)
{
    %client = %obj.client;
    if(isObject(%client))
    {
        return %client.playRandomAmbiantTrack(%override);
    }
}

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
};
if(isPackage(Gamemode_Eventide_AmbiantMusic))
{
    deactivatePackage(Gamemode_Eventide_AmbiantMusic);
}
activatePackage(Gamemode_Eventide_AmbiantMusic);