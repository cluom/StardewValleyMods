## Context

ControlTree mod highlights fruit trees and allows texture replacement. It already supports vanilla trees and RSV (Ridgeside Village) trees. SVE (Stardew Valley Expanded) adds:
- 4 fruit trees (Pear, Nectarine, Persimmon, Money Tree) - handled by FruitTreePatch.cs
- 2 wild trees (Birch, Fir) - handled by TreePatch.cs

SVE fruit trees are identified via texture path containing `FlashShifter.StardewValleyExpandedCP`. All 4 SVE fruit trees share a single texture sheet `SVEFruitTrees` with `TextureSpriteRow` 0-3. SVE wild trees use treeType value matching.

## Goals / Non-Goals

**Goals:**
- Detect SVE fruit trees via texture path matching
- Allow per-tree texture replacement toggles (Pear, Nectarine, Persimmon, Money Tree)
- Add SVE wild trees (Birch, Fir) to ControlTreeTypeValues for texture replacement
- Mirror the existing RSV implementation pattern exactly

**Non-Goals:**
- Modify any existing RSV or vanilla tree behavior

## Decisions

**Decision: Identify SVE trees via texture path containing `FlashShifter.StardewValleyExpandedCP`**
- Rationale: Same pattern used for RSV (`Rafseazz.RSVCP`). SVE uses this mod ID in all its asset paths.
- Alternative: Could check `treeType.Value` against SVE sapling IDs, but texture path is more robust.

**Decision: Use `TextureSpriteRow` 0-3 to index SVE trees**
- Rationale: SVE defines all 4 trees in one texture sheet. SpriteRow matches the texture row index.
- Same pattern as RSV which uses rows 0-7 for its 8 trees.

**Decision: Follow RSV pattern exactly for config, i18n, and ModEntry**
- Rationale: Consistency reduces bugs and makes maintenance easier.
- Config toggles, tree type enums, and detection will all follow RSV naming conventions.

## Risks / Trade-offs

[SVE texture sheet reorder] → Mitigation: SpriteRow mapping documented from SVE's FruitTrees.json; if order changes, user can toggle individual trees off.

[Future SVE updates add trees] → Mitigation: System extends naturally - just add new enum, config toggle, and row mapping.

[SVE wild tree type values unknown] → Mitigation: Need to inspect SVE's TreeType values in code. Pattern should follow existing TreePatch logic.

## Open Questions

- What are SVE's exact treeType values for Birch and Fir? Need to verify from SVE codebase before implementation.
