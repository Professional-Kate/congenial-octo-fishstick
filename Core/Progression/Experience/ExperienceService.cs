using IdelPog.Core.Progression.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Progression.Experience
{
    public class ExperienceService : IExperienceService
    {
        private readonly ILevelAssertion _levelAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion;
        
        public ExperienceService(ILevelAssertion levelAssertion, IObjectNullAssertion objectNullAssertion)
        {
            _levelAssertion = levelAssertion;
            _objectNullAssertion = objectNullAssertion;
        }

        public void AddExperience(Levelable levelable)
        {
            _objectNullAssertion.AssertNotNull(levelable, nameof(levelable));
            _levelAssertion.AssertBelowMaxLevel(levelable);

            uint newExperience = levelable.ExperiencePerAction + levelable.Experience;
            levelable.Experience = newExperience;
        }
    }
}