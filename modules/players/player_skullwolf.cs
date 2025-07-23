datablock TSShapeConstructor(SkullwolfDTS) 
{
	baseShape = "./models/skullwolf.dts";
	sequence0 = "./models/skullwolf.dsq";
};

datablock PlayerData(PlayerSkullWolf : PlayerRenowned) 
{
	uiName = "Skullwolf Player";
	shapeFile = SkullwolfDTS.baseShape;	

	killerchaselvl1music = "musicData_Eventide_FiendNear";
	killerchaselvl2music = "musicData_Eventide_FiendChase";

	killeridlesound = "skullwolf_idle";
	killeridlesoundamount = 12;

	killerchasesound = "skullwolf_chase";
	killerchasesoundamount = 4;

	killermeleesound = "skullwolf_melee";
	killermeleesoundamount = 7;
	
	killerweaponsound = "skullwolf_weapon";
	killerweaponsoundamount = 6;	

	killermeleehitsound = "skullwolf_hit";
	killermeleehitsoundamount = 3;

	rightclickicon = "color_vanish";
	leftclickicon = "color_melee";
	rightclickspecialicon = "";
	leftclickspecialicon = "color_consume";

	// Weapon: Claws
	hitprojectile = KillerRoughHitProjectile;
	hitobscureprojectile = KillerKatanaClankProjectile;
	meleetrailskin = "baseClaw";
	meleetrailoffset = "0.3 1.4 0.7"; 	
	meleetrailangle1 = "0 90 0";
	meleetrailangle2 = "0 -90 0";
	meleetrailscale = "4 4 3";
	
	killerraisearms = true;
	killerlight = NoFlareRLight;

	renderFirstPerson = false;

	rechargeRate = 0.25;
	maxDamage = 696; //800
	maxTools = 0;
	maxWeapons = 0;
	maxForwardSpeed = 6.84;
	maxBackwardSpeed = 3.91;
	maxSideSpeed = 5.87;
	//+15% Speed

	maxForwardCrouchSpeed = 4.84;
	maxBackwardCrouchSpeed = 1.91;
	maxSideCrouchSpeed = 2.87;
	crouchBoundingBox = PlayerStandardArmor.boundingBox;
	jumpForce = 0;
};

function PlayerSkullWolf::killerGUI(%this,%obj,%client)
{	
	%energylevel = %obj.getEnergyLevel();

	// Some dynamic varirables
	%leftclickstatus = (%obj.getEnergyLevel() >= 25) ? "hi" : "lo";
	%rightclickstatus = (%obj.getEnergyLevel() == %this.maxEnergy || %obj.isInvisible) ? "hi" : "lo";
	%leftclicktext = (%this.leftclickicon !$= "") ? "<just:left>\c6Left click" : "";
	%rightclicktext = (%this.rightclickicon !$= "") ? "<just:right>\c6Right click" : "";		

	// Regular icons
	%leftclickicon = (%this.leftclickicon !$= "") ? "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickicon @ ">" : "";
	%rightclickicon = (%this.rightclickicon !$= "") ? "<just:right><bitmap:" @ $iconspath @ %rightclickstatus @ %This.rightclickicon @ ">" : "";

	// Change them to special if they exist
	if(%obj.getEnergyLevel() >= 25 && %this.leftclickspecialicon !$= "" && isObject(%obj.gazing) && %obj.gazing.getdataBlock().isDowned)
	{		
		%leftclickstatus = (%obj.gazing.getDamagePercent() > 0.25) ? "hi" : "lo";
		%leftclickicon = "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickspecialicon @ ">";
	}

	%client.bottomprint(%leftclicktext @ %rightclicktext @ "<br>" @ %leftclickicon @ %rightclickicon, 1);
}

function PlayerSkullWolf::onNewDatablock(%this,%obj)
{
	Parent::onNewDatablock(%this,%obj);	
	%obj.schedule(1, setEnergyLevel, 0);
	%obj.setScale("1.15 1.15 1.15");
	%obj.unHideNode("ALL");
}

function PlayerSkullWolf::disappear(%this,%obj,%alpha)
{
	if(!isObject(%obj) || isEventPending(%obj.reappearsched)) return;

	if(%alpha == 1)
	{
		%obj.playaudio(1,"skullwolf_cloak_sound");
		
		if(isObject(%obj.light)) 
		{
			%obj.light.delete();
		}

		%obj.startFade(0, 0, true);
	}
	
	%alpha = mClampF(%alpha-0.025,0,1);

	if(%alpha == 0)
	{
		%obj.HideNode("ALL");
		%obj.stopaudio(0);
		%obj.setmaxforwardspeed(9);
		%obj.isInvisible = true;	
		%obj.reappearsched = %this.schedule(12500, reappear, %obj, 0);
		return;
	}
	else 
	{
		%obj.setNodeColor("ALL","0.05 0.05 0.05" SPC %alpha);
		%obj.setEnergyLevel(%alpha*100);
	}

	%obj.disappearsched = %this.schedule(25, disappear, %obj, %alpha);	
}

