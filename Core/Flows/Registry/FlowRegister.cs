using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Logging;
using IdelPog.Core.Messaging.Assertion;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Flows.Registry
{
    public class FlowRegister : ISingleRegister, IBatchRegister, IRegisterReader
    {
        private readonly List<IListener> _registeredListeners = [];
        private readonly IBufferManager _bufferManager;
        private readonly ILogger _logger;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public FlowRegister(IBufferManager bufferManager, ILogger logger, IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _bufferManager = bufferManager;
            _logger = logger;
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void Register<TCommand, TError>(ISingleController<TCommand> controller, IErrorFactory<TError, TCommand> factory) 
            where TCommand : struct 
            where TError : struct
        {
            AssertCommandIsUnique<TCommand>();
            IContextualHandler<TCommand> dispatchHandler = new DispatchingHandler<TError, TCommand>(CreateErrorDispatcher<TError>(), factory);
            ISingleControllerExecutionAssertion<TCommand> executionAssertion = new SingleControllerExecutionAssertion<TCommand>(dispatchHandler);
            ISingleListener<TCommand> commandListener = new ManagedSingleListener<TCommand>(controller, executionAssertion, _logger);
            
            _registeredListeners.Add(commandListener);
        }

        public void Register<TCommand, TError>(IBatchController<TCommand> controller, IErrorFactory<TError, IReadOnlyList<TCommand>> factory) 
            where TCommand : struct 
            where TError : struct
        {
            AssertCommandIsUnique<TCommand>();
            IContextualHandler<IReadOnlyList<TCommand>> dispatchHandler = new DispatchingHandler<TError, IReadOnlyList<TCommand>>(CreateErrorDispatcher<TError>(), factory);
            IBatchControllerExecutionAssertion<TCommand> executionAssertion = new BatchControllerExecutionAssertion<TCommand>(dispatchHandler);
            IBufferListener<TCommand> commandListener = new ManagedBufferListener<TCommand>(controller, executionAssertion, _logger);
            
            _registeredListeners.Add(commandListener);
        }

        public IReadOnlyList<IListener> GetListeners()
        {
            return _registeredListeners.ToArray();
        }

        private IDispatchOne<TError> CreateErrorDispatcher<TError>() where TError : struct
        {
            IDispatchOne<TError> errorDispatcher = new ManagedDispatcher<TError>(_bufferManager, _logger, _objectNullAssertion, _collectionAssertion);
            return  errorDispatcher;
        }

        private void AssertCommandIsUnique<TCommand>() where TCommand : struct
        {
            _uniqueAssertion.AssertUnique(typeof(TCommand), _registeredListeners.Exists(listener => listener.ListenerType == typeof(TCommand)));
        }
    }
}