using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Skill.Factory.Interface;

namespace IdelPog.Skill.Factory
{
    public class SetSkillFactory : ISetSkillFactory
    {
        public SetSkill Create(SkillID skillID)
        {
            return new SetSkill
            {
                SkillID = skillID
            };
        }
    }
}