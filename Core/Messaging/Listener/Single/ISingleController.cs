namespace IdelPog.Core.Messaging.Listener.Single
{
    public interface ISingleController<in T> where T : struct
    {
        public void HandleMessage(T message);
    }
}