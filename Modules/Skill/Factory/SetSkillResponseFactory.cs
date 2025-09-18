using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Factory.Interface;
using IdelPog.Skill.Factory.Interface;

namespace IdelPog.Skill.Factory
{
    public class SetSkillResponseFactory : ISetSkillResponseFactory
    {
        private readonly ILevelProgressFactory _levelProgressFactory;

        public SetSkillResponseFactory(ILevelProgressFactory levelProgressFactory)
        {
            _levelProgressFactory = levelProgressFactory;
        }

        public SetSkillResponse Create(Contracts.Skill skill)
        {
            return new SetSkillResponse
            {
                SkillID = skill.SkillID, 
                LevelProgress = _levelProgressFactory.CreateLevelProgress(skill.Levelable)
            };
        }
    }
}