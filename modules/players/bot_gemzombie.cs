datablock PlayerData(GemZombieBot : EventidePlayer)
{
	uiName = "Gem Zombie";
	isKiller = true;
    maxDamage = 45;
	maxForwardSpeed = 5.95;
	maxBackwardSpeed = 3.4;
	maxSideSpeed = 5.1;
};

function GemZombieBot::EventideAppearance(%this,%obj,%client)
{
	%obj.hideNode("ALL");	
}

function GemZombieBot::onNewDatablock(%this,%obj)
{
	Parent::onNewDatablock(%this,%obj);
	
    %this.onBotLoop(%obj);
	%obj.mountImage("ZombieBodyImage",1);
	%obj.mountImage("Blockhead666Image", 2);
}

function GemZombieBot::onBotLoop(%this, %obj)
{
    //hReturnCloseBlockhead code, with a couple of tweaks.
    %type = $TypeMasks::PlayerObjectType;
    %pos = %obj.getPosition();
    %scale = getWord(%obj.getScale(),0);
    %radius = brickToRadius( %obj.hSearchRadius )*%scale;

    initContainerRadiusSearch(%pos,%radius,%type);
    while((%target = containerSearchNext()) != 0)
    {
        %target = %target.getID();

	// take into consideration LOS
	if( %target != %obj && !%target.isCloaked && hLOSCheck( %obj, %target ) )
	{
            if(%target.hType $= "Survivors") //Check the target's team to make sure it's the specific team that needs to be attacked. Not put into the above line for readability.
            {
               // remember to check FOV before continuing
	       if( %obj.hSearchFOV )
	       {
	           if( %obj.hFOVCheck( %target ) )
                   {
                        %obj.hFollowPlayer( %target, 1, 0 ); //Makes the bot attack the target if it's team matches.
                   }
	        }
            }
        }
        else
        {
            if(%target.hType $= "Survivors")
            {
                %obj.hFollowPlayer( %target, 1, 0 );
            }
        }
    }
}