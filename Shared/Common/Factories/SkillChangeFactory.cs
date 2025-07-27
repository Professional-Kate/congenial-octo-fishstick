using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public class SkillChangeFactory : ISkillChangeFactory
    {
        public SetSkill CreateSkillChange(SkillID skillID)
        {
            return new SetSkill
            {
                SkillID = skillID,
            };
        }
    }
}