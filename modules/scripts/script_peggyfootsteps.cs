package Eventide_PeggyFootsteps
{
	function Armor::onNewDatablock(%this,%obj)
	{		
		Parent::onNewDatablock(%this,%obj);

		// Don't do anything if it's a vehicle or the schedule is already pending
		if(%this.rideable || isEventPending(%obj.peggstep)) return;

		%obj.touchcolor = "";
		%obj.surface = parseSoundFromNumber($Pref::Server::PF::defaultStep, %obj);
		%obj.isSlow = 0;
		%obj.peggstep = schedule(50,0,PeggFootsteps,%obj);
	}
};

// Deactivate the original peggsteps and deactivate this package if it's already activated, then reactivate it
if(isPackage(peggsteps)) deactivatePackage(peggsteps);
if(isPackage(Eventide_PeggyFootsteps)) deactivatePackage(Eventide_PeggyFootsteps);
activatePackage(Eventide_PeggyFootsteps);

//+++ Set BrickFX to custom sounds:
function servercmdSetFootstep(%client, %type, %sound)
{
	%i = 0;
	%types[%i]   = "pearl";
	%types[%i++] = "chrome";
	%types[%i++] = "glow";
	%types[%i++] = "blink";
	%types[%i++] = "swirl";
	%types[%i++] = "rainbow";
	%types[%i++] = "undulo";
	%types[%i++] = "default";
	%types[%i++] = "terrain";
	%types[%i++] = "vehicle";
	//Default 0 Basic 1 Dirt 2 Fabric 3 Grass 4 Metal 5 Sand 6 Snow 7 Stone 8 Water 9 Wood 10
	%j = 0;
	%sounds[%j]   = "default";
	%sounds[%j++] = "basic";
	%sounds[%j++] = "dirt";
	%sounds[%j++] = "fabric";
	%sounds[%j++] = "grass";
	%sounds[%j++] = "metal";
	%sounds[%j++] = "sand";
	%sounds[%j++] = "snow";
	%sounds[%j++] = "stone";
	%sounds[%j++] = "water";
	%sounds[%j++] = "wood";
	%match = false;
	%hit = 0;
	for(%a=0; %a <= %j; %a++)
	{
		if(trim(%sound)$= %sounds[%a])
		{
			%match = true;
			%hit = %a;
			break;
		}
	}

	if (!%match)
	{
		for (%a=0; %a <= %j; %a++) messageClient(%client, '', "\c5 - " @ %sounds[%a]);
		
		messageClient(%client,'',"\c0There is no sound with the name '\c3" @ trim(%sound) @ "\c0'.\c6 The sounds you can choose are listed above. <color:aaaaaa>(PgUp to see all options)");
		return;
	}

	switch$ (%type)
	{
		case "pearl": $Pref::Server::PF::brickFXsounds::pearlStep = %hit;
		case "chrome": $Pref::Server::PF::brickFXsounds::chromeStep = %hit;
		case "glow": $Pref::Server::PF::brickFXsounds::glowStep = %hit;
		case "blink": $Pref::Server::PF::brickFXsounds::blinkStep = %hit;
		case "swirl": $Pref::Server::PF::brickFXsounds::swirlStep = %hit;
		case "rainbow": $Pref::Server::PF::brickFXsounds::rainbowStep = %hit;
		case "undulo": $Pref::Server::PF::brickFXsounds::unduloStep = %hit;
		case "default": if(%hit == 0)
						{
							for(%a=1; %a <= %j; %a++) messageClient(%client, '', "\c5 - " @ %sounds[%a]);
				
							messageClient(%client,'',"\c0The default sound can't be set to itself, choose another sound.\c6 The sounds you can choose are listed above. <color:aaaaaa>(PgUp to see all options)");
							return;
						}
						$Pref::Server::PF::defaultStep = %hit;

		case "terrain": $Pref::Server::PF::terrainStep = %hit;
		
		case "vehicle": $Pref::Server::PF::vehicleStep = %hit;
		default: for(%a=0; %a <= %i; %a++) messageClient(%client, '', "\c5 - " @ %types[%a]);
				 messageClient(%client,'',"\c0There is no surface with the name '\c3" @ trim(%type) @ "\c0'.\c6 The surfaces you can choose are listed above. <color:aaaaaa>(PgUp to see all options)");
				 return;
	}
	messageClient(%client, '', "\c6You have selected the sound '\c5" @ trim(%sound) @ "\c6' for the '\c3" @ trim(%type) @ "\c6' surface.");
}

