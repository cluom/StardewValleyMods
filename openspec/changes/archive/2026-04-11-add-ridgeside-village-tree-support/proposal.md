## Why

ControlTree v1.1.5 provides tree highlighting, texture replacement, and minimization features, but only supports vanilla wood tree types (Oak, Maple, Pine, etc.). The Ridgeside Village mod adds 8 custom fruit tree types which are not recognized by ControlTree, causing compatibility issues where these trees are ignored or incorrectly processed.

## What Changes

- Add support for Ridgeside Village's 8 custom fruit tree types in ControlTree's tree type detection system
- Extend `TreeTypeEnum` to include Ridgeside Village tree type identifiers
- Add configuration options to enable/disable processing of each Ridgeside Village tree type
- Ensure highlighting, texture replacement, and minimization features work correctly with these custom trees

## Capabilities

### New Capabilities

- `ridgeside-fruit-tree-support`: Add recognition and processing support for Ridgeside Village's 8 custom fruit tree types (Cherry Pluot, Desert Tangelo, Ember Blood Lime, Highland Jostaberry, Mountain Plumcot, Northern Limequat, Paradise Rangpur, Tropi Ugli Fruit). Each tree type will have a corresponding toggle in the mod config.

## Impact

- **Modified Files**:
  - `ControlTree/Framework/TreeTypeEnum.cs` - Add Ridgeside Village tree type definitions
  - `ControlTree/Framework/ModConfig.cs` - Add per-tree-type boolean flags for Ridgeside Village trees
  - `ControlTree/Patches/TreePatch.cs` - Update `ControlTreeType` HashSet population to include Ridgeside Village trees
- **New Dependencies**: None (Ridgeside Village detection via content pack unique ID)
- **Compatibility**: Requires Ridgeside Village mod to be installed; gracefully skips if not present
