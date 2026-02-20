// Made by Robbinson Block (BLID: 37814, Discord: @robbinsonblock)

//To be run on datablocks or ScriptObjects. 
//Take advantage of `eval` and the lax-arity function model of TorqueScript to define a function that calls the equivalent parent function.
$OOP_fileObject = new FileObject();
function SimObject::inheritFunctionsFromSuperClass(%this, %superClass)
{
    %superClassID = nameToID(%superClass);
    if(%superClassID == -1)
    {
        error("ERROR: inheritFunctionsFromSuperClass() - Provided superclass does not exist.");
        return;
    }

    %objectClass = (%this.class !$= "") ? %this.class : %this.getName();
    if(%objectClass $= "")
    {
        error("ERROR: inheritFunctionsFromSuperClass() - Target object does not have a namespace.");
        return;
    }
    else if(%objectClass $= %superClass)
    {
        error("ERROR : inheritFunctionsFromSuperClass() - An object cannot parent itself.");
        return;
    }
    else if(%superClassID.superClass !$= "" && %superClassID.superClass $= %objectClass)
    {
        error("ERROR : inheritFunctionsFromSuperClass() - Circular object dependency.");
        return;
    }

    %this.objectClass = %objectClass;
    %this.superClass = %superClass;

    //The only way to determine the functions of an object is through the `dump()` method of SimObjects.
    %introspectLogLocation = "config/introspect.log";

    //Delete the file if it exists already, to prevent interference.
    if(isWriteableFileName(%introspectLogLocation))
    {
        $OOP_fileObject.openForWrite(%introspectLogLocation);
        $OOP_fileObject.close();
    }
    else
    {
        error("ERROR : inheritFunctionsFromSuperClass() - Introspect.log location not writeable.");
        return;
    }

    //Prepare a separate console object, prevent the main one from logging spam.
    enableWinConsole(0);
    $Con::logBufferEnabled = 0;
    %objectIntrospectLogger = new ConsoleLogger(objectIntrospectLogger, %introspectLogLocation, false);
    %objectIntrospectLogger.level = 0;
    %objectIntrospectLogger.attach();

    %superClassID.dump();

    //Re-enable main console, delete secondary one.
    %objectIntrospectLogger.detach();
    %objectIntrospectLogger.delete();
    $Con::logBufferEnabled = 1;
    enableWinConsole($Server::Dedicated); //Inaccurate, but most people don't use the `-console` argument as far as I know.

    %evalCall = "";
    %methodList = "";
    %ignoredMethods = "|delete|dump|getId|setName|getTaggedField|getType|serializeEventToString|";
    %readingMethod = false;
    %lastLine = "";
    $OOP_fileObject.openForRead(%introspectLogLocation);
    while(!$OOP_fileObject.isEOF())
    {
        %currentLine = trim($OOP_fileObject.readLine());

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
        if(strstr(%ignoredMethods, "|" @ %methodName @ "|") > -1)
        {
            continue;
        }

        if(%methodList $= "")
        {
            %methodList = %methodName;
        }
        else
        {
            %methodList = %methodList SPC %methodName;
        }
        %lastLine = %currentLine;
    }
    $OOP_fileObject.close();

    //For each detected function, use eval to define a stub function illustrated below.
    %methodCount = getWordCount(%methodList);
    for(%i = 0; %i < %methodCount; %i++)
    {
        %function = getWord(%methodList, %i);

        //Class::function(%v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %vA, %vB, %vC, %vD, %vE, %vF, %vG, %vH)
        //{
        //      SuperClass::function(%v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %vA, %vB, %vC, %vD, %vE, %vF, %vG, %vH);
        //}
        %functionDefinition = "function " @ %objectClass @ "::" @ %function @ "(%t,%v0,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%vA,%vB,%vC,%vD,%vE,%vF,%vG,%vH){return " @ %superClass @ "::" @ %function @ "(%t,%v0,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%vA,%vB,%vC,%vD,%vE,%vF,%vG,%vH);}";

        //Collapse all function calls into a single eval statement, if possible.
        //I've heard rumors of a max string length of 5 KB or similar. Let's test that.
        %evalCall = %evalCall @ %functionDefinition;
    }

    if(%evalCall !$= "")
    {
        eval(%evalCall);
    }

    //Return the target datablock, so the scripts can chain off of this call if they want to.
    return %this;
}

