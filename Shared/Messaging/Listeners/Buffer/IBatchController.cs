namespace IdelPog.Messaging.Listeners.Buffer
{
    public interface IBatchController<in T> : IController
    {
        public void HandleMessages(IReadOnlyList<T> messages);
    }
}