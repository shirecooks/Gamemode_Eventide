//
// Particle and emitter for the wrench.
//

datablock ParticleData(goldenWrenchSparkleParticle)
{
	dragCoefficient = 4;
	gravityCoefficient = -1;
	inheritedVelFactor = 0;
	constantAcceleration = 0;
	lifetimeMS = 500;
	lifetimeVarianceMS = 250;
	textureName = "./particles/alphaFlare.png";
	spinSpeed = 0;
	spinRandomMin = 0;
	spinRandomMax = 0;
	colors[0] = "0.6 0.45 0 0.6";
	colors[1] = "0.6 0.45 0 0.6";
	colors[2] = "0.6 0.45 0 0.6";
	colors[3] = "0.6 0.45 0 0.6";
	sizes[0] = 0;
	sizes[1] = 2;
	sizes[2] = 0.5;
	sizes[3] = 0.5;
	times[0] = 0;
	times[1] = 0.1;
	times[2] = 0.2;
	times[3] = 1;
	useInvAlpha = true;
};

datablock ParticleEmitterData(goldenWrenchSparkleEmitter)
{
	uiName = "Aged Wrench Sparkle";
	ejectionPeriodMS = 6000;
	periodVarianceMS = 100;
	ejectionVelocity = 0.5;
	ejectionOffset = 0.5;
	thetaMin = 0;
	thetaMax = 180;
	phiReferenceVel = 0;
	phiVariance = 360;
	overrideAdvance = false;
	useEmitterColors = false;
	particles = goldenWrenchSparkleParticle;
};

//
// Explosion data.
datablock ParticleEmitterData(goldenWrenchSparkleExplosionEmitter)
{
	lifeTimeMS = 100;
	ejectionPeriodMS = 8;
	periodVarianceMS = 0;

	ejectionVelocity = 1.0;
	velocityVariance = 0.0;

	ejectionOffset = 1.0; //How far away from the origin point particles can spawn.

	thetaMin = 0; //Particles spawn up.
	thetaMax = 90; //Particles spawn down.

	//Make the particles spawn around the origin point, never on.
	phiReferenceVel = 720;
	phiVariance = 360;

	overrideAdvance = false;
	useEmitterColors = false;
	orientParticles = false;
	particles = goldenWrenchSparkleParticle;
};

datablock ExplosionData(goldenWrenchSparkleExplosion)
{
    explosionShape = "base/data/shapes/empty.dts";

    damageRadius = 0; 
	radiusDamage = 0;
	impulseRadius = 0;
	impulseForce = 0;
	playerBurnTime = 0;

	lifetimeMS = 100;
	emitter[0] = goldenWrenchSparkleExplosionEmitter;

    particleDensity = 5;
    particleRadius = 1.0;
};

datablock ProjectileData(goldenWrenchSparkleExplosionProjectile)
{
    Explosion = goldenWrenchSparkleExplosion;
    explodeOnDeath = 1;

	directDamage = 0;
	impactImpulse = 0;
	verticalImpulse = 0;

	muzzleVelocity = 0;
	velInheritFactor = 1;

	armingDelay = 0;
	lifetime = 0;
	fadeDelay = 70;

	bounceElasticity = 0;
	bounceFriction = 0;
	isBallistic = 0;
	gravityMod = 0;
	hasLight = 0;
};

//
// Item and image.
//

datablock ItemData(goldenWrenchItem)
{
	shapeFile = "./models/goldenWrench/goldenWrench.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = false;
	
	uiName = "Aged Wrench";
	iconName = "base/client/ui/itemIcons/Wrench";
	
	doColorShift = true;
	colorShiftColor = "0.6 0.45 0.0 1.0";
	
	image = goldenWrenchImage;
	canDrop = true;
};

