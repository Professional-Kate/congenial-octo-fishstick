using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager, ICurrentSkillSetter currentSkillSetter)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(new ThrowHandler());
            ICollectionAssertion collectionAssertion = new CollectionAssertion(new ThrowHandler());

            IDispatchOne<SkillChangeDTO> skillChangeDTODispatcher =
                new ManagedDispatcher<SkillChangeDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            ISkillChangeFactory skillChangeFactory = new SkillChangeFactory();

            ISkillChangeMediator skillChangeMediator = new SkillChangeMediator(currentSkillSetter, skillChangeFactory, skillChangeDTODispatcher);
            ISkillController skillController = new SkillController(skillChangeMediator);
            SkillChangeListener skillChangeListener = new(skillController);

            bufferMessenger.Subscribe(skillChangeListener);
        }
    }
}