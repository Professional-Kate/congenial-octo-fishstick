using IdelPog.Messaging.Buffer;

namespace IdelPog.Messaging.Factory
{
    public interface IBufferFactory
    {
        public IBuffer<T> CreateBuffer<T>(BufferRequest request);
    }
}