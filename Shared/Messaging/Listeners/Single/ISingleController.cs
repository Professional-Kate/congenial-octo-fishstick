namespace IdelPog.Messaging.Listeners.Single
{
    public interface ISingleController
    {
        public void HandleMessage<T>(T message);
    }
}