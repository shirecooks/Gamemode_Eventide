$Eventide_TextStyles["isGlobalFontConfigArray"] = true; //Stores any created fonts.

//
// Font object creation and functionality.
//

function createTextStyle(%category, %printChannel, %prefix, %suffix, %displayTime)
{
    %textStyle = new ScriptObject()
    {
        class = TextStyle;
        isTextStyle = true;
        category = %category;
        prefix = %prefix;
        suffix = %suffix;
        printChannel = %printChannel;
        displayTime = %displayTime;
    };
    %textStyle.setName("textStyle_" @ %category);

    %existingTextStyle = $Eventide_TextStyles[%category];
    if(isObject(%existingTextStyle))
    {
        %existingTextStyle.delete();
    }

    $Eventide_TextStyles[%category] = %textStyle;
    return %textStyle;
}

function TextStyle::formatString(%this, %string)
{
    return %this.prefix @ %string @ %this.suffix;
}

//
// Accessing font objects.
//

function getTextStyle(%category)
{
    return $Eventide_TextStyles[%category];
}

function styleFormatString(%category, %string)
{
    %textStyle = getTextStyle(%category);
    if(%textStyle $= "")
    {
        return;
    }

    return %textStyle.formatString(%string);
}

//
// Displaying text processed by a font.
//

function GameConnection::printFormatString(%client, %category, %string, %displayTime)
{
    %textStyle = getTextStyle(%category);
    if(!%textStyle)
    {
        return;
    }

    if(%displayTime $= "")
    {
        %displayTime = %textStyle.displayTime;
    }

    switch$(strlwr(%textStyle.printChannel))
    {
        case "centerprint":
            %client.centerPrint(%textStyle.formatString(%string), %displayTime);
        case "bottomprint":
            %client.bottomPrint(%textStyle.formatString(%string), %displayTime);
        case "chatmessage":
            %client.chatMessage(%textStyle.formatString(%string));
        default:
            %client.centerPrint(%textStyle.formatString(%string), %displayTime);
    }
}

//
// Baked-in Eventide fonts.
//

createTextStyle("hint", "bottomprint", "\c6", "", 8);
createTextStyle("urgent", "centerPrint", "<font:impact:32>\c3", "", 5);