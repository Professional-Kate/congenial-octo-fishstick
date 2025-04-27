using IdelPog.Validation.Assertions;

namespace IdelPog.Staging.Collection
{
    public class Buffer<T>: IBuffer
    {
        private readonly IAssertNotNull _assert;
        public event Action<IBuffer>? Ready;
        
        public T[] Data { get; private set; }
        
        internal Buffer(IAssertNotNull assert, BufferRequest<T> request)
        {
            _assert = assert;
            Data = new T[request.Length];
        }

        public void MarkReady()
        {
            Ready?.Invoke(this);
        }

        public void Assign(T[] data)
        {
            _assert.AssertObjectNotNull(data);

            Data = data;
        }
    }
}