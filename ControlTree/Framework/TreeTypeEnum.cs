using Netcode;

namespace ControlTree.Framework
{
    public class TreeTypeEnum
    {
        public NetString Id { get; private set; }

        private TreeTypeEnum(string id)
        {
            Id = new NetString(id);
        }

        // "1" : oak tree : 橡树
        public static readonly TreeTypeEnum Oak = new("1");

        // "2" : maple tree : 枫树
        public static readonly TreeTypeEnum Maple = new("2");

        // "3" : pine tree : 松树
        public static readonly TreeTypeEnum Pine = new("3");

        // "7" : mushroom tree : 蘑菇树
        public static readonly TreeTypeEnum Mushroom = new("7");

        // "8" : mahogany tree : 桃花心木
        public static readonly TreeTypeEnum Mahogany = new("8");

        // "10": green rain type 1 tree : 苔藓树1
        public static readonly TreeTypeEnum GreenRainType1 = new("10");

        // "11" : green rain type 2 tree : 苔藓树2
        public static readonly TreeTypeEnum GreenRainType2 = new("11");

        // "12" : green rain type 3 tree : 蕨树
        public static readonly TreeTypeEnum GreenRainType3 = new("12");

        // "13" : mystic tree : 神秘树
        public static readonly TreeTypeEnum Mystic = new("13");

        // ===== 里奇赛德村果树 =====
        // Rafseazz.RSVCP_Cherry_Pluot : Cherry Pluot Tree
        public static readonly TreeTypeEnum CherryPluot = new("Rafseazz.RSVCP_Cherry_Pluot");

        // Rafseazz.RSVCP_Desert_Tangelo : Desert Tangelo Tree
        public static readonly TreeTypeEnum DesertTangelo = new("Rafseazz.RSVCP_Desert_Tangelo");

        // Rafseazz.RSVCP_Ember_Blood_Lime : Ember Blood Lime Tree
        public static readonly TreeTypeEnum EmberBloodLime = new("Rafseazz.RSVCP_Ember_Blood_Lime");

        // Rafseazz.RSVCP_Highland_Jostaberry : Highland Jostaberry Tree
        public static readonly TreeTypeEnum HighlandJostaberry = new("Rafseazz.RSVCP_Highland_Jostaberry");

        // Rafseazz.RSVCP_Mountain_Plumcot : Mountain Plumcot Tree
        public static readonly TreeTypeEnum MountainPlumcot = new("Rafseazz.RSVCP_Mountain_Plumcot");

        // Rafseazz.RSVCP_Northern_Limequat : Northern Limequat Tree
        public static readonly TreeTypeEnum NorthernLimequat = new("Rafseazz.RSVCP_Northern_Limequat");

        // Rafseazz.RSVCP_Paradise_Rangpur : Paradise Rangpur Tree
        public static readonly TreeTypeEnum ParadiseRangpur = new("Rafseazz.RSVCP_Paradise_Rangpur");

        // Rafseazz.RSVCP_Tropi_Ugli_Fruit : Tropi Ugli Fruit Tree
        public static readonly TreeTypeEnum TropiUgliFruit = new("Rafseazz.RSVCP_Tropi_Ugli_Fruit");
    }
}