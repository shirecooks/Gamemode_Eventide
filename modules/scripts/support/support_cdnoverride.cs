package CustomCDNServer
{
	function GameConnection::onConnectRequest(%client, %netAddress, %LANname, %blid, %clanPrefix, %clanSuffix, %clientNonce,%g, %h, %i, %j, %k, %l, %m, %n, %o, %p)
	{
		%ret = Parent::onConnectRequest(%client, %netAddress, %LANname, %blid, %clanPrefix, %clanSuffix, %clientNonce,%g, %h, %i, %j, %k, %l, %m, %n, %o, %p);			
		
		%client.customCDN = false;

		for(%cnt = 0; %cnt < getLineCount(%h); %cnt++)
		{
			%line = getLine(%h, %cnt);
			if(getField(%line, 0) $= "CustomCDN")
			{
				%client.customCDN = true;
				%client.customCDNUrl = $CustomCDN::CDN_to_clients;
				%client.customCDNversion = getField(%line, 1);
				break;
			}
		}
        
		if(!%client.customCDN)
        {
           warn("Client" SPC %client.getBLID() SPC "is missing CustomCDN, refusing connection");
		   return "<a:https://blocklandglass.com/addons/addon/1580>Support_CustomCDN</a> is required to play on this server";
        }

        return %ret;
	}
};

if(isPackage(CustomCDNServer)) 
{
    deactivatePackage(CustomCDNServer);
    activatePackage(CustomCDNServer);
}