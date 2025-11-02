function GameConnection::counterPrint(%this, %amount, %message, %time)
{
    %outputString = "";

    if(%time $= "")
    {
        %time = 1;
    }

    %symbol = "|";
    for(%i = 0; %i < %amount; %i++) 
	{
		%outputString = %outputString @ %symbol;
	}

    %this.centerPrint("<font:impact:30>\c3" @ %message @ " <br>\c2" @ %outputString, 1);
}

//
// Functions and a package too lock the player's inputs for cutscenes.
//

function Observer::checkInputsLocked(%this, %obj)
{
    %client = %obj.getControllingClient();
    %player = %client.player;
    if(!%player)
    {
        return false;
    }

    return (%client.lockInputs || (%player && %player.lockInputs));
}

function GameConnection::checkInputsLocked(%client)
{
    %player = %client.player;
    if(!%player)
    {
        return false;
    }

    return (%player.lockInputs || %client.lockInputs);
}

function Player::checkInputsLocked(%obj)
{
    %client = %obj.client;

    return (%obj.lockInputs || (%client && %client.lockInputs));
}

//
//// Lock tools specifically.
function Observer::checkToolsLocked(%this, %obj)
{
    %client = %obj.getControllingClient();
    %player = %client.player;

    return (!%player || %player.lockTools || %client.lockTools);
}

function GameConnection::checkToolsLocked(%client)
{
    %player = %client.player;

    return (!%player || %player.lockTools || %client.lockTools);
}

function Player::checkToolsLocked(%obj)
{
    %client = %obj.client;

    return (!%player || %player.lockTools || (%client && %client.lockTools));
}

package Support_Client
{
    function Observer::onTrigger(%this, %obj, %trigger, %state)
    {
        if(%this.checkInputsLocked(%obj))
        {
            return;
        }

        return Parent::onTrigger(%this, %obj, %trigger, %state);
    }

    function serverCmdUseTool(%client, %slot)
    {
        if(%client.checkInputsLocked() || %client.checkToolsLocked())
        {
            return;
        }

        return parent::ServerCmdUseTool(%client, %slot);
    }

    function ServerCmdUnUseTool(%client)
    {
        if(%client.checkInputsLocked() || %client.checkToolsLocked())
        {
            return;
        }

        parent::ServerCmdUnUseTool(%client);
    }

    function ServerCmdPlantBrick(%client)
    {
        if(%obj.checkInputsLocked())
        {
            return;
        }

        return parent::ServerCmdPlantBrick(%client);
    }
};
if(isPackage(Support_Client))
{
    deactivatePackage(Support_Client);
}
activatePackage(Support_Client);

//
// Also for cutscenes: setting the camera to orbit the player.
//

function Player::createCameraOrbit(%obj)
{
    %client = %obj.client;
    if(!%client)
    {
        return;
    }

    %camera = %client.camera;
    %client.setControlObject(%camera);
	%camera.setMode("Corpse", %obj);
}

function Player::restoreCameraFromOrbit(%obj)
{
    %client = %obj.client;
    if(!%client)
    {
        return;
    }

    %client.setControlObject(%obj);
	%client.camera.setMode("Observer");
}