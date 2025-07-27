if(!isFunction(serverCmdHat))
{
    return;
}

//Don't automatically mount HatMod hats for Eventide-based players.
package Gamemode_Eventide_HatMod 
{
	function serverCmdHat(%client, %na, %nb, %nc, %nd, %ne)
	{
		%player = %client.player;
		%playerDatablock = %player.getDatablock();

		if(isObject(%player) && %playerDatablock.isEventideClass)
		{
			return;
		}
		else
		{
			return Parent::serverCmdHat(%client, %na, %nb, %nc, %nd, %ne);
		}
	}
};
if(isPackage(Gamemode_Eventide_HatMod))
{
	deactivatePackage(Gamemode_Eventide_HatMod);
}
activatePackage(Gamemode_Eventide_HatMod);