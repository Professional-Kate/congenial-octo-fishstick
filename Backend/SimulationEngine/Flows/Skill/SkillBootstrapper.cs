using IdelPog.Common.Commands;
using IdelPog.Common.DTO;
using IdelPog.Common.DTO.Error;
using IdelPog.Common.Factories;
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

            IDispatchOne<SkillChangeDTO> skillChangeDTODispatcher = new ManagedDispatcher<SkillChangeDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            ISkillChangeDTOFactory skillChangeDTOFactory = new SkillChangeDTOFactory();

            ISkillChangeMediator skillChangeMediator = new SkillChangeMediator(currentSkillSetter, skillChangeDTOFactory, skillChangeDTODispatcher);
            ISingleController<SkillChange> skillController = new SkillController(skillChangeMediator);

            IErrorDTOFactory errorDTOFactory = new ErrorDTOFactory();
            IErrorFactory<SkillChangeErrorDTO, SkillChange> skillChangeErrorFactory = new SkillChangeErrorDTOFactory(errorDTOFactory, skillChangeDTOFactory);
            IDispatchOne<SkillChangeErrorDTO> skillChangeDispatcher = new ManagedDispatcher<SkillChangeErrorDTO>(bufferManager,  objectNullAssertion, collectionAssertion);
            IContextualHandler<SkillChange> changeDispatchHandler = new DispatchingHandler<SkillChangeErrorDTO, SkillChange>(skillChangeDispatcher, skillChangeErrorFactory);
            ISingleControllerExecutionAssertion<SkillChange> singleControllerExecutionAssertion = new SingleControllerExecutionAssertion<SkillChange>(changeDispatchHandler);
            ISingleListener<SkillChange> skillChangeListener = new ManagedSingleListener<SkillChange>(skillController, singleControllerExecutionAssertion);

            bufferMessenger.Subscribe(skillChangeListener);
        }
    }
}