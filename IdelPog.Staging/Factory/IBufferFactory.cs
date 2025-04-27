using IdelPog.Staging.Collection;

namespace IdelPog.Staging.Factory
{
    public interface IBufferFactory
    {
        public Buffer<T> CreateBuffer<T>(BufferRequest<T> request);
    }
}