namespace IdelPog.Core.Messaging.Listener.Buffer
{
    public interface IBatchController<in T>
    {
        public void HandleMessages(IReadOnlyList<T> messages);
    }
}