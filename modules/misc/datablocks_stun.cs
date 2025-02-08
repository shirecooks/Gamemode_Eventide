datablock ParticleData(sm_stunParticle)
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
datablock ParticleEmitterData(sm_stunEmitter)
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
	particles = sm_stunParticle;
};
datablock ShapeBaseImageData(sm_stunImage)
{
	shapeFile = "base/data/shapes/empty.dts";
	emap = false;

	mountPoint = $HeadSlot;
	offset = "0 0 0.4";
	eyeOffset = "0 0 999";

	stateName[0]				= "Ready";
	stateTimeoutValue[0]		= 0.01;
	stateTransitionOnTimeout[0]	= "FireA";

	stateName[1]				= "FireA";
	stateEmitter[1]				= sm_stunEmitter;
	stateEmitterTime[1]			= 1.2;
	stateTimeoutValue[1]		= 1.2;
	stateTransitionOnTimeout[1]	= "Done";
	stateWaitForTimeout[1]		= true;

	stateName[2]				= "Done";
	stateTimeoutValue[2]		= 0.01;
	stateTransitionOnTimeout[2]	= "FireA";
};

function sm_stunImage::onMount(%this,%obj)
{
	%obj.schedule(2500,unmountImage,3);
	%obj.setactionthread("sit",1);
	%obj.stunned = 1;
	%playerDatablock = %obj.getDatablock();

	switch$(%obj.getclassName())
	{
		case "Player": 	%obj.client.setControlObject(%obj.client.camera);
						%obj.client.camera.setMode("Corpse",%obj);
						%playerDatablock.onEnterStun(%obj);
		case "AIPlayer": %obj.stopholeloop();
	}
}

function sm_stunImage::onunMount(%this,%obj)
{
	%obj.stunned = 0;
	%obj.playThread(3,"undo");
	%obj.setactionthread("root",1);
	%playerDatablock = %obj.getDatablock();
	
	switch$(%obj.getclassName())
	{
		case "Player": 	%obj.client.setControlObject(%obj);
						%obj.client.camera.setMode("Observer");
						%playerDatablock.onExitStun(%obj);
		case "AIPlayer": %obj.startholeloop();
	}
}