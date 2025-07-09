//Not used for anything currently, but it might at some point.
function SimObject::isSubclassOf(%this, %parentDatablock)
{
    %datablock = %this.getDatablock();
    while(isObject(%datablock))
    {
        if(%datablock == %parentDatablock)
        {
            return true;
        }
        else
        {
            %datablock = %datablock.superClass;
        }
    }

    return false;
}

//Call a parent class function with arbitrary arguments.
//Take advantage of the fact that everything in TorqueScript is a string, so nothing can be lost by presenting all arguments as a string.
$OOP_recursionDepth = 0;
function SimObject::super(%this, %function, %v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %v10, %v11, %v12, %v13, %v14, %v15, %v16, %v17)
{
    //Calling a superclass function that has another call to this function results in an infinite loop. We can stop this by measuring recursion depth.
    $OOP_recursionDepth++;

    //Recursively obtain a superclass, if necessary.
    %superClass = %this.superClass;
    if($OOP_recursionDepth > 1)
    {
        for(%i = 1; %i < $OOP_recursionDepth; %i++)
        {
            %superClass = %superClass.superClass;
            if(%superClass $= "")
            {
                return;
            }
        }
    }

    //SuperClass::function(%v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %v10, %v11, %v12, %v13, %v14, %v15, %v16, %v17);
    %returnValue = eval(%superClass @ "::" @ %function @ "(\""@ %v0 @"\",\""@ %v1 @"\",\""@ %v2 @"\",\""@ %v3 @"\",\""@ %v4 @"\",\""@ %v5 @"\",\""@ %v6 @"\",\""@ %v7 @"\",\""@ %v8 @"\",\""@ %v9 @"\",\""@ %v10 @"\",\""@ %v11 @"\",\""@ %v12 @"\",\""@ %v13 @"\",\""@ %v14 @"\",\""@ %v15 @"\",\""@ %v16 @"\",\""@ %v17 @"\");");
    
    //An iteration reaching this line has hit the end of the chain, so we can simply zero out the recursion depth.
    $OOP_recursionDepth--;
    
    return %returnValue;
}

//To be run on datablocks or ScriptObjects. 
//Take advantage of `eval` and the lax-arity function model of TorqueScript to define a function that calls the equivalent parent function.
function SimObject::inheritFunctionsFromSuperClass(%this)
{
    %superClass = %this.superClass;

    %superClassID = nameToID(%this.superClass);
    if(!isObject(%superClassID))
    {
        return;
    }

    //The only way to determine the functions of an object is through the `dump()` method of SimObjects.
    %functionList = "";
    %introspectLogLocation = "config/introspect.log";

    //Delete the file if it exists already, to prevent interference.
    if(isFile(%introspectLogLocation))
    {
        fileDelete(%introspectLogLocation);
    }

    %objectIntrospectLogger = new ConsoleLogger(objectIntrospectLogger, %introspectLogLocation, false);
    %objectIntrospectLogger.level = 0;
    %objectIntrospectLogger.attach();
    %superClassID.dump();
    %objectIntrospectLogger.detach();
    %objectIntrospectLogger.delete();

    %introspectLog = new FileObject();
    %readingMethod = false;
    %lastLine = "";
    %introspectLog.openForRead(%introspectLogLocation);
    while(!%introspectLog.isEOF())
    {
        %currentLine = trim(%introspectLog.readLine());

        //ConsoleLogger objects have a bug where they output a single line repeatedly, so we need to compensate for that by disregarding duplicate lines.
        if(%currentLine $= %lastLine)
        {
            continue;
        }

        //We reached the methods section, prepare to evaluate the following lines.
        if(%currentLine $= "Methods:")
        {
            %readingMethod = true;
            %lastLine = %currentLine;
            continue;
        }
        else if(!%readingMethod)
        {
            //We aren't reading a method, so don't do anything.
            %lastLine = %currentLine;
            continue;
        }

        //Add the method to a list, which will be auto-generated into a stub in the for-loop below.
        %methodName = getWord(%currentLine, 0);
        %methodName = getSubStr(%methodName, 0, (strlen(%methodName) - 2));

        //Some functions cannot be overwritten, ignore those.
        if(%methodName $= "delete" || %methodName $= "dump" || %methodName $= "getId" || %methodName $= "call" || %methodName $= "setName" || %methodName $= "getTaggedField" || %methodName $= "getType" || %methodName $= "serializeEventToString")
        {
            continue;
        }

        if(%functionList $= "")
        {
            %functionList = %methodName;
        }
        else
        {
            %functionList = %functionList SPC %methodName;
        }

        %lastLine = %currentLine;
    }
    %introspectLog.close();
    %introspectLog.delete();

    //For each detected function, use eval to define a stub function illustrated below.
    for(%i = 0; %i < getWordCount(%functionList); %i++)
    {
        %function = getWord(%functionList, %i);
        //Class::function(%v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %v10, %v11, %v12, %v13, %v14, %v15, %v16, %v17, %v18)
        //{
        //      SuperClass::function(%v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %v10, %v11, %v12, %v13, %v14, %v15, %v16, %v17, %v18);
        //}
        eval("function " @ %this.class @ "::" @ %function @ "(%this, %v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %v10, %v11, %v12, %v13, %v14, %v15, %v16, %v17, %v18){" @ %superClass @ " :: " @ %function @ "(%this, %v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %v10, %v11, %v12, %v13, %v14, %v15, %v16, %v17, %v18);}");
    }
}