//
// Documentation for making a custom Slayer gamemode: https://bitbucket.org/Greek2me/slayer/wiki/Modding#rst-header-creating-a-custom-game-mode
//

//
// Slayer gamemode template.
//

new ScriptGroup(Slayer_GameModeTemplateSG)
{
    // Game mode settings
    className = "Slayer_Eventide";
    uiName = "Eventide";
    useTeams = true;
    isEventide = true;
    // disable_teamCreation = true;

    // Team settings
    teams_minTeams = 2;
    // teams_maxTeams = 4;

    // Default minigame settings
    default_title = "Eventide";
    default_isDefaultMinigame = true;
    default_botDamage = true;
    default_selfDamage = false;
    default_vehicleDamage = true;
    default_fallingDamage = true;
    default_teams_allySameColors = true;
    default_playerDatablock = nameToID("EventidePlayer");
    default_lives = 1;

    default_enableBuilding = false;
    default_enablePainting = false;
    default_enableWand = false;

    default_bKills_enable = false;
    default_clearStats = false;

    default_startEquip0 = 0;
    default_startEquip1 = 0;
    default_startEquip2 = 0;

    // Locked minigame settings
    locked_hostName = "Muna";
    locked_weaponDamage = true;
    locked_chat_enableTeamChat = true;

    //Teams need to be shuffled manually.
    //I'm planning to make a queueing system where everyone get's to play the killer consistently.
    locked_teams_autoSort = false;
    locked_teams_lock = true;
    locked_teams_shuffleTeams = false;

    // Teams
    new ScriptObject()
    {
        // Prevent team from being deleted
        disable_delete = true;
        disable_edit = false;

        default_syncLoadout = true;
        default_uniform = 0;

        // Locked team settings
        locked_name = "Survivors";
        locked_lives = 1;
        locked_color = Slayer_Support::getClosestPaintColor("1 1 1 1"); //White.
        locked_sort = false;
        locked_lock = true;
    };

    new ScriptObject()
    {
        // Prevent team from being deleted
        disable_delete = true;
        disable_edit = false;

        default_syncLoadout = true;
        default_uniform = 0;
        default_playerDatablock = NameToID("PlayerNoJet");

        // Locked team settings
        locked_name = "Hunters";
        locked_lives = 1;
        locked_color = Slayer_Support::getClosestPaintColor("0 0 0 1"); //Black.
        locked_sort = false;
        locked_lock = true;
    };
};
//Custom field to store players who have yet to play Hunter.
$Eventide_HunterQueue = new SimSet(Eventide_HunterQueue);

function Slayer_Eventide::onMinigameReset(%this, %client)
{
    %mini = %this.minigame;
    %survivorTeam = %mini.teams.getTeamFromName("Survivors");
	%hunterTeam = %mini.teams.getTeamFromName("Hunters");

    if(isObject($Eventide_HunterQueue))
    {
        $Eventide_HunterQueue.delete();
    }
    $Eventide_HunterQueue = new SimSet(Eventide_HunterQueue);

    %queuedPlayers = $Eventide_HunterQueue.getCount();
    %numberOfMinigameMembers = %this.numMembers["GameConnection"];

    //First, decide who will be the Hunter, based on who hasn't already played the hunter.
    %selectedHunter = "";
    if(%queuedPlayers == 0)
    {
        //The queue is empty. Fill it up with everyone, then pick a random person to play hunter.
        %selectedHunter = %this.member["GameConnection", getRandom(0, (%numberOfMinigameMembers - 1))];
        for(%i = 0; %i < %numberOfMinigameMembers; %i++)
        {
            %client = %this.member["GameConnection", %i];
            if(%client.getID() != %selectedHunter.getID())
            {
                $Eventide_HunterQueue.add(%client);
            }
        }
    }
    else
    {
        //Some people still haven't played Hunter, decide which one gets to go next.
        %selectedHunter = $Eventide_HunterQueue.getObject(getRandom(0, ($Eventide_HunterQueue.getCount() - 1)));
        $Eventide_HunterQueue.remove(%selectedHunter);
    }

    //Now, assign everyone a new team - Hunter if they got chosen, Survivor if not.
    %oldNotify = %mini.teams_notifyMemberChanges;
	%mini.teams_notifyMemberChanges = false; //Prevent chat message spam as people get assigned to their new team.

    for(%i = 0; %i < %numberOfMinigameMembers; %i++)
    {
        %client = %this.member["GameConnection", %i];
        if(%client.getID() == %selectedHunter.getID())
        {
            %hunterTeam.addMember(%client, "", true);
        }
        else
        {
            %survivorTeam.addMember(%client, "", true);
        }
    }

	%mini.teams_notifyMemberChanges = %oldNotify;
}

function Slayer_Eventide::onRoundStart(%this)
{
    %mini = %this.minigame;
    %mini.assignSurvivorClasses();
}