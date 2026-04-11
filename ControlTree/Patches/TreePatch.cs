using ControlTree.Framework;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TerrainFeatures;

namespace ControlTree.Patches;

[Harmony]
public static class TreePatch
{
    // ReSharper disable once InconsistentNaming
    // 模组配置
    private static ModConfig? _config;

    // 被控制的树木集合
    private static readonly HashSet<NetString> ControlTreeType = new();

    // 用于字符串值比较的集合（解决 NetString 引用相等的问题）
    private static readonly HashSet<string> ControlTreeTypeValues = new();

    // 调试用 Monitor
    private static IMonitor? LogMonitor;

    // 供外部访问的配置
    internal static ModConfig? Config => _config;

    // 贴图映射
    public static readonly Dictionary<string, Texture2D> TextureMapping = new();

    // 高亮框贴图
    private static readonly Texture2D HighlightBoxTexture;

    static TreePatch()
    {
        // 初始化高亮框贴图
        HighlightBoxTexture = CreateTransparentTexture(60, 60, 4);
    }

    public static void InitConfig(ModConfig config, IMonitor monitor)
    {
        _config = config;
        LogMonitor = monitor;
    }

    // 用于添加或移除被控制的树木
    public static void ChangeTreeType(NetString treeType, bool flag = true)
    {
        var value = treeType.Value;
        if (flag)
        {
            ControlTreeType.Add(treeType);
            if (value != null) ControlTreeTypeValues.Add(value);
            return;
        }

        if (ControlTreeType.Contains(treeType))
        {
            ControlTreeType.Remove(treeType);
        }
        if (value != null && ControlTreeTypeValues.Contains(value))
        {
            ControlTreeTypeValues.Remove(value);
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    // 用于绘制提示物品
    private static void DrawTipItem(Tree tree, ParsedItemData itemData, float offsetLayer = 0f)
    {
        var tileLocation = tree.Tile;
        var totalGameTime = Game1.currentGameTime.TotalGameTime;
        var globalPosition1 = tileLocation * 64f + new Vector2(
            -8.0f,
            (float)(4.0 * Math.Round(Math.Sin(totalGameTime.TotalMilliseconds / 250.0), 2)) - 64f
        );
        var vector22 = new Vector2(40f, 36f);
        Game1.spriteBatch.Draw(
            Game1.mouseCursors,
            Game1.GlobalToLocal(Game1.viewport, globalPosition1),
            new Rectangle(141, 465, 20, 24),
            Color.White * 0.75f,
            0.0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            1000f + tileLocation.Y + offsetLayer
        );

        Game1.spriteBatch.Draw(
            itemData.GetTexture(),
            Game1.GlobalToLocal(Game1.viewport, globalPosition1 + vector22),
            itemData.GetSourceRect(),
            Color.White * 0.75f,
            0.0f,
            new Vector2(8f, 8f),
            4f,
            SpriteEffects.None,
            1001f + tileLocation.Y + offsetLayer
        );
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    // 用于绘制高亮框
    private static void DrawHighlightBox(Tree tree, Color color)
    {
        if (Config is null) return;
        var tileLocation = tree.Tile;

        var sinValue = (float)Math.Abs(Math.Sin((double)DateTime.Now.Ticks / TimeSpan.TicksPerSecond * Math.PI));

        // 绘制高亮框
        Game1.spriteBatch.Draw(
            HighlightBoxTexture,
            Game1.GlobalToLocal(Game1.viewport, new Vector2(tileLocation.X * 64f + 2f, tileLocation.Y * 64f + 2f)),
            null,
            // ReSharper disable once PossibleLossOfFraction
            color * (
                0.2f + sinValue * 0.8f
            ),
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0f
        );
    }

    // 用于创建框状贴图
    private static Texture2D CreateTransparentTexture(int width, int height, int borderWidth)
    {
        var texture = new Texture2D(Game1.graphics.GraphicsDevice, width, height);
        var data = new Color[width * height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var isBorder = y < borderWidth ||
                               y >= height - borderWidth ||
                               x < borderWidth ||
                               x >= width - borderWidth;
                data[y * width + x] = isBorder ? Color.White : Color.Transparent;
            }
        }

        texture.SetData(data);
        return texture;
    }

    // 用于透明化树木
    private static void TransparentTree(Vector2 tile)
    {
        if (!Config!.TransparentTree)
        {
            return;
        }
        
        var downTile = tile + new Vector2(0, 1);
        Game1.currentLocation.terrainFeatures.TryGetValue(downTile, out var tempObject);
        if (tempObject is Tree tree && tree.growthStage.Value >= 5)
        {
            tree.alpha = 0.2f;
        }

        if (Config is not { TextureChange: false, MinishTree: false }) return;

        for (var offX = -1; offX <= 1; offX++)
        {
            for (var offY = 2; offY <= 5; offY++)
            {
                var offsetTile = tile + new Vector2(offX, offY);
                Game1.currentLocation.terrainFeatures.TryGetValue(offsetTile, out var offsetObject);
                if (offsetObject is Tree offsetTree && offsetTree.growthStage.Value >= 5)
                {
                    offsetTree.alpha = 0.2f;
                }
            }
        }
    }
    
    [HarmonyPrefix, HarmonyPatch(typeof(Tree), "draw")]
    // ReSharper disable once InconsistentNaming
    // ReSharper disable once UnusedMember.Global
    public static void PrefixDraw(Tree __instance)
    {
        if (Config is not { ModEnable: true })
        {
            return;
        }

        switch (__instance.growthStage.Value)
        {
            // 绘制种子高亮
            case 0 when Config.HighlightTreeSeed:
            {
                if (Config.NotHighlightTreeSeedByFertilized && __instance.fertilized.Value) return;
                TransparentTree(__instance.Tile);

                DrawHighlightBox(__instance, Config.HighlightTreeSeedColor);
                break;
            }
            case 0:
                break;
            case <= 2 when Config.HighlightSapling:
            {
                if (Config.NotHighlightTreeSeedByFertilized && __instance.fertilized.Value) return;
                var downTile = __instance.Tile + new Vector2(0, 1);
                Game1.currentLocation.terrainFeatures.TryGetValue(downTile, out var tempObject);
                TransparentTree(__instance.Tile);

                DrawHighlightBox(__instance, Config.HighlightSaplingColor);
                break;
            }
        }

        // 绘制种子提示
        if (__instance.hasSeed.Value && Config.ShowTreeSeedTips && __instance.growthStage.Value >= 5)
        {
            // 检测树上是否装有树液采集器
            bool hasTapper = Game1.currentLocation.objects.TryGetValue(__instance.Tile, out var tileObj) &&
                             tileObj.IsTapper();
            bool shouldShowSeedTip = !hasTapper || Config.ShowTreeSeedTipWhenTapperInstalled;

            if (shouldShowSeedTip)
            {
                DrawTipItem(
                    __instance,
                    ItemRegistry.GetDataOrErrorItem(
                        ItemRegistry.Create(__instance.GetData().SeedItemId).QualifiedItemId
                    ),
                    5f
                );
            }
        }

        // 绘制苔藓提示
        if (__instance.hasMoss.Value && Config.ShowTreeMossTips && __instance.growthStage.Value >= 5)
        {
            DrawTipItem(
                __instance,
                ItemRegistry.GetDataOrErrorItem(ItemRegistry.Create("(O)Moss").QualifiedItemId)
            );
        }

        var treeType = __instance.treeType;
        var treeTypeValue = treeType.Value;

        // 如果树木不在被控制的树木集合中 则返回（依靠 ControlTreeTypeValues 配置检查，不绕过设置）
        // 使用字符串值比较解决 NetString 引用相等的问题
        if (treeTypeValue == null || !ControlTreeTypeValues.Contains(treeTypeValue))
        {
            return;
        }

        if (__instance.stump.Value && !__instance.falling.Value)
        {
            return;
        }

        if (__instance.growthStage.Value < 5)
        {
            return;
        }

        // 如果不启用贴图替换，检查是否需要缩小（只缩小不换贴图）
        if (!Config!.TextureChange)
        {
            if (Config.MinishTree)
            {
                // 不设置 ForceMinish，使用普通 MinishTree 路径（带位置修正）
                SpriteBatchPatch.CanChange = true;
            }
            return;
        }

        // 设置标记CanChange为true
        SpriteBatchPatch.CanChange = true;
        if (__instance.TextureName is null)
        {
            return;
        }

        // 获取要替换的树木贴图 如果没有则返回 如果有则设置SpriteBatchPatch.Texture为对应的贴图
        var textureName = __instance.TextureName.Replace("TerrainFeatures\\", "");
        // 移除Mods\\前缀（适用于SVE等mod的树）
        if (textureName.StartsWith("Mods\\")) textureName = textureName.Substring(5);
        // 移除mod ID前缀（适用于 FlashShifter.StardewValleyExpandedCP_Birch_Tree 格式）
        if (textureName.Contains("_Tree") || textureName.Contains("_Sapling"))
        {
            var parts = textureName.Split('\\', '/');
            textureName = parts[^1]; // 取最后一部分作为文件名
        }
        var key = textureName + ".png";
        if (TextureMapping.TryGetValue(key, out var value)) SpriteBatchPatch.Texture = value;
    }

    [HarmonyPostfix, HarmonyPatch(typeof(Tree), "draw")]
    // ReSharper disable once UnusedMember.Global
    public static void PostfixDraw()
    {
        // 重置各种标记
        SpriteBatchPatch.CanChange = false;
        SpriteBatchPatch.Texture = null;
        SpriteBatchPatch.ForceMinish = false;
    }
}