using IdelPog.Core.Messaging.Listener;

namespace IdelPog.Core.Messaging.Messenger
{
    public interface IBufferMessenger
    {
        public void Subscribe(IListener listener);

        public void Unsubscribe(IListener listener);
    }
}