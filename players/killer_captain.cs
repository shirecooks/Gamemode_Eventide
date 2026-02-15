//
// Foil jacket image, equipped by survivors who detect Sky Captain.
//

function foilJacketImage::onMount(%this, %obj)
{
    %obj.playAudio(1, "foilJacket_on" @ getRandom(1, 5) @ "_sound");

	%obj.playThread(3, activate2);
	%obj.playThread(2, plant);
}

function foilJacketImage::onUnMount(%this, %obj)
{
    %obj.playAudio(1, "foilJacket_off" @ getRandom(1, 4) @ "_sound");

	%obj.playThread(3, activate2);
	%obj.playThread(2, jump);
}