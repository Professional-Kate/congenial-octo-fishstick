using IdelPog.Staging.Assertions;
using IdelPog.Staging.Assertions.Pipelines;

namespace IdelPog.Staging.Collection
{
    public class Buffer<T>: IInternalBuffer, IBuffer<T>
    {
        private readonly IBufferAsserter _assertAsserter;
        private readonly IAssertBufferState _assertBufferState;
        
        private event Action<IInternalBuffer>? Ready;
        
        event Action<IInternalBuffer>? IInternalBuffer.Ready
        {
            add => Ready += value;
            remove => Ready -= value;
        }

        public BufferState State { get; private set; } = BufferState.CREATED;

        private readonly T[] _data;
        public IReadOnlyList<T> Data => _data;
        
        internal Buffer(IBufferAsserter bufferAsserter, IAssertBufferState assertBufferState, BufferRequest<T> request)
        {
            _assertAsserter = bufferAsserter;
            _assertBufferState = assertBufferState;
            _data = new T[request.Length];
        }

        public void MarkReady()
        {
            _assertBufferState.AssertState(BufferState.FILLED, State);
            State = BufferState.READY;
            
            Ready?.Invoke(this);
        }

        public void Assign(T[] source)
        {
            _assertBufferState.AssertState(BufferState.CREATED, State);
            _assertAsserter.CollectionAsserter(Data.Count, source);

            CopyIntoInternalArray(source);
            
            State = BufferState.FILLED;
        }

        private void CopyIntoInternalArray(T[] source)
        {
            for (int i = 0; i < source.Length; i++)
            {
                _data[i] = source[i];
            }
        }
    }
}