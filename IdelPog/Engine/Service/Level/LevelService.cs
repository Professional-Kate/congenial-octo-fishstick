using IdelPog.Engine.Models;
using IdelPog.Engine.Validation.Pipelines;

namespace IdelPog.Engine.Service
{
    public class LevelService(ILevelableAsserter levelableAsserter) : ILevelService
    {
        public void LevelUpJob(ILevelable levelable)
        {
            levelableAsserter.AssertLevelable(levelable);

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