## 1. Update TreeTypeEnum.cs

- [x] 1.1 Add RidgesideVillage enum with tree type identifiers (Rafseazz.RSVCP_Cherry_Pluot, Rafseazz.RSVCP_Desert_Tangelo, etc.)

## 2. Update ModConfig.cs

- [x] 2.1 Add 8 boolean fields: ChangeCherryPluot, ChangeDesertTangelo, ChangeEmberBloodLime, ChangeHighlandJostaberry, ChangeMountainPlumcot, ChangeNorthernLimequat, ChangeParadiseRangpur, ChangeTropiUgliFruit
- [x] 2.2 Register these fields in the GMCM integration

## 3. Update TreePatch.cs

- [x] 3.1 Add ChangeTreeType calls for all 8 Ridgeside Village trees in static constructor or initialization method
- [x] 3.2 Verify the prefix check logic (`Rafseazz.RSVCP`) correctly identifies Ridgeside trees in existing type matching

## 4. Generate Resized Textures

- [x] 4.1 Create a temporary ImageSharp tool to read `FruitTrees.png` (384x1024, 8 trees, 96x128 each)
- [x] 4.2 Generate `fruit_trees_resized.png` at 50% scale (48x64 per sprite, 192x512 total)
- [x] 4.3 Generate `fruit_tree_saplings_resized.png` at 50% scale from `FruitTreeSaplings.png` (8x8 per sprite, 64x64 total)
- [x] 4.4 Place generated textures in `ControlTree/assets/` and add entries to `textures.json`
