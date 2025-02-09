package Gamemode_Evenitde_CDN
{
	function GameConnection::startLoad(%client)
	{
		if(!%client.customCDN)
		{
			%client.delete("<a:https://blocklandglass.com/addons/addon/1580>Support_CustomCDN</a> is required to play on this server.");
			announce(%client.getPlayerName() SPC "is missing CustomCDN, everyone point and laugh at them XD");
			serverPlay2D("clown_horn_sound");
		}
		parent::startLoad(%client);
	}
};
activatePackage(Gamemode_Evenitde_CDN);