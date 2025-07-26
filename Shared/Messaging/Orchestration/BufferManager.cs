using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Factory;
using IdelPog.Messaging.Messenger;
using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Orchestration
{
    public class BufferManager(IBufferFactory bufferFactory, IBufferDispatcher bufferDispatcher, IObjectNullAssertion objectNullAssertion) : IBufferManager
    {
        public IBuffer<T> RequestBuffer<T>(BufferRequest request)
        {
            objectNullAssertion.AssertNotNull(request, nameof(request));

            Buffer<T> buffer = bufferFactory.CreateBuffer<T>(request);

            if (buffer is IInternalBuffer internalBuffer)
            {
                internalBuffer.Ready += _ => HandleBufferReady(buffer.Data);
            }

            return buffer;
        }

        private void HandleBufferReady<T>(IReadOnlyList<T> buffer)
        {
            bufferDispatcher.DispatchMessage(buffer);
        }
    }
}