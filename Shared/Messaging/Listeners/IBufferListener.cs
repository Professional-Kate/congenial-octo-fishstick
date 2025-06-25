namespace IdelPog.Messaging.Listeners
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