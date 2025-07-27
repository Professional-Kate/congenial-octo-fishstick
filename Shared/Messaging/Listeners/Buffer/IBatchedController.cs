namespace IdelPog.Messaging.Listeners.Buffer
{
    public interface IBatchedController<in T>
    {
        public void HandleMessages(IReadOnlyList<T> message);
    }
}