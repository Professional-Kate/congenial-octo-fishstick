using IdelPog.Messaging.Listeners;

namespace IdelPog.Messaging.Messaging
{
    public interface IBufferMessenger
    {
        public void Subscribe(IListener listener);
        
        public void Unsubscribe(IListener listener);
    }
}