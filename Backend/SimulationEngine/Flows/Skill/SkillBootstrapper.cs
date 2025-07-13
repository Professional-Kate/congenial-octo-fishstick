using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Orchestration;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillBootstrapper
    {
        public static void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager, ICurrentSkillSetter currentSkillSetter)
        {
            ISkillChangeDispatcher skillChangeDispatcher = new SkillChangeDispatcher(bufferManager);
            ISkillChangeFactory skillChangeFactory = new SkillChangeFactory();
            
            ISkillChangeMediator skillChangeMediator = new SkillChangeMediator(currentSkillSetter, skillChangeFactory, skillChangeDispatcher);
            ISkillController skillController = new SkillController(skillChangeMediator);
            SkillChangeListener skillChangeListener = new(skillController);
            
            bufferMessenger.Subscribe(skillChangeListener);
        }
    }
}