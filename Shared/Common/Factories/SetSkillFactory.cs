using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public class SetSkillFactory : ISetSkillFactory
    {
        public SetSkill Create(SkillID skillID)
        {
            return new SetSkill
            {
                SkillID = skillID,
            };
        }
    }
}