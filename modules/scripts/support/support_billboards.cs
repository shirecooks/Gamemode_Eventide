datablock PlayerData(DefaultBillboardMount)
{
    shapeFile = "base/data/shapes/empty.dts";
	boundingBox = vectorScale("20 20 20", 4);

	splash = PlayerSplash;
    splashEmitter[0] = PlayerFoamDropletsEmitter;
    splashEmitter[1] = PlayerFoamEmitter;
    splashEmitter[2] = PlayerBubbleEmitter;

    mediumSplashSoundVelocity = 10;
    hardSplashSoundVelocity = 20;
    exitSplashSoundVelocity = 5;

    impactWaterEasy = Splash1Sound;
    impactWaterMedium = Splash1Sound;
    impactWaterHard = Splash1Sound;
    exitingWater = exitWaterSound;

    jetEmitter = playerJetEmitter;
    jetGroundEmitter = playerJetGroundEmitter;
    jetGroundDistance = 4;
    footPuffNumParts = 10;
    footPuffRadius = 0.25;

	rideAble = false;
	className = "BillboardMount";
};

//To prevent errors on `doDismount` calls.
function DefaultBillboardMount::doDismount(%this, %obj, %forced)
{
	
}

datablock PlayerData(OverheadBillboardMount : DefaultBillboardMount)
{
    shapeFile = "base/data/shapes/empty.dts";
	rideAble = false;
};

function BillboardMount::OnAdd(%db,%bbm)
{
	%bbm.applyDamage(1000);
	%bbm.isBillboardMount = true;

	%bbm.billBoardGroup = new scriptGroup();
}

function BillboardMount::OnRemove(%db,%bbm)
{
	BillboardMount_CLearBillboards(%bbm);
	%bbm.billBoardGroup.delete();
}

function BillboardMount::Make(%db)
{
	%obj = new AiPlayer()
	{
		dataBlock = %db;
	};

	return %obj;
}

function BillboardMount_AddBillboard(%bbm,%lightData,%dontGhost)
{
	if(!%bbm.isBillboardMount || %lightData.className !$= "Billboard")
	{
		return;
	}

	%obj = new fxLight()
	{
		dataBlock = %lightData;
	};
	%obj.setNetFlag(6,true);
	if(!%dontGhost)
	{
		%obj.setScopeAlways();
	}
	else
	{
		%obj.setNetFlag(8,false);
	}

	%bbm.billBoardGroup.add(%obj);
	BillboardMount_FinishAddBillboard(%bbm,%obj);
	return %obj;
}

function BillboardMount_FinishAddBillboard(%bbm,%light)
{
	%group = clientGroup;
	%count = %group.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		if(%group.getObject(%i).getGhostID(%bbm) == -1)
		{
			schedule(100,%bbm,"BillboardMount_FinishAddBillboard",%bbm,%light);
			return "";
		}
	}
	%light.attachToObject(%bbm);
}

function BillboardMount_ClearBillboards(%bbm)
{
	%group = %bbm.billBoardGroup;
	%count = %group.getCount();
	for(%i = %count - 1; %i >= 0; %i--)
	{
		%obj = %group.getObject(%i);
		%obj.delete();
	}

	return %bbm;
}

function BillboardMount_AddAVBillboard(%bbm,%avbbg,%lightData,%tag)
{
	if(!%avbbg.loaded)
	{
		return "";
	}
	if(!%bbm.isBillboardMount || %avbbg.class !$= "AVBillboardGroup" || %lightData.className !$= "AVBillboard")
	{
		return "";
	}
	if(%avbbg.loadedClient.getGhostID(%bbm) == -1)
	{
		schedule(100,%bbm,"BillboardMount_AddAVBillboard",%bbm,%avbbg,%lightData,%tag);
		return "";
	}
	%group = %avbbg;
	%count = %group.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%obj = %group.getObject(%i);
		if(!%obj.active)
		{
			break;
		}
	}
	if(%i >= %count)
	{
		return "";
	}
	%bb = %avbbg.getObject(%i);
	%bb.tag = %tag;
	%bb.active = true;

	%bb.setNetFlag(8,true);
	%bb.setDatablock(%lightData);
	%bb.setEnable(true);
	%bb.attachToObject(%bbm);
	%bb.setNetFlag(8,false);
	
	return %bbm;
}

datablock fxLightData(DefaultBillboard)
{
	LightOn = false;

	flareOn = true;
	flarebitmap = "base/data/shapes/blank.png";
	ConstantSize = 1;
    ConstantSizeOn = true;
    FadeTime = 0.000001;

	LinkFlare = false;
	blendMode = 1;
	flareColor = "1 0 0 1";

	AnimOffsets = true;
	startOffset = "0 0 0";
	endOffset = "0 0 0";

	className = "Billboard";
};

function Billboard::OnAdd(%db,%bb)
{
	%bb.isBillboard = true; 
}

function Billboard_Ghost(%bb,%client)
{
	%bb.ScopeToClient(%client);
}

function Billboard_ClearGhost(%bb,%client)
{
	%bb.ClearScopeToClient(%client);
}

