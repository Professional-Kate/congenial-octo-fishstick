using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.Skill.Factory.Interface
{
    public interface ISetSkillResponseFactory
    {
        public SetSkillResponse Create(SetSkill setSkill);
    }
}