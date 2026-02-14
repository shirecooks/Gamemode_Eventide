//Minimum and maximum integer values supported by TGE.
$minInt = -2147483648;
$maxInt = 2147483647;

function cloneScriptObject(%targetObject)
{
    %targetObjectName = %targetObject.getName();
    %targetObject.setName("targetScriptObject");

    %cloneObject = new ScriptObject(cloneScriptObject : targetScriptObject);

    %targetObject.setName(%targetObjectName);
    %cloneObject.setName("");

    return %cloneObject;
}

function getFileString(%pattern)
{
    %fileString = "";
    for(%file = findFirstFile(%pattern); %file !$= ""; %file = findNextFile(%pattern))
    {
        if(%fileString $= "")
        {
            %fileString = %file;
        }
        else
        {
            %fileString = %fileString TAB %file;
        }
    }

    return %fileString;
}

function pushServerPackageToBack(%package) 
{
    $Con::logBufferEnabled = 0;
    enableWinConsole(0);

	//Make sure a package certain package on a function gets called last. In this case, the flashlight override.
	for(%i = getNumActivePackages() - 1; %i >= $numClientPackages; %i--) 
	{
		%current = getActivePackage(%i);
		if(%current !$= %package) 
		{
			%stack = ltrim(%stack SPC %current);
			deactivatePackage(%current);
		}
	}

	if(%stack !$= "") 
	{
		for(%i = getWordCount(%stack) - 1; %i >= 0; %i--) 
		{
			activatePackage(getWord(%stack, %i));
		}
	}

    $Con::logBufferEnabled = 1;
    enableWinConsole($Server::Dedicated);
}

//Seconds to milliseconds.
function sFromMs(%milliseconds)
{
    return mCeil(%milliseconds / 1000);
}

function VectorToEuler(%vec) 
{
	%vec = vectorNormalize(%vec);
	%yaw   = mRadToDeg(mATan(getWord(%vec, 0), getWord(%vec, 1)));
	%pitch = mRadToDeg(mASin(getWord(%vec, 2)));
	return %pitch SPC 0 SPC %yaw;
}

$SessionSeed = getRandom($minInt, $maxInt);
function generateUniqueIdentifier()
{
    return sha1($SessionSeed @ getRealTime() @ getRandom($minInt, $maxInt) @ getRandom($minInt, $maxInt));
}

//Allows Projectiles to execute datablock-based code on creation.
package Support_Common
{
    function Projectile::onAdd(%obj)
    {
        parent::onAdd(%obj);

        %datablock = %obj.Datablock;
        if(isFunction(%datablock, "onAdd"))
        {
            %datablock.onAdd(%obj);
        }
    }

    function destroyServer()
    {
        deleteVariables("$Eventide_*");
        Parent::destroyServer();
    }
};
if(isPackage(Support_Common))
{
    deactivatePackage(Support_Common);
}
activatePackage(Support_Common);