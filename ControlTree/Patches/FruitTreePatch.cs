using System.Collections.Generic;
using ControlTree.Framework;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace ControlTree.Patches;

[Harmony]
public static class FruitTreePatch
{
    // 标记是否应该绘制果实弹窗
    private static bool ShouldDrawFruitTip;

    [HarmonyPrefix, HarmonyPatch(typeof(FruitTree), "draw")]
    public static void PreDraw(FruitTree __instance)
    {
        // 重置标记
        ShouldDrawFruitTip = false;

        if (TreePatch.Config is not { ModEnable: true }) return;
        if (__instance.growthStage.Value < 4) return;

        var data = __instance.GetData();
        if (data is null) return;

        var config = TreePatch.Config;

        bool isRsv = data.Texture?.Contains("Rafseazz.RSVCP") == true;
        bool isSve = data.Texture?.Contains("FlashShifter.StardewValleyExpandedCP") == true;
        // 原版果树：Texture 为 null（默认）或明确指向原版贴图
        string texturePath = data.Texture?.Replace("\\", "/") ?? "null";
        bool isVanilla = !isRsv && !isSve && (data.Texture == null || texturePath.Contains("TileSheets/fruitTrees"));

        if (!isRsv && !isSve && !isVanilla) return;

        // 检查配置是否启用该类型
        if (isVanilla && !IsVanillaTreeEnabled(config, data)) return;
        if (isRsv && !IsRsvTreeEnabled(config, __instance)) return;
        if (isSve && !IsSveTreeEnabled(config, __instance)) return;

        var textureKey = isRsv ? "fruit_trees_resized.png" : "vanilla_fruit_trees_resized.png";

        // 设置替换贴图（预缩放版本）
        if (config.TextureChange && TreePatch.TextureMapping.TryGetValue(textureKey, out var tex))
        {
            SpriteBatchPatch.Texture = tex;
            SpriteBatchPatch.CanChange = true;
            ShouldDrawFruitTip = true;
        }

        // 当 TextureChange 关闭但 MinishTree 开启时，使用 ForceMinish 进行代码缩放
        if (!config.TextureChange && config.MinishTree)
        {
            SpriteBatchPatch.ForceMinish = true;
            SpriteBatchPatch.CanChange = true;
            ShouldDrawFruitTip = true;
        }
    }

    private static bool IsVanillaTreeEnabled(ModConfig config, StardewValley.GameData.FruitTrees.FruitTreeData data)
    {
        return data.TextureSpriteRow switch
        {
            0 => config.ChangeCherry,
            1 => config.ChangeApricot,
            2 => config.ChangeOrange,
            3 => config.ChangePomegranate,
            4 => config.ChangePeach,
            5 => config.ChangeApple,
            6 => config.ChangeBanana,
            _ => true
        };
    }

    private static bool IsRsvTreeEnabled(ModConfig config, FruitTree tree)
    {
        // RSV 果树使用 TextureSpriteRow 来区分类型 (0-7)
        // 从 FruitTreeData 获取 row
        var data = tree.GetData();
        int row = data?.TextureSpriteRow ?? 0;

        // RSV 的 8 种果树按 row 排列
        return row switch
        {
            0 => config.ChangeCherryPluot,
            1 => config.ChangeDesertTangelo,
            2 => config.ChangeEmberBloodLime,
            3 => config.ChangeHighlandJostaberry,
            4 => config.ChangeMountainPlumcot,
            5 => config.ChangeNorthernLimequat,
            6 => config.ChangeParadiseRangpur,
            7 => config.ChangeTropiUgliFruit,
            _ => true
        };
    }

    private static bool IsSveTreeEnabled(ModConfig config, FruitTree tree)
    {
        // SVE 果树使用 TextureSpriteRow 来区分类型 (0-3)
        // 从 FruitTreeData 获取 row
        var data = tree.GetData();
        int row = data?.TextureSpriteRow ?? 0;

        // SVE 的 4 种果树按 row 排列
        return row switch
        {
            0 => config.ChangePear,
            1 => config.ChangeNectarine,
            2 => config.ChangePersimmon,
            3 => config.ChangeMoneyTree,
            _ => true
        };
    }

