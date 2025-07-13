using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Assertions.Pipelines;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Factory;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Interfaces;

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
            IAssertCollectionSize assertCollectionSize = new AssertCollectionSize(new ThrowHandler());
            IAssertValidCollectionSize assertValidCollectionSize = new AssertValidCollectionSize(new ThrowHandler());
            IAssertBufferState assertBufferState = new AssertBufferState(new ThrowHandler());
            
            IBufferAsserter bufferAsserter = new BufferAsserter(_assertNotNull, assertCollectionSize, assertValidCollectionSize);
            _bufferFactory = new BufferFactory(bufferAsserter, assertBufferState, _assertNotNull);
        }

        [SetUp]
        protected void BaseSetUp()
        {
            IAssertListenerFound assertListenerFound = new AssertListenerFound(new ThrowHandler());
            
            BufferMessenger bufferMessenger = new(_assertNotNull, assertListenerFound);
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