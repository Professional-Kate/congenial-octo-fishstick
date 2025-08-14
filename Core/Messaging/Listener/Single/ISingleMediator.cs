namespace IdelPog.Core.Messaging.Listener.Single
{
    public interface ISingleMediator<in T> where T : struct
    {
        public void HandleMessage(T message);
    }
}