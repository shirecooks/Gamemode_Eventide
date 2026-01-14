# AI Coding Assistant Instructions for Gamemode_Eventide

## Project Overview
This is a Dead by Daylight-inspired gamemode for Blockland, a sandbox game using the Torque Game Engine. The codebase consists of TorqueScript (.cs) files defining game mechanics, player classes, items, and environmental systems.

## Architecture & Key Components

### Core Structure
- **server.cs**: Main entry point that loads all components (scripts, items, players, bricks)
- **players/**: Player classes (Survivors vs Killers) with unique abilities and behaviors
- **items/**: Weapons, tools, consumables, and status effects
- **bricks/**: Custom building bricks and ritual systems
- **scripts/**: Game systems (OOP, status effects, audio, faces, etc.)
- **sounds/**: Audio assets and voice systems

### TorqueScript Patterns

#### Datablocks
Define game objects using datablocks. Always inherit from base datablocks:
```torquescript
datablock PlayerData(PlayerSurvivor : PlayerEventide)
{
    class = "PlayerSurvivor";
    superClass = "PlayerEventide";
    uiName = "Basic Survivor";
    maxTools = 3;
    // ... properties
};
```

#### OOP System
Uses custom inheritance via `superClass` field, referring to another datablock/class. Call parent methods with:
```torquescript
%this.super("onNewDatablock", %this, %obj);
```
Always call `inheritFunctionsFromSuperClass()` after datablock definition to auto-generate parent method stubs.

The system automatically introspects parent classes to create method stubs, enabling proper inheritance chains. Use `$Pref::OOP::Debug = true;` to see introspection logs.

#### Status Effects
Apply temporary effects using the status system:
```torquescript
%obj.applyStatusEffect("Epoxy", "Debuff", 6000); // 6 second duration
```
Define effects in `scripts/status_*.cs` with `beginStatusEffect` and `finalizeStatusEffect` functions.

Check for existing effects:
```torquescript
if(%obj.hasStatusEffect("Epoxy", "Debuff")) {
    // Effect is active
}
```

Remove effects manually:
```torquescript
%obj.clearStatusEffect("Epoxy", "Debuff");
```

Status effects are automatically cleaned up when players are removed.

#### Ghosting Utils
Control object visibility (scoping) to clients for performance and gameplay:
```torquescript
// Scope object to specific client
%object.adjustObjectScopeOnClient(%client, true);

// Scope object to all clients except exclusions
%object.adjustObjectScopeToAll(true, %exclusionsGroup);

// Hide object from all clients
%object.adjustObjectScopeToAll(false);
```

#### Object Collision
Custom collision handling system for ShapeBase-derived objects (players, vehicles, bricks, etc.):
```torquescript
function DatablockName::onObjectCollision(%this, %obj, %col)
{
    // Return true to allow collision, false to prevent
    return true; // or false based on custom logic
}
```
Define `onObjectCollision` functions in datablocks to control collision behavior between objects.

#### Cutscenes & Camera Control
Input locking and camera effects for cinematic sequences:
```torquescript
// Lock player inputs during cutscene
%player.lockInputs = true;

// Create automated camera orbit
%player.createCameraOrbit();

// Restore normal camera control
%player.restoreCameraFromOrbit();

// Shake camera for impact effects
%player.shakeCamera(1.0);
```
- Input locking prevents movement, tool usage, and brick planting during cutscenes
- Camera orbit switches to observer mode for automated camera movement
- Camera shake uses projectile explosions for screen effects
- Managed in `support_cutscenes.cs`

### Player Classes
- **Survivors**: Base gameplay class with fear/chase mechanics (player_survivor.cs, player_fighter.cs, player_runner.cs, etc.)
- **Killers**: Various killer types with unique abilities (killer_wrathful.cs, killer_shire.cs, killer_angler.cs, etc.)
- All killer player datablocks inherit from the `PlayerKiller` datablock
- All survivor player datablocks inherit from the `PlayerSurvivor` datablock
- All player datablocks inherit from the `PlayerEventide` datablock

### Items & Weapons
Items follow pattern: Datablock → Projectile/Explosion → Image → Placement logic
- Weapons use `ShapeBaseImageData` for equipping/using
- Projectiles handle damage and effects
- Explosions manage particle effects and area damage
- Examples: weapon_epoxyBomb.cs, weapon_butterflyKnife.cs, weapon_shotgun.cs

### Key Systems

#### Face System
Dynamic facial expressions based on game state:
```torquescript
// Initialize face system for player
%player.createFaceConfig(%facePack);

// Show specific expression
%player.faceConfigShowFace("Scared");

// Switch to sub-face pack (hurt, scared, etc.)
%player.createSubfaceConfig("Hurt");
```
- Face packs loaded from `players/faces/` directory
- Automatic blinking system with configurable timing
- Talking animations that sync mouth movements with chat messages
- Damage-based face switching (hurt expressions when health < 33%)
- Voice lines triggered by events (chase, injury, etc.)
- Configured in `script_faceSystem.cs` and `script_voiceSystem.cs`

#### Voice System
Dynamic voice line playback with cooldowns and variety:
```torquescript
// Play voice line with cooldown
%player.playVoiceLine("Chase");

// Play managed sound (multiple concurrent allowed)
%player.playManagedSound("Injury");

// Play unmanaged sound (no cooldown, for effects)
%player.playUnmanagedSound("Swing");
```
- Voice packs loaded from `sounds/voicePacks/` directory with `.etvp` and `.etsvp` files
- Automatic cooldown system prevents voice line spam (default 6 seconds)
- Smart randomization avoids playing same voice line twice in a row
- Multiple audio slots allow concurrent sound playback
- Voice lines triggered by events (chase, injury, etc.)
- Managed in `script_voiceSystem.cs`

#### Ambiant Music
Dynamic music system that changes based on game state:
```torquescript
// Play specific music track
%player.playAmbiantMusic(%musicDatablock, 1.0, "Chase");

// Play random ambient track
%player.playRandomAmbiantTrack();

// Stop ambient music
%player.stopAmbiantMusic();
```
- Ambient tracks change based on proximity to killers
- Separate system for accessory/status effect music
- Automatic music playback on player spawn
- Music stops when switching to non-Eventide classes
- Managed in `script_ambiantMusic.cs`

#### Ritual System
Interactive ritual circles that collect specific items:
```torquescript
// Ritual items inherit from base_ritual.cs
datablock ItemData(myRitualItem : ritualItem)
{
    ritualType = "MyType";           // Category for counting
    maxRitualsOnCircle = 3;          // Max items of this type per circle
    possibleOffset1 = "1 0 0.1 0 0 0"; // Position offsets on circle
    placeSound = "my_sound";         // Sound when placed
    placeSoundAmount = 1;
};
```
- Ritual circles detect dropped items via collision and `placeOnRitualCircle()` method
- Items are positioned on circle using predefined offsets, become impossible to pick up.
- Progress tracked by `ritualType` categories and total count vs `ritualsNeeded`
- Completion triggers `onAllRitualsPlaced()` event with visual/sound effects
- Automatic reset on minigame reset, cleanup on removal
- Managed in `bricks/brick_ritualCircle.cs` and `items/base_ritual.cs`

#### Footstep System
Dynamic footstep sounds based on surface materials and movement speed:
- Footstep materials loaded from `sounds/footsteps/` directory with `.etmp` files
- RGB color matching determines surface type (brick color → material type)
- Speed-based cadence calculation using logistic growth equation
- Surface detection via raycasting to determine ground material
- Automatic footstep scheduling and sound playback
- Special handling for swimming and vehicle states
- Managed in `script_footsteps.cs`

## Development Workflow

### No Build Process
This is a script-based mod loaded directly by Blockland. No compilation required.

### Testing
- Load gamemode in Blockland server
- Test in multiplayer environment
- Use console commands for debugging
- Check server logs for errors

### Debugging
- Use TorqueScript `echo()` for logging
- Console accessible via `~` key in-game
- Test status effects, player abilities, and item interactions

### File Organization
- Keep datablocks in dedicated files (e.g., `weapon_epoxyBomb.cs`)
- Group related functions with datablocks
- Use `base_*.cs` for blueprint classes that should be inherited from but not instantiated on their own
- Support scripts in `scripts/support_*.cs`

## Common Patterns

### Item Creation
1. Define particle/explosion datablocks
2. Create projectile datablock with damage/effects
3. Define item datablock with model/icon
4. Create image datablock with usage logic
5. Implement placement/activation functions

### Player Abilities
- Override base class methods for custom behavior
- Use schedules for timed effects
- Track state with SimSets (e.g., `chasingKillers`, `nearbyKillers`)

### Status Effects
- Define in separate status files (`scripts/status_*.cs`)
- Use categories for stacking rules (e.g., "Debuff", "Buff")
- Implement `beginStatusEffect` and `finalizeStatusEffect` functions
- Effects are stored in ScriptGroups with caching for performance
- Automatic cleanup occurs when objects are removed

## Dependencies
Requires specific Blockland add-ons and DLLs (see README.md). Always test with full dependency set.

## Code Style
- Use TorqueScript syntax consistently
- Comment complex logic, especially vector math
- Follow existing naming conventions
- Keep functions focused and modular

## Documentation Maintenance
When users ask about features, files, or systems not covered in these instructions, analyze the relevant code and dynamically update this document with new sections or expanded information. Include code examples, key functions, and integration details to help future AI assistants understand the codebase. Before making suggestions, analyze the relevant files and compare them to this document to ensure it is up to date.