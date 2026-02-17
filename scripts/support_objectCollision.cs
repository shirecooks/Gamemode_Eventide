//
// Returning true causes a collision, false prevents it.
// Return true by default.
//
function onObjectCollisionTest(%obj, %col)
{
    if(%obj == %col)
    {
        return true;
    }

    %hostDatablock = %obj.Datablock;
    %targetDatablock = %col.Datablock;

    //Check if the host has custom collision handling. If so, cache the result for more performance next time.
    %hostCustomCollision = %hostDatablock.handlesCollision;
    if(%hostDatablock.handlesCollision $= "")
    {
        %hostDatablock.handlesCollision = isFunction(%hostDatablock, onObjectCollision);
        %hostCustomCollision = %hostDatablock.handlesCollision;
    }

    //Do the same for the target object.
    %targetCustomCollision = %targetDatablock.handlesCollision;
    if(%targetDatablock.handlesCollision $= "")
    {
        %targetDatablock.handlesCollision = isFunction(%targetDatablock, onObjectCollision);
        %targetCustomCollision = %targetDatablock.handlesCollision;
    }

    //Based on which object has custom collision handling, call the appropriate functions.
    //If BOTH have custom collision handling, aggregate both results. Both must agree to allow the collision to occur.
	if(%hostCustomCollision && %targetCustomCollision)
    {
        return (%hostDatablock.onObjectCollision(%obj, %col) && %targetDatablock.onObjectCollision(%col, %obj));
    }
    else if(%hostCustomCollision)
    {
        return %hostDatablock.onObjectCollision(%obj, %col);
    }
    else if(%targetCustomCollision)
    {
        return %targetDatablock.onObjectCollision(%col, %obj);
    }

    return true;
}
