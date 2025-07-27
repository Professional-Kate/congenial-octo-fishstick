using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillChangeListener(ISkillController skillController) : ISingleListener<SetSkill>
    {
        public Type ListenerType { get; } = typeof(SetSkill);

        public void Handle(SetSkill setSkill)
        {
            skillController.ChangeSkill(setSkill);
        }
    }
}