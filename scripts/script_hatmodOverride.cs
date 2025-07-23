if(isPackage(HatMod_Package))
{
    deactivatePackage(HatMod_Package);
}
else
{
    return;
}

package HatMod_Package 
{
	function serverCmdAddEvent(%client, %enabled, %inputID, %delay, %targetID, %namedTarget, %outputID, %par1, %par2, %par3, %par4) 
    {
		%grantIdx = outputEvent_GetOutputEventIdx("GameConnection", "grantHat");
		%PlHatIdx = outputEvent_GetOutputEventIdx("Player", "setHat");

		%brick = %client.wrenchBrick;
		%target = inputEvent_getTargetClass(%brick.getClassName(), %inputID, %targetID);

		if(!%client.isAdmin && %outputID == %PlHatIdx && %target $= "Player") 
        {
			%client.chatMessage("\c2Only Admins can use \c3setHat\c2 on players.");
			return;
		}
		if(!%client.isSuperAdmin && %outputID == %grantIdx && %target $= "GameConnection") 
        {
			%client.chatMessage("\c2Only Super Admins can use \c3grantHat\c2 event.");
			return;
		}

		return parent::serverCmdAddEvent(%client, %enabled, %inputID, %delay, %targetID, %namedTarget, %outputID, %par1, %par2, %par3, %par4);
	}

	function Armor::onAdd(%data, %player) 
    {
		%r = parent::onAdd(%data, %player);

        %client = %player.client;
        %playerDatablock = %player.getDataBlock();

        //Custom check to prevent Eventide-based datablocks from automatically getting a hat.
        if(%playerDatablock.isEventideClass)
        {
            return;
        }

		if(isObject(%client)) 
        {
			if(!$Pref::HatMod::ForceRandom) 
            {
				%hat = $HatMod::save::wornHat[%client.bl_id];
				if((isHat(%hat) && %client.hasHat(%hat)) || %hat $= "random")
                {
                    schedule(50, 0, serverCmdHat, %client, %hat);
                }
			} 
            else
            {
                schedule(50, 0, serverCmdHat, %client, "random");
            }
		} 
        else
        {
			if($Pref::HatMod::ForceRandom)
            {
				%player.schedule(50, wearRandomHat); //Bot support
            }
        }

		HatMod_TickCheck(); //The tick can stop when there are no players
		return %r;
	}

	function ItemData::onAdd(%data, %item) 
    {
		%r = parent::onAdd(%data, %item);

		if(%data.className $= "Hat")
        {
            %item.canPickup = false;
        }
        
		return %r;
	}

	function Armor::onTrigger(%DB, %player, %slot, %isActive) 
    {
		%client = %player.client;

		if(isObject(%client) && !%client.HatModNotAFK && $Pref::HatMod::RandomHats)
        {
            %client.HatModNotAFK = %player.getVelocity();
        }

		return parent::onTrigger(%DB, %player, %slot, %isActive);
	}

	function GameConnection::applyBodyParts(%client) 
    {
		%r = Parent::applyBodyParts(%client);

		if(isObject(%player = %client.player) && isObject(%image = %player.getMountedImage(2)) && %image.hatName !$= "") 
        {
			//Hide all hat and accent nodes
			for(%i=0; $hat[%i] !$= ""; %i++)
            {
                %player.hideNode($hat[%i]);
            }
				
			for(%i=0; $accent[%i] !$= ""; %i++)
            {
                %player.hideNode($accent[%i]);
            }
		}

		return %r;
	}

	function GameConnection::autoAdminCheck(%client) 
    {
		commandToClient(%client, 'HatMod_CheckVersion', $HatMod::LatestVersion);
		return parent::autoAdminCheck(%client);
	}
};
activatePackage(HatMod_Package);