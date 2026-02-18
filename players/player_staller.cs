//
// Cloak/decloak emitter effects.
//

datablock ParticleData(stallerCloakSmokeParticle)
{
   dragCoefficient = 1.0;
   constantAcceleration = 0.0;
   gravityCoefficient = -3.0;
   inheritedVelFactor = 1.0;
   spinSpeed = 0;

   lifetimeMS = 400;
   lifetimeVarianceMS = 100;

   textureName = "base/data/particles/cloud";
   useInvAlpha = true;

   colors[0] = "0 0 0 0.5";
   colors[1] = "0.1 0.1 0.1 1";
   colors[2] = "0.15 0.15 0.15 0";

   sizes[0] = 3.0;
   sizes[1] = 2.0;
   sizes[2] = 1.5;

   times[0] = 0.0;
   times[1] = 0.5;
   times[2] = 1.0;
};

datablock ParticleData(stallerCloakInvertedStarParticle : stallerCloakSmokeParticle)
{
	textureName = "Add-Ons/Gamemode_Eventide/players/particles/invertedStar";

	lifetimeMS = 800;
	gravityCoefficient = -0.5;

	colors[0] = "1 0 0 1";
	colors[1] = "1 0 0 1";
	colors[2] = "1 0 0 0";

	sizes[0] = 0.75;
	sizes[1] = 0.75;
	sizes[2] = 0.75;
};

datablock ParticleEmitterData(stallerCloakSmokeEmitter)
{
	lifeTimeMS = 300;
	ejectionPeriodMS = 1;
	periodVarianceMS = 0;

	ejectionVelocity = 1.0;
	velocityVariance = 0.0;

	ejectionOffset = 2.0; //How far away from the origin point particles can spawn.

	thetaMin = 0; //Particles spawn up.
	thetaMax = 90; //Particles spawn down.

	//Make the particles spawn around the origin point, never on.
	phiReferenceVel = 720;
	phiVariance = 360;

	overrideAdvance = false;
	useEmitterColors = false;
	orientParticles = false;
	particles = "stallerCloakSmokeParticle";
};

datablock ParticleEmitterData(stallerCloakInvertedStarEmitter : stallerCloakSmokeEmitter)
{
	ejectionPeriodMS = 45;
	ejectionOffset = 3.5; //How far away from the origin point particles can spawn.
	particles = "stallerCloakInvertedStarParticle";
};

datablock ExplosionData(stallerCloakExplosion)
{
	lifeTimeMS = 800;
	particleDensity = 100;
	particleRadius = 1.0;
	emitter[0] = stallerCloakSmokeEmitter;
	emitter[1] = stallerCloakInvertedStarEmitter;

	damageRadius = 0; 
	radiusDamage = 0;
	impulseRadius = 0;
	impulseForce = 0;
	playerBurnTime = 0;

	shakeCamera = false;
	explosionShape = "base/data/shapes/empty.dts";
};

datablock ProjectileData(stallerCloakProjectile)
{
	directDamage = 0;
	directDamageType = $DamageType::Default;
	radiusDamageType = $DamageType::Default;
	impactImpulse = 0;
	verticalImpulse = 0;

	explosion = stallerCloakExplosion;
	waterExplosion = stallerCloakExplosion;
	particleEmitter = "";
	particleWaterEmitter = "";
	splash = "";

	armingDelay = 0;
	lifeTime = 1;
	isBallistic = false;
	explodeOnDeath = true;
	
	muzzleVelocity = 0;
	velInheritFactor = 1.0;

	hasLight = false;
	projectileShapeName = "base/data/shapes/empty.dts";
};

//
// Core and appearance.
//

datablock PlayerData(PlayerStaller : PlayerSurvivor)
{
    uiName = "Staller";

	voicePack = "Staller";
	facePack = "";

	fadeSteps = 10;
	fadeColor = "0 0 0";
};
//Inherits functions from `PlayerSurvivor`.
PlayerStaller.inheritFunctionsFromSuperClass("PlayerSurvivor");

