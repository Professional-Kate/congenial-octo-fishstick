namespace IdelPog.Core.Messaging.Messenger
{
    public interface IBufferDispatcher
    {
        public void DispatchMessage<T>(IReadOnlyList<T> buffer) where T : struct;
    }
}