namespace IdelPog.Messaging.Listeners.Single
{
    public interface ISingleMediator<in T> : IMediator
    {
        public void HandleMessage(T message);
    }
}