//+++ Toggle Prefs:
function servercmdToggle(%client, %toggle)
{
	if(!%client.isAdmin) messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
	
	else if ( %toggle $= "SwimmingFX" )
	{
		if( !$Pref::Server::PF::waterSFX )
		{
			messageClient(%client,'',"<color:00ff00>You have activated the swimming SFX package.");
			chatMessageAll('',"<color:ffffff>Swimming sound effects are now enabled.");
			$Pref::Server::PF::waterSFX = 1;
		}
		else
		{
			messageClient(%client,'',"<color:ff0000>You have de-activated the swimming SFX package.");
			chatMessageAll('',"<color:ff0000>Swimming sound effects are now disabled.");
			$Pref::Server::PF::waterSFX = 0;
		}
	}
	else if ( %toggle $= "footsteps" )
	{
		if( !$Pref::Server::PF::footstepsEnabled )
		{
			messageClient(%client,'',"<color:00ff00>You have activated the footstep SFX package.");
			chatMessageAll('',"<color:ffffff>Footstep sound effects are now enabled.");
		}
		else
		{
			messageClient(%client,'',"<color:ff0000>You have de-activated the footstep SFX package.");
			chatMessageAll('',"<color:ff0000>Footstep sound effects are now disabled.");
		}
		$Pref::Server::PF::footstepsEnabled = !$Pref::Server::PF::footstepsEnabled;
	}
	else if ( %toggle $= "BrickFX" )
	{
		if( !$Pref::Server::PF::brickFXSounds::enabled )
		{
			messageClient(%client,'',"<color:00ff00>You have activated the brick FX custom sounds package.");
			chatMessageAll('',"<color:ffffff>Footstep sound effects for brick special FX are now enabled.");
		}
		else
		{
			messageClient(%client,'',"<color:ff0000>You have de-activated the brick FX custom sounds package.");
			chatMessageAll('',"<color:ff0000>Footstep sound effects for brick special FX are now disabled.");
		}
		$Pref::Server::PF::brickFXSounds::enabled = !$Pref::Server::PF::brickFXSounds::enabled;
	}
	else if ( %toggle $= "LandingFX" )
	{
		if( !$Pref::Server::PF::landingFX )
		{
			messageClient(%client,'',"<color:00ff00>You have activated the landing FX custom sounds package.");
			chatMessageAll('',"<color:ffffff>Footstep sound effects for landing after falling are now enabled.");
		}
		else
		{
			messageClient(%client,'',"<color:ff0000>You have de-activated the landing FX custom sounds package.");
			chatMessageAll('',"<color:ff0000>Footstep sound effects for landing after falling are now disabled.");
		}
		$Pref::Server::PF::landingFX = !$Pref::Server::PF::landingFX;
	}
	else
	{
		if ( %toggle $= "" )
		{
			messageClient(%client,'',"<color:ff0000>You must enter a parameter to use this command. Use '/pegghelp' to learn more.");
		}
		else
		{
			messageClient(%client,'',"<color:ff0000>" @ %toggle @ " is not a valid parameter. Use '/pegghelp' to learn more.");
		}
	}
}

//+++ Running Speed:
function servercmdSetMinRunSpeed(%client, %value)
{
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
	}
	else
	{
		if(%value > 20.0)
		{
			messageClient(%client,'',"<color:ffffff>That value is too high to use as the minimum running speed.");
			return;
		}
		if(%value <= 0)
		{
			messageClient(%client,'',"<color:ffffff>That value is too low to use as the minimum runnings speed.");
			return;
		}
		$Pref::Server::PF::runningMinSpeed = %value;
		messageClient(%client,'',"<color:ffffff>You have now set the minimum running value to " @ %value @ ". Gowing below this speed will play a walking noise instead of running.");
	}
}

