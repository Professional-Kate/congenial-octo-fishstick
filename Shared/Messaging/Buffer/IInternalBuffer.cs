namespace IdelPog.Messaging.Buffer
{
    internal interface IInternalBuffer
    {
        public event Action<IInternalBuffer> Ready;
    }
}