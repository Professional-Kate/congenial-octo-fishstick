namespace IdelPog.Core.Messaging.Dispatcher.Buffer
{
    public interface IDispatchMany<in T> : IDispatcher where T : struct
    {
        public void Dispatch(IReadOnlyList<T> payload);
    }
}