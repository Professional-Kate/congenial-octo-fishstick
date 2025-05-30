using IdelPog.Staging.Assertions;
using IdelPog.Staging.Assertions.Pipelines;
using IdelPog.Staging.Collection;
using IdelPog.Validation.Assertions;

namespace IdelPog.Staging.Factory
{
    public class BufferFactory(IBufferAsserter bufferAsserter, IAssertBufferState assertBufferState, IAssertNotNull assertNotNull) : IBufferFactory
    {
        public Buffer<T> CreateBuffer<T>(BufferRequest request)
        {
            assertNotNull.AssertObjectNotNull(request);
            
            Buffer<T> createdBuffer = new(bufferAsserter, assertBufferState, request);
            
            return createdBuffer;
        }
    }
}