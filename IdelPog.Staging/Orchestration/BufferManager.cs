using IdelPog.Staging.Collection;
using IdelPog.Staging.Factory;
using IdelPog.Staging.Messaging;

namespace IdelPog.Staging.Orchestration
{
    public class BufferManager(IBufferFactory bufferFactory, IBufferMessenger bufferMessenger) : IBufferManager
    {
        public IBuffer<T> RequestBuffer<T>(BufferRequest request)
        {
            Buffer<T> buffer = bufferFactory.CreateBuffer<T>(request);
            
            if (buffer is IInternalBuffer internalBuffer)
            {
                internalBuffer.Ready += _ => HandleBufferReady(buffer);
            }

            return buffer;
        }

        private void HandleBufferReady<T>(IBuffer<T> buffer)
        {
            bufferMessenger.DispatchMessage(buffer);
        }
    }
}