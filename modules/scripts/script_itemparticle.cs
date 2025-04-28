package Eventide_ItemParticle
{	
	function ItemData::onAdd(%this, %obj)	
	{
		Parent::onAdd(%this,%obj);

		if(!%obj.static)
		{
			itemEmitterLoop(%obj);
		}		
	}

	function ItemData::onRemove(%this, %obj)
	{
		if(isObject(%obj.emitter)) %obj.emitter.delete();
		parent::onRemove(%this,%obj);
	}	
};

// In case the package is already activated, deactivate it first before reactivating it
if(isPackage(Eventide_ItemParticle)) deactivatePackage(Eventide_ItemParticle);
activatePackage(Eventide_ItemParticle);

/// This function creates an emitter node for the given item and moves it to the same position as the item. .
/// It will loop every 50ms to keep the emitter in the same position as the item.
/// This function was contributed by Conan
/// @param %obj The item to create the emitter for
/// @param %emitterNode The emitter node to use, if any. If not provided, it will be created using the emitter node setting or the default GenericEmitterNode
$ItemEmitterDatablock = "WandEmitterA";
function itemEmitterLoop(%obj, %emitterNode)
{	
	// check if the object is still valid, delete the emitter node if not
	if (!isObject(%obj) || %obj.isRitual)
	{
		if (isObject(%emitterNode))
		{
			%emitterNode.delete();
		}
		return;
	}

	%ItemEmitterDatablock = $ItemEmitterDatablock;

	if(%obj.getDataBlock().image.isRitual)
	{
		%ItemEmitterDatablock = "brickDeployExplosionEmitter";
	}

	if (!isObject(%emitterNode))
	{
		// creates emitter node using either the emitter node setting, or the default GenericEmitterNode
		%nodeData = %ItemEmitterDatablock.pointEmitterNode;

		if (!isObject(%nodeData))
		{
			%nodeData = GenericEmitterNode;
		}
		
		%emitterNode = new ParticleEmitterNode ("")
		{
			dataBlock = %nodeData;
			emitter = %ItemEmitterDatablock;
		};

		%emitterNode.setEmitterDataBlock(%ItemEmitterDatablock);
		MissionCleanup.add(%emitterNode);
	}

	%emitterNode.setTransform(%obj.getTransform()); // moves the node
	%emitterNode.inspectPostApply(); // sends updated position to clients
	%obj.emitter = %emitterNode; // just for convenience if some script wants to use this
	
	cancel(%obj.itemEmitterLoopSchedule);
	%obj.itemEmitterLoopSchedule = schedule(50, 0, itemEmitterLoop, %obj, %emitterNode); // schedule a repeat call to loop this function
}