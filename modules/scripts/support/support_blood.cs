package Eventide_DSBloodPackage 
{
	function Armor::onEnterLiquid(%data, %obj, %coverage, %type)
	{
        if(%obj.getdatablock().getName() $= "PlayerRender" || !isObject(%obj.client))
		{
			return Parent::onEnterLiquid(%data, %obj, %coverage, %type);
		}

		%obj.bloody["lshoe"] = false;
		%obj.bloody["rshoe"] = false;
		%obj.bloody["lhand"] = false;
		%obj.bloody["rhand"] = false;
		%obj.bloody["chest_front"] = false;
		%obj.bloody["chest_back"] = false;
		%obj.bloody["chest_lside"] = false;
		%obj.bloody["chest_rside"] = false;
		%obj.bloodyFootprints = 0;

		%obj.client.applyBodyParts();
	}
};
if (isPackage(Eventide_DSBloodPackage))
{
	deactivatePackage("Eventide_DSBloodPackage");
}
activatePackage("Eventide_DSBloodPackage");