using IdelPog.SimulationEngine.Orchestration;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillController(IJobMediator jobMediator) : ISkillController
    {
        public ServiceResponse SwitchSkill(SkillChange skillChange)
        {
            throw new NotImplementedException();
        }
    }
}