## 1. Add Config Option for Tapper Seed Tip

- [x] 1.1 Add `ShowTreeSeedTipWhenTapperInstalled` property to `ModConfig.cs` with default value `false`
- [x] 1.2 Add i18n translation keys for the new option in all language files
- [x] 1.3 Add the new option to the config menu in `CreateConfigMenu()`
- [x] 1.4 Test that the new config option appears and works correctly

## 2. Hide Seed Tip for Trees with Tappers

- [x] 2.1 Add tapper detection logic in TreePatch.PrefixDraw to check if tree has a tapper attached
- [x] 2.2 Skip drawing seed tip when tapper is detected AND `ShowTreeSeedTipWhenTapperInstalled` is false
- [x] 2.3 Test that seed tips are hidden for trees with tappers (default) but shown when configured

## 3. Fix Wild Tree Minish When TextureChange Disabled

- [x] 3.1 Add ForceMinish logic in TreePatch.PrefixDraw when MinishTree is on but TextureChange is off
- [x] 3.2 Test that wild trees shrink correctly when only MinishTree is enabled (TextureChange off)
