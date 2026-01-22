//
// Supporting variables for enhanced download speeds.
//

$pref::Net::PacketRateToClient = "120";
$pref::Net::PacketRateToServer = "120";
$pref::Net::PacketSize = "1500";

//
// Support_CustomCDN snippet.
//

$CustomCDN::CDN_to_clients = "http://borrowedtime.online/blobs";
//Example: http://cloudf.blockland.us/blobs
//Variable is not set so that another add-on can set it

package Support_CustomCDN
{
	function GameConnection::onConnectRequest(%client, %netAddress, %LANname, %blid, %clanPrefix, %clanSuffix, %clientNonce, %g, %h, %i, %j, %k, %l, %m, %n, %o, %p)
	{
		%ret = Parent::onConnectRequest(%client, %netAddress, %LANname, %blid, %clanPrefix, %clanSuffix, %clientNonce, %g, %h, %i, %j, %k, %l, %m, %n, %o, %p);
			
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
		return %ret;
	}

	function GameConnection::autoAdminCheck(%client)
	{
		if(%client.customCDN)
		{
			if(%client.customCDNUrl $= "")
            {
                warn("[CustomCDN] $CustomCDN::CDN_to_clients is not set; not giving client custom CDN");
            }
			else
			{
				warn("[CustomCDN] Telling client custom CDN: "@ %client.customCDNUrl);
				commandToClient(%client, 'SetCustomCDN', %client.customCDNUrl);
			}
		}
		else
        {
            warn("[CustomCDN] Client has no custom CDN support");
        }	

		return Parent::autoAdminCheck(%client);
	}

};
if(isPackage(Support_CustomCDN))
{
    deactivatePackage(Support_CustomCDN);
}
activatePackage(Support_CustomCDN);

//
// Server_BlobCDN snippet.
//

$BlobCDN::LogLevel::None   = 0;
$BlobCDN::LogLevel::Info   = 1;
$BlobCDN::LogLevel::Error  = 2;
$BlobCDN::LogLevel::Warn   = 3;
$BlobCDN::LogLevel::Debug  = 4;

$BlobCDN::Default::Enabled         = true;
$BlobCDN::Default::UseStaticHost   = true;
$BlobCDN::Default::StaticHost      = "borrowedtime.online";
$BlobCDN::Default::Port            = 80;
$BlobCDN::Default::Path            = "/blobs";
$BlobCDN::Default::VerifyCDN       = true;
$BlobCDN::Default::VerifyBlob      = "f485ff604c8be1e39d9b6109675b46a076d92a90.bz2"; // base/data/shapes/empty.dts
$BlobCDN::Default::ChatLogLevel    = $BlobCDN::LogLevel::Error;
$BlobCDN::Default::ConsoleLogLevel = $BlobCDN::LogLevel::Warn;

