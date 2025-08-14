using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Messenger;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Messaging.Buffer
{
    public sealed class Buffer<T> : IBuffer<T> where T : struct
    {
        private readonly IBufferAssertion _bufferAssertion;
        private readonly IBufferDispatcher _bufferDispatcher;
        private readonly IObjectNullAssertion _objectNullAssertion;

        public BufferState State { get; private set; } = BufferState.CREATED;

        private readonly T[] _data;
        public IReadOnlyList<T> Data => _data;

        public Buffer(IBufferAssertion bufferAssertion, IBufferDispatcher bufferDispatcher, IObjectNullAssertion objectNullAssertion, BufferRequest request)
        {
            _bufferAssertion = bufferAssertion;
            _bufferDispatcher = bufferDispatcher;
            _objectNullAssertion = objectNullAssertion;
            _data = new T[request.Length];
        }

        public void MarkReady()
        {
            _bufferAssertion.AssertStateEquals(State, BufferState.FILLED);
            State = BufferState.READY;
            _bufferDispatcher.DispatchMessage(Data);
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