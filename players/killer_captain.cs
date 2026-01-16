//
// Foil jacket image, equipped by survivors who detect Sky Captain.
//

datablock ShapeBaseImageData(foilJacketImage)
{
    className = "ItemImage";

	shapeFile = $Eventide_BaseDirectory @ "/items/models/foilJacket/foilJacket.dts";
	emap = 0;

	mountPoint = $HipSlot;
	offset = "0.01 0 0.64";
	eyeOffset = "0 0 10";
	rotation = eulerToMatrix("0 0 180");
	scale = "1.25 1.25 1.25";

	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0;
	stateTransitionOnTimeout[0] = "Ready";
	stateName[1] = "Ready";
};

function foilJacketImage::onMount(%this, %obj, %slot)
{
    %obj.playAudio(1, "foilJacket_on" @ getRandom(1, 5) @ "_sound");

	%obj.playThread(3, activate2);
	%obj.playThread(2, plant);
}

function foilJacketImage::onUnMount(%this, %obj, %slot)
{
    %obj.playAudio(1, "foilJacket_off" @ getRandom(1, 4) @ "_sound");

	%obj.playThread(3, activate2);
	%obj.playThread(2, jump);
}