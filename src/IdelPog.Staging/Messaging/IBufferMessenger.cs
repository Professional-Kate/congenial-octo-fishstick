namespace IdelPog.Staging.Messaging
{
    public interface IBufferMessenger
    {
        public void Subscribe<T>(IBufferListener<T> bufferListener);
        
        public void Unsubscribe<T>(IBufferListener<T> bufferListener);
        
        public void DispatchMessage<T>(IReadOnlyList<T>  buffer);
    }
}