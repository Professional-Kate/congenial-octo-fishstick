using IdelPog.Staging.Assertions.Pipelines;

namespace IdelPog.Staging.Collection
{
    public class Buffer<T>: IBuffer
    {
        private readonly IBufferAsserter _assertAsserter;
        public event Action<IBuffer>? Ready;

        public readonly T[] Data;
        
        internal Buffer(IBufferAsserter bufferAsserter, BufferRequest<T> request)
        {
            _assertAsserter = bufferAsserter;
            Data = new T[request.Length];
        }

        public void MarkReady()
        {
            Ready?.Invoke(this);
        }

        public void Assign(T[] source)
        {
            _assertAsserter.CollectionAsserter(Data.Length, source);

            Array.Copy(source, Data, Data.Length);
        }
        
        public void StreamInto(ICollection<T> source)
        {
            
            _assertAsserter.CollectionAsserter(Data.Length, source);

            source.CopyTo(Data, 0);
        }
    }
}