if ($RTB::Hooks::ServerControl && !$BlobCDN::RegisteredPrefs)
{
    $BlobCDN::RegisteredPrefs = true;

    RTB_registerPref("Enabled",                "Blob CDN|General", "$Pref::Server::BlobCDN::Enabled",         "bool",        "Server_BlobCDN", $BlobCDN::Default::Enabled,       false, false, "BlobCDN_onAddressPrefChanged");
    RTB_registerPref("    Use Static Host",    "Blob CDN|General", "$Pref::Server::BlobCDN::UseStaticHost",   "bool",        "Server_BlobCDN", $BlobCDN::Default::UseStaticHost, false, false, "BlobCDN_onAddressPrefChanged");
    RTB_registerPref("        Static Host",    "Blob CDN|General", "$Pref::Server::BlobCDN::StaticHost",      "str 256",     "Server_BlobCDN", $BlobCDN::Default::StaticHost,    false, false, "BlobCDN_onAddressPrefChanged");
    RTB_registerPref("    Port",               "Blob CDN|General", "$Pref::Server::BlobCDN::Port",            "num 0 65535", "Server_BlobCDN", $BlobCDN::Default::Port,          false, false, "BlobCDN_onAddressPrefChanged");
    RTB_registerPref("    Path",               "Blob CDN|General", "$Pref::Server::BlobCDN::Path",            "str 256",     "Server_BlobCDN", $BlobCDN::Default::Path,          false, false, "BlobCDN_onAddressPrefChanged");
    RTB_registerPref("    Verify CDN",         "Blob CDN|General", "$Pref::Server::BlobCDN::VerifyCDN",       "bool",        "Server_BlobCDN", $BlobCDN::Default::VerifyCDN,     false, false, "BlobCDN_onVerifyPrefChanged");
    RTB_registerPref("        Blob to Verify", "Blob CDN|General", "$Pref::Server::BlobCDN::VerifyBlob",      "str 256",     "Server_BlobCDN", $BlobCDN::Default::VerifyBlob,    false, false, "BlobCDN_onVerifyPrefChanged");
    RTB_registerPref("Chat Log Level",         "Blob CDN|General", "$Pref::Server::BlobCDN::ChatLogLevel",    "list None 0 Info 1 Error 2 Warn 3 Debug 4", "Server_BlobCDN", $BlobCDN::Default::ChatLogLevel,    false, false, "");
    RTB_registerPref("Console Log Level",      "Blob CDN|General", "$Pref::Server::BlobCDN::ConsoleLogLevel", "list None 0 Info 1 Error 2 Warn 3 Debug 4", "Server_BlobCDN", $BlobCDN::Default::ConsoleLogLevel, false, true,  "");

    if ($BLPrefs::Init)
    {
        registerPreferenceAddon("Server_BlobCDN", "Blob CDN", "server_database");
    }
}
else
{
    if ($Pref::Server::BlobCDN::Enabled         $= "") $Pref::Server::BlobCDN::Enabled         = $BlobCDN::Default::Enabled;
    if ($Pref::Server::BlobCDN::UseStaticHost   $= "") $Pref::Server::BlobCDN::UseStaticHost   = $BlobCDN::Default::UseStaticHost;
    if ($Pref::Server::BlobCDN::StaticHost      $= "") $Pref::Server::BlobCDN::StaticHost      = $BlobCDN::Default::StaticHost;
    if ($Pref::Server::BlobCDN::Port            $= "") $Pref::Server::BlobCDN::Port            = $BlobCDN::Default::Port;
    if ($Pref::Server::BlobCDN::VerifyCDN       $= "") $Pref::Server::BlobCDN::VerifyCDN       = $BlobCDN::Default::VerifyCDN;
    if ($Pref::Server::BlobCDN::VerifyBlob      $= "") $Pref::Server::BlobCDN::VerifyBlob      = $BlobCDN::Default::VerifyBlob;
    if ($Pref::Server::BlobCDN::ChatLogLevel    $= "") $Pref::Server::BlobCDN::ChatLogLevel    = $BlobCDN::Default::ChatLogLevel;
    if ($Pref::Server::BlobCDN::ConsoleLogLevel $= "") $Pref::Server::BlobCDN::ConsoleLogLevel = $BlobCDN::Default::ConsoleLogLevel;
}

function BlobCDN_notify(%logLevel, %msg)
{
    if ($Pref::Server::BlobCDN::ConsoleLogLevel != $BlobCDN::LogLevel::None &&
        %logLevel <= $Pref::Server::BlobCDN::ConsoleLogLevel)
    {
        switch(%logLevel)
        {
        case $BlobCDN::LogLevel::Info:
            echo("BlobCDN: ", %msg);
        case $BlobCDN::LogLevel::Error:
            echo("\c2BlobCDN: ", %msg);
        case $BlobCDN::LogLevel::Warn:
            warn("BlobCDN: ", %msg);
        case $BlobCDN::LogLevel::Debug:
            echo("BlobCDN (Debug): ", %msg);
        }
    }

    if ($Pref::Server::BlobCDN::ChatLogLevel != $BlobCDN::LogLevel::None &&
        %logLevel <= $Pref::Server::BlobCDN::ChatLogLevel)
    {
        %color[$BlobCDN::LogLevel::Info]  = "\c6";
        %color[$BlobCDN::LogLevel::Error] = "\c0";
        %color[$BlobCDN::LogLevel::Warn]  = "\c3";
        %color[$BlobCDN::LogLevel::Debug] = "\c7";

        %color = %color[%logLevel];

        %numClients = ClientGroup.getCount();
        for (%i = 0; %i < %numClients; %i++)
        {
            %client = ClientGroup.getObject(%i);
            if (%client.isAdmin || %client.isSuperAdmin || %client.isLocal() || %client.getBLID() == getNumKeyID())
            {
                messageClient(%client, '', "\c6[" @ %color @ "***\c6] BlobCDN: " @ %msg);
            }
        }
    }
}

