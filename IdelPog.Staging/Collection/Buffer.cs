using IdelPog.Staging.Assertions.Pipelines;

namespace IdelPog.Staging.Collection
{
    public class Buffer<T>: IInternalBuffer, IBuffer<T>
    {
        private readonly IBufferAsserter _assertAsserter;
        
        private event Action<IInternalBuffer>? Ready;
        
        event Action<IInternalBuffer>? IInternalBuffer.Ready
        {
            add => Ready += value;
            remove => Ready -= value;
        }

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

            source.CopyTo(Data, 0);
        }
    }
}