datablock ShapeBaseImageData(goldenWrenchImage : cooldownImage)
{
	shapeFile = goldenWrenchItem.shapeFile;
	emap = false;
	mountPoint = 0;
	isSpecial = true;

	doColorShift = goldenWrenchItem.doColorShift;
	colorShiftColor = goldenWrenchItem.colorShiftColor;
	
	className = "WeaponImage";
	item = goldenWrenchItem;
	
	armReady = true;

    stateName[1] = "Ready";
    stateEmitter[1] = goldenWrenchSparkleEmitter;
	stateEmitterNode[1] = "muzzlePoint";
	stateTimeoutValue[1] = 6.0;
    stateEmitterTime[1] = 5.9;
    stateTransitionOnTimeout[1] = "Loop";
    stateWaitForTimeout[1] = false;
    stateTransitionOnTriggerDown[1] = "Spinning";

    stateName[2] = "Loop";
    stateTransitionOnTimeout[2] = "Ready";
    stateTimeoutValue[2] = 0.01;

    stateName[3] = "Spinning";
    stateScript[3] = "onSpin";
    stateTransitionWaitForTimeout[3] = true;
    stateTimeoutValue[3] = 1.0;
    stateTransitionOnTimeout[3] = "Cast";
    stateAllowImageChange[3] = false;

    stateName[4] = "Cast";
    stateScript[4] = "onCast";
    stateTransitionWaitForTimeout[4] = true;
    stateTimeoutValue[4] = 0.8;
    stateAllowImageChange[4] = false;
    stateTransitionOnTimeout[4] = "CooldownCheck";

    cooldown = 45000;
};
goldenWrenchImage.implementCooldownCallbacks();

function goldenWrenchImage::getHintMessage(%this, %obj)
{
	return "Doesn't sparkle like it used to, but it can still group-heal.";
}

//
// Sequence callbacks - animations and group healing functionality.
//

function goldenWrenchImage::onSpin(%this, %obj)
{
    //Play an animation for raising the wrench and spinning it.
    %obj.setArmThread(spearReady);
	%obj.playThread(1, spearReady);
    for(%i = 1; %i <= 3; %i++)
    {
        %obj.schedule(250 * %i, playThread, 3, rotCW);
    }
}

function goldenWrenchImage::onCast(%this, %obj)
{
    //Cast the wrench down and end the animation.
    %obj.setArmThread(look);
	%obj.playThread(1, spearThrow);

    //Heal outselves for 33% of our health, plus a heal effect.
    %obj.addHealth(%obj.Datablock.maxDamage * 0.33);
    %obj.setWhiteOut(0.5);
    new Projectile()
    {
        datablock = goldenWrenchSparkleExplosionProjectile;
        initialPosition = %obj.getHackPosition();
        initialVelocity = VectorAdd(%obj.getVelocity(), "0 0 0.1");
    }.explode();

    //Heal everyone in a 32-stud radius, as long as they are in view.
    %healer = %obj.getID();
    %position = %obj.getMuzzlePoint(%this.mountPoint);
    %obstructions = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType | $TypeMasks::StaticShapeObjectType | $TypeMasks::TerrainObjectType;

    initContainerRadiusSearch(%position, 16, $TypeMasks::PlayerObjectType);
    while(%patient = containerSearchNext())
    {
        %patientPosition = %patient.getHackPosition();

        //If the player is an enemy or can't be seen, don't heal.
        if(%patent == %healer || minigameCanDamage(%obj, %patient) || containerRayCast(%position, %patientPosition, %obstructions) != 0)
        {
            continue;
        }

        //Spawn a visual heal effect.
        new Projectile()
        {
            datablock = goldenWrenchSparkleExplosionProjectile;
            initialPosition = %patientPosition;
            initialVelocity = VectorAdd(%patient.getVelocity(), "0 0 0.1");
        }.explode();

        //66% of their health.
        %patient.addHealth(%obj.Datablock.maxDamage * 0.66);
        %patient.setWhiteOut(0.5);
    }

    //Begin the cooldown.
    %obj.weaponCooldown(%slot, "The wrench is depleted, and won't recover for " @ sFromMs(%this.cooldown) @ " seconds.", "The golden wrench has channelled more power.", 6);
}