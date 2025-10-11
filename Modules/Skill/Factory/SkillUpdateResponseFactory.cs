using IdelPog.Core.Factory.Interface;
using IdelPog.Skill.Contracts.Response;
using IdelPog.Skill.Factory.Interface;

namespace IdelPog.Skill.Factory
{
    public class SkillUpdateResponseFactory : ISkillUpdateResponseFactory
    {
        private readonly ILevelProgressFactory _levelProgressFactory;

        public SkillUpdateResponseFactory(ILevelProgressFactory levelProgressFactory)
        {
            _levelProgressFactory = levelProgressFactory;
        }

        public SkillUpdateResponse Create(Contracts.Skill skill, bool hasLeveled)
        {
            return new SkillUpdateResponse
            {
                SkillID = skill.SkillID,
                HasLeveled = hasLeveled,
                ReadOnlyLevelable = _levelProgressFactory.CreateLevelProgress(skill.Levelable)
            };
        }
    }
}