using IdelPog.Messaging.Listeners.Single;

namespace IdelPog.Messaging.Listeners.Buffer
{
    public interface IBufferListener<in T> : IListener
    {
        void Handle(IReadOnlyList<T> buffer);
    }

    public interface ISingleListener<in T> : IListener
    {
        void Handle(T item);
    }
}