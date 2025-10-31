exec("./scripts/support_oop.cs");
exec("./scripts/support_common.cs");
exec("./scripts/support_imageAttributes.cs");
exec("./scripts/support_restoredAmmoSystem.cs");
exec("./scripts/support_inventoryOperations.cs");
exec("./scripts/support_vectorUtilities.cs");
exec("./scripts/support_extraResources.cs");
exec("./scripts/support_ghostingUtils.cs");
exec("./scripts/support_client.cs");
exec("./scripts/support_weaponCooldown.cs");
exec("./scripts/support_statusEffects.cs");
exec("./scripts/support_meleeTrails.cs");
exec("./scripts/support_textStyles.cs");
exec("./scripts/support_objectCollision.cs");
exec("./scripts/script_footsteps.cs");
exec("./scripts/script_weaponUsageTips.cs");
exec("./scripts/status_stun.cs");
exec("./scripts/status_frozen.cs");
exec("./scripts/status_fear.cs");

exec("./sounds/datablock_sounds.cs");
exec("./players/datablock_textures.cs");
exec("./players/datablock_lights.cs");
exec("./players/datablock_killerTrails.cs");
exec("./players/datablock_killerBlood.cs");
exec("./items/datablock_woodFrag.cs");

exec("./items/weapon_eventideMelee.cs");
exec("./items/weapon_butterflyKnife.cs");
exec("./items/weapon_revolver.cs");
exec("./items/weapon_chair.cs");
exec("./items/weapon_poolCue.cs");
exec("./items/weapon_barStool.cs");
exec("./items/weapon_throwingSpear.cs");
exec("./items/weapon_frailSword.cs");
exec("./items/weapon_shotgun.cs");
exec("./items/weapon_ritualDagger.cs");

//Both "blueSoda" and "redSoda" share the status effect code in "datablock_speedSoda.cs"
exec("./items/status_speedSoda.cs");
exec("./items/item_blueSoda.cs");
exec("./items/item_redSoda.cs");

exec("./players/player_eventide.cs");
exec("./players/player_survivor.cs");
exec("./players/player_staller.cs");
exec("./players/player_tinkerer.cs");
exec("./players/player_hoarder.cs");
exec("./players/player_fighter.cs");
exec("./players/player_mender.cs");
exec("./players/player_runner.cs");
exec("./players/player_sheriff.cs");
exec("./players/player_killer.cs");
exec("./players/player_renowned.cs");
exec("./players/player_wrathful.cs");

exec("./scripts/script_hatmodOverride.cs");
exec("./scripts/script_ambiantMusic.cs");

%currentWorkingDirectory = filePath($Con::File);
exec("./scripts/script_faceSystem.cs"); parseFacePacks(%currentWorkingDirectory @ "/players/faces");
exec("./scripts/script_voiceSystem.cs"); parseVoicePacks(%currentWorkingDirectory @ "/sounds/voicePacks");