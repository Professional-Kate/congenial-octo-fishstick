using IdelPog.Constants;
using IdelPog.Model;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Models.Levelable;

namespace Tests.Utils
{
    internal static class JobFactory
    {
        internal static Job CreateMining()
        {
            return new Job(new Levelable(1, 0, 10000, 0), JobType.MINING, JobConstants.WOOD_INFO);
        }
    }
}