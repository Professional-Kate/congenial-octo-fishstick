using IdelPogTemp.Main.Structures.Models.Levelable;
using IdelPogTemp.Main.Validation.Pipelines.Interfaces;

namespace IdelPogTemp.Main.Service.Level
{
    public class LevelService : ILevelService   
    {
        private readonly ILevelableAsserter _levelableAsserter;
        
        public LevelService(ILevelableAsserter levelableAsserter)
        {
            _levelableAsserter = levelableAsserter;
        }
        
        public void LevelUpJob(ILevelable levelable)
        {
            _levelableAsserter.AssertLevelable(levelable);

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