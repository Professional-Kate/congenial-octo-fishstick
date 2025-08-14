namespace IdelPog.Core.Messaging.Listener.Single
{
    public interface ISingleListener<in T> : IListener where T : struct
    {
        void Handle(T message);
    }
}