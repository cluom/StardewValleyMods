## Context

ControlTree 通过 Harmony 对 `FruitTree.draw` 做 Prefix/Postfix，并在 Prefix 阶段根据 `FruitTreeData.TextureSpriteRow` 与 `data.Texture` 路径判断当前果树属于 Vanilla / RSV / SVE，然后通过 `SpriteBatchPatch` 将 `SpriteBatch.Draw` 的贴图替换为预缩放的 `vanilla_fruit_trees_resized.png` 或 `fruit_trees_resized.png`。完成 `draw` 后在 `PostDraw` 中额外渲染果实气泡提示。

当前实现存在三个问题：
1. **贴图消失**：实测香蕉树（vanilla row 6）、芒果树（vanilla row 7）贴图消失。代码 `IsVanillaTreeEnabled` 的 `switch` 只覆盖 row 0–6，`ModConfig` 中完全缺少 `ChangeMango`，所以 row 7 (mango) 走 `_ => true` 默认分支被强制替换，而 `vanilla_fruit_trees_resized.png` 则不一定含有 row 7；香蕉的消失则可能是资源文件 row 6 缺失或 y 偏移错位。
2. **贴图错位**：实测 SVE 的 Pear / Nectarine / Persimmon 被错误替换为另一棵树的贴图。原因：`IsSveTreeEnabled` 中代码侧映射为 `0=Pear, 1=Nectarine, 2=Persimmon, 3=MoneyTree`，但 `fruit_trees_resized.png` 的 SVE 分区（紧接 RSV 8 行之后的 4 行）行顺序不一定与该映射一致，导致 `SpriteBatch.Draw` 使用同一个 source rect 取到的是错位的 row。
3. **贴图错位二次原因**：`PreDraw` 中 `isVanilla` 的回退条件过于宽松：`data.Texture == null` 或路径包含 `TileSheets/fruitTrees` 即视为原版，随后 `_ => true` 默认分支让任何未知 row 也进入替换流程。
4. **气泡层级**：`baseLayer + 1E-06f` 低于 `FruitTree.draw` 内部树冠绘制使用的偏移，导致气泡被树冠覆盖。
5. **树干上大果实残留**：`FruitTree.draw` 会在树冠位置绘制 `fruitsOnTree` 数量的大果实（调用 `ItemRegistry.GetDataOrErrorItem(...).GetSourceRect()` 并直接 `SpriteBatch.Draw` 到 tile 上方）。当 ControlTree 把树缩小一半后，这些大果实仍按原尺寸绘制，视觉上果实比树大。因为已有气泡提示展示果实，这些树上果实可被整体隐藏。

## Goals / Non-Goals

**Goals:**
- 修复香蕉树 / 芒果树贴图消失（vanilla row 6 / row 7），补全 mango 配置。
- 修复 SVE Pear / Nectarine / Persimmon 贴图错位。
- 避免未知 row / 第三方果树被错误替换为已知树种的缩小贴图。
- 果实气泡、物品图标、数量文字始终渲染在果树前方。
- 果树被缩小时，树干上的大果实图元不再按原尺寸露出（通过气泡提示展示果实信息）。
- 保持对原版、RSV、SVE 三类果树的现有替换与缩小行为不回归。

**Non-Goals:**
- 不新增对其他 mod 果树（非 RSV/SVE）的专属替换贴图支持。
- 不改变 `MinishTree` / `TextureChange` 的配置语义。
- 不重构 `SpriteBatchPatch` 的通用裁剪/缩放逻辑。

## Decisions

### 决策 1：贴图缺失 / 错位的修复方式 — 系统性校对 + 补齐 mango 配置
方案 A（采纳）：
- 对 `vanilla_fruit_trees_resized.png` 做一次完整行比对（row 0–7，含 banana 与 mango），确认每一行 y 起点、行高、行宽与原版 `TileSheets/fruitTrees` 按 50% 缩放的结果一致；缺失 row 6 / row 7 则补齐。
- 对 `fruit_trees_resized.png` 的 RSV 分区（行 0–7）和 SVE 分区（行 8–11 或现有位置）做同样比对；特别核对 Pear / Nectarine / Persimmon / MoneyTree 的行顺序是否与 `IsSveTreeEnabled` 的映射一致。若资源内部行顺序与代码约定相反，则修正资源（而非代码），避免 RSV 分区跟着移动。
- 在 `ModConfig` 新增 `ChangeMango` 字段并在 `IsVanillaTreeEnabled` 加入 `7 => config.ChangeMango`。

方案 B（放弃）：仅针对已报告的 5 种树做特判补丁。只能修当前发现的样例，后续新增树种需反复打补丁，且不解决 `ModConfig` 缺 mango 的根本问题。

