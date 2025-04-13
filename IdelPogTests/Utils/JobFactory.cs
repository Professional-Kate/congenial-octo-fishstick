using IdelPog.Engine.Constants;
using IdelPog.Engine.Models;
using IdelPog.Engine.Structures.Enums;

namespace IdelPogTests.Utils
{
    internal static class JobFactory
    {
        internal static Job CreateMining()
        {
            ILevelable levelable = new Levelable(1, 0, 10, 0);
            
            return new Job(levelable, JobType.MINING, JobConstants.MINING_INFO);
        }
    }
}