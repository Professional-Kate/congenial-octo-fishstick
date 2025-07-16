namespace IdelPog.Messaging.Dispatch
{
    public interface IDispatchMany<in T>
    {
        public void Dispatch(IReadOnlyList<T> payload);
    }
}