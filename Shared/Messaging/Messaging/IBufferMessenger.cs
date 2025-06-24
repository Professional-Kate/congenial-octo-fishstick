namespace IdelPog.Messaging.Messaging
{
    public interface IBufferMessenger
    {
        public void Subscribe(IListener listener);
        
        public void Unsubscribe(IListener listener);
        
        public void DispatchMessage<T>(IReadOnlyList<T>  buffer);
    }
}