function PlayerStaller::getFacePack(%this, %obj)
{
	return "";
}

function PlayerStaller::getVoicePack(%this, %obj)
{
	return "staller";
}

function PlayerStaller::eventideBodyParts(%this, %obj)
{
	%obj.hideNode("ALL");

	%obj.unHideNode("chest");	
	%obj.unHideNode("rhand");
	%obj.unHideNode("lhand");
	%obj.unHideNode("rarm");
	%obj.unHideNode("larm");
	%obj.unHideNode("headskin");
	%obj.unhideNode("stallerHood");
	%obj.unHideNode("skirt");
	%obj.setHeadUp(0);

	%obj.setDecalName("AAA-None");

    //Custom player scale.
    %obj.setScale("1.05 1.05 1.05");
}

function PlayerStaller::eventideBodyColors(%this, %obj)
{
	if(%obj.isInvisible)
	{
		//Both cloaking and decloaking contain a fade period where normal colors can't apply.
		return;
	}

    %skinColor = "0.5 0.5 0.5 1";
	%clothingColor = "0.1 0.1 0.1 1";
	%shirtColor = "0.541 0.698 0.553 1";

    //Set core body part colors.
	%obj.setNodeColor("headskin", "0 0 0 1");
	%obj.setNodeColor("stallerHood", %clothingColor);
	%obj.setNodeColor("chest", %clothingColor);
	%obj.setNodeColor("skirt", %clothingColor);
	%obj.setNodeColor("rarm", %clothingColor);
	%obj.setNodeColor("larm", %clothingColor);
	%obj.setNodeColor("Rhand", %skinColor);
	%obj.setNodeColor("Lhand", %skinColor);

    //Set blood node colors, only shown upon taking damage.
	%bloodColor = "0.4 0 0 1";
	%obj.setNodeColor("lhand_blood", %bloodColor);
	%obj.setNodeColor("rhand_blood", %bloodColor);
	%obj.setNodeColor("chest_blood_front", %bloodColor);
	%obj.setNodeColor("chest_blood_back", %bloodColor);
}

//
// Cloaking mechanic and datablock.
//

function PlayerStaller::spawnCloakEffect(%this, %obj)
{
	new Projectile()
	{
		dataBlock = stallerCloakProjectile;
		initialVelocity = VectorAdd(%obj.getVelocity(), "0 0 0.1"); //Particle positioning messes up if velocity is exactly 0. Let's fix it!
		initialPosition = %obj.getTransform();
		sourceObject = %obj;
		sourceSlot = 3;
		client = %obj.client;
	}.explode();
}

function PlayerStaller::fadeEffect(%this, %obj, %fadeIn)
{
	%totalSteps = %this.fadeSteps;
	%fadeColor = %this.fadeColor;
	%stepTime = 10;

	%obj.startFade(0, 0, 1);
	for(%i = 0; %i <= %totalSteps; %i++)
	{
		%fadeProgress = (%fadeIn) ? (%i / %totalSteps) : (1 - (%i / %totalSteps));
		%obj.schedule(%stepTime * %i, setNodeColor, "ALL", %fadeColor SPC %fadeProgress);
	}
	%this.schedule(%stepTime * %totalSteps, _completeFadeEffect, %obj, %fadeIn);
}

function PlayerStaller::_completeFadeEffect(%this, %obj, %fadeIn)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead")
	{
		return;
	}

	%obj.isInvisible = !(%fadeIn);
	if(%fadeIn)
	{
		%obj.startFade(0, 0, 0);
		%this.eventideBodyColors(%obj);
	}
	else
	{
		%obj.hideNode("ALL");
	}

	%client = (%fadeIn) ? "" : %obj.client;
	%obj.adjustObjectScopeToAll(%fadeIn, %client);
}

