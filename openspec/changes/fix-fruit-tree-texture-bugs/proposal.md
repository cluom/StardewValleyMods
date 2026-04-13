## Why

ControlTree 的果树贴图替换功能目前存在四个影响使用体验的缺陷：

1. **贴图消失**：已确认受影响的有香蕉树（vanilla row 6）、芒果树（vanilla row 7，`ModConfig` 中完全缺失 mango 配置与 `IsVanillaTreeEnabled` 映射）；
2. **贴图错位**：已确认 SVE 的 Pear / Nectarine / Persimmon 被替换为另一个树种的贴图，怀疑 `fruit_trees_resized.png` 的 SVE 分区 row 顺序与代码中 `0=Pear, 1=Nectarine, 2=Persimmon, 3=MoneyTree` 的映射不一致；
3. **果实气泡层级错误**：启用缩小后的果实提示气泡被渲染在树身后面，被树冠挡住；
4. **缩小后树干上残留大果实**：树被缩小了但 `FruitTree.draw` 中挂在树上的大果实（`fruitsOnTree` 绘制）没有一起缩小/隐藏，看起来果实大于整棵树。

这些问题让果树相关的 Minish/贴图替换功能在实际游玩中基本不可用，需要尽快修复。

## What Changes

- 修正 vanilla mango 支持：在 `ModConfig` 增加 `ChangeMango` 配置，在 `IsVanillaTreeEnabled` 将 row 7 映射到该配置，并确保 `vanilla_fruit_trees_resized.png` 包含 row 7。
- 核对并补齐 `vanilla_fruit_trees_resized.png`（特别是 row 6 香蕉、row 7 芒果）与 `fruit_trees_resized.png` 的全部 row。
- 修正 SVE row → `fruit_trees_resized.png` 源矩形的映射错位（Pear / Nectarine / Persimmon），使每个 row 取到的图块与其实际树种一致。
- 修正贴图替换错误 fallback：`_ => true` 默认分支改为 `_ => false`，未登记 row 不再替换。
- 修正果实提示气泡的图层深度：`DrawFruitTip` 的 `layerDepth` 偏移提升到足够高于 `FruitTree.draw` 中树冠的绘制偏移。
- **新增**：果树被替换为预缩放贴图 或 `ForceMinish` 激活时，`SpriteBatchPatch` 在 `SpriteBatch.Draw` 层取消 `FruitTree.draw` 中树上大果实的渲染（识别方式：在 `draw` 范围内、origin 不为 Vector2.Zero 且 source rect 命中原版 fruit item 贴图或特定尺寸），避免挂着一颗比树还大的果实；依赖已有的气泡提示展示果实信息。

## Capabilities

### New Capabilities
- `fruit-tree-texture-replacement`: ControlTree 对果树的贴图替换、缩小与果实提示气泡渲染的需求与行为契约。

### Modified Capabilities
<!-- 无 -->

## Impact

- 受影响代码：
  - `ControlTree/Patches/FruitTreePatch.cs`（`PreDraw` / `DrawFruitTip` / `PostDraw`）
  - `ControlTree/Patches/SpriteBatchPatch.cs`（替换逻辑守卫条件）
  - `ControlTree/assets/vanilla_fruit_trees_resized.png` 与 `ControlTree/assets/fruit_trees_resized.png`（可能需要补齐缺失/错位的行，覆盖 vanilla/RSV/SVE 全部树种）
- 无 API 变更，无破坏性配置变更。
- 依赖：保持对 SMAPI / Harmony / RSV / SVE 的现有兼容性，不引入新依赖。
