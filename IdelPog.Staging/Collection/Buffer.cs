using IdelPog.Staging.Assertions;
using IdelPog.Validation.Assertions;

namespace IdelPog.Staging.Collection
{
    public class Buffer<T>: IBuffer
    {
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertCollectionSize _assertCollectionSize;
        public event Action<IBuffer>? Ready;

        public readonly T[] Data;
        
        internal Buffer(IAssertNotNull assertNotNull, IAssertCollectionSize assertCollectionSize, BufferRequest<T> request)
        {
            _assertNotNull = assertNotNull;
            _assertCollectionSize = assertCollectionSize;
            Data = new T[request.Length];
        }

        public void MarkReady()
        {
            Ready?.Invoke(this);
        }

        public void Assign(T[] source)
        {
            _assertNotNull.AssertObjectNotNull(source);
            _assertCollectionSize.AssertSize(Data.Length, source.Length);

            Array.Copy(source, Data, Data.Length);
        }

        public void StreamInto(IEnumerable<T> source)
        {
            throw new NotImplementedException();
        }

        public void StreamInto(ICollection<T> source)
        {
            _assertCollectionSize.AssertSize(Data.Length, source.Count);

            throw new NotImplementedException();
        }
    }
}