//+++ Landing Speed:
function servercmdSetMinLandSpeed(%client, %value)
{
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
	}
	else
	{
		if(%value > 20.0)
		{
			messageClient(%client,'',"<color:ffffff>That value is too high to use as the minimum running speed.");
			return;
		}
		if(%value < 8.0)
		{
			messageClient(%client,'',"<color:ffffff>That value is too low to use as the minimum runnings speed.");
			return;
		}
		$Pref::Server::PF::minLandSpeed = %value;
		messageClient(%client,'',"<color:ffffff>You have now set the minimum landing speed to " @ %value @ ". Gowing below this speed will not play a sound at all when you land.");
	}
}

//+++ Basic help command
function serverCmdPeggHelp(%client)
{
	messageClient(%client,'',"\c6The following is a list of admin commands for the \c3Peggy Footsteps\c6 add-on:");
	messageClient(%client,'',"\c6/toggle Footsteps");
	messageClient(%client,'',"\c6/toggle SwimmingFX");
	messageClient(%client,'',"\c6/toggle BrickFX");
	messageClient(%client,'',"\c6/toggle LandingFX");
	messageClient(%client,'',"\c6/getPeggPrefs");
	messageClient(%client,'',"\c6/getCustomSounds");
	messageClient(%client,'',"\c6/setMinRunSpeed \c0<\c3decimal value\c0>\c6");
	messageClient(%client,'',"\c6/setMinLandSpeed \c0<\c3decimal value\c0>\c6");
	messageClient(%client,'',"\c6/setColorToFootstep \c0<\c3sound\c0> <color:dddddd>(This command will set the current color you have selected on your paint selector to play a sound you select.)");
	messageClient(%client,'',"\c6/clearCustomSound <color:dddddd>(This command will remove the SFX for the CURRENT color selected on your paint selector.)");
	messageClient(%client,'',"\c6/clearCustomSounds <color:dddddd>(This command will remove the custom SFX for ALL colors.)");
	messageClient(%client,'',"\c6/setFootstep \c0<\c3surface\c0> \c0<\c3sound\c0> <color:dddddd>(This command will change what noise plays for the surface specified. If the surface is a type of brick FX, the sound specified will overide what sound would be played based off color.)");
	messageClient(%client,'',"<color:aaaaaa>(PgUp to see all options)");
}

