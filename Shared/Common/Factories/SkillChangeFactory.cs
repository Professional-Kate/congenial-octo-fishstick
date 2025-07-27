using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public class SkillChangeFactory : ISkillChangeFactory
    {
        public SkillChange CreateSkillChange(SkillID skillID)
        {
            return new SkillChange
            {
                SkillID = skillID,
            };
        }
    }
}