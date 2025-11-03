datablock AudioDescription(AudioFootstep3D)
{
	volume = 0.65;
	isLooping = false;
	is3D = 1;
	ReferenceDistance = 5;
	maxDistance = 15;
	type = $SimAudioType;
};

//Create the sound datablocks.
%patterns = ".wav\t.ogg";
for(%i = 0; %i < getFieldCount(%patterns); %i++)
{
    %pattern = getField(%patterns, %i);
    for(%file = findFirstFile("./*" @ %pattern); %file !$= ""; %file = findNextFile("./*" @ %pattern)) 
    {
        %soundName = strreplace(filename(strlwr(%file)), %pattern, "");

        // Automatic sound instancing based on name of the sound
        if(strstr(%file, "voicePacks") != -1) 
        {
            %description = "AudioDefault3d";
        }
        else if(strstr(%file, "normal") != -1) 
        {
            %description = "AudioClose3d";

            if(strstr(%file, "loop") != -1) 
            {
                %description = "AudioCloseLooping3d";
            }        
        } 
        else if(strstr(%file, "quiet") != -1) 
        {
            %description = "AudioClosest3d";

            if(strstr(%file, "loop") != -1) 
            {
                %description = "AudioClosestLooping3d";
            }
        } 
        else if(strstr(%file, "loud") != -1) 
        {
            %description = "AudioDefault3d";

            if(strstr(%file, "loop") != -1) 
            {
                %description = "AudioDefaultLooping3d";
            }
        }
        else if(strstr(%file, "music") != -1) 
        {
            %soundName = fileBase(%file);
            %description = "AudioMusicLooping3d";
        }
        else if(strstr(%file, "footsteps"))
        {
            %description = "AudioFootstep3D";
        }
        else 
        {
            %description = ""; // No match, do not create datablock
        }

        // Skip if no valid description
        if(%description $= "") 
        {
            continue;
        }

        // Create AudioProfile datablock
        if(%description $= "AudioMusicLooping3d")
        {
            eval("datablock AudioProfile(musicData_" @ %soundName @ ") { preload = true; description = " @ %description @ "; filename = \"" @ %file @ "\"; uiName=\"" @ %soundName @ "\"; };");
        }
        else
        {
            eval("datablock AudioProfile(" @ %soundName @ "_sound) { preload = true; description = " @ %description @ "; filename = \"" @ %file @ "\"; };");
        }
    }
}