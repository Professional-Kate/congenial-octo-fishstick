using IdelPog.Messaging.Dispatch;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, ICurrentSkillSetter currentSkillSetter)
        {
            ISkillController skillController = new SkillController(currentSkillSetter);
            SkillChangeListener skillChangeListener = new(skillController);
            
            bufferMessenger.Subscribe(skillChangeListener);
        }
    }
}