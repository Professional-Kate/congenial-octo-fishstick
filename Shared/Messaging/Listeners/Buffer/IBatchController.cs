namespace IdelPog.Messaging.Listeners.Buffer
{
    public interface IBatchController<in T>
    {
        public void HandleMessages(IReadOnlyList<T> messages);
    }
}