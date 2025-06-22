using IdelPog.SimulationEngine.Orchestration;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillController(IJobMediator jobMediator) : ISkillController
    {
        public ServiceResponse SwitchSkill(SkillID skillID)
        {
            ServiceResponse response = jobMediator.ProcessJobAction(skillID);
            if (response.IsSuccess == false)
            {
                // TODO : Log to file
                Console.WriteLine(response.Message);
            }
            
            return response;
        }
    }
}