## Context

用户反馈了两个与渲染相关的问题：

1. **种子提示误导**: 当树装备了树液采集器后，玩家无法摇晃树来获取种子（这是原版行为），但模组仍然显示种子气泡，这是误导性的
   - 表现：装了采集器的树显示种子提示，但玩家实际无法获取种子
   - 原因：这是原版行为 - 装备采集器后无法摇晃。模组只需同步这个行为
   - 新增配置选项让玩家控制这个行为

2. **野树缩小失效**: 当 `TextureChange` 关闭但 `MinishTree` 开启时，野树不会缩小
   - 表现：取消勾选"替换贴图"后，野树不缩小，但果树正常缩小
   - 原因：`TreePatch.PrefixDraw` 在 `TextureChange` 关闭时直接返回，没有设置 `CanChange = true`

（果树泡泡被遮挡问题无法复现，暂不处理）

## Goals / Non-Goals

**Goals:**
- 当树装备了树液采集器时，根据配置隐藏种子提示
- 修复野树在关闭贴图替换时无法缩小的问题

**Non-Goals:**
- 不改变现有的渲染效果（缩小、透明化）的工作方式
- 不添加新功能（果树泡泡问题暂不处理）

## Decisions

### Issue 1: 有树液采集器的树不显示种子提示

**分析:**
- 在原版中，当树装备了树液采集器后，玩家无法摇晃树来获取种子
- 模组的 `ShowTreeSeedTips` 功能在这种情况下仍然显示种子提示，这是误导性的
- 需要检查树是否装备了树液采集器，如果是则根据配置决定是否隐藏种子提示

**方案:**
- 新增配置选项 `ShowTreeSeedTipWhenTapperInstalled`，默认为 `false`
- 在 `TreePatch.PrefixDraw` 中，检查树是否装备了树液采集器
- 如果有采集器且配置为 false，则不显示种子提示
- 如果有采集器且配置为 true，则显示种子提示

**代码修改位置:**
- `ModConfig.cs` - 新增配置属性
- `TreePatch.cs` 的 `PrefixDraw` - 检查采集器和配置

### Issue 2: 野树缩小失效

**分析:**
- 果树有特殊处理：当 `TextureChange` 关闭但 `MinishTree` 开启时，设置 `ForceMinish = true`
- 野树没有这个处理，直接在 `TextureChange` 关闭时返回
- `SpriteBatchPatch` 需要 `CanChange = true` 才会应用缩小逻辑

**方案:**
- 在 `TreePatch.PrefixDraw` 中，当 `MinishTree` 开启但 `TextureChange` 关闭时
- 设置 `SpriteBatchPatch.ForceMinish = true` 和 `SpriteBatchPatch.CanChange = true`
- 参考 `FruitTreePatch.cs` 中的实现方式

**代码修改位置:** `TreePatch.cs` 的 `PrefixDraw` 中，在检查 `TextureChange` 之前

## Risks / Trade-offs

- **采集器检测性能**: 需要遍历 objects 查找采集器，可能影响性能。需要优化检测逻辑
