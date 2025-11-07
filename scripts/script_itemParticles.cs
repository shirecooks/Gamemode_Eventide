//
// Creating a companion object.
//

function ItemData::createEmitter(%this, %obj, %emitterDatablock)
{
    %obj.emitterOffset = (%emitterDatablock.emitterOffset !$= "") ? %emitterDatablock.emitterOffset : "0 0 0";

    %emitter = %obj.emitter;
    if(%emitter)
    {
        %emitter.delete();
    }

    %nodeDatablock = (%emitterDatablock.pointEmitterNode !$= "") ? %emitterDatablock.pointEmitterNode : GenericEmitterNode;
    %emitter = new ParticleEmitterNode()
    {
        dataBlock = %nodeDatablock;
        emitter = %emitterDatablock;
    };
    
    %obj.emitter = %emitter;
    %obj.emitterLoop();

function ItemData::createLight(%this, %obj, %lightDatablock)
{
    %obj.lightOffset = (%lightDatablock.lightOffset !$= "") ? %lightDatablock.lightOffset : "0 0 0";

    %light = %obj.light;
    if(%light)
    {
        %light.delete();
    }

    %light = new fxLight()
    {
        dataBlock = %lightDatablock;
    };
    
    %obj.light = %light;
    %obj.lightLoop();
}

//
// Functions to make companion object follow the item.
//

function ItemData::updateEmitter(%this, %obj)
{
    %emitter = %obj.emitter;
    %itemTransform = %obj.getTransform();
	%emitter.setTransform(VectorAdd(posFromTransform(%itemTransform), %obj.emitterOffset) SPC rotFromTransform(%itemTransform)); // moves the node
	%emitter.inspectPostApply(); // sends updated position to clients
}

function ItemData::updateLight(%this, %obj)
{
    %light = %obj.light;
    %itemTransform = %obj.getTransform();
	%light.setTransform(VectorAdd(posFromTransform(%itemTransform), %obj.lightOffset) SPC rotFromTransform(%itemTransform));
}

//
// Loops to make it happen continously.
//

function Item::emitterLoop(%obj)
{
	%obj.Datablock.updateEmitter(%obj);
    %obj.emitterLoopSchedule = %obj.schedule(33, emitterLoop);
}

function Item::stopEmitter(%obj)
{
    cancel(%obj.emitterLoopSchedule);
    
	%emitter = %obj.emitter;
    if(isObject(%emitter))
    {
        %emitter.delete();
    }
}

function Item::lightLoop(%obj)
{
	%obj.updateLight();
    %obj.lightLoopSchedule = %obj.schedule(33, lightLoop);
}

function Item::stopLight(%obj)
{
    cancel(%obj.lightLoopSchedule);
    
	%light = %obj.light;
    if(isObject(%light))
    {
        %light.delete();
    }
}

//
// Package to make it automatic.
//

//This function does not exist in the base game.
//A stub is needed to prevent an error when the packaged function is called.
function ItemData::onRemove(%this, %obj)
{

}

package Script_ItemParticles
{	
	function ItemData::onAdd(%this, %obj)	
	{
		Parent::onAdd(%this,%obj);

        %emitterDatablock = %this.emitterDatablock;
        if(%emitterDatablock !$= "")
        {
            %obj.createEmitter(%obj, %emitterDatablock);
        }

        %lightDatablock = %this.lightDatablock;
        if(%lightDatablock !$= "")
        {
            %obj.createLight(%obj, %lightDatablock);
        }
	}

	function ItemData::onRemove(%this, %obj)
	{
        %emitter = %obj.emitter;
		if(isObject(%emitter))
        {
            %emitter.delete();
        }

        %light = %obj.light;
		if(isObject(%light))
        {
            %light.delete();
        }

        parent::onRemove(%this, %obj);
	}	
};
if(isPackage(Script_ItemParticles))
{
    deactivatePackage(Script_ItemParticles);
}
activatePackage(Script_ItemParticles);