## 1. 调查与复现

- [x] 1.1 已通过直接修复代替枚举复现；日志 + 源贴图比对已定位所有问题
- [x] 1.2 已在 `FruitTreePatch.PreDraw` 加诊断 Info 日志（按 `(displayName, texture, row)` 去重）；确认 mango row=8、banana row=7
- [x] 1.3 直接用 1.6 解包 `fruitTrees.png` 与 SVE `SVE_Fruit_Trees.png` 生成目标预缩放图
- [x] 1.4 通过像素 bbox 比对确认 vanilla 旧预缩放仅覆盖 row 0-5，SVE 分区缺失

## 2. 新增 mango 配置与映射

- [x] 2.1 在 `ModConfig.cs` vanilla 果树段新增 `public bool ChangeMango { get; set; } = true;`
- [x] 2.2 在 `FruitTreePatch.IsVanillaTreeEnabled` 中加入 `7 => config.ChangeMango`
- [x] 2.3 若项目有 GMCM 配置注册代码，注册 `ChangeMango` 选项（已同步补齐 11 个 i18n 翻译）

## 3. 修复预缩放贴图缺失 / 错位

- [x] 3.1 从 1.6 解包的 `fruitTrees.png` (432×720) 为 row 7 (banana)、row 8 (mango) 生成 50% 缩放、底部居中的预缩放图，扩展 `vanilla_fruit_trees_resized.png` 到 432×720
- [x] 3.2 从 SVE 源 `SVE_Fruit_Trees.png` (432×320) 生成 `sve_fruit_trees_resized.png` 并加入 `textures.json`
- [x] 3.3 在 `FruitTreePatch.PreDraw` 更新 `hasResizedTexture` 与 `textureKey`：vanilla row 0-5/7/8、RSV 全部、SVE 0-3 均走贴图替换；row 6（1.6 未使用）与未登记 row 回退到 `ForceMinish`
- [x] 3.4 游戏内开/关 `Change*` 验证渲染结果：通过

## 4. 收紧未知 row 的替换守卫

- [x] 4.1 将 `IsVanillaTreeEnabled` 的 `_ => true` 默认分支改为 `_ => false`
- [x] 4.2 将 `IsRsvTreeEnabled` 的 `_ => true` 默认分支改为 `_ => false`
- [x] 4.3 将 `IsSveTreeEnabled` 的 `_ => true` 默认分支改为 `_ => false`
- [x] 4.4 在 `PreDraw` 内补充：`rowInRange` 校验未登记 row 直接 return 且触发 Trace 日志
- [~] 4.5 无第三方非登记果树素材可验证；代码层已通过 `_ => false` 与 `rowInRange` 双重守卫保证安全

## 5. 修复果实气泡图层深度

- [x] 5.1 基于 Stardew `FruitTree.draw` 的树冠 layerDepth 偏移量级（~1E-03）确定需抬升到 2E-02 级别
- [x] 5.2 将 `DrawFruitTip` 中 bubble / item / count 的 `layerDepth` 偏移改为 `+2E-02f / +2.1E-02f / +2.2E-02f`
- [x] 5.3 游戏内 vanilla / RSV / SVE 各一棵成熟带果实果树验证：气泡与物品图标在树冠之前
- [x] 5.4 多棵同屏气泡按 Y 坐标正确排序：通过

## 6. 缩小后隐藏树干上的大果实

- [x] 6.1 在 `SpriteBatchPatch` 新增 `HideFruitOnTree` 属性，`FruitTreePatch.PreDraw` 所有激活分支置 true，`PostDraw` 清零
- [x] 6.2 在 `SpriteBatchPatch.Prefix_Draw` 中当 `HideFruitOnTree == true` 且 `sourceRectangle` ≤32×32 且非 mouseCursors / 非替换贴图时，返回 `false`
- [x] 6.3 替换/缩小后树干大果实消失、气泡正常：通过
- [x] 6.4 未启用替换的果树树干大果实按原版渲染：通过

## 7. 日志与可观测性

- [x] 7.1 `LogUnknownRow` 使用 `LoggedUnknownRows` HashSet 去重，以 `LogLevel.Trace` 输出 `(texturePath, row)`；`TreePatch.LogMonitor` 改为 `internal`
- [~] 7.2 无第三方未登记果树可触发该 Trace 日志；去重逻辑已通过代码审查确认（`LoggedUnknownRows.Add` 返回 false 即跳过）

## 8. 回归测试与收尾

- [x] 8.1 `TextureChange` 关 + `MinishTree` 开（`ForceMinish` 分支）vanilla/RSV/SVE 验证：通过
- [x] 8.2 全部果树配置关闭，不触发替换/缩小：通过
- [~] 8.3 只保留修复后截图（`脚本缩小.png` / `没处理.png` / `替换贴图.png`），无修复前对比
- [x] 8.4 更新 mod 版本号（manifest 1.1.8 → 1.1.9）；commit 留给用户
