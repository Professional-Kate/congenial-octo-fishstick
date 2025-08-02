using IdelPog.Common.Commands;
using IdelPog.Common.Responses;

namespace IdelPog.Common.Factories
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