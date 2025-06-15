using IdelPog.Messaging.Collection;

namespace IdelPog.Messaging.Factory
{
    public interface IBufferFactory
    {
        public Buffer<T> CreateBuffer<T>(BufferRequest request);
    }
}