datablock PlayerData(PlayerStallerCloaked : PlayerStaller)
{
	maxForwardCrouchSpeed = PlayerStaller.maxForwardSpeed;
	maxSideCrouchSpeed = PlayerStaller.maxSideSpeed;
	maxBackwardCrouchSpeed = PlayerStaller.maxBackwardSpeed;

    uiName = "";
	isEventideClass = true;

	maxEnergy = 100;
	rechargeRate = -0.4375; //32 ticks per second * 14 = 8 seconds of cloak at full energy.
};
//Inherits functions from `PlayerStaller`.
PlayerStallerCloaked.inheritFunctionsFromSuperClass("PlayerStaller");

//
// Cloaking.

//Triggering a cloak.
function PlayerStaller::onTrigger(%this, %obj, %trigger, %state)
{
	%returnValue = %this.super("onTrigger", %this, %obj, %trigger, %state);
	if(%trigger == 3 && %state)
	{
		%this.cloak(%obj);
	}
	return %returnValue;
}

//Cloak effects.
function PlayerStaller::cloak(%this, %obj)
{
	//Energy needs to be full to cloak, for balance reasons.
	if(%obj.getEnergyLevel() != %this.maxEnergy)
	{
		return;
	}
	//Prevent cloak spam by adding a 1-second delay.
	else if((getSimTime() - %obj.lastCloakTime) < 1000)
	{
		return;
	}

	//Store last cloaking time.
	%obj.lastCloakTime = getSimTime();

	//Change the player to the cloaked datablock, so all the nodes will be hidden and the crouching speed will be increased.
	%targetDatablock = PlayerStallerCloaked;
	%obj.setDatablock(%targetDatablock);

	//Make a cloud of smoke and inverted stars.
	%this.fadeEffect(%obj, false);
	%this.spawnCloakEffect(%obj);

	//Play the cloaking sound effect.
	%obj.playManagedSound("Cloak");

	//Start the tick loop, to determine when the player has run out of energy.
	cancel(%obj.cloakTickSchedule);
	%targetDatablock.cloakTick(%obj);
}

//Cloaking tick, handles uncloaking if the player runs out of energy.
function PlayerStallerCloaked::cloakTick(%this, %obj)
{
	//If the player no longer exists, we no longer need to do this.
	if(!isObject(%obj))
	{
		return;
	}

	//If the player has run out of energy, uncloak them.
	if(%obj.getEnergyLevel() == 0)
	{
		return %this.uncloak(%obj);
	}

	//Loop this check, as energy drains continuously
	cancel(%obj.cloakTickSchedule);
	%obj.cloakTickSchedule = %this.schedule(33, cloakTick, %obj);
}

//
// Decloak.

//Triggering a cloak.
function PlayerStallerCloaked::onTrigger(%this, %obj, %trigger, %state)
{
	%returnValue = %this.super("onTrigger", %this, %obj, %trigger, %state);
	if(%trigger == 3 && !%state)
	{
		%this.uncloak(%obj);
	}
	return %returnValue;
}

function PlayerStallerCloaked::uncloak(%this, %obj)
{
	//Cancel the looping check for energy, if it exists.
	cancel(%obj.cloakTickSchedule);

	//Resume the normal datablock and appearance code.
	%obj.setDatablock(%this.superClass);

	//Reghost the player to everyone, so they are no longer invisible.
	%obj.adjustObjectScopeToAll(true);

	//Make a cloud of smoke and inverted stars.
	%this.fadeEffect(%obj, true);
	%this.spawnCloakEffect(%obj);

	//Play the decloaking sound effect.
	%obj.playManagedSound("Uncloak");
}

//
// Package to mute Staller and clean up the hood when switch datablocks.
//

