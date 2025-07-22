datablock ParticleData(Disfigured_BleedParticle)
{
   dragCoefficient = 3;
   gravityCoefficient = 0.5;
   inheritedVelFactor = 0.3;
   constantAcceleration = 0;
   lifetimeMS         = 100;
   lifetimeVarianceMS = 50;
   textureName = "base/data/particles/cloud";
   spinSpeed     = 0;
   spinRandomMin = -20;
   spinRandomMax = 20;
   colors[0] = "0.6 0 0 1";
   colors[1] = "0.5 0 0 0.3 ";
   colors[2] = "0.4 0 0 0";
   sizes[0] = 0.12;
   sizes[1] = 0.4;
   sizes[2] = 0.08;
   times[1] = 0.5;
   times[2] = 1;
   useInvAlpha = true;
};

datablock ParticleEmitterData(Disfigured_BleedEmitter)
{
   ejectionPeriodMS = 50;
   periodVarianceMS = 0;
   ejectionVelocity = 1;
   velocityVariance = 0;
   ejectionOffset   = 0;
   thetaMin = 0;
   thetaMax = 180;
   phiReferenceVel = 0;
   phiVariance     = 360;
   overrideAdvance = false;
   particles = "Disfigured_BleedParticle";

   uiName = "";
};

datablock ShapeBaseImageData(Disfigured_BleedImage) 
{
	shapeFile			= "base/data/shapes/empty.dts";
	mountPoint			= 2;
	offset = "-0.5 0.05 -0.45";
	correctMuzzleVector	= false;
	stateName[0]				= "Disfigured_Bleed";
	stateEmitter[0]				= Disfigured_BleedEmitter;
	stateEmitterTime[0]			= 1000;
	stateWaitForTimeout[0]		= true;
	stateTimeoutValue[0]		= 1000;
	stateTransitionOnTimeout[0]	= "Disfigured_Bleed";
	stateScript[0]				= "onDisfigured_Bleed";
};

datablock ShapeBaseImageData(Disfigured_FogImage) 
{
	shapeFile			= "base/data/shapes/empty.dts";
	mountPoint			= 2;
	offset = "0 -0.5 -0.25";
	correctMuzzleVector	= false;
	stateName[0]				= "Fog";
	stateEmitter[0]				= FogEmitter;
	stateEmitterTime[0]			= 1000;
	stateWaitForTimeout[0]		= true;
	stateTimeoutValue[0]		= 1000;
	stateTransitionOnTimeout[0]	= "Fog";
	stateScript[0]				= "onFog";
};

datablock PlayerData(PlayerDisfigured : PlayerRenowned) 
{
	uiName = "Disfigured Player";
	enablePeggFootsteps = false;
	
	hitprojectile = KillerRoughHitProjectile;
	hitobscureprojectile = "";
	meleetrailskin = "magic";

	killerChaseLvl1Music = "musicData_Eventide_DisfiguredNear";
	killerChaseLvl2Music = "musicData_Eventide_DisfiguredChase";

	killeridlesound = "disfigured_idle";
	killeridlesoundamount = 5;

	killerchasesound = "disfigured_idle";
	killerchasesoundamount = 5;

	killermeleesound = "disfigured_attack";
	killermeleesoundamount = 3;
	
	killerweaponsound = "disfigured_weapon";
	killerweaponsoundamount = 4;	

	killermeleehitsound = "melee_tanto";
	killermeleehitsoundamount = 3;
	
	killerlight = "NoFlarePLight";

	rightclickicon = "color_dash";
	leftclickicon = "color_melee";	

	rechargeRate = 0.3;
	runForce = 950;
	maxDamage = 818; //1000
	maxTools = 0;
	maxWeapons = 0;
	maxForwardSpeed = 7.7;
	maxBackwardSpeed = 4.4;
	maxSideSpeed = 6.6;
	jumpForce = 0;
	minimpactspeed = 10;
};

function PlayerDisfigured::killerGUI(%this,%obj,%client)
{	
	%energylevel = %obj.getEnergyLevel();

	// Some dynamic varirables
	%leftclickstatus = (%obj.getEnergyLevel() >= %this.maxEnergy/4) ? "hi" : "lo";
	%rightclickstatus = (%obj.getEnergyLevel() >= %this.maxEnergy/2) ? "hi" : "lo";
	%leftclicktext = (%this.leftclickicon !$= "") ? "<just:left>\c6Left click" : "";
	%rightclicktext = (%this.rightclickicon !$= "") ? "<just:right>\c6Right click" : "";

	// Regular icons
	%leftclickicon = (%this.leftclickicon !$= "") ? "<just:left><bitmap:" @ $iconspath @ %leftclickstatus @ %this.leftclickicon @ ">" : "";
	%rightclickicon = (%this.rightclickicon !$= "") ? "<just:right><bitmap:" @ $iconspath @ %rightclickstatus @ %This.rightclickicon @ ">" : "";
	
	if(%obj.getDataBlock() $= %this)
	{
		%client.bottomprint(%leftclicktext @ %rightclicktext @ "<br>" @ %leftclickicon @ %rightclickicon, 1);
	}
	else 
	{
		%client.bottomprint(%rightclicktext @ "<br>" @ %rightclickicon, 1);
	}
}

