namespace IdelPog.Buffer.Collection
{
    public class Buffer<T> : IBuffer
    {
        private readonly IList<T> _data = new List<T>();

        public event Action<IBuffer>? Ready;

        public void MarkReady()
        {
            throw new NotImplementedException();
        }
    }
}