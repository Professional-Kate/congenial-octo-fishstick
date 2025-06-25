using IdelPog.Messaging.Buffer;

namespace IdelPog.Messaging.Factory
{
    public interface IBufferFactory
    {
        public Buffer<T> CreateBuffer<T>(BufferRequest request);
    }
}