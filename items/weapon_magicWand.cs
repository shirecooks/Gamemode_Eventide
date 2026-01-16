datablock ParticleData(wandGreenSparkleParticle)
{
  dragCoefficient       = 3;
  gravityCoefficient    = 0.2;
  inheritedVelFactor    = 0.15; 
  constantAcceleration  = 0.0;
  
  lifetimeMS            = 500; 
  lifetimeVarianceMS    = 100;  
  
  textureName           = "base/data/particles/star1";
  
  spinSpeed             = 50;  
  spinRandomMin         = -150.0; 
  spinRandomMax         = 150.0; 
  
  colors[0]             = "0 0.5 0.1";                      
  colors[1]             = "0 0.6 0.2";
  colors[2]             = "0 0.8 0.4";
  colors[3]             = "0 0.8 0.4";
  
  sizes[0]              = 0.1;
  sizes[1]              = 0.1;
  sizes[2]              = 0.2;
  sizes[3]              = 0.3;
  
  times[0]              = 0.0;
  times[1]              = 0.3;
  times[2]              = 0.6;
  times[3]              = 0.9;
  
  useInvAlpha           = false; 
};

datablock ParticleEmitterData(wandGreenSparkleEmitter)
{
  lifetimeMS        = 2500;
  ejectionPeriodMS  = 8;
  periodVarianceMS  = 2;   
  
  ejectionVelocity  = 1.0;
  velocityVariance  = 0.0;  
  
  ejectionOffset    = 0; 
  thetaMin          = 0;
  thetaMax          = 90;
   
  phiReferenceVel   = 0; 
  phiVariance       = 360;
   
  overrideAdvance   = false; 
  useEmitterColors  = false;     
  orientParticles   = false;
  
  particles         = wandGreenSparkleParticle;
};

//Explosions and Projectiles
datablock ExplosionData(MagicWandExplosion)
{
  lifeTimeMS = 500;  
  
  soundProfile = "magic_hit_sound";
  
  particleEmitter = wandGreenSparkleEmitter;
  particleDensity = 50;
  particleRadius = 1; 
  
  emitter[0] = wandGreenSparkleEmitter;
  emitter[1] = wandGreenSparkleEmitter;  
  
  shakeCamera = false;
  camShakeFreq = "10.0 11.0 10.0";  
  camShakeAmp = "1.0 1.0 1.0";
  camShakeDuration = 0.5; 
  camShakeRadius = 10.0; 
  camShakeFallOff = 1.0;
  
  faceViewer     = false;    
  explosionScale = "1 1 1";   
  damageRadius = 3;  
  radiusDamage = 20; 
  impulseRadius = 4; 
  impulseForce = 1500;
  
  playerBurnTime = 0;
  
  lightStartRadius = 4;
  lightEndRadius = 3;
  lightStartColor = "0.0 0.4 0.2";
  lightEndColor = "0.1 0.8 0.2"; 
  lightHasCorona = true;
};

AddDamageType("MagicWand",'<bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_wand> %1', '%2 <bitmap:Add-Ons/Gamemode_Eventide/items/icons/ci_wand> %1', 0.75, 1);

datablock ProjectileData(wandGreenProjectile)
{   
  directDamage        = 40;
  directDamageType = $DamageType::MagicWand; 
  radiusDamageType = $DamageType::MagicWand;
  
  impactImpulse      = 1000; 
  verticalImpulse      = 1000;
   
  explosion           = MagicWandExplosion; 
  particleEmitter     = wandGreenSparkleEmitter; 
  waterExplosion      = MagicWandExplosion; 
  particleWaterEmitter= wandGreenSparkleEmitter;
  
  muzzleVelocity      = 30;    
  velInheritFactor    = 0.3;
    
  armingDelay         = 0;
  lifetime            = 2500;   
  fadeDelay           = 10; 
  
  bounceElasticity    = 0;  
  bounceFriction      = 0; 
  
  isBallistic         = true; 
  gravityMod = 2; 
  
  Explodeondeath = true; 
  
  hasLight    = true; 
  lightRadius = 4; 
  lightColor  = "0.1 0.8 0.2"; 
  hasWaterLight     = true;  
  waterLightColor   = "0.1 0.8 0.2";  
  lightHasCorona = true;
};

//Item
datablock ItemData(MagicWandItem)
{
  category = "Tools";
  className = "Item";

	shapeFile = "base/data/shapes/wand.dts";
  mass = 1;
  density = 0.2;
  elasticity = 0.2;
  friction = 0.6;
  emap = true;

  uiName = "Magic Wand";
  iconName = "";
  doColorShift = false;
  colorShiftColor = "0 0.6 0";
  
  image = MagicWandImage;
  canDrop = true;
};

//Image
datablock ShapeBaseImageData(MagicWandImage)
{
  shapeFile = "base/data/shapes/wand.dts";
  scale = "2 0.5 0.77";
  emap = true;

  mountPoint = 0;
  offset = "0 0 0";

  correctMuzzleVector = false;

  className = "WeaponImage";

  item = MagicWandItem;
  projectile = wandGreenProjectile;
  projectileType = Projectile;

  melee = false;
  doRetraction = false;

  armReady = true;

  doColorShift = false;
  colorShiftColor = MagicWandItem.colorShiftColor;

  stateName[0]                     = "Activate";
  stateTimeoutValue[0]             = 0.15;
  stateTransitionOnTimeout[0]      = "Ready";
  stateScript[0]                   = "onActivate"; 

  stateName[1]                     = "Ready";
  stateTransitionOnTriggerDown[1]  = "Fire";
  stateAllowImageChange[1]         = true;
  stateSequence[1]	= "Ready";

  stateName[2]                     = "Fire";
  stateTransitionOnTriggerUp[2]    = "CoolDown";
  stateFire[2]                     = true;
  stateAllowImageChange[2]         = false;
  stateSequence[2]                 = "Fire";
  stateScript[2]                   = "onFire";
  stateSound[2]                    = MagicWandFire;
  stateWaitForTimeout[2]           = true; 

  stateName[3]                     = "Cooldown";
  stateTransitionOnTimeout[3]      = "Ready";
  stateTimeoutValue[3]             = 0.8;
  stateAllowImageChange[3]         = false;
  stateWaitForTimeout[3]           = true;
};