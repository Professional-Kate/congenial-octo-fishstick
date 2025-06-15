using IdelPog.Messaging.Collection;
using IdelPog.Messaging.Factory;
using IdelPog.Messaging.Messaging;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Messaging.Orchestration
{
    public class BufferManager(IBufferFactory bufferFactory, IBufferMessenger bufferMessenger, IAssertNotNull assertNotNull) : IBufferManager
    {
        public IBuffer<T> RequestBuffer<T>(BufferRequest request)
        {
            assertNotNull.AssertObjectNotNull(request);
            
            Buffer<T> buffer = bufferFactory.CreateBuffer<T>(request);
            
            if (buffer is IInternalBuffer internalBuffer)
            {
                internalBuffer.Ready += _ => HandleBufferReady(buffer.Data);
            }

            return buffer;
        }

        private void HandleBufferReady<T>(IReadOnlyList<T> buffer)
        {
            bufferMessenger.DispatchMessage(buffer);
        }
    }
}