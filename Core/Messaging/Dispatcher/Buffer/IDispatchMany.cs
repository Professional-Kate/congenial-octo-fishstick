namespace IdelPog.Core.Messaging.Dispatcher.Buffer
{
    public interface IDispatchMany<in T> : IDispatcher
    {
        public void Dispatch(IReadOnlyList<T> payload);
    }
}