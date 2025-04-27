using IdelPog.Validation.Assertions;

namespace IdelPog.Staging.Collection
{
    public class Buffer<T>: IBuffer
    {
        private readonly IAssertNotNull _assert;
        public event Action<IBuffer>? Ready;

        public readonly T[] Data;
        
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
            
            // TODO: create assertion for this
            if (data.Length != Data.Length)
            {
                throw new Exception();
            }

            Array.Copy(data, Data, Data.Length);
        }

        public void StreamInto(IEnumerable<T> source)
        {
            throw new NotImplementedException();
        }
    }
}