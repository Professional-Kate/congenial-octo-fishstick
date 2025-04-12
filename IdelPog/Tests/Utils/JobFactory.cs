using IdelPogTemp.Main.Constants;
using IdelPogTemp.Main.Structures.Enums;
using IdelPogTemp.Main.Structures.Models;
using IdelPogTemp.Main.Structures.Models.Builders.Job;
using IdelPogTemp.Main.Structures.Models.Builders.Levelable;
using IdelPogTemp.Main.Structures.Models.Levelable;

namespace IdelPogTemp.Tests.Utils
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