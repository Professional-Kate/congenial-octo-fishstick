using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Messenger;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Messaging.Buffer.Factory
{
    public class BufferFactory : IBufferFactory
    {
        private readonly IBufferAssertion _bufferAssertion;
        private readonly IObjectNullAssertion _objectNullAssertion; 
        private readonly IBufferDispatcher _bufferDispatcher;

        public BufferFactory(IBufferAssertion bufferAssertion, IObjectNullAssertion objectNullAssertion, IBufferDispatcher bufferDispatcher)
        {
            _bufferAssertion = bufferAssertion;
            _objectNullAssertion = objectNullAssertion;
            _bufferDispatcher = bufferDispatcher;
        }
        
        public IBuffer<T> CreateBuffer<T>(BufferRequest request) where T : struct
        {
            _objectNullAssertion.AssertNotNull(request, nameof(request));

            IBuffer<T> createdBuffer = new Buffer<T>(_bufferAssertion, _bufferDispatcher, _objectNullAssertion, request);

            return createdBuffer;
        }
    }
}