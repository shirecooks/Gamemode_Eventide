//
// Automated weapon tips, powered by the Text Styles system.
//

function WeaponImage::getHintMessage(%this, %obj, %slot)
{
    return "";
}

function WeaponImage::getHintStyle(%this, %obj, %slot)
{
    return "hint";
}

package Support_WeaponUsageTips
{
    function WeaponImage::onMount(%this, %obj, %slot)
    {
        parent::onMount(%this, %obj, %slot);
        %currentTime = getSimTime();
        %obj.currToolMountTime = %currentTime;

        %client = %obj.client;
        if(%client)
        {
            //Since the welder of the weapon has a client, we can possibly show them a hint message. Let's see if the weapon is set to use one.
            %hintMessage = %this.getHintMessage();
            %hintStyle = %this.getHintStyle();
            //TODO: Due to Blockland Jank, weapon hints will sometimes display when you spawn, even if you haven't equipped anything. Here is a quick-fix.
            if(%hintMessage !$= "" && %hintStyle !$= "" && (%currentTime > (%obj.spawnTime + 100)))
            {
                //The tool being held has a hint message baked in, display it.
                %client.printFormatString(%hintStyle, %hintMessage);
            }
        }
    }

    function WeaponImage::onUnMount(%this, %obj, %slot)
    {
        parent::onUnMount(%this, %obj, %slot);

        %client = %obj.client;
        if(%client)
        {
            //Determine when the hint message was (is?) due to end.
            %textStyle = getTextStyle(%this.getHintStyle());
            %hintEndTime = (%obj.currToolMountTime + (%textStyle.displayTime * 1000));
            if(getSimTime() < %hintEndTime)
            {
                //The hint message is still being displayed, clear it.
                switch$(strlwr(%textStyle.printChannel))
                {
                    case "bottomprint":
                        clearBottomPrint(%client);
                    case "centerprint":
                        clearCenterPrint(%client);
                }
            }
        }
    }
};
if(isPackage(Support_WeaponUsageTips))
{
    deactivatePackage(Support_WeaponUsageTips);
}
activatePackage(Support_WeaponUsageTips);