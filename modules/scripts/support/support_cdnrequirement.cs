package Gamemode_Eventide_CDN_Reject
{
	function serverCmdMissionStartPhase1Ack(%client, %seq)
	{
		parent::serverCmdMissionStartPhase1Ack(%client, %seq);
		if(!%client.customCDN)
		{
			announce(%client.getPlayerName() SPC "is missing CustomCDN, everyone point and laugh at them XD");
			serverPlay2D("clown_horn_sound");
			%client.delete("<a:https://blocklandglass.com/addons/addon/1580>Support_CustomCDN</a> is required to play this server");
		}
	}
};
activatePackage(Gamemode_Eventide_CDN_Reject);