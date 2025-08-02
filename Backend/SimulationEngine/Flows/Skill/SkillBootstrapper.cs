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
        /// Registers the <see cref="SkillChange"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch <see cref="SkillChangeResponse"/></param>
        /// <param name="flowDescriptorDispatcher">Used to dispatch a <see cref="FlowDescriptor"/></param>
        /// <param name="currentSkillSetter">Used together with <see cref="ICurrentSkillProvider"/></param>
        /// <remarks>
        /// Listens to -> <see cref="SkillChange"/>. On Success -> <see cref="SkillChangeResponse"/>. On Error -> <see cref="SkillChangeError"/>
        /// </remarks>
        public static void RegisterSkillChange(IBufferManager bufferManager, IDispatchOne<FlowDescriptor> flowDescriptorDispatcher, ICurrentSkillSetter  currentSkillSetter)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SkillChangeError, SkillChange> skillChangeErrorFactory = new SkillChangeErrorFactory(baseErrorFactory );
            
            IDispatchOne<SkillChangeError> skillChangeErrorDispatcher = new ManagedDispatcher<SkillChangeError>(bufferManager, objectNullAssertion, collectionAssertion);
            ISkillChangeResponseFactory skillChangeResponseFactory = new SkillChangeResponseFactory();
            
            IDispatchOne<SkillChangeResponse> skillChangeResponseDispatcher = new ManagedDispatcher<SkillChangeResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            ISingleMediator<SkillChange> skillChangeMediator = new SkillChangeMediator(currentSkillSetter, skillChangeResponseFactory, skillChangeResponseDispatcher);
            ISingleController<SkillChange> skillChangeController = new ManagedSingleController<SkillChange>(skillChangeMediator);
            
            FlowDescriptor flowDescriptor = new FlowBuilder()
                .ForCommand(typeof(SkillChange))
                .SetDispatchMode(BufferMode.SINGLE)
                .SetDescription(typeof(SkillChange), typeof(SkillChangeResponse), typeof(SkillChangeError))
                .WithController(skillChangeController)
                .WithResponseDispatcher(skillChangeErrorDispatcher)
                .WithErrorFactory(skillChangeErrorFactory)
                .Build();
            
            flowDescriptorDispatcher.Dispatch(flowDescriptor);
        }
    }
}