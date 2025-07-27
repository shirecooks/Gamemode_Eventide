exec("./scripts/support_oop.cs");
exec("./scripts/support_common.cs");
exec("./scripts/support_vectorUtilities.cs");
exec("./scripts/support_extraResources.cs");
exec("./scripts/support_ghostingUtils.cs");
exec("./scripts/support_client.cs");

exec("./sounds/datablock_sounds.cs");
exec("./players/datablock_textures.cs");
exec("./players/datablock_lights.cs");
exec("./players/datablock_killerTrails.cs");
exec("./players/datablock_killerBlood.cs");
exec("./players/datablock_killerMeleeProp.cs");

exec("./players/player_eventide.cs");
exec("./players/player_survivor.cs");
exec("./players/player_staller.cs");
exec("./players/player_tinkerer.cs");
exec("./players/player_hoarder.cs");
exec("./players/player_fighter.cs");
exec("./players/player_mender.cs");
exec("./players/player_runner.cs");
exec("./players/player_killer.cs");
exec("./players/player_renowned.cs");

exec("./scripts/script_hatmodOverride.cs");
exec("./scripts/script_ambiantMusic.cs");

%currentWorkingDirectory = filePath($Con::File);
exec("./scripts/script_faceSystem.cs"); parseFacePacks(%currentWorkingDirectory @ "/players/faces");
exec("./scripts/script_voiceSystem.cs"); parseVoicePacks(%currentWorkingDirectory @ "/sounds/voicePacks");