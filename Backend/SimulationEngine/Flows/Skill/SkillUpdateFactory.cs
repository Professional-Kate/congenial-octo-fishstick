using IdelPog.Common.Responses;
using IdelPog.SimulationEngine.Structures.Level;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillUpdateFactory : ISkillUpdateFactory
    {
        private readonly ILevelProgressFactory _levelProgressFactory;

        public SkillUpdateFactory(ILevelProgressFactory levelProgressFactory)
        {
            _levelProgressFactory = levelProgressFactory;
        }

        public SkillUpdateResponse CreateSkillUpdate(Models.Skill skill, bool hasLeveled)
        {
            return new SkillUpdateResponse
            {
                SkillID = skill.SkillID,
                HasLeveled = hasLeveled,
                LevelProgress = _levelProgressFactory.CreateLevelProgress(skill.Levelable)
            };
        }
    }
}