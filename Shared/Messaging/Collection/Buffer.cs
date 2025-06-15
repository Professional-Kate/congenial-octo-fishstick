using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Assertions.Pipelines;

namespace IdelPog.Messaging.Collection
{
    public class Buffer<T>: IInternalBuffer, IBuffer<T>
    {
        private readonly IBufferAsserter _bufferAsserter;
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
        
        internal Buffer(IBufferAsserter bufferAsserter, IAssertBufferState assertBufferState, BufferRequest request)
        {
            _bufferAsserter = bufferAsserter;
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
            _bufferAsserter.AssertCollection(Data.Count, source);

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