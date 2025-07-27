namespace IdelPog.Messaging.Buffer
{
    public interface IInternalBuffer
    {
        public event Action<IInternalBuffer> Ready;
    }
}