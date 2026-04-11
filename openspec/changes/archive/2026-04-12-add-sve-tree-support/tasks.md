## 1. Add SVE Tree Type Enums

- [x] 1.1 Add `Pear`, `Nectarine`, `Persimmon`, `MoneyTree` entries to `TreeTypeEnum.cs` following RSV pattern

## 2. Add SVE Configuration Toggles

- [x] 2.1 Add `ChangePear`, `ChangeNectarine`, `ChangePersimmon`, `ChangeMoneyTree` properties to `ModConfig.cs`
- [x] 2.2 Add i18n translation keys in `i18n/default.json` for all 4 SVE trees (name and tooltip)

## 3. Add SVE Detection in ModEntry

- [x] 3.1 Add `FlashShifter.StardewValleyExpandedCP` mod ID constant
- [x] 3.2 Add SVE detection check using `Helper.ModRegistry.IsLoaded`
- [x] 3.3 Register SVE tree types conditionally when SVE is loaded and config toggles are enabled
- [x] 3.4 Add `sve_fruit_trees` page to config menu when SVE is loaded (mirror RSV's `ridgeside_fruit_trees` page pattern)
- [x] 3.5 Add page link and create page with all 4 SVE tree toggles in `CreateConfigMenu()`

## 4. Add SVE Fruit Tree Detection in FruitTreePatch

- [x] 4.1 Add `isSve` detection via texture path check (`FlashShifter.StardewValleyExpandedCP`)
- [x] 4.2 Add `GetSveTreeEnabled(row)` method mirroring `GetRsvTreeEnabled(row)` pattern
- [x] 4.3 Integrate SVE detection into `Prefix` method's texture replacement logic

## 5. Add SVE Wild Tree Support in TreePatch

- [x] 5.1 Investigate SVE's Birch and Fir treeType values in SVE codebase
- [x] 5.2 Add SVE wild tree types to `ControlTreeTypeValues` HashSet in TreePatch.cs
- [x] 5.3 Add SVE wild tree detection (treeType starts with `FlashShifter.StardewValleyExpandedCP`)
