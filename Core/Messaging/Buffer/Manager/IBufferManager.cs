namespace IdelPog.Core.Messaging.Buffer.Manager
{
    public interface IBufferManager
    {
        public IBuffer<T> RequestBuffer<T>(BufferRequest request);
    }
}