namespace IdelPog.Messaging.Messaging
{
    public interface IBufferListener<in T> : IListener
    {
        void Handle(IReadOnlyList<T> buffer);
    }
}