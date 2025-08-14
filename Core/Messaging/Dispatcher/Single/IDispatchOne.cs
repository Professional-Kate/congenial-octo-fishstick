namespace IdelPog.Core.Messaging.Dispatcher.Single
{
    public interface IDispatchOne<in T> : IDispatcher where T : struct
    {
        public void Dispatch(T payload);
    }
}