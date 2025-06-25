namespace IdelPog.Messaging.Dispatch
{
    public interface IBufferDispatcher
    {
        public void DispatchMessage<T>(IReadOnlyList<T>  buffer);
    }
}