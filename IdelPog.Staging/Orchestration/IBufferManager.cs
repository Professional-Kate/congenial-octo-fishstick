using IdelPog.Staging.Collection;

namespace IdelPog.Staging.Orchestration
{
    public interface IBufferManager
    {
        public IBuffer<T> RequestBuffer<T>(BufferRequest request);
    }
}