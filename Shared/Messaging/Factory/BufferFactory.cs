using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Buffer;
using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Factory
{
    public class BufferFactory(IBufferAssertion bufferAssertion, IAssertNotNull assertNotNull) : IBufferFactory
    {
        public Buffer<T> CreateBuffer<T>(BufferRequest request)
        {
            assertNotNull.AssertObjectNotNull(request);

            Buffer<T> createdBuffer = new(bufferAssertion, assertNotNull, request);

            return createdBuffer;
        }
    }
}