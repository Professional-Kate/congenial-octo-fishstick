using IdelPog.Staging.Collection;

namespace IdelPog.Staging.Messaging
{
    public interface IBufferMessenger
    {
        public void DispatchMessage<T>(IBuffer<T> buffer);
    }
}