using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;
using IdelPog.Skill.Factory.Interface;

namespace IdelPog.Skill.Factory
{
    public class SetSkillResponseFactory : ISetSkillResponseFactory
    {
        public SetSkillResponse Create(SetSkill setSkill)
        {
            return new SetSkillResponse
            {
                SkillID = setSkill.SkillID
            };
        }
    }
}