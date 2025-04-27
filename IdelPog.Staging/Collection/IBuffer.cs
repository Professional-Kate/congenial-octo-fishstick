namespace IdelPog.Staging.Collection
{
    public interface IBuffer
    {
        public event Action<IBuffer> Ready;
        public void MarkReady();
    }
}