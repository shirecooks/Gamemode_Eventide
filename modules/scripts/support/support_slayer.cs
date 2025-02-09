package Eventide_SlayerOverride
{
    function GameConnection::spectateInit(%this)
    {
    	// If the player is a zombie, don't allow them to spectate
        if(isObject(%this.getControlObject()) && %this.getControlObject().getDataBlock().getName() $= "ShireZombieBot")
        {
            return;
        }
        
        %this.isSpectator = true;
        %target = %this.spectateNextTarget();
        if(!isObject(%target))
        {
            %this.spectateFree();
        }

    	return %target;
    }
};

// Deactivate and reactivate the package to ensure it overrides the original
if(isPackage(Slayer_GameConnection))
{
    deactivatePackage(Slayer_GameConnection);
    activatePackage(Slayer_GameConnection);
}    

// Activate the package to override the original
activatePackage(Eventide_SlayerOverride);