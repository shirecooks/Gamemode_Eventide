//Add the icons to the extra resource list.
%iconpath = "./icons/*.png"; 
for (%iconfile = findFirstFile(%iconpath); %iconfile !$= ""; %iconfile = findNextFile(%iconpath)) 
{
	addExtraResource(%iconfile);
}

%write = new FileObject();

//IFL file for faces.
%faceiflpath = expandFilename("./models/face.ifl");
if(isWriteableFileName(%faceiflpath))
{
	%write.openForWrite(%faceiflpath);
	%write.writeLine("base/data/shapes/player/faces/smiley.png");

	for(%faceFile = findFirstFile("Add-Ons/Face_Default/*.png"); %faceFile !$= ""; %faceFile = findNextFile("Add-Ons/Face_Default/*.png")) 
	{	
		if(strstr(strlwr(%faceFile), "/thumbs/") == -1) 
		{
			%write.writeLine(%faceFile);
		}
	}
	
	%facePath = "./faces/*.png";
	for(%facefile = findFirstFile(%facePath); %facefile !$= ""; %facefile = findNextFile(%facePath))
	{
		addExtraResource(%facefile);
		%write.writeLine(%facefile);
	}

	%write.close();
	addExtraResource(findFirstFile(%faceiflpath));
}

//IFL file for decals.
%decalfilepath = expandFilename("./models/decal.ifl");
if(isWriteableFileName(%decalfilepath))
{
	%write.openForWrite(%decalfilepath);
	%write.writeLine("base/data/shapes/player/decals/AAA-none.png");

	//Add all the default decals to the IFL file.
	%decalPrefixes = "WORM Jirue Hoodie PlayerFitNE Default";
	%decalPrefixCount = getWordCount(%decalPrefixes);
	for(%i = 0; %i < %decalPrefixCount; %i++) 
	{
    	%prefix = getWord(%decalPrefixes, %i);
    	for(%decalFile = findFirstFile("Add-Ons/Decal_" @ %prefix @ "/*.png"); %decalFile !$= ""; %decalFile = findNextFile("Add-Ons/Decal_" @ %prefix @ "/*.png")) 
		{
			if(strstr(strlwr(%decalfile), "/thumbs/") == -1) 
			{
				%write.writeLine(%decalFile);
			}
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