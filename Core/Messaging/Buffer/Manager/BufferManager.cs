using IdelPog.Core.Messaging.Buffer.Factory;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Messaging.Buffer.Manager
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