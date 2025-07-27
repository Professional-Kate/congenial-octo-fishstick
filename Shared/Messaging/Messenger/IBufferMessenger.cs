using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.Messaging.Messenger
{
    public interface IBufferMessenger
    {
        public void Subscribe(IListener listener);

        public void Unsubscribe(IListener listener);
    }
}