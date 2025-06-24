namespace IdelPog.Messaging.Listeners
{
    public interface IBufferMessenger
    {
        public void Subscribe(IListener listener);
        
        public void Unsubscribe(IListener listener);
    }
}