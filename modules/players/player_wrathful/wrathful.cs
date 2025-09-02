datablock TSShapeConstructor(WrathfulDts)
{
    baseShape  = "Add-Ons/Player_Wrathful/wrathful.dts";
    sequence0  = "Add-Ons/Player_Wrathful/w_root.dsq root";
    sequence1  = "Add-Ons/Player_Wrathful/w_run.dsq run";
    sequence2  = "Add-Ons/Player_Wrathful/w_walk.dsq walk";
    sequence3  = "Add-Ons/Player_Wrathful/w_back.dsq back";
    sequence4  = "Add-Ons/Player_Wrathful/w_side.dsq side";
    sequence5  = "Add-Ons/Player_Wrathful/w_crouch.dsq crouch";
    sequence6  = "Add-Ons/Player_Wrathful/w_crouchRun.dsq crouchRun";
    sequence7  = "Add-Ons/Player_Wrathful/w_crouchBack.dsq crouchBack";
    sequence8  = "Add-Ons/Player_Wrathful/w_crouchSide.dsq crouchSide";
    sequence9  = "Add-Ons/Player_Wrathful/w_look.dsq look";
    sequence10 = "Add-Ons/Player_Wrathful/w_headside.dsq headside";
    sequence11 = "Add-Ons/Player_Wrathful/w_jump.dsq jump";
    sequence12 = "Add-Ons/Player_Wrathful/w_standJump.dsq standjump";
    sequence13 = "Add-Ons/Player_Wrathful/w_fall.dsq fall";
    sequence14 = "Add-Ons/Player_Wrathful/w_land.dsq land";
    sequence15 = "Add-Ons/Player_Wrathful/w_armAttack.dsq armAttack";
    sequence16 = "Add-Ons/Player_Wrathful/w_armReadyLeft.dsq armReadyLeft";
    sequence17 = "Add-Ons/Player_Wrathful/w_armReadyRight.dsq armReadyRight";
    sequence18 = "Add-Ons/Player_Wrathful/w_armReadyBoth.dsq armReadyBoth";
    sequence19 = "Add-Ons/Player_Wrathful/w_talk.dsq talk";
    sequence20 = "Add-Ons/Player_Wrathful/w_death1.dsq death1";
    sequence21 = "Add-Ons/Player_Wrathful/w_sit.dsq sit";
    sequence22 = "Add-Ons/Player_Wrathful/w_activate.dsq activate";
    sequence23 = "Add-Ons/Player_Wrathful/w_activate2.dsq activate2";
    sequence24 = "Add-Ons/Player_Wrathful/w_leftRecoil.dsq leftrecoil";
    sequence25 = "Add-Ons/Player_Wrathful/w_melee.dsq attack1";
    sequence26 = "Add-ons/Player_Wrathful/w_melee.dsq";
    sequence27 = "Add-ons/Player_Wrathful/w_charge.dsq";
    sequence28 = "Add-ons/Player_Wrathful/w_chargecycle.dsq";
    sequence29 = "Add-ons/Player_Wrathful/w_chargerecovery.dsq";
    sequence30 = "Add-ons/Player_Wrathful/w_handcannon.dsq";
    sequence31 = "Add-ons/Player_Wrathful/w_rage.dsq";
    sequence32 = "Add-ons/Player_Wrathful/w_stomp.dsq";
};    

//sequence11 = "Add-Ons/Player_Wrathful/w_headup.dsq headUp";
///Add-Ons/Player_Wrathful/Wrathful/Shape

datablock PlayerData(PlayerWrathful : PlayerStandardArmor)
{
   shapeFile = WrathfulDts.baseshape;

	uiName = "The Wrathful";
    canjet = false;
   //cameraVerticalOffset = 0.8;
   isEventideModel = true;
	isKiller = true;
    firstPersonOnly = true;
	showEnergyBar = true;
	useCustomPainEffects = true;
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

	rightclickicon = "";
	leftclickicon = "color_melee";
	rightclickspecialicon = "";
	leftclickspecialicon = "";
	
	killerChaseLvl1Music = "musicData_Eventide_WrathfulNear";
	killerChaseLvl2Music = "musicData_Eventide_WrathfulChase";
	
	killeridlesound = "";
	killeridlesoundamount = 5;
	
	killerchasesound = "wrathful_melee";
	killerchasesoundamount = 5;

	killermeleesound = "";
	killermeleesoundamount = 3;    

	killerweaponsound = "wrathful_weapon";
	killerweaponsoundamount = 2;	

	killermeleehitsound = "wrathful_punchHit";
	killermeleehitsoundamount = 1;    

	PainSound = "";
	DeathSound = "";
	JumpSound = "";
	jumpForce = 0;
	
	rechargeRate = 0.375;	
	maxDamage = 1250; //1500
	maxForwardSpeed = 7.7;
	maxBackwardSpeed = 4.4;
	maxSideSpeed = 6.6;
};

function PlayerWrathful::onNewDatablock(%this,%obj)
{
	Parent::onNewDatablock(%this,%obj);
   
    %obj.schedule(1, setEnergyLevel,0);
}

function PlayerWrathful::onPeggFootstep(%this,%obj)
{
	serverplay3d("wrathful_walking" @ getRandom(1,4) @ "_sound", %obj.getHackPosition());
	%obj.spawnExplosion("Eventide_footstepShakeProjectile", 0.5 + (getRandom() / 2));
}

function PlayerWrathful::onTrigger(%this, %obj, %trig, %press) 
{		
	Parent::onTrigger(%this, %obj, %trig, %press);
	
	if(%press) switch(%trig)
	{
		case 0: if(%obj.getEnergyLevel() >= 25)
				{
					%this.killerMelee(%obj, 4);
				}				

		case 4: %this.Rage(%obj);
	}
}

function PlayerWrathful::Rage(%this,%obj)
{
	%obj.playthread(1,w_rage);
	%obj.playaudio(3,"wrathful_rage_sound");
	%obj.setTempSpeed(0);
	%obj.schedule(2000,setTempSpeed,1);
}