$AVBillboard::loadMount = $AVBillboard::loadMount || DefaultBillboardMount.Make();
$AVBillboard::loadTransform = "0 0 1000 0 0 0 1";
$AVBillboard::loadMountTransform = vectorAdd(getWords($AVBillboard::loadTransform,0,2),matrixMulVector($AVBillboard::loadTransform,"0 4 0")) SPC getWords($AVBillboard::loadTransform,3);
$AVBillboard::loadMount.setTransform($AVBillboard::loadMountTransform);
$AVBillboard::loadMount.setNetFlag(8,false);

datablock fxLightData(DefaultAVBillboard)
{
	LightOn = false;

	flareOn = true;
	flarebitmap = "base/data/shapes/blank.png";
	ConstantSize = 1;
    ConstantSizeOn = true;
    FadeTime = inf;

	LinkFlare = false;
	blendMode = 1;
	flareColor = "1 0 0 1";

	AnimOffsets = true;
	startOffset = "0 0 0";
	endOffset = "0 0 0";

	classname = "AVBillboard";
};


function AVBillboard::OnAdd(%db,%bb)
{
	%bb.isAVBillboard = true; 
}

function AVBillboardGroup_Make()
{
	%obj = new ScriptGroup()
	{
		class = "AVBillboardGroup";
	};

	return %obj;
}

datablock CameraData(BillboardLoadingCamera)
{
	mode = "Observer";
};

function AVBillboardGroup::Load(%avbbg,%client,%num)
{
	if(%avbbg.loadedClient !$= "")
	{
		return;
	}

	if(%num <= 0)
	{
		return;
	}
	
	%camera = %client.AVBillboardGroup_LoadCamera = %client.AVBillboardGroup_LoadCamera ||  new Camera(){dataBlock = BillboardLoadingCamera;};
	%dummyCamera = %client.AVBillboardGroup_LoadDummyCamera = %client.AVBillboardGroup_LoadDummyCamera || new Camera(){dataBlock = BillboardLoadingCamera;};
	
	$AVBillboard::loadMount.scopeToClient(%client);
	%camera.setTransform($AVBillboard::loadTransform);
	%camera.setcontrolObject(%dummyCamera);
	%client.setControlObject(%camera);

	%avbbg.loadedClient = %client;

	AVBillboardGroup_StartLoad(%avbbg,%num);

	return %avbbg;
}

function AVBillboardGroup_StartLoad(%avbbg,%num)
{
	%client = %avbbg.loadedClient;
	if(%client.getGhostId(%client.AVBillboardGroup_LoadCamera) == -1 || %client.AVBillboardGroup_LoadDummyCamera == -1 || %client.getGhostId($AVBillboard::loadMount) == -1)
	{
		schedule(100,%client,"AVBillboardGroup_StartLoad",%avbbg,%num);
		return;
	}

	for(%i = 0; %i < %num; %i++)
	{
		%bb = new fxLight()
		{
			dataBlock = DefaultBillboard;
		};
		%bb.setNetFlag(6,true);
		%bb.setNetFlag(8,false);
		%bb.attachToObject($AVBillboard::loadMount);
		Billboard_Ghost(%bb,%client);
		%avbbg.add(%bb);
	}

	for(%i = 0; %i < %num; %i++)
	{
		AVBillboardGroup_CheckLoadProgress(%avbbg.getObject(%i));
	}
}

function AVBillboardGroup_CheckLoadProgress(%bb)
{
	%avbbg = %bb.getGroup();
	%client = %avbbg.loadedClient;
	if(%avbbg.loadedClient $= "" || %avbbg.loaded)
	{
		return;
	}

	if(%client.getGhostID(%bb) == -1)
	{
		schedule(100,%bb,"AVBillboardGroup_CheckLoadProgress",%bb);
		return;
	}

	schedule(2000,%bb,"AVBillboardGroup_FinishLoad",%bb);
}

function AVBillboardGroup_FinishLoad(%bb)
{
	%avbbg = %bb.getGroup();
	%client = %avbbg.loadedClient;

	%bb.setNetFlag(8,true);
	%bb.setDatablock(DefaultAVBillboard);
	%bb.setEnable(false);
	%bb.setNetFlag(8,false);
	%avbbg.loadedCount++;
	if(%avbbg.getCount() == %avbbg.loadedCount)
	{
		%avbbg.loaded = true;
		if(isObject(%client.player))
		{
			%client.setControlObject(%client.player);
		}
		else
		{
			%client.setControlObject(%client.camera);
		}
	}
}

function AVBillboardGroup::Clear(%avbbg,%tag)
{
	if(!%avbbg.loaded)
	{
		return;
	}

	%group = %avbbg;
	%count = %group.getCount();
	for(%i = 0; %i < %count; %i++)
	{
		%bb = %group.getObject(%i);
		if((%tag !$= "" && %tag !$= %bb.tag) || !%bb.active)
		{
			continue;
		}
		%bb.setNetFlag(8,true);
		%bb.setEnable(false);
		%bb.setNetFlag(8,false);
		%bb.tag = "";
		%bb.active = false;
	}
	return %group;
}

function AVBillboardGroup::OnRemove(%avbbg)
{
	%avbbg.deleteall();
}