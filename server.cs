$Eventide_BaseDirectory = filePath($Con::File);

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
exec("./scripts/support_cutscenes.cs");
exec("./scripts/support_customCdn.cs");
exec("./scripts/script_weaponUsageTips.cs");
exec("./scripts/script_itemParticles.cs");

exec("./statusEffects/status_stun.cs");
exec("./statusEffects/status_frozen.cs");
exec("./statusEffects/status_fear.cs");
exec("./statusEffects/status_deafen.cs");
exec("./statusEffects/status_nearSight.cs");

exec("./sounds/datablock_sounds.cs");
exec("./players/datablock_textures.cs");
exec("./players/datablock_lights.cs");
exec("./players/datablock_killerBlood.cs");
exec("./items/datablock_woodFrag.cs");
exec("./items/datablock_sharpMelee.cs");

exec("./items/base_ritual.cs");
exec("./items/item_gem.cs");
exec("./items/item_candle.cs");
exec("./items/item_book.cs");
exec("./items/item_wrench.cs");
exec("./items/item_pickaxe.cs");
exec("./items/item_rum.cs");
exec("./items/item_camera.cs");
exec("./items/item_medpack.cs");

exec("./items/base_eventideMelee.cs");
exec("./items/weapon_butterflyKnife.cs");
exec("./items/weapon_revolver.cs");
exec("./items/weapon_chair.cs");
exec("./items/weapon_poolCue.cs");
exec("./items/weapon_barStool.cs");
exec("./items/weapon_throwingSpear.cs");
exec("./items/weapon_frailSword.cs");
exec("./items/weapon_shotgun.cs");
exec("./items/weapon_magicWand.cs");
exec("./items/weapon_epoxyBomb.cs");
exec("./items/weapon_ritualDagger.cs");

//Both "blueSoda" and "redSoda" share the status effect code in "datablock_speedSoda.cs"
exec("./statusEffects/status_speedSoda.cs");
exec("./items/item_blueSoda.cs");
exec("./items/item_redSoda.cs");

exec("./players/base_eventide.cs");
exec("./players/player_survivor.cs");
exec("./players/player_staller.cs");
exec("./players/player_tinkerer.cs");
exec("./players/player_hoarder.cs");
exec("./players/player_fighter.cs");
exec("./players/player_mender.cs");
exec("./players/player_runner.cs");
exec("./players/player_sheriff.cs");
exec("./players/player_knifer.cs");
exec("./players/base_killer.cs");
exec("./players/killer_renowned.cs");
exec("./players/killer_wrathful.cs");
exec("./players/killer_shire.cs");
exec("./players/killer_angler.cs");
exec("./players/killer_captain.cs");

exec("./scripts/script_hatmodOverride.cs");
exec("./scripts/script_ambiantMusic.cs");
exec("./scripts/script_flashlight.cs");
exec("./scripts/script_noItemDespawn.cs");

exec("./scripts/script_faceSystem.cs"); parseFacePacks($Eventide_BaseDirectory @ "/players/faces");
exec("./scripts/script_voiceSystem.cs"); parseVoicePacks($Eventide_BaseDirectory @ "/sounds/voicePacks");
exec("./scripts/script_footsteps.cs"); parseFootstepMaterials($Eventide_BaseDirectory @ "/sounds/footsteps");

exec("./bricks/brick_ritualCircle.cs");