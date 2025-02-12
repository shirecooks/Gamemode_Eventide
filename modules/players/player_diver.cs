datablock PlayerData(PlayerDiver : PlayerAngler) 
{
	uiName = "Diver Player";
	shapeFile = EventideplayerDts.baseShape;

	killeridlesound = "";
	killeridlesoundamount = 1;

	killerchasesound = "";
	killerchasesoundamount = 1;	

	killermeleesound = "";
	killermeleesoundamount = 1;
};

function PlayerDiver::onNewDatablock(%this,%obj)
{
	Parent::onNewDatablock(%this,%obj);

	%obj.mountImage("AnglerHookImage",1);
	%obj.schedule(1, setEnergyLevel, 0);
	%obj.setScale("1.2 1.2 1.2");
}

function PlayerDiver::EventideAppearance(%this,%obj,%client)
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
	%obj.unhideNode("tank");
	
	%suitColor = "0.8 0.35 0 1";
	%headColor = "0 0 0 1";
	%shoesColor = "0.075 0.075 0.075 1";
	%tankColor = "0.3 0.3 0.3 1";
	
	%obj.setDecalName("SeaDwealer");
	%obj.setNodeColor("chest",%suitColor);
	%obj.setNodeColor("pants",%suitColor);
	%obj.setNodeColor("Lhand",%suitColor);
	%obj.setNodeColor("Rhand",%suitColor);
	%obj.setNodeColor("Larm",%suitColor);
	%obj.setNodeColor("Rarm",%suitColor);
	%obj.setNodeColor("LShoe",%shoesColor);
	%obj.setNodeColor("RShoe",%shoesColor);
	%obj.setNodeColor("HeadSkin",%headColor);
	%obj.setNodeColor("tank",%tankColor);
}

function PlayerDiver::onImpact(%this, %obj, %col, %vec, %force)
{
	Parent::onImpact(%this, %obj, %col, %vec, %force);	
	
	// Only play the animation if we are moving fast enough and not dead
	if(%obj.getState() !$= "Dead" && getWord(%vec,2) > %this.minImpactSpeed)
	%obj.playthread(3,"land");
}

function PlayerDiver::onTrigger(%this, %obj, %trig, %press) 
{		
	if(%press) switch(%trig)
	{
		case 0: if(%obj.getEnergyLevel() >= 25) return %this.killerMelee(%obj,4); //Only attack if we have enough energy
			
		case 4: if(isObject(%obj.getMountedImage(1)) && %obj.getEnergyLevel() >= %this.maxEnergy)
				{
					serverplay3d("angler_hookcast_sound",%obj.getposition());
					%p = new projectile()
					{
						dataBlock = "AnglerHookProjectile";
						initialVelocity = vectorScale(%obj.getEyeVector(),50);
						initialPosition = %obj.getEyePoint();
						sourceObject = %obj;
						client = %obj.client;
					};

					MissionCleanup.add(%p);
					%obj.unmountImage(1);
					%obj.playthread(2,"leftrecoil");
					%obj.setEnergyLevel(20);

					if(isObject(%obj.hookrope)) %obj.hookrope.delete();
					else
					{
						%hookrope = new StaticShape()
						{
							dataBlock = AnglerHookRope;
							source = %obj;
							end = %p;
						};	
						%obj.hookrope = %hookrope;
						%p.hookrope = %hookrope;
					}								
				}
	}

	Parent::onTrigger(%this, %obj, %trig, %press);	
}

function PlayerDiver::onPeggFootstep(%this,%obj)
{
	serverplay3d("angler_walking" @ getRandom(1,8) @ "_sound", %obj.getHackPosition());
	%obj.spawnExplosion("Eventide_footstepShakeProjectile", 0.5 + (getRandom() / 2));
}

function PlayerDiver::onExitStun(%this, %obj)
{
	%obj.playAudio(1, "angler_enraged_sound");
	%obj.setTempSpeed(1.2);
	%obj.schedule(5000, settempspeed,1);
	%obj.mountImage(HateImage, 2);
}

function PlayerDiver::onKillerHit(%this,%obj,%hit)
{		
	if(isObject(%obj.hookrope))
	{
		%obj.hookrope.delete();
	}
	return true;
}

function AnglerHookRope::onAdd(%this,%obj)
{
	Parent::onAdd(%this,%obj);
	%obj.setNodeColor("ALL","0.5 0.5 0.5 1");
	%this.onHookLoop(%obj);
	MissionCleanup.add(%obj);
}

