using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Models;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.SimulationEngine.Assertions
{
    public class LevelAssertion : BaseAssertion, ILevelAssertion
    {
        public LevelAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertBelowMaxLevel(Levelable levelable)
        {
            Assert<MaxLevelException>(() =>
            {
                if (levelable.Level >= SkillConstants.MAX_SKILL_LEVEL)
                {
                    throw new MaxLevelException(levelable, nameof(levelable));
                }
            });
        }
    }
}