package Player_Staller
{
	//Unmount the Staller hood if the player has it equipped and is no longer a Staller.
	function GameConnection::applyBodyParts(%this)
	{
		%player = %this.player;
		if(!%player)
		{
			return;
		}
		
		%playerDatablock = %player.getDatablock();
		%hoodMountPoint = PlayerStaller.hoodMountPoint;
		%thirdMountedImage = %player.getMountedImage(%hoodMountPoint);

		if(%player && %playerDatablock.getName() !$= "PlayerStaller" && %thirdMountedImage && %thirdMountedImage.getName() $= "stallerHoodImage")
		{
			%player.unmountImage(%hoodMountPoint);
		}

		//Continue on with expected appearance functionality.
		return Parent::applyBodyParts(%this);
	}

	//Make the Staller more cold and emotionless by preventing them from emoting.
	function Player::emote(%player, %data, %skipSpam)
	{
		if(%player.getDataBlock().getName() $= "PlayerStaller")
		{
			%client = %player.client;
			if(%client)
			{
				%client.centerPrint("<color:ffffff>...", 3);
			}

			return;
		}
		Parent::emote(%player, %data, %skipSpam);
	}

	//Prevent the Stalller from visually talking when chatting, to help create that disconnect while still allowing the Staller to chat.
	function serverCmdTeamMessageSent(%client, %text)
	{
		//If the player is not a Staller, do normal functionality.
		%player = %client.player;
		if(!%player || %player.getDatablock().getName() !$= "PlayerStaller")
		{
			return Parent::serverCmdMessageSent(%client, %text);
		}

		%trimText = trim(%text);
		if (%client.lastChatText $= %trimText)
		{
			%chatDelta = (getSimTime() - %client.lastChatTime) / getTimeScale();
			if (%chatDelta < 15000)
			{
				%client.spamMessageCount = $SPAM_MESSAGE_THRESHOLD;
				messageClient(%client, '', '\c5Do not repeat yourself.');
			}
		}
		%client.lastChatTime = getSimTime();
		%client.lastChatText = %trimText;

		%text = chatWhiteListFilter(%text);
		%text = StripMLControlChars(%text);
		%text = trim(%text);
		if (strlen(%text) <= 0)
		{
			return;
		}
		if ($Pref::Server::MaxChatLen > 0)
		{
			if (strlen(%text) >= $Pref::Server::MaxChatLen)
			{
				%text = getSubStr(%text, 0, $Pref::Server::MaxChatLen);
			}
		}
		%protocol = "http://";
		%protocolLen = strlen(%protocol);
		%urlStart = strpos(%text, %protocol);
		if (%urlStart == -1)
		{
			%protocol = "https://";
			%protocolLen = strlen(%protocol);
			%urlStart = strpos(%text, %protocol);
		}
		if (%urlStart == -1)
		{
			%protocol = "ftp://";
			%protocolLen = strlen(%protocol);
			%urlStart = strpos(%text, %protocol);
		}
		if (%urlStart != -1)
		{
			%urlEnd = strpos(%text, " ", %urlStart + 1);
			%skipProtocol = 0;
			if (%protocol $= "http://")
			{
				%skipProtocol = 1;
			}
			if (%urlEnd == -1)
			{
				%fullUrl = getSubStr(%text, %urlStart, strlen(%text) - %urlStart);
				%url = getSubStr(%text, %urlStart + %protocolLen, (strlen(%text) - %urlStart) - %protocolLen);
			}
			else
			{
				%fullUrl = getSubStr(%text, %urlStart, %urlEnd - %urlStart);
				%url = getSubStr(%text, %urlStart + %protocolLen, (%urlEnd - %urlStart) - %protocolLen);
			}
			if (strlen(%url) > 0)
			{
				%url = strreplace(%url, "<", "");
				%url = strreplace(%url, ">", "");
				if (%skipProtocol)
				{
					%newText = strreplace(%text, %fullUrl, "<a:" @ %url @ ">" @ %url @ "</a>\c6");
				}
				else
				{
					%newText = strreplace(%text, %fullUrl, "<a:" @ %protocol @ %url @ ">" @ %url @ "</a>\c6");
				}
				%text = %newText;
			}
		}
		if ($Pref::Server::ETardFilter)
		{
			if (!chatFilter(%client, %text, $Pref::Server::ETardList, '\c5This is a civilized game.  Please use full words.'))
			{
				return 0;
			}
		}
		chatMessageTeam(%client, %client.team, '\c7%1\c3%2\c7%3\c4: %4', %client.clanPrefix, %client.getPlayerName(), %client.clanSuffix, %text);
		echo("(T)", %client.getSimpleName(), ": ", %text);
	}

	//Prevent the Stalller from visually talking when chatting, to help create that disconnect while still allowing the Staller to chat.
	function serverCmdMessageSent(%client, %text)
	{
		//If the player is not a Staller, do normal functionality.
		%player = %client.player;
		if(!%player || %player.getDatablock().getName() !$= "PlayerStaller")
		{
			return Parent::serverCmdMessageSent(%client, %text);
		}

		//All this just to stop the Staller from talking.
		%trimText = trim(%text);
		if (%client.lastChatText $= %trimText)
		{
			%chatDelta = (getSimTime() - %client.lastChatTime) / getTimeScale();
			if (%chatDelta < 15000)
			{
				%client.spamMessageCount = $SPAM_MESSAGE_THRESHOLD;
				messageClient(%client, '', '\c5Do not repeat yourself.');
			}
		}
		%client.lastChatTime = getSimTime();
		%client.lastChatText = %trimText;
		%player = %client.Player;

		%text = chatWhiteListFilter(%text);
		%text = StripMLControlChars(%text);
		%text = trim(%text);
		if (strlen(%text) <= 0)
		{
			return;
		}
		if ($Pref::Server::MaxChatLen > 0)
		{
			if (strlen(%text) >= $Pref::Server::MaxChatLen)
			{
				%text = getSubStr(%text, 0, $Pref::Server::MaxChatLen);
			}
		}
		%protocol = "http://";
		%protocolLen = strlen(%protocol);
		%urlStart = strpos(%text, %protocol);
		if (%urlStart == -1)
		{
			%protocol = "https://";
			%protocolLen = strlen(%protocol);
			%urlStart = strpos(%text, %protocol);
		}
		if (%urlStart == -1)
		{
			%protocol = "ftp://";
			%protocolLen = strlen(%protocol);
			%urlStart = strpos(%text, %protocol);
		}
		if (%urlStart != -1)
		{
			%urlEnd = strpos(%text, " ", %urlStart + 1);
			%skipProtocol = 0;
			if (%protocol $= "http://")
			{
				%skipProtocol = 1;
			}
			if (%urlEnd == -1)
			{
				%fullUrl = getSubStr(%text, %urlStart, strlen(%text) - %urlStart);
				%url = getSubStr(%text, %urlStart + %protocolLen, (strlen(%text) - %urlStart) - %protocolLen);
			}
			else
			{
				%fullUrl = getSubStr(%text, %urlStart, %urlEnd - %urlStart);
				%url = getSubStr(%text, %urlStart + %protocolLen, (%urlEnd - %urlStart) - %protocolLen);
			}
			if (strlen(%url) > 0)
			{
				%url = strreplace(%url, "<", "");
				%url = strreplace(%url, ">", "");
				if (%skipProtocol)
				{
					%newText = strreplace(%text, %fullUrl, "<a:" @ %url @ ">" @ %url @ "</a>\c6");
				}
				else
				{
					%newText = strreplace(%text, %fullUrl, "<a:" @ %protocol @ %url @ ">" @ %url @ "</a>\c6");
				}
				echo(%newText);
				%text = %newText;
			}
		}
		if ($Pref::Server::ETardFilter)
		{
			if (!chatFilter(%client, %text, $Pref::Server::ETardList, '\c5This is a civilized game.  Please use full words.'))
			{
				return 0;
			}
		}
		chatMessageAll(%client, '\c7%1\c3%2\c7%3\c6: %4', %client.clanPrefix, %client.getPlayerName(), %client.clanSuffix, %text);
		echo(%client.getSimpleName(), ": ", %text);
	}
};
if(isPackage(Player_Staller))
{
	deactivatePackage(Player_Staller);
}
activatePackage(Player_Staller);