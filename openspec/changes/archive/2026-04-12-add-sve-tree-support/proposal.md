## Why

ControlTree currently supports fruit tree texture replacement for vanilla trees and Ridgeside Village (RSV) trees. SVE (Stardew Valley Expanded) adds 4 unique fruit trees (Pear, Nectarine, Persimmon, Money Tree) that users want to highlight/replace with custom textures, but there's no support for them yet.

## What Changes

- Add SVE fruit tree detection (via `FlashShifter.StardewValleyExpandedCP` texture identifier)
- Add 4 SVE fruit tree type enums (Pear, Nectarine, Persimmon, Money Tree)
- Add 4 configuration toggles in ModConfig for each SVE fruit tree
- Add SVE detection in FruitTreePatch to route to correct config
- Add SVE page to config menu with translation keys
- Add i18n translations for all 4 SVE fruit trees
- **Add SVE wild tree support (Birch, Fir) in TreePatch via ControlTreeTypeValues**

## Capabilities

### New Capabilities
- `sve-tree-support`: Support for highlighting/replacing SVE (Stardew Valley Expanded) fruit tree and wild tree textures. This mirrors the existing RSV tree support pattern.

### Modified Capabilities
- `wild-tree-support`: SVE wild trees (Birch, Fir) use `ControlTreeTypeValues` and TreePatch - need to add SVE's tree type values to the collection.

### Modified Capabilities
- (none)

## Impact

**Files Modified:**
- `ControlTree/Framework/TreeTypeEnum.cs` - Add SVE fruit tree type constants (Pear, Nectarine, Persimmon, Money Tree)
- `ControlTree/Framework/ModConfig.cs` - Add `ChangePear`, `ChangeNectarine`, `ChangePersimmon`, `ChangeMoneyTree` toggles
- `ControlTree/ModEntry.cs` - Add SVE mod detection and conditional registration for fruit trees
- `ControlTree/Patches/FruitTreePatch.cs` - Add SVE fruit tree identification and row mapping
- `ControlTree/Patches/TreePatch.cs` - Add SVE wild tree (Birch, Fir) to ControlTreeTypeValues
- `ControlTree/i18n/default.json` - Add SVE fruit tree translation keys

**Dependencies:**
- Requires SVE mod (`FlashShifter.StardewValleyExpandedCP`) installed for feature to activate
- SVE fruit trees use `TextureSpriteRow` 0-3 (same texture sheet for all 4 trees)
- SVE wild trees (Birch, Fir) identified via treeType value matching in TreePatch
