//Array to store melee trails and their name, in case file locations ever change in the future.
$Eventide_MeleeTrails["isGlobalMeleeTrailArray"] = true;

//Load a slash-shaped mesh that can be skinned with textures.
datablock StaticShapeData(KillerTrailShape)
{
	shapeFile = "./models/trails/meleeTrail.dts";
};

//Load every texture used to skin the mesh.
%filePattern = filePath($Con::File) @ "/models/trails/*.png";
for(%file = findFirstFile(%filePattern); %file !$= ""; %file = findNextFile(%filePattern))
{
	addExtraResource(%file);
	$Eventide_MeleeTrails[fileBase(%file)] = %file;
}

//Function to spawn this trail on-demand.
function Player::spawnMeleeTrail(%obj, %skin, %time, %offset, %angle, %scale)
{
	//Provide default values if arguments are not given.
	%time = (%time !$= "") ? %time : 1000;
	%offset = (%offset !$= "") ? %offset : "0.4 1.2 0.375";
	%angle = (%angle !$= "") ? %angle : "0 -90 0";
	%scale = (%scale !$= "") ? %scale : "3 2.5 0.4";

	//Determine the rotation and position of the melee slash. Didn't write this, couldn't tell you the specifics.
	%rotation = relativeVectorToRotation(%obj.getLookVector(), %obj.getUpVector());
	%clamped = mClampF(firstWord(%rotation), -89.9, 89.9) SPC restWords(%rotation);		
	%local = %obj.getHackPosition() SPC %clamped;
	%combined = %offset SPC eulerToQuat(%angle);
	%actual = matrixMultiply(%local, %combined);

	//Spawn the melee trail mesh object.
	%shape = new StaticShape()
	{
		dataBlock = KillerTrailShape;
		scale = %scale;
	};
	%shape.setSkinName(%skin);
	%shape.setTransform(%actual);

	//Play an animation before deleting it.
	%shape.playThread(0, "rotate");
	%shape.schedule(%time, delete);	
}

//
// Package to automatically spawn a melee trail when melee weapons are swung.
//

package Support_MeleeTrails
{
	function WeaponImage::onFire(%this, %obj, %slot)
	{
		%returnValue = Parent::onFire(%this, %obj, %slot);
		if(%this.melee && !%this.useCustomMeleeTrail && %this.meleeTrailSkin !$= "")
		{
			%obj.spawnMeleeTrail(%this.meleeTrailSkin, %this.meleeTrailTime, %this.meleeTrailOffset, %this.meleeTrailAngle, %this.meleeTrailScale);
		}
		return %returnValue;
	}
};
if(isPackage(Support_MeleeTrails))
{
	deactivatePackage(Support_MeleeTrails);
}
activatePackage(Support_MeleeTrails);