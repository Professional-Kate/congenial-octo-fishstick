namespace IdelPog.Messaging.Messaging
{
    public interface IBufferMessenger
    {
        public void Subscribe(IListener listener);
        
        public void Unsubscribe<T>(IBufferListener<T> bufferListener);
        
        public void DispatchMessage<T>(IReadOnlyList<T>  buffer);
    }
}