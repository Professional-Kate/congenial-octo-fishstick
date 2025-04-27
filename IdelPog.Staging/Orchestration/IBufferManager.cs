using IdelPog.Staging.Collection;

namespace IdelPog.Staging.Orchestration
{
    public interface IBufferManager
    {
        public Buffer<T> RequestBuffer<T>(BufferRequest<T> request);
    }
}