//
// Playertype data.
//

datablock TSShapeConstructor(SkinwalkerDTS)
{
    baseShape  = "./models/skinwalker.dts";
    sequence0  = "./models/skinwalker.dsq";
    sequence1  = "./models/skinwalker_melee.dsq";
};

datablock PlayerData(PlayerSkinwalker : PlayerStandardArmor)
{
	isEventideModel = true;
	isKiller = true;
    firstPersonOnly = true;
	showEnergyBar = true;
	canJet = false;	
	useCustomPainEffects = true;
	shapeFile = SkinwalkerDTS.baseShape;
	killerraisearms = true;
	killerlight = "NoFlareRLight";

	renderFirstPerson = false;
	
	// Weapon: Claws
	HitProjectile = KillerRoughHitProjectile;
	hitobscureprojectile = "";
	
	meleetrailskin = "raggedClaw";
	meleetrailoffset = "0.9 1.4 0.7"; 	
	meleetrailangle1 = "0 90 0";
	meleetrailangle2 = "0 -90 0";
	meleetrailscale = "4 4 3";	

	rightclickicon = "color_skinwalker_reveal";
	leftclickicon = "color_melee";
	rightclickspecialicon = "";
	leftclickspecialicon = "color_consume";
	
	killerChaseLvl1Music = "musicData_Eventide_SkinwalkerNear";
	killerChaseLvl2Music = "musicData_Eventide_SkinwalkerChase";
	
	killeridlesound = "skinwalker_idle";
	killeridlesoundamount = 5;
	
	killerchasesound = "skinwalker_attack";
	killerchasesoundamount = 5;

	killermeleesound = "skinwalker_chase";
	killermeleesoundamount = 3;    

	killerweaponsound = "skinwalker_weapon";
	killerweaponsoundamount = 4;	

	killermeleehitsound = "melee_tanto";
	killermeleehitsoundamount = 3;    

	PainSound = "skinwalker_pain_sound";
	DeathSound = "";
	JumpSound = "";
	uiName = "Skinwalker Player";
	jumpForce = 0;
	
	rechargeRate = 0.375;	
	maxDamage = 9999;
	maxForwardSpeed = 6.16;
	maxBackwardSpeed = 3.52;
	maxSideSpeed = 5.28;
	boundingBox = "4.5 4.5 9.5";
	crouchBoundingBox = "4.5 4.5 3.6";
};

//
// GUI.
//

function PlayerSkinwalker::killerGUI(%this,%obj,%client)
{	
	%energylevel = %obj.getEnergyLevel();

	// Some dynamic varirables
	%leftclickstatus = (%obj.getEnergyLevel() >= 25) ? "hi" : "lo";
	%rightclickstatus = (%obj.getEnergyLevel() == %this.maxEnergy) ? "hi" : "lo";
	%leftclicktext = (%this.leftclickicon !$= "") ? "<just:left>\c6Left click" : "";
	%rightclicktext = (%this.rightclickicon !$= "") ? "<just:right>\c6Right click" : "";

	// Regular icons
	%leftclickicon = (%this.leftclickicon !$= "") ? "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickicon @ ">" : "";
	%rightclickicon = (%this.rightclickicon !$= "") ? "<just:right><bitmap:" @ $iconspath @ %rightclickstatus @ %This.rightclickicon @ ">" : "";

	// Change them to special if they exist
	if(%obj.getEnergyLevel() >= 25 && %this.leftclickspecialicon !$= "" && isObject(%obj.gazing) && %obj.gazing.getdataBlock().isDowned)
	{		
		%leftclickstatus = (%obj.gazing.getDamagePercent() > 0.05) ? "hi" : "lo";
		%leftclickicon = "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickspecialicon @ ">";
	}

	
	if(%obj.getDataBlock() $= %this)
	{
		%client.bottomprint(%leftclicktext @ %rightclicktext @ "<br>" @ %leftclickicon @ %rightclickicon, 1);
	}
	else 
	{
		%client.bottomprint(%rightclicktext @ "<br>" @ %rightclickicon, 1);
	}
}

//
// Appearance and initialization.
//

function PlayerSkinwalker::onNewDatablock(%this,%obj)
{
	Parent::onNewDatablock(%this,%obj);

	%obj.setScale("1.2 1.2 1.2");
    %obj.playthread(0,"roar");    
    %obj.playaudio(0,"skinwalker_roar_sound");
    %obj.schedule(1, setEnergyLevel,0);
	for(%i = 0; %i < getRandom(1,2); %i++) %obj.schedule(50,spawnExplosion,"goryExplosionProjectile",%obj.getScale());   
    %obj.isSkinwalker = true;
}

