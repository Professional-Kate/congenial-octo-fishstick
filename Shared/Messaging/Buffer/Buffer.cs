using IdelPog.Messaging.Assertions;
using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Buffer
{
    public class Buffer<T> : IInternalBuffer, IBuffer<T>
    {
        private readonly IBufferAssertion _bufferAssertion;
        private readonly IAssertNotNull _assertNotNull;

        private event Action<IInternalBuffer>? Ready;

        event Action<IInternalBuffer>? IInternalBuffer.Ready
        {
            add => Ready += value;
            remove => Ready -= value;
        }

        public BufferState State { get; private set; } = BufferState.CREATED;

        private readonly T[] _data;
        public IReadOnlyList<T> Data => _data;

        internal Buffer(IBufferAssertion bufferAssertion, IAssertNotNull assertNotNull, BufferRequest request)
        {
            _bufferAssertion = bufferAssertion;
            _assertNotNull = assertNotNull;
            _data = new T[request.Length];
        }

        public void MarkReady()
        {
            _bufferAssertion.AssertStateEquals(BufferState.FILLED, State);
            State = BufferState.READY;

            Ready?.Invoke(this);
        }

        public void Assign(IReadOnlyList<T> source)
        {
            _assertNotNull.AssertObjectNotNull(source);
            _bufferAssertion.AssertStateEquals(BufferState.CREATED, State);
            _bufferAssertion.AssertCountEquals(source.Count, Data.Count);

            CopyIntoInternalArray(source);

            State = BufferState.FILLED;
        }

        private void CopyIntoInternalArray(IReadOnlyList<T> source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                _data[i] = source[i];
            }
        }
    }
}