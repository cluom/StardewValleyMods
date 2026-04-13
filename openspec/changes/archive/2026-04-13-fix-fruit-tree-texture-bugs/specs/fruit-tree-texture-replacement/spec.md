## ADDED Requirements

### Requirement: 所有已登记果树的替换贴图必须可见且不错位
当玩家启用 `TextureChange` 且对应 `Change*` 配置为 true 时，任意已登记的果树（vanilla row 0–7 含 banana 与 mango、RSV row 0–7、SVE row 0–3）SHALL 渲染出与该树种匹配的完整替换贴图，禁止出现空白、透明或错位为其他树种的渲染结果。

#### Scenario: vanilla 所有 row 替换后可见（含 banana 与 mango）
- **WHEN** 玩家分别种植 vanilla 的 8 种果树（cherry / apricot / orange / pomegranate / peach / apple / banana / mango）各一棵至成熟，且 ControlTree 配置为 `ModEnable=true, TextureChange=true` 并对应 `Change*=true`
- **THEN** 每一棵的树冠与树干均以 `vanilla_fruit_trees_resized.png` 中对应 row 的预缩放贴图绘制，无空白、无错位

#### Scenario: mango 配置可用
- **WHEN** 玩家在 GMCM 配置界面打开 ControlTree 的果树分组
- **THEN** 界面包含 `ChangeMango` 条目，切换该项可控制芒果树是否被替换

#### Scenario: RSV 所有 row 替换后可见
- **WHEN** 玩家分别召唤 RSV 的 8 种果树各一棵至成熟，并启用对应 `Change*=true`
- **THEN** 每一棵均以 `fruit_trees_resized.png` 对应 row 正确渲染

#### Scenario: SVE 每棵树替换后与其本体匹配
- **WHEN** 玩家分别召唤 SVE 的 Pear / Nectarine / Persimmon / MoneyTree 各一棵至成熟，并启用对应 `Change*=true`
- **THEN** 每一棵渲染出的树冠贴图与该树本体匹配（Pear 显示梨树、Nectarine 显示油桃树、Persimmon 显示柿子树、MoneyTree 显示金钱树），不出现树种错位

#### Scenario: 关闭对应 Change 配置时保持原版
- **WHEN** 某一树种对应 `Change*=false`，其他配置不变
- **THEN** 该树以游戏原版贴图原始尺寸渲染，不触发 SpriteBatch 替换

### Requirement: 未登记 row 的果树不得被错误替换
对于 vanilla / RSV / SVE 三类果树，`TextureSpriteRow` 超出各自已登记范围时，ControlTree SHALL NOT 替换其贴图，也 SHALL NOT 对其应用 `ForceMinish` 缩小。

#### Scenario: 第三方果树使用原版贴图路径但行号越界
- **WHEN** 一棵第三方 mod 果树的 `data.Texture` 指向 `TileSheets/fruitTrees` 且 `TextureSpriteRow == 9`
- **THEN** ControlTree 跳过该果树的贴图替换与缩小，使其以该 mod 自身贴图原样渲染

#### Scenario: RSV 行号越界
- **WHEN** RSV 命名空间下的果树被检测到 `TextureSpriteRow == 12`
- **THEN** 不启用替换与缩小，`SpriteBatchPatch.CanChange` 保持为 false

### Requirement: 果实气泡 SHALL 渲染在果树树冠之前
当 `ShouldDrawFruitTip` 为 true 时，果实气泡背景、物品图标和数量文字的 `layerDepth` MUST 大于 `FruitTree.draw` 内部绘制树冠所使用的最大 `layerDepth`，以保证视觉上气泡始终可见且不被树冠遮挡。

#### Scenario: 成熟果树气泡可见
- **WHEN** 一棵成熟且结有果实的果树（任意 vanilla / RSV / SVE 支持的树种）在玩家视野内，并启用了果实提示渲染
- **THEN** 气泡背景、果实图标、数量文字均显示在树冠前方，不被任何果树自身图元遮挡

#### Scenario: 同屏多棵果树气泡层级一致
- **WHEN** 同一地图位置附近存在多棵成熟果树
- **THEN** 每棵果树自己的气泡都在各自树冠前方，且不同果树之间的气泡按 Y 坐标顺序正确排列

### Requirement: 缩小后 SHALL 隐藏树干上的大果实
当某棵果树被 ControlTree 替换或缩小（`SpriteBatchPatch.CanChange == true`）时，`FruitTree.draw` 内部绘制的"挂在树冠上的大果实图元"SHALL NOT 渲染；果实数量与种类信息由气泡提示（`DrawFruitTip`）替代展示。

#### Scenario: 被缩小的果树没有超尺寸大果实
- **WHEN** 一棵启用了替换或 `MinishTree` 的果树已有成熟果实挂在树上
- **THEN** 树干位置不再出现原尺寸大果实图元，只保留树冠上方的气泡 + 物品图标 + 数量

#### Scenario: 未启用替换的果树保持原样
- **WHEN** 对应 `Change*=false` 且 `MinishTree=false`，或该树未被 ControlTree 接管
- **THEN** 树干上的大果实按游戏默认方式渲染，不被隐藏

### Requirement: 调试日志 SHALL 记录未识别的果树
当 ControlTree 遇到 `data.Texture` 为 null 或包含原版路径、但 `TextureSpriteRow` 不在已知映射范围内的果树时，SHALL 以 SMAPI Trace 级别输出一次日志，包含 `data.Texture`、`TextureSpriteRow`、`FruitTreeId`。

#### Scenario: 未识别 row 触发 Trace 日志
- **WHEN** 在地图中出现一棵 `TextureSpriteRow == 20` 的果树
- **THEN** SMAPI Trace 日志中出现一条 ControlTree 的记录，内容包含该 row 值与贴图路径，且同一棵树不重复刷屏（通过 HashSet 去重）
