using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Messaging.Dispatcher
{
    public sealed class ManagedDispatcher<T> : IDispatchOne<T>, IDispatchMany<T>
    {
        private readonly IBufferManager _bufferManager;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public ManagedDispatcher(IBufferManager bufferManager, IObjectNullAssertion objectNullAssertion, ICollectionAssertion collectionAssertion)
        {
            _bufferManager = bufferManager;
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public void Dispatch(T payload)
        {
            Dispatch([payload]);
        }

        public void Dispatch(IReadOnlyList<T> payload)
        {
            _objectNullAssertion.AssertNotNull(payload, nameof(payload));
            _collectionAssertion.AssertNotEmpty(payload);
            CreateAndDispatchBuffer(payload);
        }

        private void CreateAndDispatchBuffer(IReadOnlyList<T> payload)
        {
            IBuffer<T> buffer = _bufferManager.RequestBuffer<T>(new BufferRequest(payload.Count));
            buffer.Assign(payload);
            buffer.MarkReady();
        }
    }
}