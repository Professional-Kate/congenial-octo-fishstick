namespace IdelPog.Messaging.Dispatch.Single
{
    public interface IDispatchOne<in T> : IDispatcher
    {
        public void Dispatch(T payload);
    }
}