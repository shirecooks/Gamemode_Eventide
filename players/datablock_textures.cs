//Add the icons to the extra resource list.
%iconpath = "./icons/*.png"; 
for (%iconfile = findFirstFile(%iconpath); %iconfile !$= ""; %iconfile = findNextFile(%iconpath)) 
{
	addExtraResource(%iconfile);
}

%write = new FileObject();

//IFL file for faces.
if(isWriteableFileName(%faceiflpath = "./models/face.ifl"))
{
	%write.openForWrite(findFirstFile(%faceiflpath));
	%write.writeLine("base/data/shapes/player/faces/smiley.png");

	for(%faceFile = findFirstFile("Add-Ons/Face_Default/*.png"); %faceFile !$= ""; %faceFile = findNextFile("Add-Ons/Face_Default/*.png")) 
	{	
		%write.writeLine(%faceFile);
	}
	
	%decalpath = "./faces/*.png";
	for (%decalfile = findFirstFile(%decalpath); %decalfile !$= ""; %decalfile = findNextFile(%decalpath))
	{
		addExtraResource(%decalfile);
		%write.writeLine(%decalfile);
	}

	%write.close();
	addExtraResource(findFirstFile(%faceiflpath));
}

//IFL file for decals.
if(isWriteableFileName(%decalfilepath = "./models/decal.ifl"))
{
	%write.openForWrite(findFirstFile(%decalfilepath));
	%write.writeLine("base/data/shapes/player/decals/AAA-none.png");

	//Add all the default decals to the IFL file.
	%decalPrefixes = "WORM Jirue Hoodie PlayerFitNE Default";
	for (%i = 0; %i < getWordCount(%decalPrefixes); %i++) 
	{
    	%prefix = getWord(%decalPrefixes, %i);
    	for (%decalFile = findFirstFile("Add-Ons/Decal_" @ %prefix @ "/*.png"); %decalFile !$= ""; %decalFile = findNextFile("Add-Ons/Decal_" @ %prefix @ "/*.png")) 
		{
			if(strstr(strlwr(%decalfile), "/thumbs/") == -1) %write.writeLine(%decalFile);
    	}
	}
	
	// Then add all the custom decals into the IFL directory
	%decalpath = "./decals/*.png";
	for(%decalfile = findFirstFile(%decalpath); %decalfile !$= ""; %decalfile = findNextFile(%decalpath))
	{
		addExtraResource(%decalfile);
		%write.writeLine(%decalfile);
	}

	%write.close();
	addExtraResource(findFirstFile(%decalfilepath));
}

%write.delete();