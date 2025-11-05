function Item::createEmitter(%obj, %emitterDatablock)
{
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
    %obj.updateEmitter();
}

function Item::updateEmitter(%obj)
{
    %emitter = %obj.emitter;
	%emitter.setTransform(%obj.getTransform()); // moves the node
	%emitter.inspectPostApply(); // sends updated position to clients
}

function Item::emitterLoop(%obj)
{
	%obj.updateEmitter();
    %obj.emitterLoopSchedule = %obj.schedule(33, "emitterLoop");
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
            %obj.createEmitter(%emitterDatablock);
            %obj.emitterLoop();
        }
	}

	function ItemData::onRemove(%this, %obj)
	{
        %emitter = %obj.emitter;
		if(%emitter) 
        {
            %emitter.delete();
        }

        parent::onRemove(%this, %obj);
	}	
};
if(isPackage(Script_ItemParticles))
{
    deactivatePackage(Script_ItemParticles);
}
activatePackage(Script_ItemParticles);