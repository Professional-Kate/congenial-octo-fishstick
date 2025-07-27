namespace IdelPog.Messaging.Listeners.Single
{
    public interface ISingleController<in T>
    {
        public void HandleMessage(T message);
    }
}