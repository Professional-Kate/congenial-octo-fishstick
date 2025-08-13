namespace IdelPog.Core.Messaging.Listener.Single
{
    public interface ISingleController<in T>
    {
        public void HandleMessage(T message);
    }
}