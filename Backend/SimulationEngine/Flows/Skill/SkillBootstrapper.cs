using IdelPog.Common.Commands;
using IdelPog.Common.Errors;
using IdelPog.Common.Factories;
using IdelPog.Common.Responses;
using IdelPog.Flows.Builder;
using IdelPog.Flows.Types;
using IdelPog.Messaging.Controller;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Skill
{
    public static class SkillBootstrapper
    {
        /// <summary>
        /// Registers the <see cref="SetSkill"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SetSkillResponse"/></param>
        /// <param name="flowDescriptorDispatcher">Used to dispatch a <see cref="FlowDescriptor"/></param>
        /// <param name="currentSkillSetter">Used together with <see cref="ICurrentSkillProvider"/></param>
        /// <remarks>
        /// Listens to -> <see cref="SetSkill"/>. On Success -> <see cref="SetSkillResponse"/>. On Error -> <see cref="SetSkillError"/>
        /// </remarks>
        public static void RegisterSetSkill(IBufferManager bufferManager, IDispatchOne<FlowDescriptor> flowDescriptorDispatcher, ICurrentSkillSetter  currentSkillSetter)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetSkillError, SetSkill> setSkillErrorFactory = new SetSkillErrorFactory(baseErrorFactory );
            
            IDispatchOne<SetSkillError> setSkillErrorDispatcher = new ManagedDispatcher<SetSkillError>(bufferManager, objectNullAssertion, collectionAssertion);
            ISetSkillResponseFactory setSkillResponseFactory = new SetSkillResponseFactory();
            
            IDispatchOne<SetSkillResponse> setSkillResponseDispatcher = new ManagedDispatcher<SetSkillResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            ISingleMediator<SetSkill> setSkillMediator = new SetSkillMediator(currentSkillSetter, setSkillResponseFactory, setSkillResponseDispatcher);
            ISingleController<SetSkill> setSkillController = new ManagedSingleController<SetSkill>(setSkillMediator);
            
            FlowDescriptor flowDescriptor = new FlowBuilder()
                .ForCommand(typeof(SetSkill))
                .SetDispatchMode(BufferMode.SINGLE)
                .SetDescription(typeof(SetSkill), typeof(SetSkillResponse), typeof(SetSkillError))
                .WithController(setSkillController)
                .WithErrorDispatcher(setSkillErrorDispatcher)
                .WithErrorFactory(setSkillErrorFactory)
                .Build();
            
            flowDescriptorDispatcher.Dispatch(flowDescriptor);
        }
    }
}