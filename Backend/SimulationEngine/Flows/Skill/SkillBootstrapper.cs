using IdelPog.Common.Commands;
using IdelPog.Common.Errors;
using IdelPog.Common.Factories;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Listeners;
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

            IDispatchOne<SkillChangeResponse> skillChangeDTODispatcher = new ManagedDispatcher<SkillChangeResponse>(bufferManager, objectNullAssertion, collectionAssertion);

            ISkillChangeResponseFactory skillChangeResponseFactory = new SkillChangeResponseFactory();

            ISkillChangeMediator skillChangeMediator = new SkillChangeMediator(currentSkillSetter, skillChangeResponseFactory, skillChangeDTODispatcher);
            ISingleController<SkillChange> skillController = new SkillController(skillChangeMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SkillChangeError, SkillChange> skillChangeErrorFactory = new SkillChangeErrorDTOFactory(baseErrorFactory, skillChangeResponseFactory);
            IDispatchOne<SkillChangeError> skillChangeDispatcher = new ManagedDispatcher<SkillChangeError>(bufferManager,  objectNullAssertion, collectionAssertion);
            IContextualHandler<SkillChange> changeDispatchHandler = new DispatchingHandler<SkillChangeError, SkillChange>(skillChangeDispatcher, skillChangeErrorFactory);
            ISingleControllerExecutionAssertion<SkillChange> singleControllerExecutionAssertion = new SingleControllerExecutionAssertion<SkillChange>(changeDispatchHandler);
            ISingleListener<SkillChange> skillChangeListener = new ManagedSingleListener<SkillChange>(skillController, singleControllerExecutionAssertion);

            bufferMessenger.Subscribe(skillChangeListener);
        }
    }
}