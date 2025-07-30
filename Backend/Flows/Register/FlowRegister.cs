using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Flows.Builder;
using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Flows.Register
{
    public class FlowRegister : IFlowRegister
    {
        private readonly IAssetRepository<Type, IController> _controllerRepository;
        private readonly IAssetRepository<Type, IDispatcher> _dispatcherRepository;
        private readonly IAssetRepository<Type, IErrorFactory> _errorFactoryRepository;

        public FlowRegister(IAssetRepository<Type, IController> controllerRepository, IAssetRepository<Type, IDispatcher> dispatcherRepository, IAssetRepository<Type, IErrorFactory> errorFactoryRepository)
        {
            _controllerRepository = controllerRepository;
            _dispatcherRepository = dispatcherRepository;
            _errorFactoryRepository = errorFactoryRepository;
        }

        public IListener CreateFromDescriptor<TCommand, TError>(DispatchMode dispatchMode)
        {
            if (dispatchMode == DispatchMode.SINGLE)
            {
                return ConstructSingleFlow<TCommand, TError>();
            }

            return ConstructBufferedFlow<TCommand, TError>();
        }

        private IListener ConstructSingleFlow<TCommand, TError>()
        {
            ISingleController<TCommand> singleController = (ISingleController<TCommand>) _controllerRepository.Get(typeof(TCommand));
            IDispatchOne<TError> responseDispatcher = (IDispatchOne<TError>) _dispatcherRepository.Get(typeof(TError));
            IErrorFactory<TError, TCommand> errorFactory = (IErrorFactory<TError, TCommand>) _errorFactoryRepository.Get(typeof(TError));

            IContextualHandler<TCommand> contextualHandler = new DispatchingHandler<TError, TCommand>(responseDispatcher, errorFactory);
            ISingleControllerExecutionAssertion<TCommand> executionAssertion = new SingleControllerExecutionAssertion<TCommand>(contextualHandler);
            ISingleListener<TCommand> singleListener = new ManagedSingleListener<TCommand>(singleController, executionAssertion);

            return singleListener;
        }

        private IListener ConstructBufferedFlow<TCommand, TError>()
        {
            IBatchController<TCommand> batchController = (IBatchController<TCommand>) _controllerRepository.Get(typeof(TCommand));
            IDispatchOne<TError> responseDispatcher = (IDispatchOne<TError>) _dispatcherRepository.Get(typeof(TError));
            IErrorFactory<TError, IReadOnlyList<TCommand>> errorFactory = (IErrorFactory<TError, IReadOnlyList<TCommand>>) _errorFactoryRepository.Get(typeof(TError));
            
            IContextualHandler<IReadOnlyList<TCommand>> contextualHandler = new DispatchingHandler<TError, IReadOnlyList<TCommand>>(responseDispatcher, errorFactory);
            IBatchControllerExecutionAssertion<TCommand> executionAssertion = new BatchControllerExecutionAssertion<TCommand>(contextualHandler);
            IBufferListener<TCommand> bufferListener = new ManagedBufferListener<TCommand>(batchController, executionAssertion);

            return bufferListener;
        }
    }
}