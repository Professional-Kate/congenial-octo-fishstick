using IdelPog.Messaging.Listeners;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillChangeListener(ISkillController skillController) : ISingleListener<SkillChange>
    {
        public Type ListenerType { get; } = typeof(SkillChange);

        public void Handle(SkillChange skillChange)
        {
            skillController.SwitchSkill(skillChange);
        }
    }
}