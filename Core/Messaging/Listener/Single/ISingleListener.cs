namespace IdelPog.Core.Messaging.Listener.Single
{
    public interface ISingleListener<in T> : IListener
    {
        void Handle(T message);
    }
}