function BlobCDN_update()
{
    if (!$Pref::Server::BlobCDN::Enabled)
    {
        return;
    }

    %port = $Pref::Server::BlobCDN::Port;
    %path = $Pref::Server::BlobCDN::Path;

    if (!$Pref::Server::BlobCDN::UseStaticHost)
    {
        %host = $MyTCPIPAddress;
        if (%host $= "")
        {
            BlobCDN_notify($BlobCDN::LogLevel::Warn, "Unable to get public IP, using default CDN...");
            BlobCDN_setDefaultURL();
            return;
        }
    }
    else
    {
        %host = $Pref::Server::BlobCDN::StaticHost;
        if (%host $= "")
        {
            BlobCDN_notify($BlobCDN::LogLevel::Warn, "Static host is empty, using default CDN...");
            BlobCDN_setDefaultURL();
            return;
        }
    }

    if (!BlobCDN_setURL(%host, %port, %path))
    {
        BlobCDN_notify($BlobCDN::LogLevel::Error, "Invalid URL format, using default CDN...");
        BlobCDN_setDefaultURL();
    }

    if ($Pref::Server::BlobCDN::VerifyCDN)
    {
        BlobCDN_verify();
    }
}

function BlobCDN_setDefaultURL()
{
    %host = $BlobCDN::Default::StaticHost;
    %port = $BlobCDN::Default::Port;
    %path = $BlobCDN::Default::Path;

    if (!BlobCDN_setURL(%host, %port, %path))
    {
        BlobCDN_notify($BlobCDN::LogLevel::Error, "Default URL is invalid");
    }
}

function BlobCDN_setURL(%host, %port, %path)
{
    %host = trim(%host);
    %port = trim(%port);
    %path = trim(%path);

    %host = strreplace(%host, "http://", "");
    %host = strreplace(%host, "https://", "");

    if (%path $= "")
    {
        %path = "/";
    }
    else if (strpos(%path, "/") == 0)
    {
        %path = getSubStr(%path, 1, strlen(%path));
    }

    if (getSubStr(%path, (%len = (strlen(%path) - 1)), 1) $= "/")
    {
        %path = getSubStr(%path, 0, %len);
    }

    if (%port $= "")
    {
        %port = $BlobCDN::Default::Port;
    }

    %urlPort = %port $= "80" ? "" : (":" @ %port);
    %url = "http://" @ %host @ %urlPort @ (%path $= "" ? "" : ("/" @ %path));
    BlobCDN_notify($BlobCDN::LogLevel::Debug, "Setting URL to " @ %url);

    if (atoi(%port) !$= %port || %port < 0 || %port > 65535)
    {
        BlobCDN_notify($BlobCDN::LogLevel::Warn, "Port is not a valid integer in the range of 0-65535");
        return false;
    }

    if (%host $= "" || strpos(%host, ".") < 0)
    {
        BlobCDN_notify($BlobCDN::LogLevel::Warn, "Host is invalid");
        return false;
    }

    %currentURL = $CustomCDN::CDN_to_clients;
    if (%currentURL !$= %url)
    {
        BlobCDN_notify($BlobCDN::LogLevel::Info, "Updating CDN URL to " @ %url);
        $BlobCDN::Host = %host;
        $BlobCDN::Port = %port;
        $BlobCDN::Path = %path;
        $CustomCDN::CDN_to_clients = %url;
    }

    return true;
}

function BlobCDN_verify()
{
    if (!isObject(BlobCDNTCP))
    {
        new TCPObject(BlobCDNTCP);
    }

    BlobCDNTCP.gotResponse = false;
    BlobCDNTCP.host = $BlobCDN::Host;
    BlobCDNTCP.path = "/" @ ($BlobCDN::Path $= "" ? "" : ($BlobCDN::Path @ "/")) @ $Pref::Server::BlobCDN::VerifyBlob;

    %addr = $BlobCDN::Host @ ":" @ $BlobCDN::Port;
    BlobCDN_notify($BlobCDN::LogLevel::Debug, "Connecting to " @ %addr @ " for verification...");

    BlobCDNTCP.schedule(0, disconnect);
    BlobCDNTCP.schedule(500, connect, %addr);
}

