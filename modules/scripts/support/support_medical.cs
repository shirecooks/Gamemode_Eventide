package Eventide_Medical_Package
{
    function ZombieMedpackImage::healLoop(%this, %obj)
    {
		Parent::healLoop(%this, %obj);		
		
		if(%obj.zombieMedpackUse >= 3.3)
		{
			%obj.pseudoHealth = (%obj.survivorclass $= "fighter") ? 75 : 0;
			%obj.wasDowned = false;
		}
    }

    function GauzeImage::healLoop(%this, %obj)
    {
    	Parent::healLoop(%this, %obj);
    
		if(%obj.GauzeUse >= 2.3)
		{
			%obj.pseudoHealth = (%obj.survivorclass $= "fighter") ? 75 : 0;
			%obj.wasDowned = false;
		}
    }
};
activatePackage(Eventide_Medical_Package);