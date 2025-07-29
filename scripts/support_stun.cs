//
// Emitters and particle effects.
//

datablock ParticleData(stunParticle)
{
	dragCoefficient      = 13;
	gravityCoefficient   = 0.2;
	inheritedVelFactor   = 1.0;
	constantAcceleration = 0.0;
	lifetimeMS           = 400;
	lifetimeVarianceMS   = 0;
	textureName          = "base/data/particles/star1";
	spinSpeed		   = 0.0;
	spinRandomMin		= 0.0;
	spinRandomMax		= 0.0;
	colors[0]     = "1 1 0.2 0.9";
	colors[1]     = "1 1 0.4 0.5";
	colors[2]     = "1 1 0.5 0";

	sizes[0]      = 0.5;
	sizes[1]      = 0.2;
	sizes[2]      = 0.1;

	times[0] = 0.0;
	times[1] = 0.5;
	times[2] = 1.0;

	useInvAlpha = false;
};

datablock ParticleEmitterData(stunEmitter)
{
	ejectionPeriodMS = 12;
	periodVarianceMS = 1;
	ejectionVelocity = 5.25;
	velocityVariance = 0.0;
	ejectionOffset   = 0.25;
	thetaMin         = 0;
	thetaMax         = 180;
	phiReferenceVel  = 0;
	phiVariance      = 360;
	overrideAdvance = false;
	particles = stunParticle;
};

//
// Image and stun mechanics.
//

datablock ShapeBaseImageData(stunImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = $HeadSlot;
    mountSlot = 3;
	offset = "0 0 0.4";
	eyeOffset = "0 0 999";

	stateName[0]				= "Ready";
	stateTimeoutValue[0]		= 0.01;
	stateTransitionOnTimeout[0]	= "FireA";

	stateName[1]				= "FireA";
	stateEmitter[1]				= stunEmitter;
	stateEmitterTime[1]			= 1.2;
	stateTimeoutValue[1]		= 1.2;
	stateTransitionOnTimeout[1]	= "Done";
	stateWaitForTimeout[1]		= true;

	stateName[2]				= "Done";
	stateTimeoutValue[2]		= 0.01;
	stateTransitionOnTimeout[2]	= "FireA";
};

function stunImage::onMount(%this, %obj)
{
    //Mark the player as stunned.
    %obj.stunned = 1;

    //Play the stun animation.
	%obj.setActionThread("sit", 1);
	
    //Take control away from the player while they're stunned.
	switch$(%obj.getclassName())
	{
		case "Player": 	
            %obj.client.setControlObject(%obj.client.camera);
			%obj.client.camera.setMode("Corpse",%obj);
            %playerDatablock = %obj.getDatablock();
			if(isFunction(%playerDatablock, onEnterStun))
            {
                %playerDatablock.onEnterStun(%obj);
            }
		case "AIPlayer": 
            %obj.stopholeloop();
	}
}

function stunImage::onUnMount(%this, %obj)
{
    //Mark the player as no longer being stunned.
	%obj.stunned = 0;

    //Undo the stun animation.
	%obj.playThread(3, "undo");
	%obj.setActionThread("root", 1);

    //Restore the player's control of their character.
	switch$(%obj.getClassName())
	{
		case "Player": 	
            %obj.client.setControlObject(%obj);
			%obj.client.camera.setMode("Observer");
            %playerDatablock = %obj.getDatablock();
            if(isFunction(%playerDatablock, onExitStun))
            {
                %playerDatablock.onExitStun(%obj);
            }
		case "AIPlayer": 
            %obj.startHoleLoop();
	}
}

//
// Player helper functions.
//

function Player::stun(%obj, %time)
{
    //Have the stun time default to 2.5 seconds.
    if(%time $= "")
    {
        %time = 2500;
    }

    %stunImageSlot = stunImage.mountSlot;

    //Have the player enter the stun.
    %obj.mountImage(stunImage, %stunImageSlot);

    //Schedule the player to exit the stun.
    cancel(%obj.stunCancelSchedule);
    %obj.stunCancelSchedule = %obj.schedule(%time, unmountImage, %stunImageSlot);
}