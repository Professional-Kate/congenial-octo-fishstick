using IdelPog.Constants;
using IdelPog.Model;
using IdelPog.Structures.Enums;

namespace Tests.Utils
{
    internal static class JobFactory
    {
        internal static Job CreateMining()
        {
            return new Job(JobType.MINING, JobConstants.WOOD_INFO);
        }
        
        internal static Job CreateFarming()
        {
            return new Job(JobType.FARMING, JobConstants.FARMING_INFO);
        }
    }
}