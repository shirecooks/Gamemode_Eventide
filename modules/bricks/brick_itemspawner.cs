datablock fxDTSBrickData (brickEventideItemSpawnData:brick1x1fData)
{
	category = "Special";
	subCategory = "Eventide";
	uiName = "Eventide Item Spawn";
};

function brickEventideItemSpawnData::onPlant(%data,%obj)
{
	Parent::onPlant(%data,%obj);

	//Make sure the object is hidden when first placed
	%obj.setrendering(0);
	%obj.setcolliding(0);
	%obj.setraycasting(0);

    	//Always check if the simset exists first, and to add it to the mission clean up for later if necessary
    	if(!isObject(Eventide_ItemSpawns))
	{
		missionCleanup.add(new SimSet(Eventide_ItemSpawns));
	}
	
	Eventide_ItemSpawns.add(%obj);
}

function brickEventideItemSpawnData::onloadPlant(%data, %obj)
{
	brickEventideItemSpawnData::onPlant(%this, %obj);
}
