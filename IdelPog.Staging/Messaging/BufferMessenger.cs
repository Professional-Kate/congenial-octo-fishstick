using IdelPog.Staging.Collection;

namespace IdelPog.Staging.Messaging
{
    public class BufferMessenger : IBufferMessenger
    {
        public void DispatchMessage<T>(IBuffer<T> buffer)
        {
            throw new NotImplementedException();
        }
    }
}