namespace IdelPog.Staging.Collection
{
    public class Buffer<T> : IBuffer
    {
        public event Action<IBuffer>? Ready;
        
        public IReadOnlyList<T> Data { get; private set; } = new List<T>();

        public void MarkReady()
        {
            Ready?.Invoke(this);
        }

        public void Assign(List<T> data)
        {
            ArgumentNullException.ThrowIfNull(data);

            Data = data.AsReadOnly();
        }
    }
}