function GameConnection::counterPrint(%this, %amount, %message, %time)
{
    %outputString = "";

    if(%time $= "")
    {
        %time = 1;
    }

    %symbol = "|";
    for(%i = 0; %i < %amount; %i++) 
	{
		%outputString = %outputString @ %symbol;
	}

    %this.centerPrint("<font:impact:30>\c3" @ %message @ " <br>\c2" @ %outputString, 1);
}