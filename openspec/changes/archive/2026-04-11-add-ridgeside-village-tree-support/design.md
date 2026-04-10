## Context

ControlTree v1.1.5 supports highlighting, texture replacement, and minimization for vanilla wood trees via `TreeTypeEnum.cs`. The system uses a `ControlTreeType` HashSet populated at runtime via `ChangeTreeType()` calls in `TreePatch.cs`. Each tree type is controlled by a corresponding boolean in `ModConfig.cs` (e.g., `ChangeOak`, `ChangeMaple`).

Ridgeside Village (content pack with SMAPI component) adds 8 custom fruit tree types with IDs:
- Cherry Pluot Tree, Desert Tangelo Tree, Ember Blood Lime Tree, Highland Jostaberry Tree
- Mountain Plumcot Tree, Northern Limequat Tree, Paradise Rangpur Tree, Tropi Ugli Fruit Tree

These trees are not in vanilla's tree type enum and must be detected via their unique content pack identifiers.

## Goals / Non-Goals

**Goals:**
- Add Ridgeside Village's 8 fruit tree types to ControlTree's type recognition system
- Ensure highlighting, texture replacement, and minimization work with these trees
- Add per-tree-type toggle in config for each Ridgeside Village tree

**Non-Goals:**
- Modifying Ridgeside Village's assets or behavior
- Supporting non-fruit-tree custom content from Ridgeside Village
- Adding automatic detection of other content packs (only Ridgeside Village explicitly)

## Decisions

### 1. Tree Type Identifier Format

**Decision**: Use `Rafseazz.RSVCP_<TreeName>` as the tree type identifier prefix.

**Rationale**: Ridgeside Village content pack uses `Rafseazz.RSVCP` as its namespace prefix for all custom objects. Fruit trees follow naming like `Rafseazz.RSVCP_Cherry_Pluot`. This maintains consistency with existing mod integration patterns.

**Alternatives considered**:
- Using numeric IDs: Rejected - Ridgeside Village doesn't assign fixed numeric IDs to trees
- Dynamic discovery: More complex, unnecessary for known content pack

### 2. Configuration Structure

**Decision**: Add 8 boolean fields to `ModConfig.cs` following existing naming pattern.

**Rationale**: Existing config pattern uses `Change<TreeType>` booleans. This keeps UI consistent and leverages existing GMCM rendering logic.

**Alternatives considered**:
- HashSet-based configuration: Would require custom GMCM UI components
- Grouped toggle: Adds unnecessary UI complexity for 8 options

### 3. Detection Mechanism

**Decision**: Use `Helper.ModRegistry.IsLoaded("Rafseazz.RSVCP")` to detect if Ridgeside Village is installed, and only then register RSV tree types and config options.

**Rationale**: SMAPI provides a clean mod registry API to check if another mod is loaded. This avoids any prefix-matching complexity and ensures config options only appear when RSV is present.

**Alternatives considered**:
- Prefix matching on `treeType`: Rejected — tree types only appear at render time; checking at startup via `IsLoaded` is cleaner and avoids runtime string comparisons
- Content pack API: SMAPI's `IsLoaded` already covers both the content pack and SMAPI component

### 4. Texture Replacement Assets

**Decision**: Generate 50%-scaled replacement textures from Ridgeside Village's source assets.

**Rationale**: `MinishTree` works via render-time scaling (`scale *= 0.5f`) and does not require replacement textures. However, `TextureChange` requires actual replacement texture files. To provide a complete experience, 50%-scaled versions of Ridgeside trees will be generated from the source spritesheet.

**Source assets**:
- `Ridgeside Village/Assets/Objects/FruitTrees.png` — 384x1024, 8 trees x 4 seasonal variants (96x128 each)
- `Ridgeside Village/Assets/Objects/FruitTreeSaplings.png` — 128x128, 8 saplings (16x16 each)

**Output**:
- `fruit_trees_resized.png` — 192x512, all sprites halved to 48x64
- `fruit_tree_saplings_resized.png` — 64x64, all saplings halved to 8x8

**Alternatives considered**:
- Only render-time scaling without replacement textures: Rejected — users expect texture replacement to work for custom trees too
- Ask users to provide textures: Rejected — adds friction; generating from source is automated and consistent

## Risks / Trade-offs

- **[Risk]** Ridgeside Village updates tree IDs → **Mitigation**: Make tree type strings configurable or use partial matching on prefix
- **[Risk]** Conflicts with other content packs adding similar trees → **Mitigation**: Non-destructive - only affects explicitly matched trees
- **[Trade-off]** Adding 8 new config options increases UI clutter → **Accepted**: Necessary for per-tree-type control
- **[Risk]** Scaled-down sprites may appear blurry at 50% → **Accepted**: Consistent with existing texture replacement behavior for vanilla trees
