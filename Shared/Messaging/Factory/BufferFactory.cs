using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Buffer;
using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Factory
{
    public class BufferFactory(IBufferAssertion bufferAssertion, IObjectNullAssertion objectNullAssertion) : IBufferFactory
    {
        public Buffer<T> CreateBuffer<T>(BufferRequest request)
        {
            objectNullAssertion.AssertNotNull(request, nameof(request));

            Buffer<T> createdBuffer = new(bufferAssertion, objectNullAssertion, request);

            return createdBuffer;
        }
    }
}