function PlayerSkinwalker::EventideAppearance(%this,%obj,%client)
{
    %clientappearance = isObject(%obj.victimreplicatedclient) ? %obj.victimreplicatedclient : %client;

    %obj.unHideNode("ALL");
    %obj.setNodeColor($hat[$clientappearance.hat],%clientappearance.hatColor);
	%obj.setFaceName(%clientappearance.faceName);
	%obj.setDecalName(%clientappearance.decalName);
	%obj.setNodeColor("head",%clientappearance.headColor);
	%obj.setNodeColor("chestskinwalker",%clientappearance.chestColor);
	%obj.setNodeColor("pants",%clientappearance.hipColor);
	%obj.setNodeColor("rarm",%clientappearance.rarmColor);
	%obj.setNodeColor("Larm",%clientappearance.larmColor);
	%obj.setNodeColor("lhand",%clientappearance.lhandColor);
	%obj.setNodeColor("rhand",%clientappearance.rhandColor);
	%obj.setNodeColor("rshoe",%clientappearance.rlegColor);
	%obj.setNodeColor("lshoe",%clientappearance.llegColor);

	//HatMod support. Remove their hat, if they have one.
	//We need to avoid any calls to applyCharacterPrefs here, or we will cause an infinite loop.
	%hat = $HatMod::save::wornHat[%clientappearance.bl_id];
	if(isHat(%hat))
	{
		%obj.unmountImage(2);
	}
}

function PlayerSkinwalker::onPeggFootstep(%this,%obj)
{
	serverplay3d("skinwalker_walking" @ getRandom(1,5) @ "_sound", %obj.getHackPosition());
	%obj.spawnExplosion("Eventide_footstepShakeProjectile", 0.5 + (getRandom() / 2));
}

//
// Transformation.
//

function PlayerSkinwalker::onTrigger(%this, %obj, %trig, %press) 
{		
	Parent::onTrigger(%this, %obj, %trig, %press);
	
	if(%press) switch(%trig)
	{
		case 0: if(%obj.getEnergyLevel() >= 25)
				{
					%this.killerMelee(%obj, 4);
				}				

		case 4: %this.Transform(%obj);
	}
}

function PlayerSkinwalker::Transform(%this,%obj,%bool,%count)
{	
	// Check if the player is dead, has enough energy, or is already transforming before continuing
    if(!isObject(%obj) || %obj.getState() $= "Dead" || %obj.getEnergyLevel() != %obj.getDataBlock().maxEnergy || isEventPending(%obj.Transformschedule)) 
	{
		return;
	}

	// Check if the player has a victim, if they do, they can't transform
	if(isObject(%obj.victim))
	{
		return;
	}

	// Transform the player
    if(%count <= 10)
    {
        // To prevent the audio from playing multiple times
		if(%count == 1)
        {
            %obj.playaudio(3,"skinwalker_change_sound");            
        }

        %obj.playthread(0,"plant");
        %obj.Transformschedule = %this.schedule(100,Transform,%obj,%bool,%count+1);
    }
    else 
    {		
		if(isObject(%obj.light)) 
		{
			%obj.light.delete();
		}		

		if(%obj.getDataBlock() !$= %this)
		{
			%obj.setdatablock("PlayerSkinwalker");
		}
		else
		{
			%obj.setdatablock("EventidePlayer");

			for (%j = 0; %j < 4; %j++)
			{
				%obj.playThread(%j,"root");
			}
		}	        
    }
}

//
// Melee.
//

function PlayerSkinWalker::onKillerHit(%this,%obj,%hit)
{
	if(isObject(%obj.victim) || !%hit.getdataBlock().isDowned || %hit.getDamagePercent() < 0.05)
	{
		return true;
	}
	
	if(isObject(%hit.client)) 
	{
		%obj.stunned = true;
		%hit.client.setControlObject(%hit.client.camera);
		%hit.client.camera.setMode("Corpse",%hit);
	}

	%obj.victim = %hit;
	%obj.victimreplicatedclient = %hit.client;																
	%obj.playthread(1,"eat");
	%obj.playthread(2,"talk");
	%obj.playaudio(1,"skinwalker_grab_sound");
	%obj.mountobject(%hit,6);
	%hit.schedule(2250,kill);
	%hit.setarmthread("activate2");
	%hit.schedule(2250,spawnExplosion,"goryExplosionProjectile",%hit.getScale()); 
	%hit.schedule(2295,kill);        
	%hit.schedule(2300,delete);        
	%obj.schedule(2250,playthread,1,"root");
	%obj.schedule(2250,playthread,2,"root");
	%obj.schedule(2250,setField,victim,0);
	%this.schedule(2250,EventideAppearance,%obj,%obj.client);
	return false;
}

//
// Packages.
//

package Eventide_Skinwalker
{
	function Player::addItem(%player, %image, %client)
	{
		//Skinwalkers should not be able to pick up items
		if(!%obj.isSkinwalker)
		{
			Parent::addItem(%player, %image, %client);
		}		
    }
};

if(isPackage("Eventide_Skinwalker")) 
{
	deactivatePackage("Eventide_Skinwalker");
}
activatePackage("Eventide_Skinwalker");

//
// Hatmod package.
//

package Eventide_Skinwalker_Hatmod
{
	function serverCmdHat(%client, %na, %nb, %nc, %nd, %ne)
	{
		%player = %client.player;
		if(isObject(%player) && %player.getDataBlock().isKiller)
		{
			return;
		}
		parent::serverCmdHat(%client, %na, %nb, %nc, %nd, %ne);
	}
};
if(isFunction(serverCmdHat)) //Is HatMod enabled?
{
	activatePackage(Eventide_Skinwalker_Hatmod);
}