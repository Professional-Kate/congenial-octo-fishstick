namespace IdelPog.Messaging.Collection
{
    internal interface IInternalBuffer
    {
        public event Action<IInternalBuffer> Ready;
    }
}