//+++ Get all of the current preferences for PeggyFootsteps
function servercmdGetPeggPrefs(%client)
{
	if(!%client.isAdmin && !%client.isSuperAdmin)
	{
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return;
	}
	messageClient(%client,'',"\c6The following is a list of preferences for the \c3Peggy Footsteps\c6 add-on:");
	messageClient(%client,'',"<color:ffffff>Footsteps Enabled: " @ ($Pref::Server::PF::footstepsEnabled ? "<color:00ff00>enabled" : "<color:ff0000>disabled"));
	messageClient(%client,'',"<color:ffffff>Swimming SoundFX: " @ ($Pref::Server::PF::waterSFX ? "<color:00ff00>enabled" : "<color:ff0000>disabled"));
	messageClient(%client,'',"<color:ffffff>Special BrickFX make custom SoundFX: " @ ($Pref::Server::PF::brickFXSounds::enabled ? "<color:00ff00>enabled" : "<color:ff0000>disabled"));
	messageClient(%client,'',"<color:ffffff>Landing SoundFX: " @ ($Pref::Server::PF::landingFX ? "<color:00ff00>enabled" : "<color:ff0000>disabled"));
	if ( $Pref::Server::PF::landingFX ) messageClient(%client,'',"<color:ffffff>Landing SoundFX Minimum Speed: <color:ffff00>" @ $Pref::Server::PF::minLandSpeed);
	messageClient(%client,'',"<color:ffffff>Running Minimum Speed: <color:ffff00>" @ $Pref::Server::PF::runningMinSpeed);
	messageClient(%client,'',"<color:ffffff>Default Footstep SFX: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::defaultStep, -1) );
	messageClient(%client,'',"<color:ffffff>Terrain Footsteps: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::terrainStep, -1) );
	messageClient(%client,'',"<color:ffffff>Vehicle Footsteps: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::vehicleStep, -1) );
	if ( $Pref::Server::PF::brickFXSounds::enabled )
	{
		messageClient(%client,'',"<color:ffffff>Pearl SoundFX: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::brickFXsounds::pearlStep, -1) );
		messageClient(%client,'',"<color:ffffff>Chrome SoundFX: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::brickFXsounds::chromeStep, -1) );
		messageClient(%client,'',"<color:ffffff>Glowing SoundFX: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::brickFXsounds::glowStep, -1) );
		messageClient(%client,'',"<color:ffffff>Blinking SoundFX: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::brickFXsounds::blinkStep, -1) );
		messageClient(%client,'',"<color:ffffff>Swirl SoundFX: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::brickFXsounds::swirlStep, -1) );
		messageClient(%client,'',"<color:ffffff>Rainbow SoundFX: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::brickFXsounds::rainbowStep, -1) );
		messageClient(%client,'',"<color:ffffff>Undulo SoundFX: <color:ffff00>" @ parseSoundFromNumber($Pref::Server::PF::brickFXsounds::unduloStep, -1) );
	}
	messageClient(%client,'',"<color:aaaaaa>(PgUp to see all options)");
}

function Armor::onLand(%data, %obj, %horiz)
{
	if (!$Pref::Server::PF::landingFX || %obj.isInvisible) return;

	$oldTimescale = getTimescale();
	setTimescale((getRandom(75,150)*0.01) * $oldTimescale);

	%minLandSpeed = $Pref::Server::PF::minLandSpeed;
	%soundIndex = getRandom(1,3);
	%soundBase = (%horiz > %minLandSpeed + 8)  ? "land_medium" : "land_lite";

	%obj.playthread("3", "plant");

	serverplay3d(%soundBase @ %soundIndex @ "_sound", %obj.getHackPosition());
	setTimescale($oldTimescale);
}

//+++ Clear the custom list.
function serverCmdClearCustomSounds(%client)
{
	if( !%client.isAdmin )
	{
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return;
	}
	messageClient(%client,'',"\c6You have cleared all the custom foostep SFX for every color of brick.");
	for(%i = 0; %i < $Pref::Server::PF::customsounds; %i++) $Pref::Server::PF::colorPlaysFX[%i] = "";
	
	$Pref::Server::PF::customsounds = 0;
}

//+++ Clear a single entry on the custom list
function serverCmdClearCustomSound(%client)
{
	if( !%client.isAdmin )
	{
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return;
	}
	%color = getColorIDTable(%client.currentColor);
  	%r = getWord(%color, 0) * 255;
	%g = getWord(%color, 1) * 255;
	%b = getWord(%color, 2) * 255;
	%hex = rgbToHex(%color);
	for(%i = 0; %i < $Pref::Server::PF::customsounds; %i++)
	{
		%hit = $Pref::Server::PF::colorPlaysFX[%i];
		%hitcolor = getWords(%hit, 0, 3);
		%hr = getWord(%hitcolor, 0) * 255;
		%hg = getWord(%hitcolor, 1) * 255;
		%hb = getWord(%hitcolor, 2) * 255;
		// if there's a match, delete it.
		if ( %hr == %r && %hg == %g && %hb == %b)
		{
			$Pref::Server::PF::colorPlaysFX[%i] = "";
			// if the match isn't the last one in the list, update the list to fill the hole made by removing the hit.
			if ( %i < $Pref::Server::PF::customsounds-1 )
			{
				for ( %j = %i; %j < $Pref::Server::PF::customsounds; %j++ )
				{
					if ( %j < $Pref::Server::PF::customsounds-1 )
					{
						$Pref::Server::PF::colorPlaysFX[%j] = $Pref::Server::PF::colorPlaysFX[%j+1];
						$Pref::Server::PF::colorPlaysFX[%j+1] = "";
					}
					else
					{
						$Pref::Server::PF::colorPlaysFX[%j] = "";
					}
				}
			}
			$Pref::Server::PF::customsounds--;
			messageClient(%client,'',"\c6You have cleared the custom sound for <color:" @ %hex @ ">THIS COLOR\c6.");
			break;
		}
		else if ( %i == $Pref::Server::PF::customsounds-1 )
		{
			messageClient(%client,'',"\c0Error.\c6 There was no sound effect found for <color:" @ %hex @ ">THIS COLOR\c6.");
		}
	}
}

//+++ Get a list of custom sounds.
function servercmdGetCustomSounds(%client)
{
	if( !%client.isAdmin )
	{
		messageClient(%client,'',"<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return;
	}
	messageClient(%client,'',"\c5List of Custom Sounds for Each Color:");
	for ( %i = 0; %i < $Pref::Server::PF::customsounds; %i++ )
	{
		%hit = $Pref::Server::PF::colorPlaysFX[%i];
		%hitcolor = getWords(%hit, 0, 3);
		%hitsound = getWord(%hit, 4);
		%hex =  rgbToHex(%hitcolor);
		messageClient(%client,'',"<color:" @ %hex @ ">THIS COLOR\c6 is assigned to the sound '\c4" @ %hitsound @ "\c6'.");
	}
}

//+++ Set a color to a new footstep.
function servercmdSetColorToFootstep(%client, %sound)
{
	if ( !%client.isAdmin )
	{
		messageClient(%client, '', "<color:ff0000>You dingus!<color:ffff00> Narry a man, save admin or super admin, hath the divine power to do hither command!");
		return;
	}
	%color = getColorIDTable(%client.currentColor);
	%hex = rgbToHex(%color);
	%i = 0;
	%sounds[%i]   = "basic";
	%sounds[%i++]   = "water";
	%sounds[%i++] = "stone";
	%sounds[%i++] = "grass";
	%sounds[%i++] = "snow";
	%sounds[%i++] = "wood";
	%sounds[%i++] = "metal";
	%sounds[%i++] = "dirt";
	%sounds[%i++] = "sand";
	%sounds[%i++] = "fabric";
	%match = false;

	for ( %a=0; %a <= %i; %a++ )
	{
		if( trim(%sound)$= %sounds[%a] )
		{
			%match = true;
			break;
		}
	}
	if ( !%match )
	{
		for ( %a=0; %a <= %i; %a++ )
		{
			messageClient(%client, '', "\c5 - " @ %sounds[%a]);
		}
		messageClient(%client,'',"\c0There is no sound with the name '\c3" @ trim(%sound) @ "\c0'.\c6 The sounds you can choose are listed above. <color:aaaaaa>(PgUp to see all options)");
		return;
	}
	%overwrite = false;
	for ( %i = 0; %i < $Pref::Server::PF::customsounds; %i++ )
	{
		%hit = $Pref::Server::PF::colorPlaysFX[%i];
		%hitcolor = getWords(%hit, 0, 3);
		%hr = getWord(%hitcolor, 0);
		%hg = getWord(%hitcolor, 1);
		%hb = getWord(%hitcolor, 2);
		%r = getWord(%color, 0);
		%g = getWord(%color, 1);
		%b = getWord(%color, 2);
		if ( %hr == %r && %hg == %g && %hb == %b)
		{
			%hitsound = getWord(%hit, 4);
			if ( %sound $= %hitsound )
			{
				messageClient(%client,'',"\c6The sound, '\c4" @ trim(%sound) @ "\c6', is already playing for <color:" @ %hex @ ">THIS COLOR\c6.");
				return;
			}
			else
			{
				$Pref::Server::PF::colorPlaysFX[%i] = %color SPC %sound;
				%overwrite = true;
				break;
			}
		}
	}
	if ( ! %overwrite )
	{
		$Pref::Server::PF::colorPlaysFX[$Pref::Server::PF::customsounds] = %color SPC %sound;
		$Pref::Server::PF::customsounds++;
	}
	messageClient(%client, '', "\c6The sound, '\c4" @ trim(%sound) @ "\c6', now plays for <color:" @ %hex @ ">THIS COLOR\c6.");
}

function rgbToHex(%rgb) //! Ripped from Greek2Me's SLAYER
{
	// use % to find remainder
	%r = getWord(%rgb,0);
	%g = getWord(%rgb,1);
	%b = getWord(%rgb,2);
	// in-case the rgb value isn't on the right scale
	%r = ( %r <= 1 ) ? %r * 255 : %r;
	%g = ( %g <= 1 ) ? %g * 255 : %g;
	%b = ( %b <= 1 ) ? %b * 255 : %b;
	// the hexidecimal numbers
	%a = "0123456789ABCDEF";

	%r = getSubStr(%a,(%r-(%r % 16))/16,1) @ getSubStr(%a,(%r % 16),1);
	%g = getSubStr(%a,(%g-(%g % 16))/16,1) @ getSubStr(%a,(%g % 16),1);
	%b = getSubStr(%a,(%b-(%b % 16))/16,1) @ getSubStr(%a,(%b % 16),1);

	return %r @ %g @ %b;
}

function PeggFootsteps_getSound(%surface, %speed)
{
	switch$(%surface)
	{
		case "under water": return "swim" @ getRandom(1,6) @ "_sound";

		case "metal": 	if(%speed $= "walking") return "fs_walk_metal" @ getRandom(1,4) @ "_sound";
						else return "fs_run_metal" @ getRandom(1,5) @ "_sound";

		case "dirt": 	if(%speed $= "walking") return "fs_walk_dirt" @ getRandom(1,5) @ "_sound";
						else return "fs_run_dirt" @ getRandom(1,5) @ "_sound";

		case "grass": 	if(%speed $= "walking") return "fs_walk_grass" @ getRandom(1,4) @ "_sound";
						else return "fs_run_grass" @ getRandom(1,5) @ "_sound";

		case "stone": 	if(%speed $= "walking") return "fs_walk_stone" @ getRandom(1,5) @ "_sound";
						else return "fs_run_stone" @ getRandom(1,5) @ "_sound";

		case "water":	if(%speed $= "walking") return "fs_water_walk" @ getRandom(1,5) @ "_sound";
						else return "fs_water_run" @ getRandom(1,5) @ "_sound";

		case "wood": 	if(%speed $= "walking") return "fs_walk_wood" @ getRandom(1,5) @ "_sound";
						else return "fs_run_wood" @ getRandom(1,5) @ "_sound";

		case "sand": 	if(%speed $= "walking") return "fs_walk_sand" @ getRandom(1,4) @ "_sound";
						else return "fs_run_sand" @ getRandom(1,6) @ "_sound";

		case "basic": 	if(%speed $= "walking") return "fs_walk_basic" @ getRandom(1,4) @ "_sound";
						else return "fs_run_basic" @ getRandom(1,4) @ "_sound";

		case "fabric": 	if(%speed $= "walking") return "fs_walk_fabric" @ getRandom(1,5) @ "_sound";
						else return "fs_run_fabric" @ getRandom(1,5) @ "_sound";

		case "snow":	return "fs_run_snow" @ getRandom(1,3) @ "_sound";// snowsteps only have one speed.

		case "default":	if(%speed $= "walking") return "fs_walk_basic" @ getRandom(1,4) @ "_sound";
						else return "fs_run_basic" @ getRandom(1,4) @ "_sound";

		default: 	if(%speed $= "walking") return "fs_walk_basic" @ getRandom(1,4) @ "_sound";
					else return "fs_run_basic" @ getRandom(1,4) @ "_sound";
	}
}

//+++ 	Calculate the noise based off what the player is stepping on.
function checkPlayback(%obj)
{
	%surface = (%obj.touchcolor $= "") ? %obj.surface : %obj.touchColor;
	%speed = (!%obj.isSlow) ? "running" : "walking";

	if(%obj.touchColor $= "") return PeggFootsteps_getSound(%surface, %speed);	
	else
	{
		%r = getWord(%surface, 0) * 255;
		%g = getWord(%surface, 1) * 255;
		%b = getWord(%surface, 2) * 255;

		for(%i = 0; %i < $Pref::Server::PF::customsounds; %i++)
		{
			%hit = $Pref::Server::PF::colorPlaysFX[%i];
			%hitcolor = getWords(%hit, 0, 3);
			%hr = getWord(%hitcolor, 0) * 255;
			%hg = getWord(%hitcolor, 1) * 255;
			%hb = getWord(%hitcolor, 2) * 255;
			if(%hr == %r && %hg == %g && %hb == %b)
			{
				%hitsound = getWord(%hit, 4);
				return PeggFootsteps_getSound(%hitsound, %speed);
			}
		}

		if(%r >= %b && %b >= %g || (%r > 180 && %g > 180)) %surface = "dirt";		
		else if (%r >= %g && %g > %b) %surface = "wood";		
		else if (%g >= %b && %g > %r) %surface = "grass";		
		else %surface = "stone";
		
		if(%r == %b && %r == %g && %b == %g)
		{
			%surface = "stone";
			if(%r > 230 && %g > 230 && %b > 230) %surface = "snow";			
		}
	}
	if (%obj.isSwimming) %surface = "water";//splashing in the water	
	%obj.surface = %surface; //Robb's bugfix.
	return PeggFootsteps_getSound(%surface, %speed);
}

//+++ Return the surface's name when given a number from a list.
function parseSoundFromNumber(%val, %obj) // brick is an optional parameter
{
	if (!$Pref::Server::PF::brickFXSounds::enabled ) return "default";
	//Default 0 Basic 1 Dirt 2 Grass 3 Metal 4 Sand 5 Snow 6 Stone 7 Water 8 Wood 9
	switch (%val)
	{
		case 0: 	if(%obj.touchColor !$= "") return "color";
					else return "default";
		case 1: return "basic";
		case 2: return "dirt";
		case 3: return "fabric";
		case 4: return "grass";
		case 5: return "metal";
		case 6: return "sand";
		case 7: return "snow";
		case 8: return "stone";
		case 9: return "water";
		case 10: return "wood";
	}
}

//+++ Drop some rad peggstep noise in here!
function PeggFootsteps(%obj, %lastVert)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead" || %obj.getdataBlock().enablePeggFootsteps) return;

	cancel(%obj.peggstep);
	if(%obj.getdataBlock().getName() $= "EventidePlayer")
	{
		%velz = getword(%obj.getVelocity(), 2);
		if (%velz < 0)
		{
		    // Make the player fall faster
			%obj.addvelocity("0 0 -0.1");

		    if (%velz < -25)
		    {
		        if (%obj.lastFallSpamClick+getRandom(50,250) < getSimTime())
				{
					%obj.lastFallSpamClick = getSimTime();
					%obj.activateStuff();
				}

		        if (!%obj.isFalling)
		        {
		            %obj.playthread(2, "side");		            
		            %obj.getDatablock().TunnelVision(%obj, true);
		            %obj.isFalling = true;

					%genderSound = (!%obj.client.chest) ? "male" : "female";
					%genderSoundAmount = (!%obj.client.chest) ? 3 : 5;
					%sound = %genderSound @ "_shock" @ getRandom(1, %genderSoundAmount) @ "_sound";

					%obj.playaudio(0,%sound);
		        }
		    }
		}
		else if (%obj.isFalling)
		{
		    %obj.playthread(2, "root");
		    %obj.getDatablock().TunnelVision(%obj, false);
		    %obj.isFalling = false;
		}
	}	

	if($Pref::Server::PF::footstepsEnabled)
	{
		if(%obj.isMounted() || %obj.isInvisible)
		{
			%obj.peggstep = schedule(50,0,PeggFootsteps,%obj);
			return;
		}
		%vel = %obj.getVelocity();
		%vert = getWord(%vel, 2);
		%horiz = vectorLen(setWord(%vel, 2, 0));

		%pos = %obj.getPosition();
		initContainerBoxSearch(%pos, "1.25 1.25 0.1", $TypeMasks::fxBrickObjectType | $Typemasks::TerrainObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::FxPlaneObjectType);
		%col = containerSearchNext();

		if(isObject(%col))
		{
			%type = %col.getClassName();
			if ( %type $= "fxDTSbrick" && %col.isRendering() )
			{
					%obj.lastBrick =  %col;
					// by default, the surface isn't decided yet, and will be decided by the color
					%obj.touchColor = getColorIDTable(%col.getColorId());
					%obj.surface = "";
					// check to see if there is a custom sound based on the brick's special FX
					if ($Pref::Server::PF::brickFXSounds::enabled)
					{
						switch(%col.getColorFxID())
						{
							case 1: %obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::pearlStep, %obj);
									%obj.touchColor = "";
							case 2: %obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::chromeStep, %obj);
									%obj.touchColor = "";
							case 3: %obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::glowStep, %obj);
									%obj.touchColor = "";
							case 4: %obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::blinkStep, %obj);
									%obj.touchColor = "";
							case 5: %obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::swirlStep, %obj);
									%obj.touchColor = "";
							case 6: %obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::rainbowStep, %obj);
									%obj.touchColor = "";
						}
						// if there's a shape fx, which takes priority over color fx
						if(%col.getShapeFxID())
						{
							%obj.surface = parseSoundFromNumber($Pref::Server::PF::brickFXsounds::unduloStep, %obj);
							%obj.touchColor = "";
						}
						// if the preference for the shape or color fx is default, then just play the regular sound that would be made based on color
						if (%obj.surface $= "color") %obj.touchColor = getColorIDTable(%col.getColorId());				
					}
					// check to see if the brick has an event based custom sound
					if(%col.customStep !$= "")
					{
						%obj.touchColor = "";
						%obj.surface = %col.customStep;
					}
			}
			else if(%type $= "fxPlane")
			{
				%obj.touchColor = "";
				%obj.surface = parseSoundFromNumber($Pref::Server::PF::terrainStep, %obj);
			}
			else if(%type $= "WheeledVehicle" || %type $= "FlyingVehicle")
			{
				%obj.touchColor = "";
				%obj.surface = parseSoundFromNumber($Pref::Server::PF::vehicleStep, %obj);
			}
			else
			{
				%obj.touchColor = "";
				%obj.surface = parseSoundFromNumber($Pref::Server::PF::defaultStep, %obj);
			}
			if ( %obj.getWaterCoverage() > 0 )
			{
				%obj.surface = "water";
				%obj.touchColor = "";
			}
			if (!%isGround && mAbs(%lastVert) > $Pref::Server::PF::minLandSpeed * getWord(%obj.getScale(), 1) && $Pref::Server::PF::landingFX)
			%obj.getDatablock().onLand(%obj, mAbs(%lastVert));
			
			%isGround = true;
		}
		else %isGround = false;
		
		%obj.isSlow = (mAbs(%horiz) < $Pref::Server::PF::runningMinSpeed * getWord(%obj.getScale(), 0) || %obj.isCrouched());

		if(%obj.getWaterCoverage() > 0 && $Pref::Server::PF::waterSFX == 1 && mAbs(%horiz) > 0.1 && !%isGround)
		{
			%obj.touchColor = "";
			%obj.surface = "under water";
			$oldTimescale = getTimescale();
			setTimescale((getRandom(75,150)*0.01) * $oldTimescale);
			serverplay3d(checkPlayback(%obj), %obj.getHackPosition());
			setTimescale($oldTimescale);
			if(!%obj.isCrouched()) %obj.getDatablock().onPeggFootstep(%obj);
			%obj.peggstep = schedule(500 * getWord(%obj.getScale(), 0), 0, PeggFootsteps, %obj);
		}
		else if(mFloor(%horiz) == 0 || !%isGround) %obj.peggstep = schedule(50, 0, PeggFootsteps, %obj, %vert);
		
		else if (%isGround && mAbs(%horiz) > 0)
		{
			%obj.peggstep = schedule(320 * getWord(%obj.getScale(), 0), 0, PeggFootsteps, %obj, %vert);
			$oldTimescale = getTimescale();
			setTimescale((getRandom(75,150)*0.01) * $oldTimescale);
			serverplay3d(checkPlayback(%obj), %obj.getHackPosition());
			setTimescale($oldTimescale);
			if(!%obj.isCrouched()) %obj.getDatablock().onPeggFootstep(%obj);
		}

		%obj.peggstep = schedule(1000, 0, PeggFootsteps, %obj);
		return;
	}
}

function Armor::onPeggFootstep(%this,%obj)
{
	//Hello world
}

registerOutputEvent("fxDTSBrick","setFootstep","List Clear -1 Default 0 Basic 1 Dirt 2 Grass 3 Metal 4 Sand 5 Snow 6 Stone 7 Water 8 Wood 9");

function fxDTSBrick::setFootstep(%brick, %val, %client)
{
	if (%val == -1) %brick.customStep = "";
	else %brick.customStep = parseSoundFromNumber(%val, %client.player);
}