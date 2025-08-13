namespace IdelPog.Core.Messaging.Listener.Single
{
    public interface ISingleMediator<in T>
    {
        public void HandleMessage(T message);
    }
}