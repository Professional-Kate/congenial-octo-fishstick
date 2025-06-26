using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillController(ICurrentSkillSetter currentSkillSetter) : ISkillController
    {
        public ServiceResponse SwitchSkill(SkillChange skillChange)
        {
            currentSkillSetter.SetCurrentSkill(skillChange.SkillID);
            
            return ServiceResponse.Success();
        }
    }
}