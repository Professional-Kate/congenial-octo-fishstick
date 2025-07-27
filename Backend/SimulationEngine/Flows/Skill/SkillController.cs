using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillController : ISingleController<SkillChange>
    {
        private readonly ISkillChangeMediator _skillChangeMediator;

        public SkillController(ISkillChangeMediator skillChangeMediator)
        {
            _skillChangeMediator = skillChangeMediator;
        }

        public void HandleMessage(SkillChange message)
        {
            _skillChangeMediator.ChangeSkill(message);
        }
    }
}