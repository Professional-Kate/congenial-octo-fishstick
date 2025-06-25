using IdelPog.Messaging.Buffer;

namespace IdelPog.Messaging.Orchestration
{
    public interface IBufferManager
    {
        public IBuffer<T> RequestBuffer<T>(BufferRequest request);
    }
}