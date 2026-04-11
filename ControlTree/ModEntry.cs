using Common.Integrations;
using ControlTree.Framework;
using ControlTree.Patches;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ControlTree;

// ReSharper disable once UnusedType.Global
internal class ModEntry : Mod
{
    // ReSharper disable once NotAccessedField.Global
    public static ModEntry? Instance;
    
    // ReSharper disable once InconsistentNaming
    // 模组配置
    public ModConfig Config = null!;

    // 用于配置界面的高亮树木种子和树苗的颜色贴图
    private readonly Texture2D _highlightTreeSeedColorTexture = new(Game1.graphics.GraphicsDevice, 1, 1);
    private readonly Texture2D _highlightSaplingColorTexture = new(Game1.graphics.GraphicsDevice, 1, 1);

    // 用于配置界面的高亮树木种子和树苗的高亮框颜色
    private Color _highlightTreeSeedColor = Color.Red;
    private Color _highlightSaplingColor = Color.Green;

    public override void Entry(IModHelper helper)
    {
        Instance = this;
        // Monitor.Log("Hello World", LogLevel.Debug);
        // 读取配置
        Config = Helper.ReadConfig<ModConfig>();

        // 注册事件
        helper.Events.Input.ButtonsChanged += OnButtonsChanged!;
        helper.Events.GameLoop.GameLaunched += OnGameLaunched!;

        // 初始化生效树的集合
        if (Config.ChangeOak) TreePatch.ChangeTreeType(TreeTypeEnum.Oak.Id);
        if (Config.ChangeMaple) TreePatch.ChangeTreeType(TreeTypeEnum.Maple.Id);
        if (Config.ChangePine) TreePatch.ChangeTreeType(TreeTypeEnum.Pine.Id);
        if (Config.ChangeMushroom) TreePatch.ChangeTreeType(TreeTypeEnum.Mushroom.Id);
        if (Config.ChangeMahogany) TreePatch.ChangeTreeType(TreeTypeEnum.Mahogany.Id);
        if (Config.ChangeGreenRainType1) TreePatch.ChangeTreeType(TreeTypeEnum.GreenRainType1.Id);
        if (Config.ChangeGreenRainType2) TreePatch.ChangeTreeType(TreeTypeEnum.GreenRainType2.Id);
        if (Config.ChangeGreenRainType3) TreePatch.ChangeTreeType(TreeTypeEnum.GreenRainType3.Id);
        if (Config.ChangeMystic) TreePatch.ChangeTreeType(TreeTypeEnum.Mystic.Id);

        // 里奇赛德村果树 - 仅在检测到 RSV 模组时注册
        bool isRidgesideLoaded = Helper.ModRegistry.IsLoaded("Rafseazz.RSVCP");
        if (isRidgesideLoaded)
        {
            Monitor.Log("Ridgeside Village detected, registering custom fruit trees.");
            if (Config.ChangeCherryPluot) TreePatch.ChangeTreeType(TreeTypeEnum.CherryPluot.Id);
            if (Config.ChangeDesertTangelo) TreePatch.ChangeTreeType(TreeTypeEnum.DesertTangelo.Id);
            if (Config.ChangeEmberBloodLime) TreePatch.ChangeTreeType(TreeTypeEnum.EmberBloodLime.Id);
            if (Config.ChangeHighlandJostaberry) TreePatch.ChangeTreeType(TreeTypeEnum.HighlandJostaberry.Id);
            if (Config.ChangeMountainPlumcot) TreePatch.ChangeTreeType(TreeTypeEnum.MountainPlumcot.Id);
            if (Config.ChangeNorthernLimequat) TreePatch.ChangeTreeType(TreeTypeEnum.NorthernLimequat.Id);
            if (Config.ChangeParadiseRangpur) TreePatch.ChangeTreeType(TreeTypeEnum.ParadiseRangpur.Id);
            if (Config.ChangeTropiUgliFruit) TreePatch.ChangeTreeType(TreeTypeEnum.TropiUgliFruit.Id);
        }

        // SVE 果树和野生树 - 仅在检测到 SVE 模组时注册
        bool isSveLoaded = Helper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP");
        if (isSveLoaded)
        {
            Monitor.Log("SVE detected, registering custom fruit trees and wild trees.");
            // SVE 果树
            if (Config.ChangePear) TreePatch.ChangeTreeType(TreeTypeEnum.Pear.Id);
            if (Config.ChangeNectarine) TreePatch.ChangeTreeType(TreeTypeEnum.Nectarine.Id);
            if (Config.ChangePersimmon) TreePatch.ChangeTreeType(TreeTypeEnum.Persimmon.Id);
            if (Config.ChangeMoneyTree) TreePatch.ChangeTreeType(TreeTypeEnum.MoneyTree.Id);
            // SVE 野生树
            if (Config.ChangeBirch) TreePatch.ChangeTreeType(TreeTypeEnum.Birch.Id);
            if (Config.ChangeFir) TreePatch.ChangeTreeType(TreeTypeEnum.Fir.Id);
        }

        // 初始化高亮树木种子和树苗的高亮框颜色(用于配置界面)
        _highlightTreeSeedColor = Config.HighlightTreeSeedColor;
        _highlightSaplingColor = Config.HighlightSaplingColor;
        _highlightTreeSeedColorTexture.SetData(new[] { Config.HighlightTreeSeedColor });
        _highlightSaplingColorTexture.SetData(new[] { Config.HighlightSaplingColor });

        // 根据textures.json加载用于替换树木的贴图
        foreach (var textureName in helper.ModContent.Load<List<string>>("assets/textures.json"))
        {
            Monitor.Log($"Load texture: {textureName}");
            TreePatch.TextureMapping[textureName] = helper.ModContent.Load<Texture2D>($"assets/{textureName}");
        }

        // 初始化补丁类 传递了配置和监视器
        TreePatch.InitConfig(Config, Monitor);
        SpriteBatchPatch.InitConfig(Config);

        // 启用Harmony补丁
        Harmony harmony = new(ModManifest.UniqueID);
        harmony.PatchAll();
    }

