datablock TSShapeConstructor(WrathfulDts)
{
    baseShape  = "./wrathful.dts";
    sequence0  = "./w_root.dsq root";
    sequence1  = "./w_run.dsq run";
    sequence2  = "./w_walk.dsq walk";
    sequence3  = "./w_back.dsq back";
    sequence4  = "./w_side.dsq side";
    sequence5  = "./w_crouch.dsq crouch";
    sequence6  = "./w_crouchRun.dsq crouchRun";
    sequence7  = "./w_crouchBack.dsq crouchBack";
    sequence8  = "./w_crouchSide.dsq crouchSide";
    sequence9  = "./w_look.dsq look";
    sequence10 = "./w_headside.dsq headside";
    sequence11 = "./w_jump.dsq jump";
    sequence12 = "./w_standJump.dsq standjump";
    sequence13 = "./w_fall.dsq fall";
    sequence14 = "./w_land.dsq land";
    sequence15 = "./w_armAttack.dsq armAttack";
    sequence16 = "./w_armReadyLeft.dsq armReadyLeft";
    sequence17 = "./w_armReadyRight.dsq armReadyRight";
    sequence18 = "./w_armReadyBoth.dsq armReadyBoth";
    sequence19 = "./w_talk.dsq talk";
    sequence20 = "./w_death1.dsq death1";
    sequence21 = "./w_sit.dsq sit";
    sequence22 = "./w_activate.dsq activate";
    sequence23 = "./w_activate2.dsq activate2";
    sequence24 = "./w_leftRecoil.dsq leftrecoil";
    sequence25 = "./w_melee.dsq attack1";
    sequence26 = "./w_melee.dsq";
    sequence27 = "./w_charge.dsq";
    sequence28 = "./w_chargecycle.dsq";
    sequence29 = "./w_chargerecovery.dsq";
    sequence30 = "./w_handcannon.dsq";
    sequence31 = "./w_rage.dsq";
    sequence32 = "./w_stomp.dsq";
};    

//sequence11 = "./w_headup.dsq headUp";
///./Wrathful/Shape

datablock PlayerData(PlayerWrathful : PlayerStandardArmor)
{
   shapeFile = WrathfulDts.baseshape;

	uiName = "The Wrathful";
    canjet = false;
   //cameraVerticalOffset = 0.8;
   isEventideModel = false;
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
	
	killerchasesound = "";
	killerchasesoundamount = 3;

	killermeleesound = "wrathful_melee";
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
	if(!isObject(%obj) || %obj.getState() $= "Dead" || %obj.isSlow)
	{
		return;
	}
	
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
	%obj.playthread(1,w_stomp);
	//%obj.schedule(2200,playthread,1,w_root);
	//%obj.playaudio(3,"wrathful_rage_sound");
	//%obj.setTempSpeed(0);
	//%obj.schedule(2200,setTempSpeed,1);
}