function PlayerSkullWolf::onPeggFootstep(%this,%obj)
{
	if(!isObject(%obj) || %obj.getState() $= "Dead" || %obj.isSlow)
	{
		return;
	}
	
	serverplay3d("skullwolf_walking" @ getRandom(1,6) @ "_sound", %obj.getHackPosition());
	%obj.spawnExplosion("Eventide_footstepShakeProjectile", 0.5 + (getRandom() / 2));
}

function PlayerSkullWolf::onImpact(%this, %obj, %col, %vec, %force)
{	
	Parent::onImpact(%this, %obj, %col, %vec, %force);	

	if(%obj.getState() !$= "Dead") 
	{				
		%zvector = getWord(%vec,2);
		if(%zvector > %this.minImpactSpeed)
		{
			%obj.playthread(3,"land");
		}
	}
}

function PlayerSkullWolf::reappear(%this,%obj,%alpha)
{
	// Early return if object is invalid or already disappearing
	if (!isObject(%obj) || isEventPending(%obj.disappearsched)) 
	{
		return;
	}

	// Initial reappearance
	if (%alpha == 0) 
	{
		%obj.unHideNode("ALL");
		%obj.isInvisible = false;
		%obj.playaudio(1, "skullwolf_uncloak_sound");
		%obj.setmaxforwardspeed(6.84);
		%obj.setEnergyLevel(0);
	}

	// Gradually increase visibility
	%alpha = mClampF(%alpha + 0.025, 0, 1);		
	%obj.setNodeColor("ALL", "0.05 0.05 0.05" SPC %alpha);
	%obj.setTempSpeed(0.375);

	// Fully visible state
	if (%alpha == 1) 
	{
		%obj.setTempSpeed(1);
		%obj.unHideNode("ALL");
		
		for(%i = 0; %i < %obj.getMountedObjectCount(); %i++)
		{
			if(isObject(%obj.getMountedObject(%i))) 
			{
				%obj.getMountedObject(%i).unhideNode("ALL");
			}
		}

		//Restore Skullwolf's shadow.
		%obj.startFade(0, 0, false);

		// Create light effect if it doesn't exist
		if(!isObject(%obj.light))
		{
			%obj.light = new fxLight("") 
			{
				dataBlock = %obj.getDataBlock().killerlight; 
			};

			MissionCleanup.add(%obj.light);
			%obj.light.setTransform(%obj.getTransform());
			%obj.light.attachToObject(%obj);
			%obj.light.Player = %obj;
		}

		return;
	}

	// Schedule next reappear step
	%obj.reappearsched = %this.schedule(20, "reappear", %obj, %alpha);	
}

function PlayerSkullWolf::onTrigger(%this,%obj,%triggerNum,%bool)
{
	Parent::onTrigger(%this,%obj,%triggerNum,%bool);

	if(%bool)
	{
		switch(%triggerNum)
		{
			case 0: if(%obj.getEnergyLevel() >= 25) 
				{
					%this.killerMelee(%obj,4.5);
				}
					
			case 4: if(!%obj.isInvisible)
				{		
					if(%obj.getEnergyLevel() == %this.maxEnergy && !isEventPending(%obj.disappearsched)) 
					%this.disappear(%obj,1);
					if(isObject(%obj.lshoe)) %obj.lshoe.hidenode("ALL");
					if(isObject(%obj.rshoe)) %obj.rshoe.hidenode("ALL");	
				}
				else 
				{
					cancel(%obj.reappearsched);
					%this.reappear(%obj,0);
				}
		}		
	}	

}

function PlayerSkullWolf::EventideAppearance(%this,%obj,%client)
{
	if (!isObject(%obj)) 
	{
		return;
	}
	
	if(%obj.isInvisible)
	{
		%obj.startFade(0, 0, true);
	}
	else 
	{
		%obj.startFade(0, 0, false);
	}

	%furcolor = "0.05 0.05 0.05 1";
	%obj.setnodecolor("skullhead",%furcolor);		
	%obj.setnodecolor("rarm",%furcolor);
	%obj.setnodecolor("larm",%furcolor);
	%obj.setnodecolor("rhand",%furcolor);
	%obj.setnodecolor("lhand",%furcolor);
	%obj.setnodecolor("femchest",%furcolor);
	%obj.setnodecolor("pants",%furcolor);
	%obj.setnodecolor("rshoe",%furcolor);
	%obj.setnodecolor("lshoe",%furcolor);
}

function PlayerSkullWolf::onKillerHit(%this,%obj,%hit)
{
	if(%hit.getDamagePercent() >= 0.7125)
	{		
		%obj.mountobject(%hit,9);
		%obj.playthread(1,"eat");
		%obj.setEnergyLevel(%obj.getEnergyLevel()+%this.maxEnergy/6);		
		%obj.schedule(700,spawnExplosion,"goryExplosionProjectile",%obj.getScale());
		%obj.schedule(700,playthread,3,"skullwolf_hit" @ getRandom(1,3) @ "_sound");
		%obj.playaudio(0,"skullwolf_chase" @ getRandom(1,%this.killerchasesoundamount) @ "_sound");
		
		%hit.mountimage("sm_stunimage",2);
		%hit.schedule(695,kill);
		%hit.schedule(705,delete);
		return false;
	}
	return true;
}