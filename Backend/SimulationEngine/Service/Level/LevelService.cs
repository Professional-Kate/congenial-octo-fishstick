using IdelPog.SimulationEngine.Assertions.Pipelines;
using IdelPog.SimulationEngine.Models;

namespace IdelPog.SimulationEngine.Service
{
    public class LevelService(ILevelableAssertionPipeline levelableAssertionPipeline) : ILevelService
    {
        public void LevelUpSkill(Levelable levelable)
        {
            levelableAssertionPipeline.AssertLevelable(levelable);

            uint total = 0;
            for (uint i = 1; i < levelable.Level; i++)
            {
                total += Convert.ToUInt32(Math.Floor(i + 83 * Math.Pow(2, i / 7.0)));
            }

            levelable.Level++;
            levelable.NextLevelExperience = total;
        }
    }
}