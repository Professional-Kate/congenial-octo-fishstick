using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Skill.Factory.Interface
{
    public interface ISetSkillFactory
    {
        public SetSkill Create(SkillID skillID);
    }
}