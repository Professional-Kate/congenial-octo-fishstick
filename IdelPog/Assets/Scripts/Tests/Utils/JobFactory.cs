using IdelPog.Constants;
using IdelPog.Model;
using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace Tests.Utils
{
    internal class JobFactory
    {
        internal static Job CreateMining()
        {
            return new Job(JobType.MINING, new Information(JobConstants.MINING_NAME, JobConstants.MINING_DESC));
        }
        
        internal static Job CreateFarming()
        {
            return new Job(JobType.FARMING, new Information(JobConstants.FARM_NAME, JobConstants.FARM_DESC));
        }
    }
}