    // 绘制果实提示 - 完全参考 Object.draw 的 ready-for-harvest 绘制方法
    private static void DrawFruitTip(FruitTree tree)
    {
        var tileLocation = tree.Tile;
        var totalGameTime = Game1.currentGameTime.TotalGameTime;

        // 计算层级深度 - 与 Object.draw 一致
        float baseLayer = (float) (((tileLocation.Y + 1) * 64) / 10000.0 + tileLocation.X / 50000.0);

        // 上下浮动动画 - 与 Object.draw 一致
        float animation = (float)(4.0 * Math.Round(Math.Sin(totalGameTime.TotalMilliseconds / 250.0), 2));

        // 使用反射获取果实列表
        var fruitField = typeof(FruitTree).GetField("fruit",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        if (fruitField == null) return;

        var fruitList = fruitField.GetValue(tree);
        if (fruitList == null) return;

        // 获取列表中的第一个果实
        Item? fruitItem = null;
        int fruitCount = 0;

        // NetList 可以通过 Count 和索引访问
        var countProp = fruitList.GetType().GetProperty("Count");
        if (countProp != null)
        {
            var countValue = countProp.GetValue(fruitList);
            if (countValue is int count)
            {
                fruitCount = count;
            }
        }

        if (fruitCount <= 0) return;

        // 获取第一个元素
        var getItemMethod = fruitList.GetType().GetMethod("get_Item");
        if (getItemMethod != null)
        {
            fruitItem = getItemMethod.Invoke(fruitList, new object[] { 0 }) as Item;
        }

        if (fruitItem == null) return;

        var itemData = ItemRegistry.GetDataOrErrorItem(fruitItem.QualifiedItemId);

        // ===== 参考 Object.draw 的绘制 =====

        // 气泡框位置: x * 64 - 8, y * 64 - 112 + animation
        Vector2 bubblePos = new Vector2(
            tileLocation.X * 64f - 8f,
            tileLocation.Y * 64f - 112f + animation
        );
        Game1.spriteBatch.Draw(
            Game1.mouseCursors,
            Game1.GlobalToLocal(Game1.viewport, bubblePos),
            new Rectangle(141, 465, 20, 24),
            Color.White * 0.75f,
            0.0f,
            Vector2.Zero,
            4f,
            SpriteEffects.None,
            baseLayer + 1E-06f
        );

        // 物品图标位置: x * 64 + 32, y * 64 - 72 + animation
        Vector2 itemPos = new Vector2(
            tileLocation.X * 64f + 32f,
            tileLocation.Y * 64f - 72f + animation
        );
        var fruitTexture = itemData.GetTexture();
        var fruitSourceRect = itemData.GetSourceRect();
        Game1.spriteBatch.Draw(
            fruitTexture,
            Game1.GlobalToLocal(Game1.viewport, itemPos),
            fruitSourceRect,
            Color.White * 0.75f,
            0.0f,
            new Vector2(8f, 8f),
            4f,
            SpriteEffects.None,
            baseLayer + 1E-05f
        );

        // 数量图标位置: 参考 Object.draw 的精确位置
        if (fruitCount > 1)
        {
            fruitItem.Stack = fruitCount;
            // 数量位置: x * 64, y * 64 - 100 + animation
            Vector2 countPos = new Vector2(
                tileLocation.X * 64f,
                tileLocation.Y * 64f - 100f + animation
            );
            fruitItem.DrawMenuIcons(Game1.spriteBatch, Game1.GlobalToLocal(Game1.viewport, countPos), 1f, 1f, baseLayer + 1.2E-05f, StackDrawType.Draw, Color.White);
        }
    }

    [HarmonyPostfix, HarmonyPatch(typeof(FruitTree), "draw")]
    public static void PostDraw(FruitTree __instance)
    {
        SpriteBatchPatch.CanChange = false;
        SpriteBatchPatch.Texture = null;
        SpriteBatchPatch.ForceMinish = false;

        // 只有在启用缩小时才绘制果实弹窗
        if (ShouldDrawFruitTip)
        {
            DrawFruitTip(__instance);
        }
    }
}
