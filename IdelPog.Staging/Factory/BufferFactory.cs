using IdelPog.Staging.Assertions;
using IdelPog.Staging.Assertions.Pipelines;
using IdelPog.Staging.Collection;

namespace IdelPog.Staging.Factory
{
    public class BufferFactory(IBufferAsserter bufferAsserter, IAssertBufferState assertBufferState) : IBufferFactory
    {
        public Buffer<T> CreateBuffer<T>(BufferRequest request)
        {
            Buffer<T> createdBuffer = new(bufferAsserter, assertBufferState, request);
            
            return createdBuffer;
        }
    }
}