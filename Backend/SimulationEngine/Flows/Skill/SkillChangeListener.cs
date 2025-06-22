using IdelPog.Messaging.Messaging;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillChangeListener(ISkillController skillController) : IBufferListener<SkillChange>   {
        
        public Type ListenerType { get; } =  typeof(SkillChange);
        
        public void Handle(IReadOnlyList<SkillChange> buffer)
        {
            // TODO: we need a new BufferListener for single items
            skillController.SwitchSkill(buffer[0]);
        }
    }
}