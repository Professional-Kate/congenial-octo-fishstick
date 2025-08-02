using IdelPog.Common.Factories;
using IdelPog.Flows.Builder;
using IdelPog.Flows.Types;
using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Dispatch.Buffer;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Flows
{
    public class FlowBuilderService : IFlowBuilderService
    {
        public IListener ConstructFlow<TCommand, TError>(FlowDescriptor flowDescriptor)
        {
            if (flowDescriptor.ListeningMode == BufferMode.SINGLE)
            {
                return ConstructSingleFlow<TCommand, TError>(flowDescriptor);
            }

            return ConstructBufferedFlow<TCommand, TError>(flowDescriptor);
        }

        private IListener ConstructSingleFlow<TCommand, TError>(FlowDescriptor flowDescriptor)
        {
            IContextualHandler<TCommand> contextualHandler = new DispatchingHandler<TError, TCommand>((IDispatchOne<TError>) flowDescriptor.ResponseDispatcher, (IErrorFactory<TError, TCommand>) flowDescriptor.ErrorFactory);
            ISingleControllerExecutionAssertion<TCommand> executionAssertion = new SingleControllerExecutionAssertion<TCommand>(contextualHandler);
            ISingleListener<TCommand> singleListener = new ManagedSingleListener<TCommand>((ISingleController<TCommand>) flowDescriptor.Controller, executionAssertion);

            return singleListener;
        }

        private IListener ConstructBufferedFlow<TCommand, TError>(FlowDescriptor flowDescriptor)
        {
            IContextualHandler<IReadOnlyList<TCommand>> contextualHandler = new DispatchingHandler<TError, IReadOnlyList<TCommand>>((IDispatchOne<TError>) flowDescriptor.ResponseDispatcher, (IErrorFactory<TError, IReadOnlyList<TCommand>>) flowDescriptor.ErrorFactory);
            IBatchControllerExecutionAssertion<TCommand> executionAssertion = new BatchControllerExecutionAssertion<TCommand>(contextualHandler);
            IBufferListener<TCommand> bufferListener = new ManagedBufferListener<TCommand>((IBatchController<TCommand>) flowDescriptor.Controller, executionAssertion);

            return bufferListener;
        }
    }
}