$Eventide_FontStyles["isGlobalFontConfigArray"] = true; //Stores any created fonts.

//
// Font object creation and functionality.
//

function createFontStyle(%category, %prefix, %suffix)
{
    %fontStyle = new ScriptObject()
    {
        class = FontStyle;
        isFontStyle = true;
        category = %category;
        prefix = %prefix;
        suffix = %suffix;
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
// Baked-in Eventide fonts.
//

createFontStyle("hint", "\c6", "");
createFontStyle("urgent", "<font:impact:32>\c3", "");