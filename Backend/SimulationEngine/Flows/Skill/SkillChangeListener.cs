using IdelPog.Messaging.Listeners;

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