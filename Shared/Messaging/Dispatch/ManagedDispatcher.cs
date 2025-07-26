using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Dispatch
{
    public sealed class ManagedDispatcher<T> : IDispatchOne<T>, IDispatchMany<T>
    {
        private readonly IBufferManager _bufferManager;
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;

        public ManagedDispatcher(IBufferManager bufferManager, IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _bufferManager = bufferManager;
            _assertNotNull = assertNotNull;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }

        public void Dispatch(T payload)
        {
            Dispatch([payload]);
        }

        public void Dispatch(IReadOnlyList<T> payload)
        {
            _assertNotNull.AssertObjectNotNull(payload);
            _assertCollectionNotEmpty.Handle(payload);
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