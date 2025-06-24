namespace IdelPog.Messaging.Messaging
{
    public interface IBufferDispatcher
    {
        public void DispatchMessage<T>(IReadOnlyList<T>  buffer);
    }
}