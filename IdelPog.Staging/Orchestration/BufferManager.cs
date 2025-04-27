using IdelPog.Staging.Collection;
using IdelPog.Staging.Factory;

namespace IdelPog.Staging.Orchestration
{
    public class BufferManager(IBufferFactory bufferFactory) : IBufferManager
    {
        public Buffer<T> RequestBuffer<T>(BufferRequest<T> request)
        {
            Buffer<T> buffer = bufferFactory.CreateBuffer(request);
            buffer.Ready += HandleBufferReady;

            return buffer;
        }

        private static void HandleBufferReady(IBuffer buffer)
        {
            // TODO inform message manage about the reayd
        }
    }
}