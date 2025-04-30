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

        // TODO: update to IReadOnlyList<T>
        public readonly T[] Data;
        
        internal Buffer(IBufferAsserter bufferAsserter, IAssertBufferState assertBufferState, BufferRequest<T> request)
        {
            _assertAsserter = bufferAsserter;
            _assertBufferState = assertBufferState;
            Data = new T[request.Length];
        }

        public void MarkReady()
        {
            _assertBufferState.AssertState(BufferState.FILLED, State);
            State = BufferState.READY;
            
            Ready?.Invoke(this);
        }

        public void Assign(T[] source)
        {
            Console.WriteLine(State);
            _assertBufferState.AssertState(BufferState.CREATED, State);
            _assertAsserter.CollectionAsserter(Data.Length, source);

            source.CopyTo(Data, 0);
            State = BufferState.FILLED;
        }
    }
}