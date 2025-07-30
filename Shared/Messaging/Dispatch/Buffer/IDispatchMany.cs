namespace IdelPog.Messaging.Dispatch.Buffer
{
    public interface IDispatchMany<in T> : IDispatcher
    {
        public void Dispatch(IReadOnlyList<T> payload);
    }
}