using IdelPog.Core.Progression.Assertion.Pipelines;

namespace IdelPog.Core.Progression.Level
{
    public class LevelService : ILevelService
    {
        private readonly ILevelableAssertionPipeline _levelableAssertionPipeline;

        public LevelService(ILevelableAssertionPipeline levelableAssertionPipeline)
        {
            _levelableAssertionPipeline = levelableAssertionPipeline;
        }

        public void LevelUp(Levelable levelable)
        {
            _levelableAssertionPipeline.AssertLevelable(levelable);

            levelable.Level++;
            
            uint total = 0;
            for (uint i = 1; i <= levelable.Level; i++)
            {
                total += Convert.ToUInt32(Math.Floor(i + 300 * Math.Pow(2, i / 7.0)));
            }

            levelable.NextLevelExperience = total;
        }
    }
}