namespace IdelPog.Messaging.Listeners.Single
{
    public interface ISingleController<in T> : IController
    {
        public void HandleMessage(T message);
    }
}