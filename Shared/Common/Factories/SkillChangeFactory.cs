using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.Common.Factories
{
    public class SkillChangeFactory : ISkillChangeFactory
    {
        public SkillChange CreateSkillChange(SkillID skillID, ResourceID resourceID)
        {
            return new SkillChange
            {
                SkillID = skillID,
                ResourceID = resourceID,
            };
        }
    }
}