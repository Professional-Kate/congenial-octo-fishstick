using IdelPog.Common.Commands;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillChangeFactory : ISkillChangeFactory
    {
        public SkillChangeDTO CreateSkillChangeDTO(SkillChange skillChange)
        {
            return new SkillChangeDTO
            {
                SkillID = skillChange.SkillID,
                ResourceID = skillChange.ResourceID,
            };
        }
    }
}