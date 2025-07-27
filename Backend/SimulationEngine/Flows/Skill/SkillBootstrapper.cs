using IdelPog.Common.Commands;
using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager, ICurrentSkillSetter currentSkillSetter)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IThrowingAssertion throwingAssertion = new ThrowingAssertion(throwHandler);

            IDispatchOne<SkillChangeDTO> skillChangeDTODispatcher = new ManagedDispatcher<SkillChangeDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            ISkillChangeFactory skillChangeFactory = new SkillChangeFactory();

            ISkillChangeMediator skillChangeMediator = new SkillChangeMediator(currentSkillSetter, skillChangeFactory, skillChangeDTODispatcher);
            ISingleController<SkillChange> skillController = new SkillController(skillChangeMediator);
            ISingleListener<SkillChange> skillChangeListener = new ManagedSingleListener<SkillChange>(skillController, throwingAssertion);

            bufferMessenger.Subscribe(skillChangeListener);
        }
    }
}