    private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
    {
        // 创建配置界面
        CreateConfigMenu();
    }

    private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
    {
        // 判断是否可以响应
        if (!ShouldEnable(forInput: true)) return;

        // 切换配置
        ToggleConfigOption(Config.ModEnableToggleKey, () => Config.ModEnable = !Config.ModEnable);
        ToggleConfigOption(Config.TextureChangeToggleKey, () => Config.TextureChange = !Config.TextureChange);
        ToggleConfigOption(Config.MinishTreeToggleKey, () => Config.MinishTree = !Config.MinishTree);
        ToggleConfigOption(Config.HighlightTreeSeedToggleKey, () => Config.HighlightTreeSeed = !Config.HighlightTreeSeed);
        ToggleConfigOption(Config.ShowTreeSeedTipsToggleKey, () => Config.ShowTreeSeedTips = !Config.ShowTreeSeedTips);
        ToggleConfigOption(Config.ShowTreeMossTipsToggleKey, () => Config.ShowTreeMossTips = !Config.ShowTreeMossTips);
        ToggleConfigOption(Config.RenderTreeTrunkToggleKey, () => Config.RenderTreeTrunk = !Config.RenderTreeTrunk);
        ToggleConfigOption(Config.RenderLeafyShadowToggleKey, () => Config.RenderLeafyShadow = !Config.RenderLeafyShadow);
    }

    private void ToggleConfigOption(KeybindList keyBind, Action toggleAction)
    {
        // 如果按键没有被按下则返回
        if (!keyBind.JustPressed()) return;
        // 执行切换配置
        toggleAction();
        // 保存配置
        Helper.WriteConfig(Config);
    }

    private static bool ShouldEnable(bool forInput = false)
    {
        // 如果不在游戏中或者不是主玩家则返回
        if (!Context.IsWorldReady || !Context.IsMainPlayer) return false;

        // 如果不是为了输入则返回true
        if (!forInput) return true;

        // 如果不是自由玩家并且事件没有结束则返回
        if (!Context.IsPlayerFree && !Game1.eventUp) return false;

        // 如果键盘分发器没有订阅者则返回
        return Game1.keyboardDispatcher.Subscriber == null;
    }

