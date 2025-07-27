using IdelPog.SimulationEngine.Assertions.Pipelines;
using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Service
{
    public class LevelService(ILevelableAssertionPipeline levelableAssertionPipeline) : ILevelService
    {
        public void LevelUpSkill(ILevelable levelable)
        {
            levelableAssertionPipeline.AssertLevelable(levelable);

            int total = 0;
            for (int i = 1; i < levelable.Level; i++)
            {
                total += Convert.ToInt32(Math.Floor(i + 83 * Math.Pow(2, i / 7.0)));
            }

            levelable.LevelUp();
            levelable.SetNextLevelExperience(total);
        }
    }
}