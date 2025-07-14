using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Messaging.Dispatch
{
    public abstract class BaseDispatcher<T> : IDispatchOne<T>, IDispatchMany<T>
    {
        private readonly IBufferManager _bufferManager;
        private readonly IAssertNotNull _assertNotNull;
        
        public BaseDispatcher(IBufferManager bufferManager, IAssertNotNull assertNotNull)
        {
            _bufferManager = bufferManager;
            _assertNotNull = assertNotNull;
        }
        
        public void Dispatch(T payload)
        {
            _assertNotNull.AssertObjectNotNull(payload);
            CreateAndDispatchBuffer([payload], 1);
        }

        public void Dispatch(IReadOnlyList<T> payload)
        {
            _assertNotNull.AssertObjectNotNull(payload);
            CreateAndDispatchBuffer(payload, payload.Count);
        }

        private void CreateAndDispatchBuffer(IReadOnlyList<T> payload, int length)
        {
            IBuffer<T> buffer = _bufferManager.RequestBuffer<T>(new BufferRequest(length));
            buffer.Assign(payload);
            buffer.MarkReady();
        }
    }
}