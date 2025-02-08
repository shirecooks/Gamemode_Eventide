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

	rightclickicon = "color_grab";
	leftclickicon = "color_melee";	

	rechargeRate = 0.3;
	runForce = 950;
	maxTools = 0;
	maxWeapons = 0;
	maxForwardSpeed = 7.7;
	maxBackwardSpeed = 4.4;
	maxSideSpeed = 6.6;
	jumpForce = 0;
};

function PlayerDisfigured::onTrigger(%this, %obj, %trig, %press) 
{
	PlayerCannibal::onTrigger(%this, %obj, %trig, %press);
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
}

function PlayerDisfigured::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);
	if(%obj.getState() !$= "Dead") %obj.playaudio(0,"disfigured_pain" @ getRandom(1, 1) @ "_sound");
}