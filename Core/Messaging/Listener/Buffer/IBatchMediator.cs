namespace IdelPog.Core.Messaging.Listener.Buffer
{
    public interface IBatchMediator<in T>
    {
        public void HandleMessages(IReadOnlyList<T> messages);
    }
}