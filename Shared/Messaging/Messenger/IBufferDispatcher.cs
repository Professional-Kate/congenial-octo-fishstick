namespace IdelPog.Messaging.Messenger
{
    public interface IBufferDispatcher
    {
        public void DispatchMessage<T>(IReadOnlyList<T>  buffer);
    }
}