$Eventide_FontStyles["isGlobalFontConfigArray"] = true; //Stores any created fonts.

//
// Font object creation and functionality.
//

function createFontStyle(%category, %printChannel, %prefix, %suffix)
{
    %fontStyle = new ScriptObject()
    {
        class = FontStyle;
        isFontStyle = true;
        category = %category;
        prefix = %prefix;
        suffix = %suffix;
        printChannel = %printChannel;
    };
    %fontStyle.setName("fontStyle_" @ %category);

    %existingFontStyle = $Eventide_FontStyles[%category];
    if(isObject(%existingFontStyle))
    {
        %existingFontStyle.delete();
    }

    $Eventide_FontStyles[%category] = %fontStyle;
    return %fontStyle;
}

function FontStyle::formatString(%this, %string)
{
    return %this.prefix @ %string @ %this.suffix;
}

//
// Accessing font objects.
//

function getFontStyle(%category)
{
    return $Eventide_FontStyles[%category];
}

function styleFormatString(%category, %string)
{
    %fontStyle = getFontStyle(%category);
    if(%fontStyle $= "")
    {
        return;
    }

    return %fontStyle.formatString(%string);
}

//
// Displaying text processed by a font.
//

function GameConnection::printFormatString(%client, %category, %string, %displayTime)
{
    %printChannel = strlwr(%category.printChannel);
    if(%printChannel $= "")
    {
        return;
    }

    switch$(%printChannel)
    {
        case "centerprint":
            %client.centerPrint(styleFormat(%category, %string), %displayTime);
        case "bottomprint":
            %client.bottomPrint(styleFormat(%category, %string), %displayTime);
        case "chatmessage":
            %client.chatMessage(styleFormat(%category, %string));
    }
}

//
// Baked-in Eventide fonts.
//

createFontStyle("hint", "bottomPrint", "\c6", "");
createFontStyle("urgent", "centerPrint", "<font:impact:32>\c3", "");