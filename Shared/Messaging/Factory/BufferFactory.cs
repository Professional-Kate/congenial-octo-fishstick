using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Assertions.Pipelines;
using IdelPog.Messaging.Collection;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Messaging.Factory
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