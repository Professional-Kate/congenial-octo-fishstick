using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillController : ISingleController<SkillChange>
    {
        private readonly ISingleMediator<SkillChange> _skillChangeMediator;

        public SkillController(ISingleMediator<SkillChange> skillChangeMediator)
        {
            _skillChangeMediator = skillChangeMediator;
        }

        public void HandleMessage(SkillChange message)
        {
            _skillChangeMediator.HandleMessage(message);
        }
    }
}