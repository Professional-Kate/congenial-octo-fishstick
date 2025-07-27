using IdelPog.Common.Commands;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillChangeFactory : ISkillChangeFactory
    {
        public SkillChangeDTO CreateSkillChangeDTO(SetSkill setSkill)
        {
            return new SkillChangeDTO
            {
                SkillID = setSkill.SkillID,
            };
        }
    }
}