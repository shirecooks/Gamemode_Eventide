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
}

//Seconds to milliseconds.
function sFromMs(%milliseconds)
{
    return mCeil(%milliseconds / 1000);
}