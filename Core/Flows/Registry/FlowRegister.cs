using IdelPog.Core.Contracts;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Logging;
using IdelPog.Core.Messaging.Assertion;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Flows.Registry
{
    public sealed class FlowRegister : IBatchRegister, IRegisterReader
    {
        private readonly List<IListener> _registeredListeners = [];
        private readonly IBufferManager _bufferManager;
        private readonly IBufferLogger _bufferLogger;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly IBaseErrorFactory _errorFactory = new BaseErrorFactory();

        public FlowRegister(IBufferManager bufferManager, IBufferLogger bufferLogger, IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _bufferManager = bufferManager;
            _bufferLogger = bufferLogger;
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void RegisterBatch<TCommand>(IBatchMediator<TCommand> mediator) 
            where TCommand : struct 
        {
            AssertCommandIsUnique<TCommand>();
            
            IErrorFactory<BufferedError<TCommand>, IReadOnlyList<TCommand>> bufferedErrorFactory = new BufferedErrorFactory<TCommand>(_errorFactory);
            IContextualHandler<IReadOnlyList<TCommand>> dispatchHandler = new DispatchingHandler<BufferedError<TCommand>, IReadOnlyList<TCommand>>(CreateErrorDispatcher<TCommand>(), bufferedErrorFactory);
            IBatchControllerExecutionAssertion<TCommand> executionAssertion = new BatchControllerExecutionAssertion<TCommand>(dispatchHandler, _bufferLogger);
            IBatchController<TCommand> controller = new ManagedBatchController<TCommand>(mediator);
            IBufferListener<TCommand> commandListener = new ManagedBufferListener<TCommand>(controller, executionAssertion, _bufferLogger);
            
            _registeredListeners.Add(commandListener);
        }

        public IReadOnlyList<IListener> GetListeners()
        {
            return _registeredListeners.ToArray();
        }

        private IDispatchOne<BufferedError<TCommand>> CreateErrorDispatcher<TCommand>() where TCommand : struct
        {
            IDispatchOne<BufferedError<TCommand>> errorDispatcher = new ManagedDispatcher<BufferedError<TCommand>>(_bufferManager, _bufferLogger, _objectNullAssertion, _collectionAssertion);
            return errorDispatcher;
        }

        private void AssertCommandIsUnique<TCommand>() where TCommand : struct
        {
            _uniqueAssertion.AssertUnique(typeof(TCommand), _registeredListeners.Exists(listener => listener.ListenerType == typeof(TCommand)));
        }
    }
}