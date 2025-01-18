using IdelPog.Structures;

namespace IdelPog.Constants
{
    public static class JobConstants
    {
        public const byte MAX_JOB_LEVEL = 100;

        public static readonly Information WOOD_INFO = Information.Create("Wood Cutting", "Chop Trees! Get Wood!");
        public static readonly Information MINING_INFO = Information.Create("Mining", "Rock and Stone! Strike the Earth!");
        public static readonly Information FARMING_INFO = Information.Create("Farming", "Magical Crops!");
    }
}