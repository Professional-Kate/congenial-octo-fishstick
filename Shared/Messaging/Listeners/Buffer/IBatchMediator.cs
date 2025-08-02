namespace IdelPog.Messaging.Listeners.Buffer
{
    public interface IBatchMediator<in T> : IMediator
    {
        public void HandleMessages(IReadOnlyList<T> messages);
    }
}