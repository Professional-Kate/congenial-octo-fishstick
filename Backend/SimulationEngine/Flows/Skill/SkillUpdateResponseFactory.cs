using IdelPog.Common.Factories;
using IdelPog.Common.Responses;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillUpdateResponseFactory : ISkillUpdateResponseFactory
    {
        private readonly ILevelProgressFactory _levelProgressFactory;

        public SkillUpdateResponseFactory(ILevelProgressFactory levelProgressFactory)
        {
            _levelProgressFactory = levelProgressFactory;
        }

        public SkillUpdateResponse Create(Models.Skill skill, bool hasLeveled)
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