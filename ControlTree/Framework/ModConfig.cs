using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;

namespace ControlTree.Framework
{
    public sealed class ModConfig
    {
        // 模组开关
        public bool ModEnable { get; set; } = true;

        // 模组开关快捷键
        public KeybindList ModEnableToggleKey { get; set; } = KeybindList.Parse("L");

        // 树木贴图更换开关
        public bool TextureChange { get; set; } = true;

        // 树木贴图更换开关快捷键
        public KeybindList TextureChangeToggleKey { get; set; } = KeybindList.Parse("");

        // 树木缩小开关
        public bool MinishTree { get; set; }

        // 树木缩小开关快捷键
        public KeybindList MinishTreeToggleKey { get; set; } = KeybindList.Parse("");

        // 对施肥过的树种取消高亮
        public bool NotHighlightTreeSeedByFertilized { get; set; } = true;

        // 树木种子提示开关
        public bool ShowTreeSeedTips { get; set; } = true;

        // 有树液采集器时是否显示种子提示（默认关闭，因为装了采集器无法摇树）
        public bool ShowTreeSeedTipWhenTapperInstalled { get; set; } = false;

        // 树木种子提示开关快捷键
        public KeybindList ShowTreeSeedTipsToggleKey { get; set; } = KeybindList.Parse("");

        // 树木苔藓提示开关
        public bool ShowTreeMossTips { get; set; } = true;

        // 树木苔藓提示开关快捷键
        public KeybindList ShowTreeMossTipsToggleKey { get; set; } = KeybindList.Parse("");

        // 树木种子高亮开关
        public bool HighlightTreeSeed { get; set; }

        // 树木种子高亮开关快捷键
        public KeybindList HighlightTreeSeedToggleKey { get; set; } = KeybindList.Parse("");

        // 树苗高亮开关
        public bool HighlightSapling { get; set; }
        
        // 树木透明开关
        public bool TransparentTree { get; set; } = true;

        // 树木种子高亮颜色
        public Color HighlightTreeSeedColor { get; set; } = Color.Red;
        
        // 树苗高亮颜色
        public Color HighlightSaplingColor { get; set; } = Color.Green;

        // 隐藏树液采集器的产物
        public bool HideTapperProduct { get; set; }

        // 渲染树干开关
        public bool RenderTreeTrunk { get; set; } = true;

        // 渲染树干开关快捷键
        public KeybindList RenderTreeTrunkToggleKey { get; set; } = KeybindList.Parse("");

        // 渲染树叶影子开关
        public bool RenderLeafyShadow { get; set; } = true;

        // 渲染树叶影子开关快捷键
        public KeybindList RenderLeafyShadowToggleKey { get; set; } = KeybindList.Parse("");

        // 橡树
        public bool ChangeOak { get; set; } = true;

        // 枫树
        public bool ChangeMaple { get; set; } = true;

        // 松树
        public bool ChangePine { get; set; } = true;

        // 蘑菇树
        public bool ChangeMushroom { get; set; } = true;

        // 桃花心木
        public bool ChangeMahogany { get; set; } = true;

        // 苔藓树1
        public bool ChangeGreenRainType1 { get; set; } = true;

        // 苔藓树2
        public bool ChangeGreenRainType2 { get; set; } = true;

        // 蕨树
        public bool ChangeGreenRainType3 { get; set; } = true;

        // 神秘树
        public bool ChangeMystic { get; set; } = true;

        // ===== 原版果树 =====
        // 行 0: 樱桃
        public bool ChangeCherry { get; set; } = true;
        // 行 1: 杏子
        public bool ChangeApricot { get; set; } = true;
        // 行 2: 橙子
        public bool ChangeOrange { get; set; } = true;
        // 行 3: 石榴
        public bool ChangePomegranate { get; set; } = true;
        // 行 4: 桃子
        public bool ChangePeach { get; set; } = true;
        // 行 5: 苹果
        public bool ChangeApple { get; set; } = true;
        // 行 6: 香蕉
        public bool ChangeBanana { get; set; } = true;

        // ===== 里奇赛德村果树 =====
        // Cherry Pluot Tree
        public bool ChangeCherryPluot { get; set; } = true;

        // Desert Tangelo Tree
        public bool ChangeDesertTangelo { get; set; } = true;

        // Ember Blood Lime Tree
        public bool ChangeEmberBloodLime { get; set; } = true;

        // Highland Jostaberry Tree
        public bool ChangeHighlandJostaberry { get; set; } = true;

        // Mountain Plumcot Tree
        public bool ChangeMountainPlumcot { get; set; } = true;

        // Northern Limequat Tree
        public bool ChangeNorthernLimequat { get; set; } = true;

        // Paradise Rangpur Tree
        public bool ChangeParadiseRangpur { get; set; } = true;

        // Tropi Ugli Fruit Tree
        public bool ChangeTropiUgliFruit { get; set; } = true;

        // ===== SVE 果树 =====
        // Pear Tree
        public bool ChangePear { get; set; } = true;

        // Nectarine Tree
        public bool ChangeNectarine { get; set; } = true;

        // Persimmon Tree
        public bool ChangePersimmon { get; set; } = true;

        // Money Tree
        public bool ChangeMoneyTree { get; set; } = true;

        // ===== SVE 野生树 =====
        // Birch Tree
        public bool ChangeBirch { get; set; } = true;

        // Fir Tree
        public bool ChangeFir { get; set; } = true;
    }
}