### 决策 2：未知 row 不再回退到替换贴图
在 `IsVanillaTreeEnabled` / `IsRsvTreeEnabled` / `IsSveTreeEnabled` 中，将 `_ => true` 的默认分支改为 `_ => false`，即「未登记的 row 一律视为未启用替换」。这样可避免第三方果树借用原版贴图路径时被错误替换。同时在 `PreDraw` 中收紧 `isVanilla` 的判定：若 `data.Texture == null` 但 `TextureSpriteRow` 超出 0–6，也不再认定为原版。

### 决策 3：果实气泡图层深度
对照 `FruitTree.draw` 的树冠绘制偏移（通常在 `(getBoundingBox().Bottom - 1) / 10000f + 某个量级 1E-03 ~ 1E-02` 的范围内），将 `DrawFruitTip` 的层级偏移统一抬升一个明确足够大的量级：气泡背景 `+ 2E-02f`，物品图标 `+ 2.1E-02f`，数量 `+ 2.2E-02f`。这确保果实提示在树冠之上，同时仍低于 UI/mouse 层。

备选方案（放弃）：调用 `spriteBatch` 时使用单独 `SpriteSortMode.Immediate`。会打断原批次导致性能抖动且可能与其他 mod 冲突。

### 决策 3a：隐藏缩小后树干上的大果实
当 `SpriteBatchPatch.CanChange == true`（即本棵果树启用了替换或 `ForceMinish`）时，在 `SpriteBatchPatch.Prefix_Draw` 中识别来自 `FruitTree.draw` 的"树上大果实"绘制并返回 `false` 取消渲染。识别特征：
- 调用来自 `FruitTree.draw`（通过已设置的 `CanChange` 标记判定调用上下文）；
- `texture` 既不是 `Game1.mouseCursors` 也不是当前树的贴图（`data.Texture` / 替换后的 `Texture` 字段）；
- 或通过 `origin != Vector2.Zero` + `texture` 为 item sprite sheet（`Game1.objectSpriteSheet` / 物品 `ItemRegistry` 贴图）。

实现思路：在 `FruitTreePatch.PreDraw` 中记录当前是否需要隐藏果实（`HideFruitOnTree=true`），`SpriteBatchPatch` 检测到上述特征时 `return false`，`PostDraw` 清理标记。物品信息已由 `DrawFruitTip` 的气泡展示，不会丢失可读性。

备选方案（放弃）：反射清空 `FruitTree.fruitsOnTree` 再在 Postfix 恢复。改动游戏运行期状态风险高，且无法与其他 mod 共存。

### 决策 4：验证路径 — 覆盖所有 row，不抽样
在实现完成后，对每一个已登记 row 分别种植或召唤一次并截图比对（vanilla row 0–7 共 8 种、RSV row 0–7 共 8 种、SVE row 0–3 共 4 种，合计 20 种），而非仅抽样几棵。理由：本次已发现 5 棵受影响，其他 row 存在相同类型问题的可能性不能用抽样排除。对每棵树确认：
- 贴图替换正确且不错位。
- 成熟时气泡、物品、数量均在树冠之前。
- 关闭 `TextureChange`、仅开启 `MinishTree` 的情况下（`ForceMinish` 分支）同样工作。

## Risks / Trade-offs

- [未知 row fallback 收紧可能让某些此前"恰好能用"的第三方果树失去缩小效果] → 通过日志输出 `TextureSpriteRow` 和 `data.Texture`，便于用户反馈并可在配置中后续扩展支持。
- [补齐预缩放贴图需要美术资源对齐] → 不预设只缺某一行；先按 50% 比例从原版 / RSV / SVE 源贴图完整重新生成一份参考图，再与现有 `*_resized.png` 做像素级比对，定位所有缺失或错位的 row 一并修正。
- [层级偏移量抬升到 2E-02f 量级，可能与极端边缘的其他图元竞争] → 仍远小于 UI/HUD 层（1f 附近），风险可控。
- [隐藏树上大果实后，玩家可能短暂失去"这棵树有果子"的直觉提示] → 由已有的气泡（含数量）承担该信息，且该分支仅在已经启用替换/缩小时生效。
- [`ModConfig` 新增 `ChangeMango` 字段可能破坏旧配置文件] → 使用 SMAPI `ReadConfig` 的默认值填充即可，无迁移成本。

## Migration Plan

- 代码变更只影响 ControlTree mod 的运行期行为，无存档/配置迁移需求。
- 若替换贴图文件被修改，随 mod 包发布即可，玩家覆盖安装生效。
- 回滚策略：回退到上一个 commit 即可。