    private void CreateConfigMenu()
    {
        // 获取配置界面API
        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null) return;

        // 注册配置界面
        configMenu.Register(
            mod: ModManifest,
            reset: () => Config = new ModConfig(),
            save: () => Helper.WriteConfig(Config)
        );

        // ===== 主页 - 通用设置 =====
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => Helper.Translation.Get("config.section_general")
        );
        // 模组开关
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.mod_enable.name"),
            tooltip: () => Helper.Translation.Get("config.mod_enable.tooltip"),
            getValue: () => Config.ModEnable,
            setValue: value => Config.ModEnable = value
        );
        // 模组开关快捷键
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.mod_enable_toggle_key.name"),
            tooltip: () => Helper.Translation.Get("config.mod_enable_toggle_key.tooltip"),
            getValue: () => Config.ModEnableToggleKey,
            setValue: value => Config.ModEnableToggleKey = value
        );
        // 贴图替换开关
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.texture_change.name"),
            tooltip: () => Helper.Translation.Get("config.texture_change.tooltip"),
            getValue: () => Config.TextureChange,
            setValue: value => Config.TextureChange = value
        );
        // 贴图替换开关快捷键
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.texture_change_toggle_key.name"),
            tooltip: () => Helper.Translation.Get("config.texture_change_toggle_key.tooltip"),
            getValue: () => Config.TextureChangeToggleKey,
            setValue: value => Config.TextureChangeToggleKey = value
        );
        // 缩小树木开关
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.minish_tree.name"),
            tooltip: () => Helper.Translation.Get("config.minish_tree.tooltip"),
            getValue: () => Config.MinishTree,
            setValue: value => Config.MinishTree = value
        );
        // 缩小树木开关快捷键
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.minish_tree_toggle_key.name"),
            tooltip: () => Helper.Translation.Get("config.minish_tree_toggle_key.tooltip"),
            getValue: () => Config.MinishTreeToggleKey,
            setValue: value => Config.MinishTreeToggleKey = value
        );
        // 透明遮挡树种的树
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.transparent_tree.name"),
            tooltip: () => Helper.Translation.Get("config.transparent_tree.tooltip"),
            getValue: () => Config.TransparentTree,
            setValue: value => Config.TransparentTree = value
        );
        // 树种提示开关
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.show_tree_seed_tips.name"),
            tooltip: () => Helper.Translation.Get("config.show_tree_seed_tips.tooltip"),
            getValue: () => Config.ShowTreeSeedTips,
            setValue: value => Config.ShowTreeSeedTips = value
        );
        // 树种提示快捷键
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.show_tree_seed_tips_toggle_key.name"),
            tooltip: () => Helper.Translation.Get("config.show_tree_seed_tips_toggle_key.tooltip"),
            getValue: () => Config.ShowTreeSeedTipsToggleKey,
            setValue: value => Config.ShowTreeSeedTipsToggleKey = value
        );
        // 有采集器时是否显示种子提示
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.show_tree_seed_tip_when_tapper_installed.name"),
            tooltip: () => Helper.Translation.Get("config.show_tree_seed_tip_when_tapper_installed.tooltip"),
            getValue: () => Config.ShowTreeSeedTipWhenTapperInstalled,
            setValue: value => Config.ShowTreeSeedTipWhenTapperInstalled = value
        );
        // 苔藓提示开关
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.show_tree_moss_tips.name"),
            tooltip: () => Helper.Translation.Get("config.show_tree_moss_tips.tooltip"),
            getValue: () => Config.ShowTreeMossTips,
            setValue: value => Config.ShowTreeMossTips = value
        );
        // 苔藓提示快捷键
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.show_tree_moss_tips_toggle_key.name"),
            tooltip: () => Helper.Translation.Get("config.show_tree_moss_tips_toggle_key.tooltip"),
            getValue: () => Config.ShowTreeMossTipsToggleKey,
            setValue: value => Config.ShowTreeMossTipsToggleKey = value
        );
        // 高亮树种开关
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.highlight_tree_seed.name"),
            tooltip: () => Helper.Translation.Get("config.highlight_tree_seed.tooltip"),
            getValue: () => Config.HighlightTreeSeed,
            setValue: value => Config.HighlightTreeSeed = value
        );
        // 对施肥过的树种取消高亮
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.not_highlight_tree_seed_by_fertilized.name"),
            tooltip: () => Helper.Translation.Get("config.not_highlight_tree_seed_by_fertilized.tooltip"),
            getValue: () => Config.NotHighlightTreeSeedByFertilized,
            setValue: value => Config.NotHighlightTreeSeedByFertilized = value
        );
        // 高亮树种快捷键
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.highlight_tree_seed_toggle_key.name"),
            tooltip: () => Helper.Translation.Get("config.highlight_tree_seed_toggle_key.tooltip"),
            getValue: () => Config.HighlightTreeSeedToggleKey,
            setValue: value => Config.HighlightTreeSeedToggleKey = value
        );
        // 树种高亮颜色预览
        configMenu.AddImage(
            mod: ModManifest,
            texture: () => _highlightTreeSeedColorTexture
        );
        // 树种高亮颜色R
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.highlight_tree_seed_color.name"),
            tooltip: () => Helper.Translation.Get("config.highlight_tree_seed_color.tooltip"),
            min: 0,
            max: 255,
            getValue: () => Config.HighlightTreeSeedColor.R,
            setValue: value =>
            {
                var newColor = Config.HighlightTreeSeedColor;
                newColor.R = (byte)value;
                _highlightTreeSeedColorTexture.SetData(new[] { newColor });
                Config.HighlightTreeSeedColor = newColor;
            },
            formatValue: i =>
            {
                if (_highlightTreeSeedColor.R == i) return $"R: {i:X}";
                _highlightTreeSeedColor.R = (byte)i;
                _highlightTreeSeedColorTexture.SetData(new[] { _highlightTreeSeedColor });
                return $"R: {i:X}";
            }
        );
        // 树种高亮颜色G
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "",
            min: 0,
            max: 255,
            getValue: () => Config.HighlightTreeSeedColor.G,
            setValue: value =>
            {
                var newColor = Config.HighlightTreeSeedColor;
                newColor.G = (byte)value;
                _highlightTreeSeedColorTexture.SetData(new[] { newColor });
                Config.HighlightTreeSeedColor = newColor;
            },
            formatValue: i =>
            {
                if (_highlightTreeSeedColor.G == i) return $"G: {i:X}";
                _highlightTreeSeedColor.G = (byte)i;
                _highlightTreeSeedColorTexture.SetData(new[] { _highlightTreeSeedColor });
                return $"G: {i:X}";
            }
        );
        // 树种高亮颜色B
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "",
            min: 0,
            max: 255,
            getValue: () => Config.HighlightTreeSeedColor.B,
            setValue: value =>
            {
                var newColor = Config.HighlightTreeSeedColor;
                newColor.B = (byte)value;
                _highlightTreeSeedColorTexture.SetData(new[] { newColor });
                Config.HighlightTreeSeedColor = newColor;
            },
            formatValue: i =>
            {
                if (_highlightTreeSeedColor.B == i) return $"B: {i:X}";
                _highlightTreeSeedColor.B = (byte)i;
                _highlightTreeSeedColorTexture.SetData(new[] { _highlightTreeSeedColor });
                return $"B: {i:X}";
            }
        );
        // 高亮树苗开关
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.highlight_sapling.name"),
            tooltip: () => Helper.Translation.Get("config.highlight_sapling.tooltip"),
            getValue: () => Config.HighlightSapling,
            setValue: value => Config.HighlightSapling = value
        );
        // 树苗高亮颜色预览
        configMenu.AddImage(
            mod: ModManifest,
            texture: () => _highlightSaplingColorTexture
        );
        // 树苗高亮颜色R
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.highlight_sapling_color.name"),
            tooltip: () => Helper.Translation.Get("config.highlight_sapling_color.tooltip"),
            min: 0,
            max: 255,
            getValue: () => Config.HighlightSaplingColor.R,
            setValue: value =>
            {
                var newColor = Config.HighlightSaplingColor;
                newColor.R = (byte)value;
                _highlightSaplingColorTexture.SetData(new[] { newColor });
                Config.HighlightSaplingColor = newColor;
            },
            formatValue: i =>
            {
                if (_highlightSaplingColor.R == i) return $"R: {i:X}";
                _highlightSaplingColor.R = (byte)i;
                _highlightSaplingColorTexture.SetData(new[] { _highlightSaplingColor });
                return $"R: {i:X}";
            }
        );
        // 树苗高亮颜色G
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "",
            min: 0,
            max: 255,
            getValue: () => Config.HighlightSaplingColor.G,
            setValue: value =>
            {
                var newColor = Config.HighlightSaplingColor;
                newColor.G = (byte)value;
                _highlightSaplingColorTexture.SetData(new[] { newColor });
                Config.HighlightSaplingColor = newColor;
            },
            formatValue: i =>
            {
                if (_highlightSaplingColor.G == i) return $"G: {i:X}";
                _highlightSaplingColor.G = (byte)i;
                _highlightSaplingColorTexture.SetData(new[] { _highlightSaplingColor });
                return $"G: {i:X}";
            }
        );
        // 树苗高亮颜色B
        configMenu.AddNumberOption(
            mod: ModManifest,
            name: () => "",
            min: 0,
            max: 255,
            getValue: () => Config.HighlightSaplingColor.B,
            setValue: value =>
            {
                var newColor = Config.HighlightSaplingColor;
                newColor.B = (byte)value;
                _highlightSaplingColorTexture.SetData(new[] { newColor });
                Config.HighlightSaplingColor = newColor;
            },
            formatValue: i =>
            {
                if (_highlightSaplingColor.B == i) return $"B: {i:X}";
                _highlightSaplingColor.B = (byte)i;
                _highlightSaplingColorTexture.SetData(new[] { _highlightSaplingColor });
                return $"B: {i:X}";
            }
        );
        // 隐藏树液采集器产物
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.hide_tapper_product.name"),
            tooltip: () => Helper.Translation.Get("config.hide_tapper_product.tooltip"),
            getValue: () => Config.HideTapperProduct,
            setValue: value => Config.HideTapperProduct = value
        );
        // 渲染树干开关
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.render_tree_trunk.name"),
            tooltip: () => Helper.Translation.Get("config.render_tree_trunk.tooltip"),
            getValue: () => Config.RenderTreeTrunk,
            setValue: value => Config.RenderTreeTrunk = value
        );
        // 渲染树干快捷键
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.render_tree_trunk_toggle_key.name"),
            tooltip: () => Helper.Translation.Get("config.render_tree_trunk_toggle_key.tooltip"),
            getValue: () => Config.RenderTreeTrunkToggleKey,
            setValue: value => Config.RenderTreeTrunkToggleKey = value
        );
        // 渲染树叶影子开关
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.render_leafy_shadow.name"),
            tooltip: () => Helper.Translation.Get("config.render_leafy_shadow.tooltip"),
            getValue: () => Config.RenderLeafyShadow,
            setValue: value => Config.RenderLeafyShadow = value
        );
        // 渲染树叶影子快捷键
        configMenu.AddKeybindList(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.render_leafy_shadow_toggle_key.name"),
            tooltip: () => Helper.Translation.Get("config.render_leafy_shadow_toggle_key.tooltip"),
            getValue: () => Config.RenderLeafyShadowToggleKey,
            setValue: value => Config.RenderLeafyShadowToggleKey = value
        );

        // ===== 主页 - 页面导航 =====
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "vanilla_trees",
            text: () => Helper.Translation.Get("config.page_vanilla_trees"),
            tooltip: () => Helper.Translation.Get("config.page_vanilla_trees_tooltip")
        );
        configMenu.AddPageLink(
            mod: ModManifest,
            pageId: "vanilla_fruit_trees",
            text: () => Helper.Translation.Get("config.page_vanilla_fruit_trees"),
            tooltip: () => Helper.Translation.Get("config.page_vanilla_fruit_trees_tooltip")
        );
        if (Helper.ModRegistry.IsLoaded("Rafseazz.RSVCP"))
        {
            configMenu.AddPageLink(
                mod: ModManifest,
                pageId: "ridgeside_fruit_trees",
                text: () => Helper.Translation.Get("config.page_ridgeside_fruit_trees"),
                tooltip: () => Helper.Translation.Get("config.page_ridgeside_fruit_trees_tooltip")
            );
        }
        // SVE 检测
        bool isSveLoadedForConfig = Helper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP");
        if (isSveLoadedForConfig)
        {
            configMenu.AddPageLink(
                mod: ModManifest,
                pageId: "sve_fruit_trees",
                text: () => Helper.Translation.Get("config.page_sve_trees"),
                tooltip: () => Helper.Translation.Get("config.page_sve_trees_tooltip")
            );
        }

        // ===== 原版树页面 =====
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "vanilla_trees",
            pageTitle: () => Helper.Translation.Get("config.page_vanilla_trees")
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => Helper.Translation.Get("config.section_vanilla_trees")
        );
        // 橡树
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_oak.name"),
            tooltip: () => Helper.Translation.Get("config.change_oak.tooltip"),
            getValue: () => Config.ChangeOak,
            setValue: value =>
            {
                Config.ChangeOak = value;
                TreePatch.ChangeTreeType(TreeTypeEnum.Oak.Id, value);
            }
        );
        // 枫树
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_maple.name"),
            tooltip: () => Helper.Translation.Get("config.change_maple.tooltip"),
            getValue: () => Config.ChangeMaple,
            setValue: value =>
            {
                Config.ChangeMaple = value;
                TreePatch.ChangeTreeType(TreeTypeEnum.Maple.Id, value);
            }
        );
        // 松树
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_pine.name"),
            tooltip: () => Helper.Translation.Get("config.change_pine.tooltip"),
            getValue: () => Config.ChangePine,
            setValue: value =>
            {
                Config.ChangePine = value;
                TreePatch.ChangeTreeType(TreeTypeEnum.Pine.Id, value);
            }
        );
        // 蘑菇树
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_mushroom.name"),
            tooltip: () => Helper.Translation.Get("config.change_mushroom.tooltip"),
            getValue: () => Config.ChangeMushroom,
            setValue: value =>
            {
                Config.ChangeMushroom = value;
                TreePatch.ChangeTreeType(TreeTypeEnum.Mushroom.Id, value);
            }
        );
        // 桃花心木
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_mahogany.name"),
            tooltip: () => Helper.Translation.Get("config.change_mahogany.tooltip"),
            getValue: () => Config.ChangeMahogany,
            setValue: value =>
            {
                Config.ChangeMahogany = value;
                TreePatch.ChangeTreeType(TreeTypeEnum.Mahogany.Id, value);
            }
        );
        // 苔藓树1
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_green_rain_type1.name"),
            tooltip: () => Helper.Translation.Get("config.change_green_rain_type1.tooltip"),
            getValue: () => Config.ChangeGreenRainType1,
            setValue: value =>
            {
                Config.ChangeGreenRainType1 = value;
                TreePatch.ChangeTreeType(TreeTypeEnum.GreenRainType1.Id, value);
            }
        );
        // 苔藓树2
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_green_rain_type2.name"),
            tooltip: () => Helper.Translation.Get("config.change_green_rain_type2.tooltip"),
            getValue: () => Config.ChangeGreenRainType2,
            setValue: value =>
            {
                Config.ChangeGreenRainType2 = value;
                TreePatch.ChangeTreeType(TreeTypeEnum.GreenRainType2.Id, value);
            }
        );
        // 蕨树
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_green_rain_type3.name"),
            tooltip: () => Helper.Translation.Get("config.change_green_rain_type3.tooltip"),
            getValue: () => Config.ChangeGreenRainType3,
            setValue: value =>
            {
                Config.ChangeGreenRainType3 = value;
                TreePatch.ChangeTreeType(TreeTypeEnum.GreenRainType3.Id, value);
            }
        );
        // 神秘树
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_mystic.name"),
            tooltip: () => Helper.Translation.Get("config.change_mystic.tooltip"),
            getValue: () => Config.ChangeMystic,
            setValue: value =>
            {
                Config.ChangeMystic = value;
                TreePatch.ChangeTreeType(TreeTypeEnum.Mystic.Id, value);
            }
        );

        // ===== 原版果树页面 =====
        configMenu.AddPage(
            mod: ModManifest,
            pageId: "vanilla_fruit_trees",
            pageTitle: () => Helper.Translation.Get("config.page_vanilla_fruit_trees")
        );
        configMenu.AddSectionTitle(
            mod: ModManifest,
            text: () => Helper.Translation.Get("config.section_vanilla_fruit_trees")
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_cherry.name"),
            tooltip: () => Helper.Translation.Get("config.change_cherry.tooltip"),
            getValue: () => Config.ChangeCherry,
            setValue: value => Config.ChangeCherry = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_apricot.name"),
            tooltip: () => Helper.Translation.Get("config.change_apricot.tooltip"),
            getValue: () => Config.ChangeApricot,
            setValue: value => Config.ChangeApricot = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_orange.name"),
            tooltip: () => Helper.Translation.Get("config.change_orange.tooltip"),
            getValue: () => Config.ChangeOrange,
            setValue: value => Config.ChangeOrange = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_pomegranate.name"),
            tooltip: () => Helper.Translation.Get("config.change_pomegranate.tooltip"),
            getValue: () => Config.ChangePomegranate,
            setValue: value => Config.ChangePomegranate = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_peach.name"),
            tooltip: () => Helper.Translation.Get("config.change_peach.tooltip"),
            getValue: () => Config.ChangePeach,
            setValue: value => Config.ChangePeach = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_apple.name"),
            tooltip: () => Helper.Translation.Get("config.change_apple.tooltip"),
            getValue: () => Config.ChangeApple,
            setValue: value => Config.ChangeApple = value
        );
        configMenu.AddBoolOption(
            mod: ModManifest,
            name: () => Helper.Translation.Get("config.change_banana.name"),
            tooltip: () => Helper.Translation.Get("config.change_banana.tooltip"),
            getValue: () => Config.ChangeBanana,
            setValue: value => Config.ChangeBanana = value
        );

        // ===== 里奇赛德村果树页面 =====
        if (Helper.ModRegistry.IsLoaded("Rafseazz.RSVCP"))
        {
            configMenu.AddPage(
                mod: ModManifest,
                pageId: "ridgeside_fruit_trees",
                pageTitle: () => Helper.Translation.Get("config.page_ridgeside_fruit_trees")
            );
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("config.section_ridgeside_fruit_trees")
            );
            // Cherry Pluot Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_cherry_pluot.name"),
                tooltip: () => Helper.Translation.Get("config.change_cherry_pluot.tooltip"),
                getValue: () => Config.ChangeCherryPluot,
                setValue: value =>
                {
                    Config.ChangeCherryPluot = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.CherryPluot.Id, value);
                }
            );
            // Desert Tangelo Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_desert_tangelo.name"),
                tooltip: () => Helper.Translation.Get("config.change_desert_tangelo.tooltip"),
                getValue: () => Config.ChangeDesertTangelo,
                setValue: value =>
                {
                    Config.ChangeDesertTangelo = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.DesertTangelo.Id, value);
                }
            );
            // Ember Blood Lime Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_ember_blood_lime.name"),
                tooltip: () => Helper.Translation.Get("config.change_ember_blood_lime.tooltip"),
                getValue: () => Config.ChangeEmberBloodLime,
                setValue: value =>
                {
                    Config.ChangeEmberBloodLime = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.EmberBloodLime.Id, value);
                }
            );
            // Highland Jostaberry Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_highland_jostaberry.name"),
                tooltip: () => Helper.Translation.Get("config.change_highland_jostaberry.tooltip"),
                getValue: () => Config.ChangeHighlandJostaberry,
                setValue: value =>
                {
                    Config.ChangeHighlandJostaberry = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.HighlandJostaberry.Id, value);
                }
            );
            // Mountain Plumcot Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_mountain_plumcot.name"),
                tooltip: () => Helper.Translation.Get("config.change_mountain_plumcot.tooltip"),
                getValue: () => Config.ChangeMountainPlumcot,
                setValue: value =>
                {
                    Config.ChangeMountainPlumcot = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.MountainPlumcot.Id, value);
                }
            );
            // Northern Limequat Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_northern_limequat.name"),
                tooltip: () => Helper.Translation.Get("config.change_northern_limequat.tooltip"),
                getValue: () => Config.ChangeNorthernLimequat,
                setValue: value =>
                {
                    Config.ChangeNorthernLimequat = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.NorthernLimequat.Id, value);
                }
            );
            // Paradise Rangpur Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_paradise_rangpur.name"),
                tooltip: () => Helper.Translation.Get("config.change_paradise_rangpur.tooltip"),
                getValue: () => Config.ChangeParadiseRangpur,
                setValue: value =>
                {
                    Config.ChangeParadiseRangpur = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.ParadiseRangpur.Id, value);
                }
            );
            // Tropi Ugli Fruit Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_tropi_ugli_fruit.name"),
                tooltip: () => Helper.Translation.Get("config.change_tropi_ugli_fruit.tooltip"),
                getValue: () => Config.ChangeTropiUgliFruit,
                setValue: value =>
                {
                    Config.ChangeTropiUgliFruit = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.TropiUgliFruit.Id, value);
                }
            );
        }

        // ===== SVE 果树页面 =====
        if (Helper.ModRegistry.IsLoaded("FlashShifter.StardewValleyExpandedCP"))
        {
            configMenu.AddPage(
                mod: ModManifest,
                pageId: "sve_fruit_trees",
                pageTitle: () => Helper.Translation.Get("config.page_sve_trees")
            );
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("config.section_sve_fruit_trees")
            );
            // Pear Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_pear.name"),
                tooltip: () => Helper.Translation.Get("config.change_pear.tooltip"),
                getValue: () => Config.ChangePear,
                setValue: value =>
                {
                    Config.ChangePear = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.Pear.Id, value);
                }
            );
            // Nectarine Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_nectarine.name"),
                tooltip: () => Helper.Translation.Get("config.change_nectarine.tooltip"),
                getValue: () => Config.ChangeNectarine,
                setValue: value =>
                {
                    Config.ChangeNectarine = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.Nectarine.Id, value);
                }
            );
            // Persimmon Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_persimmon.name"),
                tooltip: () => Helper.Translation.Get("config.change_persimmon.tooltip"),
                getValue: () => Config.ChangePersimmon,
                setValue: value =>
                {
                    Config.ChangePersimmon = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.Persimmon.Id, value);
                }
            );
            // Money Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_money_tree.name"),
                tooltip: () => Helper.Translation.Get("config.change_money_tree.tooltip"),
                getValue: () => Config.ChangeMoneyTree,
                setValue: value =>
                {
                    Config.ChangeMoneyTree = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.MoneyTree.Id, value);
                }
            );
            // SVE 野生树 section
            configMenu.AddSectionTitle(
                mod: ModManifest,
                text: () => Helper.Translation.Get("config.section_sve_wild_trees")
            );
            // Birch Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_birch.name"),
                tooltip: () => Helper.Translation.Get("config.change_birch.tooltip"),
                getValue: () => Config.ChangeBirch,
                setValue: value =>
                {
                    Config.ChangeBirch = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.Birch.Id, value);
                }
            );
            // Fir Tree
            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => Helper.Translation.Get("config.change_fir.name"),
                tooltip: () => Helper.Translation.Get("config.change_fir.tooltip"),
                getValue: () => Config.ChangeFir,
                setValue: value =>
                {
                    Config.ChangeFir = value;
                    TreePatch.ChangeTreeType(TreeTypeEnum.Fir.Id, value);
                }
            );
        }
    }
}