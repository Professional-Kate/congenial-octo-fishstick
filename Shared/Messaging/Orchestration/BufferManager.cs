using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Factory;
using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Orchestration
{
    public class BufferManager(IBufferFactory bufferFactory, IObjectNullAssertion objectNullAssertion) : IBufferManager
    {
        public IBuffer<T> RequestBuffer<T>(BufferRequest request)
        {
            objectNullAssertion.AssertNotNull(request, nameof(request));

            IBuffer<T> buffer = bufferFactory.CreateBuffer<T>(request);

            return buffer;
        }
    }
}