namespace IdelPog.Messaging.Listeners.Buffer
{
    public interface IBatchedController
    {
        public void HandleMessages<T>(IReadOnlyList<T> message);
    }
}