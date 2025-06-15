namespace IdelPog.Staging.Collection
{
    internal interface IInternalBuffer
    {
        public event Action<IInternalBuffer> Ready;
    }
}