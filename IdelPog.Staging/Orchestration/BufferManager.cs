using IdelPog.Staging.Collection;
using IdelPog.Staging.Factory;

namespace IdelPog.Staging.Orchestration
{
    public class BufferManager(IBufferFactory bufferFactory) : IBufferManager
    {
        public Buffer<T> RequestBuffer<T>(BufferRequest<T> request)
        {
            return bufferFactory.CreateBuffer<T>();
        }
    }
}