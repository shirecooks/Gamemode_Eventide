//
// Core functionality.
//

function GameConnection::addBillboard(%this, %object, %bitmapPath)
{
    CommandToClient(%this, 'EventideAddBillboard', %object.getId(), %this.getGhostID(%object), %object.getPosition(), %bitmapPath);
}

function GameConnection::removeBillboard(%this, %object)
{
    CommandToClient(%this, 'EventideRemoveBillboard', %object.getId());
}

function GameConnection::removeAllBillboards(%this)
{
    CommandToClient(%this, 'EventideRemoveAllBillboards');
}

function serverCmdEventideGetBillboardObjectPosition(%client, %object)
{
    if(!isObject(%objectId))
    {
        %returnPosition = "0 0 0";
    }
    else if(%object.getDatablock().isDowned)
    {
        %returnPosition = %object.getPosition();
    }
    CommandToClient(%client, 'EventideReceiveBillboardPosition', %object.getId(), %returnPosition);
}

//
// Downed player helpers.
//

function Player::retractDownedBillboard(%player)
{
    %minigame = getMinigameFromObject(%player);
    if(!isObject(%minigame))
    {
        return;
    }

    for(%i = 0; %i < %minigame.numMembers; %i++)
	{
		%targetClient = %minigame.member[%i];
		%targetClient.removeBillboard(%player);
	}
}

function Player::broadcastDownedBillboard(%player)
{
    %minigame = getMinigameFromObject(%player);
    if(!isObject(%minigame))
    {
        return;
    }

    for(%i = 0; %i < %minigame.numMembers; %i++)
	{
		%targetClient = %minigame.member[%i];
		%targetClient.addBillboard(%player, "Add-Ons/Client_Eventide/modules/textures/downed.png");
	}
}