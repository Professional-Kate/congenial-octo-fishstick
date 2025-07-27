using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillChangeListener(ISkillController skillController) : ISingleListener<SkillChange>
    {
        public Type ListenerType { get; } = typeof(SkillChange);

        public void Handle(SkillChange skillChange)
        {
            skillController.ChangeSkill(skillChange);
        }
    }
}