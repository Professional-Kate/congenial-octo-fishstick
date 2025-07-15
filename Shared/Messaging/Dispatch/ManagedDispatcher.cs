using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions.Interfaces;

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
            _assertNotNull.AssertObjectNotNull(payload);
            CreateAndDispatchBuffer([payload], length: 1);
        }

        public void Dispatch(IReadOnlyList<T> payload)
        {
            _assertNotNull.AssertObjectNotNull(payload);
            _assertCollectionNotEmpty.Handle(payload);
            CreateAndDispatchBuffer(payload, payload.Count);
        }

        private void CreateAndDispatchBuffer(IReadOnlyList<T> payload, int length)
        {
            Console.WriteLine("DISPATCHED");
            IBuffer<T> buffer = _bufferManager.RequestBuffer<T>(new BufferRequest(length));
            buffer.Assign(payload);
            buffer.MarkReady();
        }
    }
}