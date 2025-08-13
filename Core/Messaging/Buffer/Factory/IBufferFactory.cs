namespace IdelPog.Core.Messaging.Buffer.Factory
{
    public interface IBufferFactory
    {
        public IBuffer<T> CreateBuffer<T>(BufferRequest request);
    }
}