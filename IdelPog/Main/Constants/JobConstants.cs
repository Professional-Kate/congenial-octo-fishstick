using IdelPogTemp.Main.Structures;

namespace IdelPogTemp.Main.Constants
{
    public static class JobConstants
    {
        public const byte MAX_JOB_LEVEL = 100;

        public static readonly Information WOOD_INFO = new("Wood Cutting", "Chop Trees! Get Wood!");
        public static readonly Information MINING_INFO = new("Mining", "Rock and Stone! Strike the Earth!");
        public static readonly Information FARMING_INFO = new("Farming", "Magical Crops!");
    }
}