function SimObject::registerImageOutputEvent(%this, %eventName)
{
    %this.outputEvent = %eventName;
    registerInputEvent("fxDTSBrick", %eventName, "Self fxDTSBrick" TAB "Player Player" TAB "Client GameConnection" TAB "MiniGame MiniGame");
    return %this;
}

function ShapeBaseImageData::eventRaycast(%this, %obj)
{
    //If the tool has no event, we can't do anything.
    %outputEvent = %this.outputEvent;
    if(%outputEvent $= "")
    {
        return;
    }

    %start = %obj.getEyePoint();
	%scale = getWord(%obj.getScale(), 2);
	%end = VectorAdd(%start, VectorScale(%obj.getEyeVector(), 10 * %scale));
	%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::VehicleObjectType;
    %exempt = (%obj.isMounted()) ? %obj.getObjectMount() : %obj;

	%search = containerRayCast(%start, %end, %mask, %exempt);
	%col = getWord(%search, 0);

    //If we fire a raycast and detect no brick, we also can't do anything.
    if(%col == 0 || !(%col.getType() & $TypeMasks::FxBrickObjectType))
    {
        return;
    }

    //Finally, call the event.
    %client = %obj.client;
    $InputTarget_["Self"] = %col;
    $InputTarget_["Player"] = %obj;
    $InputTarget_["Client"] = %client;
    if($Server::LAN)
    {
        $InputTarget_["MiniGame"] = getMiniGameFromObject(%client);
    }
    else
    {
        %brickMinigame = getMiniGameFromObject(%col);
        %clientMinigame = getMiniGameFromObject(%client);

        if(%brickMinigame == %clientMinigame)
        {
            $InputTarget_["MiniGame"] = %brickMinigame;
        }
        else
        {
            $InputTarget_["MiniGame"] = 0;
        }
    }
    %col.processInputEvent(%outputEvent, %client);
}