function PlayerDisfigured::onTrigger(%this, %obj, %trig, %press) 
{
	Parent::onTrigger(%this, %obj, %trig, %press);
		
	if(%press)
	{
		switch(%trig)
		{
			case 0:	if(%obj.getEnergyLevel() >= 25)
					{
						%this.killerMelee(%obj,4);
						return;
					}
			
			case 4: if(%obj.getEnergyLevel() >= %this.maxEnergy/2)
					{
						%obj.setEnergyLevel(%obj.getEnergyLevel()-50);
						%obj.setVelocity(VectorScale(%obj.getForwardVector(),18));
						
						%soundpitch = getRandom(80,150);
						%obj.spawnExplosion("pushBroomProjectile","0.5 0.5 0.5");
						$oldTimescale = getTimescale();
						setTimescale((%soundpitch*0.01) * $oldTimescale);
						serverPlay3D("disfigured_dash_sound",%obj.getHackPosition());
						setTimescale($oldTimescale);
					}
					
		}
		
	}
}

function PlayerDisfigured::onPeggFootstep(%this,%obj)
{
	serverplay3d("huntress_walking" @ getRandom(1,6) @ "_sound", %obj.getHackPosition());	
}

function PlayerDisfigured::onNewDatablock(%this,%obj)
{
	//Face system functionality.
	%obj.createEmptyFaceConfig($Eventide_FacePacks["disfigured"]);
	%facePack = %obj.faceConfig.getFacePack();
	%obj.faceConfig.face["Neutral"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Neutral"));
	%obj.faceConfig.setFaceAttribute("Neutral", "length", -1);
	
	Parent::onNewDatablock(%this,%obj);

	%obj.setScale("1.1 1.1 1.1");
	%obj.mountImage("Disfigured_BleedImage",0);
	%obj.mountImage("Disfigured_FogImage",1);	
}

function PlayerDisfigured::EventideAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");	
	%obj.unhideNode("skirt");
	%obj.unhideNode("headskin");
	%obj.unhideNode("larmslim");
	%obj.unhideNode("rarm");
	%obj.unhideNode("rhand");
	%obj.unhideNode("femchest");

	%dressColor = "0.2 0.2 0.2 0.6";
	%skinColor = "0.63 0.71 1 0.6";
	%bloodColor = "0.36 0.07 0.07 0.6";

	if(isObject(%obj.faceConfig))
	{
		%obj.faceConfigShowFaceTimed("Neutral", -1);
	}
	%obj.setDecalName("disfigureddecal");
	%obj.setNodeColor("rarm",%skinColor);
	%obj.setNodeColor("larmslim",%bloodColor);
	%obj.setNodeColor("femchest",%dressColor);
	%obj.setNodeColor("skirt",%dressColor);
	%obj.setNodeColor("rhand",%skinColor);
	%obj.setNodeColor("lhand",%skinColor);
	%obj.setNodeColor("headskin",%skinColor);
	%obj.startFade(0, 0, true);

	if(isObject(%obj.lshoe)) %obj.lshoe.delete();
	if(isObject(%obj.rshoe)) %obj.rshoe.delete();
}

function PlayerDisfigured::onImpact(%this, %obj, %col, %vec, %force)
{			
	if(%force > %this.minImpactSpeed) %obj.spawnExplosion("pushBroomProjectile","0.5 0.5 0.5");
	
	if(isObject(%col) && (%col.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj,%col))
	{
		%hitforce = %force*1.5;
		%damage = %force*2.5;
		
		if(getRandom(1,15) == 1)// Batter up!
		{
			%hitforce = %force*4;
			%damage = %force*3;

			%soundpitch = getRandom(90,150);
			$oldTimescale = getTimescale();
			setTimescale((%soundpitch*0.01) * $oldTimescale);
			serverPlay3D("puzzlechime_sound",%col.getPosition());
			setTimescale($oldTimescale);
		}

		if(%col.getDataBlock().isDowned)
		{
			%damage = 0;
			%hitforce *= 0.5;
		}
		
		%col.setVelocity(VectorScale(vectorAdd(%obj.getForwardVector(),"0 0 0.25"), %hitforce));
		%col.damage(%obj, %col.getHackPosition(), %damage, $DamageType::Default);

		%soundpitch = getRandom(80,175);
		$oldTimescale = getTimescale();
		setTimescale((%soundpitch*0.01) * $oldTimescale);
		serverPlay3D("melee_axe" @ getRandom(1,2) @ "_sound",%col.getPosition());
		setTimescale($oldTimescale);

		%col.spawnExplosion("pushBroomProjectile",vectorScale(%obj.getScale(),getRandom(1,2)));
		if(%this.hitprojectile !$= "")
		{
			%effect = new Projectile()
			{
				dataBlock = %this.hitprojectile;
				initialPosition = %col.getHackPosition();
				initialVelocity = vectorNormalize(vectorSub(%col.getHackPosition(), %obj.getEyePoint()));
				scale = vectorScale(%obj.getScale(),getRandom(2,3));
				sourceObject = %obj;
			};

			MissionCleanup.add(%effect);
			%effect.explode();
		}

		return;
	}
}

function PlayerDisfigured::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);
	if(%obj.getState() !$= "Dead") %obj.playaudio(0,"disfigured_pain" @ getRandom(1, 1) @ "_sound");
}