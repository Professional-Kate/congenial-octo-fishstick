namespace IdelPog.Core.Messaging.Listener.Buffer
{
    public interface IBufferListener<in T> : IListener
    {
        void Handle(IReadOnlyList<T> buffer);
    }
}