//Call a parent class function with arbitrary arguments.
//Take advantage of the fact that everything in TorqueScript is a string, so nothing can be lost by presenting all arguments as a string.
$OOP_functionMap = "";
function SimObject::super(%this, %function, %v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %v10, %v11, %v12, %v13, %v14, %v15, %v16, %v17)
{
    //Calling a superclass function that has another call to this function results in an infinite loop. We can stop this by measuring recursion depth.
    %recursionDepth = %this.recursionDepth++;

    //Recursively obtain a superclass, if necessary.
    %superClass = %this.superClass;
    if(%recursionDepth > 1)
    {
        for(%i = 1; %i < %recursionDepth; %i++)
        {
            %superClass = %superClass.superClass;
            if(%superClass $= "")
            {
                return;
            }
        }
    }

    //Check if a wrapper function has already been defined for this namespaced function. If not, define one.
    %superChainCall = $OOP_functionMap[%superClass, %function];
    if(%superChainCall $= "")
    {
        //Check if the superclass is valid and not a poisoned string.
        %superClass = getSafeVariableName(%superClass);
        if(!isObject(%superClass))
        {
            error("ERROR: super() - Provided superclass does not exist.");
        }

        //Check if the function is valid and not a poisoned string.
        %function = getSafeVariableName(%function);
        if(!isFunction(%superClass, %function))
        {
            error("ERROR: super() - Provided function does not exist.");
        }

        //Finally, create the wrapper. This covers both us and any other class that might need this callback.
        %functionName = "_OOP_" @ %superClass @ "_" @ %function;
        
        // function _OOP_SuperClass_function(%v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %vA, %vB, %vC, %vD, %vE, %vF, %vG, %vH)
        // {
        //     SuperClass::function(%v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %vA, %vB, %vC, %vD, %vE, %vF, %vG, %vH);
        // }
        eval("function " @ %functionName @ "(%v0,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%vA,%vB,%vC,%vD,%vE,%vF,%vG,%vH){return " @ %superClass @ "::" @ %function @ "(%v0,%v1,%v2,%v3,%v4,%v5,%v6,%v7,%v8,%v9,%vA,%vB,%vC,%vD,%vE,%vF,%vG,%vH);}");

        //Cache it. No more checks or eval.
        $OOP_functionMap[%superClass, %function] = %functionName;
        %superChainCall = %functionName;
    }

    %returnValue = call(%superChainCall, %v0, %v1, %v2, %v3, %v4, %v5, %v6, %v7, %v8, %v9, %v10, %v11, %v12, %v13, %v14, %v15, %v16, %v17);
    
    //An iteration reaching this line has hit the end of the chain, begin unwinding the calls.
    %this.recursionDepth--;
    
    return %returnValue;
}

//Recursively check if an object belongs to a parent class/ancestor.
//Only works in the context of chaining `inheritFunctionsFromSuperClass` calls, or if `superClass` is manually set.
function SimObject::isSubclassOf(%this, %parentObject)
{
    %parentObject = NameToID(%parentObject);
    %superClass = NameToID(%this.superClass);

    for(%superClass = NameToID(%this.superClass); %superClass != -1; %superClass = NameToID(%superClass.superClass))
    {
        if(%superClass == %parentObject)
        {
            return true;
        }
    }

    return false;
}

package Support_OOP
{
    function destroyServer()
    {
        deleteVariables("$OOP_*");
        Parent::destroyServer();
    }
};
activatePackage(Support_OOP);