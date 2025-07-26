using IdelPog.Messaging.Assertions;
using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Buffer
{
    public class Buffer<T> : IInternalBuffer, IBuffer<T>
    {
        private readonly IBufferAssertion _bufferAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion;

        private event Action<IInternalBuffer>? Ready;

        event Action<IInternalBuffer>? IInternalBuffer.Ready
        {
            add => Ready += value;
            remove => Ready -= value;
        }

        public BufferState State { get; private set; } = BufferState.CREATED;

        private readonly T[] _data;
        public IReadOnlyList<T> Data => _data;

        internal Buffer(IBufferAssertion bufferAssertion, IObjectNullAssertion objectNullAssertion, BufferRequest request)
        {
            _bufferAssertion = bufferAssertion;
            _objectNullAssertion = objectNullAssertion;
            _data = new T[request.Length];
        }

        public void MarkReady()
        {
            _bufferAssertion.AssertStateEquals(State, BufferState.FILLED);
            State = BufferState.READY;

            Ready?.Invoke(this);
        }

        public void Assign(IReadOnlyList<T> source)
        {
            _objectNullAssertion.AssertNotNull(source, nameof(source));
            _bufferAssertion.AssertStateEquals(State, BufferState.CREATED);
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