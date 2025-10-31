function onObjectCollisionTest(%obj, %col)
{
    %hostDatablock = %obj.Datablock;
    %targetDatablock = %col.Datablock;

    %hostCollisionHandler = isFunction(%hostDatablock, onObjectCollision);
    %targetCollisionHandler = isFunction(%targetDatablock, onObjectCollision);

	if(%hostCollisionHandler && %targetCollisionHandler)
    {
        return (%hostDatablock.onObjectCollision(%obj, %col) && %targetDatablock.onObjectCollision(%col, %obj));
    }
    else if(%hostCollisionHandler)
    {
        return %hostDatablock.onObjectCollision(%obj, %col);
    }
    else if(%targetCollisionHandler)
    {
        return %targetDatablock.onObjectCollision(%col, %obj);
    }

    return true;
}
