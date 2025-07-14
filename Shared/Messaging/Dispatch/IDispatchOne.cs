namespace IdelPog.Messaging.Dispatch
{
    public interface IDispatchOne<in T>
    {
        public void Dispatch(T payload);
    }
}