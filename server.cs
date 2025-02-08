$Eventide_loadErrors = new ScriptGroup(Eventide_loadErrors);
%fatalError = false;

//List of required add-ons
%requiredAddOns = "Support_CustomCDN Gamemode_Slayer Event_BrickText Item_Medical Brick_Halloween Server_EnvironmentZones Weapon_Rocket_Launcher Projectile_GravityRocket Weapon_Gun";

function Eventide_storeError(%message)
{
    %error = new ScriptObject() 
    {
        message = %message;
    };
    return %error;
}

//Iterate through the array and check each add-on.
for(%i = 0; %i < getWordCount(%requiredAddOns); %i++) 
{
    %addOn = getWord(%requiredAddOns, %i);
    if(ForceRequiredAddOn(%addOn) == $Error::AddOn_NotFound) 
    {
        %fatalError = true;
        %errorMessage = %addOn @ " is required for Gamemode_Eventide to work.";
        $Eventide_loadErrors.add(Eventide_storeError(%errorMessage));
        error(%errorMessage);
    }
}

//Check for recommended DLLs.
%hasSelectiveGhosting = isFunction(ShapeBase, scopeToClient);
%hasBLPython = isFunction(py_reload_module);
if(!%hasSelectiveGhosting && !%hasBLPython)
{
    %errorMessage = "Gamemode_Eventide requires Selective Ghosting and BLPython installed in your modules folder.";
    $Eventide_loadErrors.add(Eventide_storeError(%errorMessage));
    error(%errorMessage);
}
else if(!%hasSelectiveGhosting)
{
    %errorMessage = "Gamemode_Eventide requires Selective Ghosting installed in your modules folder.";
    $Eventide_loadErrors.add(Eventide_storeError(%errorMessage));
    error(%errorMessage);
}
else if(!%hasBLPython)
{
    %errorMessage = "Gamemode_Eventide requires BLPython installed in your modules folder.";
    $Eventide_loadErrors.add(Eventide_storeError(%errorMessage));
    error(%errorMessage);
}
%dummyClient.delete();

//Temporary, run-once package to print error messages in the chat as the host starts the server.
package Eventide_StartupErrorMessages
{
    function GameConnection::startLoad(%client)
    {
        parent::startLoad(%client);

        %dummyClient = new ScriptObject(dummyClient) //Needed to prevent `chatMessageAll` from throwing an error.
        {
            isSpamming = false;
            spamMessageCount = 0;
        };
        for(%i = 0; %i < $Eventide_loadErrors.getCount(); %i++)
        {
            %error = $Eventide_loadErrors.getObject(%i);
            chatMessageAll(%dummyClient, "ERROR:" SPC %error.message);
        }
        %dummyClient.delete();
        $Eventide_loadErrors.delete();

        deactivatePackage(Eventide_StartupErrorMessages);
    }
};
activatePackage(Eventide_StartupErrorMessages);

//Don't run execute the gamemode scripts if a fatal error was encountered.
if(%fatalError)
{
    return;
}

// Execute essential scripts and preferences first
exec("./prefs.cs");
exec("./modules/scripts/module_scripts.cs");

exec("./modules/misc/module_misc.cs");
exec("./modules/bricks/module_bricks.cs");
exec("./modules/items/module_items.cs");
exec("./modules/players/module_players.cs");
//Needs to be executed after the playertypes have been loaded, so it goes here instead of inn `module_scripts`.
exec("./modules/scripts/script_slayer.cs"); 