//Base function originally by Conan, modified by Davow
function AnglerHookRope::onHookLoop(%this,%obj)//General function to pull victims closer
{		
	//Check if the source (killer) and hook are still existing objects and that the killer is not dead
	if(!isObject(%obj) || (!isObject(%source = %obj.source) || %source.getState() $= "Dead") || !isObject(%end = %obj.end))
	{
		if(isObject(%obj)) %obj.delete();
		return;
	}

	//Check if the end attachment is a player and that it isn't dead, return and delete the object if this doesnt pass
	if((%end.getClassName() $= "Player" || %end.getClassName() $= "AIPlayer") && %end.getState() $= "Dead" || %end.getdataBlock().isDowned)
	{
		if(isObject(%obj)) %obj.delete();
		return;
	}

	if(%end.getType() & $TypeMasks::ProjectileObjectType) %endpos = %end.getPosition();// Should only be a projectile when the rope is currently launched
	else if(%end.getType() & $TypeMasks::PlayerObjectType)
	{
		if(vectorDist(%end.getposition(),%source.getposition()) > 2.5)// Adjust the end's velocity to move to the source
		{ 
			if(getWord(%end.getVelocity(), 2) <= 0.75 && !%obj.hitMusic) %end.playthread(1,"activate2");

			%DisSub = vectorSub(%end.getPosition(),%source.getposition());
			%DistanceNormal = vectorNormalize(%DisSub);

			if(getWord(%end.getvelocity(),2) != 0) %force = 15;
			else %force = 10;
			%newvelocity = vectorscale(%DistanceNormal,-%force);

			if(vectorDist(%end.gethackposition(),%source.gethackposition()) > 5)
			%zinfluence = getWord(%end.getVelocity(), 2) + getWord(%newvelocity, 2)/10;
			else %zinfluence = getWord(%end.getVelocity(), 2);

			%end.setVelocity(getWords(%newvelocity, 0, 1) SPC %zinfluence);

			if(%end.lastchokecough+getrandom(250,500) < getsimtime())// Originally part of the L4B Smoker, just some sounds
			{			
				%source.playaudio(0,"" @ getRandom(0,2) @ "_sound");
				%source.playthread(2,"leftrecoil");
				%source.playthread(3,"jump");
				%end.lastchokecough = getsimtime();

				if(getWord(%end.getVelocity(), 2) >= 0.75)//If victim is being lifted up for too long, this function will eventually begin to damage the victim
				{					
					if(%source.lastdamage+getRandom(500,100) < getsimtime())
					{
						%source.ChokeUpCount++;				
						%source.playthread(3,"plant");
						%source.playthread(2,"Shiftup");
						%source.lastdamage = getsimtime();
					}

					if(%source.ChokeUpCount > 2)
					{						
						%end.playthread(2,"plant");								
						%end.damage(%source, %end.getposition(), %end.getdataBlock().maxDamage/12.5, $DamageType::sourceConstrict);		
					}
				}
			}			
		}
		%endpos = vectorSub(%end.getmuzzlePoint(2),"0 0 0.2");
	} 

	%head = %source.getmuzzlePoint(1);
	%vector = vectorNormalize(vectorSub(%endpos,%head));
	%relative = "0 1 0";
	%xyz = vectorNormalize(vectorCross(%relative,%vector));
	%u = mACos(vectorDot(%relative,%vector)) * -1;
	%obj.setTransform(vectorScale(vectorAdd(vectorAdd(%head,"0 0 0.5"),%endpos),0.5) SPC %xyz SPC %u);
	%obj.setScale(0.5 SPC vectorDist(%head,%endpos) * 2 SPC 0.5);

	%obj.HookLoop = %this.schedule(33,onHookLoop,%obj);
}

function AnglerHookRope::onRemove(%this,%obj)
{		
	if(isObject(%source = %obj.source)) %source.mountImage("AnglerHookImage",1);
	if(isObject(%end = %obj.end)) %end.ChokeUpCount = 0;
}

function AnglerHookProjectile::onCollision(%this,%proj,%col,%fade,%pos,%normal)
{
	%obj = %proj.sourceObject;
	if(!isObject(%obj.hookrope)) return;
	
	if((%col.getType() & $TypeMasks::PlayerObjectType) && minigameCanDamage(%obj,%col) == 1)
	{
		if(%col.getdataBlock().isDowned) return;
		
		%col.dismount();
		%obj.hookrope.end = %col;
		%col.playaudio(3,"angler_hookCatch_sound");
		%col.damage(%obj, %pos, 10, $DamageType::Default);
		%obj.hookrope.schedule(1750,"delete");
		return;
	}
	else %obj.hookrope.delete();

	Parent::onCollision(%this,%proj,%col,%fade,%pos,%normal);
}