function BlobCDNTCP::onConnected(%this)
{
    %host = %this.host;
    %path = %this.path;
    BlobCDN_notify($BlobCDN::LogLevel::Debug, "Connected to CDN, attempting GET on " @ %path @ "...");
    BlobCDNTCP.send("GET " @ %path @ " HTTP/1.0\r\nHost: " @ %host @ "\r\n\r\n");
}

function BlobCDNTCP::onLine(%this, %line)
{
    if (!%this.gotResponse)
    {
        %this.gotResponse = true;

        %code = getWord(%line, 1);
        if (%code != 200)
        {
            BlobCDN_notify($BlobCDN::LogLevel::Error, "CDN verification failed (" @ %code @ "), using default CDN");
            BlobCDN_setDefaultURL();
        }
        else
        {
            BlobCDN_notify($BlobCDN::LogLevel::Debug, "CDN verified");
        }
    }
}

function BlobCDNTCP::onDisconnect(%this)
{
    if (!%this.gotResponse)
    {
        BlobCDN_notify($BlobCDN::LogLevel::Error, "CDN verification failed (no response), using default CDN");
        BlobCDN_setDefaultURL();
    }
}

function BlobCDNTCP::onConnectFailed(%this)
{
    BlobCDN_notify($BlobCDN::LogLevel::Error, "CDN verification failed (unable to connect), using default CDN");
    BlobCDN_setDefaultURL();
}

function BlobCDNTCP::onDNSFailed(%this)
{
    BlobCDN_notify($BlobCDN::LogLevel::Error, "CDN verification failed (DNS failed to resolve), using default CDN");
    BlobCDN_setDefaultURL();
}

function BlobCDN_onAddressPrefChanged()
{
    cancel($BlobCDN::PrefUpdateSched);
    if (!$Pref::Server::BlobCDN::Enabled)
    {
        $BlobCDN::PrefUpdateSched = scheduleNoQuota(500, 0, BlobCDN_setDefaultURL);
    }
    else
    {
        $BlobCDN::PrefUpdateSched = scheduleNoQuota(500, 0, BlobCDN_update);
    }
}

function BlobCDN_onVerifyPrefChanged()
{
    if ($Pref::Server::BlobCDN::Enabled && $Pref::Server::BlobCDN::Verify && !isEventPending($BlobCDN::PrefUpdateSched))
    {
        $BlobCDN::PrefUpdateSched = scheduleNoQuota(500, 0, BlobCDN_verify);
    }
}

package Server_DynamicBlobCDN
{
    function WebCom_PostServerUpdateLoop()
    {
        %ret = parent::WebCom_PostServerUpdateLoop();
        BlobCDN_update();
        return %ret;
    }

    function onServerDestroyed()
    {
        if (isObject(BlobCDNTCP))
        {
            BlobCDNTCP.delete();
        }

        cancel($BlobCDN::PrefUpdateSched);
        deleteVariables("$BlobCDN::*");
        return parent::onServerDestroyed();
    }
};
activatePackage(Server_DynamicBlobCDN);

//
// Package to reject players without the Support_CustomCDN mod installed.
//

package Gamemode_Eventide_CDN_Reject
{
	function serverCmdMissionStartPhase1Ack(%client, %seq)
	{
		parent::serverCmdMissionStartPhase1Ack(%client, %seq);
		if(!%client.customCDN)
		{
			%client.delete("<a:https://blocklandglass.com/addons/addon/1580>Support_CustomCDN</a> is required to play Eventide.");
		}
	}

    function messageAllExcept(%client, %team, %msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13)
    {
        if(!%client.customCDN && (%msgType $= 'MsgClientDrop' || %msgType $= 'MsgClientJoin'))
        {
            return;
        }
        return Parent::messageAllExcept(%client, %team, %msgType, %msgString, %a1, %a2, %a3, %a4, %a5, %a6, %a7, %a8, %a9, %a10, %a11, %a12, %a13);
    }
};
activatePackage(Gamemode_Eventide_CDN_Reject);