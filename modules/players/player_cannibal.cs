datablock PlayerData(PlayerCannibal : PlayerRenowned) 
{
	uiName = "Cannibal Player";
	
	hitprojectile = KillerRoughHitProjectile;
	hitobscureprojectile = "";
	meleetrailskin = "ragged";

	killerChaseLvl1Music = "musicData_Eventide_CannibalNear";
	killerChaseLvl2Music = "musicData_Eventide_CannibalChase";

	killeridlesound = "";
	killeridlesoundamount = 1;

	killerchasesound = "";
	killerchasesoundamount = 1;

	killermeleesound = "";
	killermeleesoundamount = 1;
	
	killerweaponsound = "cannibal_weapon";
	killerweaponsoundamount = 4;	

	killermeleehitsound = "melee_tanto";
	killermeleehitsoundamount = 3;
	
	killerlight = "NoFlarePLight";

	rightclickicon = "color_dash";
	leftclickicon = "color_melee";	

	//maxdamage of 800
	rechargeRate = 0.3;
	maxTools = 0;
	maxWeapons = 0;
	maxForwardSpeed = 7.35;
	maxBackwardSpeed = 4.2;
	maxSideSpeed = 6.3;
	jumpForce = 0;
};

function PlayerCannibal::onTrigger(%this, %obj, %trig, %press)
{
	Parent::onTrigger(%this, %obj, %trig, %press);
		
	if(%press && !%trig && %obj.getEnergyLevel() >= 25)
	{
		%this.killerMelee(%obj,4);
		%obj.faceConfigShowFace("Attack");
		return;
	}
}

function PlayerCannibal::onNewDatablock(%this,%obj)
{
	//Face system functionality.
	%obj.createEmptyFaceConfig($Eventide_FacePacks["cannibal"]);
	%facePack = %obj.faceConfig.getFacePack();
	%obj.faceConfig.face["Neutral"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Neutral"));
	%obj.faceConfig.setFaceAttribute("Neutral", "length", -1);

	%obj.faceConfig.face["Attack"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Attack"));
	%obj.faceConfig.setFaceAttribute("Attack", "length", 500);

	%obj.faceConfig.face["Pain"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Pain"));
	%obj.faceConfig.setFaceAttribute("Pain", "length", 1000);
	
	%obj.faceConfig.face["Blink"] = %facePack.getFaceData(compileFaceDataName(%facePack, "Blink"));
	%obj.faceConfig.setFaceAttribute("Blink", "length", 100);

	//Everything else.
	Parent::onNewDatablock(%this,%obj);
	%obj.mountImage("meleeKnifeImage",0);
	%obj.setScale("1 1 1");
}

function PlayerCannibal::EventideAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");	
	%obj.unhideNode("pants");
	%obj.unhideNode("headskin");
	%obj.unhideNode("larm");
	%obj.unhideNode("rarm");
	%obj.unhideNode("rshoe");
	%obj.unhideNode("lshoe");
	%obj.unhideNode("lhand");
	%obj.unhideNode("rhand");
	%obj.unhideNode("chest");

	%hoodieColor = "0.05 0.05 0.05 1";
	%pantsColor = "0.05 0.05 0.05 1";
	%skinColor = "0.83 0.73 0.66 1";

	%obj.setDecalName("jgtuxedo");
	%obj.setNodeColor("rarm",%hoodieColor);
	%obj.setNodeColor("larm",%hoodieColor);
	%obj.setNodeColor("chest",%hoodieColor);
	%obj.setNodeColor("pants",%pantsColor);
	%obj.setNodeColor("rshoe",%pantsColor);
	%obj.setNodeColor("lshoe",%pantsColor);
	%obj.setNodeColor("rhand",%skinColor);
	%obj.setNodeColor("lhand",%skinColor);
	%obj.setNodeColor("headskin",%skinColor);
	%obj.unhideNode("chest_blood_front");
	%obj.unhideNode("Lhand_blood");
	%obj.unhideNode("Rhand_blood");
	%obj.unhideNode("lshoe_blood");
	%obj.unhideNode("rshoe_blood");
	
	//Set blood colors.
	%obj.setNodeColor("lshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("rshoe_blood", "0.7 0 0 1");
	%obj.setNodeColor("lhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("rhand_blood", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_front", "0.7 0 0 1");
	%obj.setNodeColor("chest_blood_back", "0.7 0 0 1");
}

function PlayerCannibal::onDamage(%this, %obj, %delta)
{
	Parent::onDamage(%this, %obj, %delta);

	if(%obj.getState() !$= "Dead")
	{
		%obj.faceConfigShowFace("Pain");
	}
}