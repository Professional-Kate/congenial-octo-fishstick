namespace IdelPog.Staging.Collection
{
    public class Buffer<T> : IBuffer
    {
        private IList<T> _data = new List<T>();

        public event Action<IBuffer>? Ready;
        
        // TODO : cache this
        public IReadOnlyList<T> Data => _data.AsReadOnly();

        public void MarkReady()
        {
            Ready?.Invoke(this);
        }

        public void Assign(List<T> data)
        {
            ArgumentNullException.ThrowIfNull(data);

            _data = data;
        }
    }
}