using IdelPog.Staging.Collection;
using IdelPog.Staging.Factory;

namespace IdelPog.Staging.Orchestration
{
    public class BufferManager(IBufferFactory bufferFactory) : IBufferManager
    {
        public IBuffer<T> RequestBuffer<T>(BufferRequest<T> request)
        {
            Buffer<T> buffer = bufferFactory.CreateBuffer(request);
            
            if (buffer is IInternalBuffer internalBuffer)
            {
                internalBuffer.Ready += HandleBufferReady;
            }

            return buffer;
        }

        private static void HandleBufferReady(IInternalBuffer buffer)
        {
            // TODO inform message manage about the reayd
        }
    }
}