function SimObject::applyStatusEffect(%obj, %class, %category, %duration)
{
    //Group stored on the object, recording their current status effects.
    if(%obj.statusEffects $= "")
    {
        %obj.statusEffects = new ScriptGroup();
    }

    //Store the status effect object on the target SimObject.
    %statusEffect = new ScriptObject()
    {
        class = %class;
        target = %obj;
        category = %category;
        duration = %duration;
    };
    %obj.statusEffects.add(%statusEffect);
    %obj.statusEffectsCache[%class, %category] = %statusEffect;

    //Run the code defined within the supplied class to apply effects of status.
    if(isFunction(%class, beginStatusEffect))
    {
        %statusEffect.beginStatusEffect(%obj);
    }

    //If the status effect is set to expire, set a schedule for it to do so.
    if(%duration > 0)
    {
        %statusEffect.clearSchedule = %obj.schedule(%duration, clearStatusEffect, %class, %category);
    }

    return %statusEffect;
}

function SimObject::hasStatusEffect(%obj, %class, %category)
{
    %statusEffect = %obj.statusEffectsCache[%class, %category];
    return isObject(%statusEffect) ? %statusEffect : 0;
}

function SimObject::clearStatusEffect(%obj, %class, %category)
{
    %statusEffect = %obj.hasStatusEffect(%class, %category);
    if(%statusEffect)
    {
        if(isFunction(%statusEffect.class, finalizeStatusEffect))
        {
            %statusEffect.finalizeStatusEffect(%obj);
        }

        %statusEffect.delete();
    }
}