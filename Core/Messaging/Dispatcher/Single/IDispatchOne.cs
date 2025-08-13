namespace IdelPog.Core.Messaging.Dispatcher.Single
{
    public interface IDispatchOne<in T> : IDispatcher
    {
        public void Dispatch(T payload);
    }
}