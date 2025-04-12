using IdelPog.Main.Constants;
using IdelPog.Main.Structures.Enums;
using IdelPog.Main.Structures.Models;
using IdelPog.Main.Structures.Models.Builders.Job;
using IdelPog.Main.Structures.Models.Builders.Levelable;
using IdelPog.Main.Structures.Models.Levelable;

namespace IdelPog.Tests.Utils
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