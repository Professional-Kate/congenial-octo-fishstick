using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Factory;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace Integration.Tests
{
    public class ManagedBuffer
    {
        protected IBufferMessenger BufferMessenger { get; private set; }
        protected IBufferManager BufferManager { get; private set; }
        private IBufferFactory _bufferFactory;
        private IAssertNotNull _assertNotNull;

        [OneTimeSetUp]
        protected void BaseOneTimeSetUp()
        {
            _assertNotNull = new AssertNotNull(new ThrowHandler());
            IBufferAssertion bufferAssertion = new BufferAssertion(new ThrowHandler());

            _bufferFactory = new BufferFactory(bufferAssertion, _assertNotNull);
        }

        [SetUp]
        protected void BaseSetUp()
        {
            IListenerAssertion listenerAssertion = new ListenerAssertion(new ThrowHandler());

            BufferMessenger bufferMessenger = new(_assertNotNull, listenerAssertion);
            BufferMessenger = bufferMessenger;
            IBufferDispatcher bufferDispatcher = bufferMessenger;
            BufferManager = new BufferManager(_bufferFactory, bufferDispatcher, _assertNotNull);
        }

        protected void ManagedSubscribe(IListener listener)
        {
            BufferMessenger.Subscribe(listener);
        }
    }
}