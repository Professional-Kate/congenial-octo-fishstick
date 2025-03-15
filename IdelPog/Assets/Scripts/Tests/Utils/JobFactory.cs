using IdelPog.Constants;
using IdelPog.Model;
using IdelPog.Structures.Builders;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Models.Levelable;

namespace Tests.Utils
{
    internal static class JobFactory
    {
        internal static Job CreateMining()
        {
            ILevelable levelable = LevelableBuilder.Builder()
                .Level(1)
                .Experience(0)
                .NextLevelExperience(10)
                .ExperiencePerAction(0)
                .Build();
            
            return JobBuilder.Builder()
                .JobType(JobType.MINING)
                .Information(JobConstants.MINING_INFO)
                .Levelable(levelable)
                .Build();
        }
    }
}