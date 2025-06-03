function GameConnection::addBillboard(%this, %object)
{
    CommandToClient(%this, 'EventideAddBillboard', %object.getId(), %this.getGhostID(%object), %object.getPosition());
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
    // else if(%object.getDatablock().isDowned)
    // {
    //     %returnPosition = %object.getPosition();
    // }
    %returnPosition = %object.getPosition();
    CommandToClient(%client, 'EventideReceiveBillboardPosition', %object.getId(), %returnPosition);
}

