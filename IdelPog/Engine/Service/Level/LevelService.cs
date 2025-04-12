using IdelPog.Engine.Structures.Levelable;
using IdelPog.Engine.Validation.Pipelines.Interfaces;

namespace IdelPog.Engine.Service.Level
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