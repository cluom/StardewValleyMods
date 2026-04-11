## Why

用户反馈了两个问题：
1. **种子提示误导**: 当树装备了树液采集器后，玩家无法摇晃树来获取种子（这是原版行为），但模组仍然显示种子气泡，这是误导性的
2. **野树缩小不工作**: 当取消勾选"替换贴图"选项时，野树不会缩小，但果树会正常缩小

（果树泡泡被遮挡问题无法复现，暂不处理）

## What Changes

### Issue 1: 有树液采集器的树不显示种子提示
- 当树装备了树液采集器时，玩家无法摇晃树获取种子（与原版行为一致）
- 模组应该在这种情况下隐藏种子提示，避免误导玩家
- **新增配置选项**: `ShowTreeSeedTipWhenTapperInstalled`，默认为 false（不显示）

### Issue 2: 野树缩小失效
- 当 `TextureChange` 关闭但 `MinishTree` 开启时，野树不会缩小
- 果树有特殊的 `ForceMinish` 处理，但野树没有
- 原因：`TreePatch.PrefixDraw` 在 `TextureChange` 关闭时直接返回，没有设置 `CanChange = true`

## Capabilities

### New Capabilities
- `tapper-seed-tip-hiding`: 当树装备了树液采集器时，根据配置决定是否隐藏种子提示
- `wild-tree-minish-fix`: 修复野树在关闭贴图替换时无法缩小的问题

### Modified Capabilities
- 无 - 当前问题属于实现 bug 修复，不涉及需求变更

## Impact

- **受影响代码**:
  - `ModConfig.cs` - 新增配置选项
  - `TreePatch.cs` - 树的渲染逻辑（种子提示显示条件、野树缩小）
- **可能影响的功能**: 种子提示显示、野树缩小
- **兼容性**: 与原版行为同步
