using IdelPog.Engine.Constants;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Models;
using IdelPog.Engine.Utilities.Builders;

namespace IdelPogTests.Utils
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