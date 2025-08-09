using IdelPog.Common.Commands;
using IdelPog.Common.Responses;

namespace IdelPog.Common.Factories
{
    public interface ISetSkillResponseFactory
    {
        public SetSkillResponse Create(SetSkill setSkill);
    }
}