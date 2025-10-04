using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Progression.Level
{
    public class LevelService : ILevelService
    {
        private readonly ILevelAssertion _levelAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion;

        public LevelService(ILevelAssertion levelAssertion, IObjectNullAssertion objectNullAssertion)
        {
            _levelAssertion = levelAssertion;
            _objectNullAssertion = objectNullAssertion;
        }

        public void LevelUp(Levelable levelable)
        {
            _objectNullAssertion.AssertNotNull(levelable, nameof(levelable));
            _levelAssertion.AssertBelowMaxLevel(levelable);

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