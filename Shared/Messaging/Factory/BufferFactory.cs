using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Messenger;
using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Factory
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
        
        public IBuffer<T> CreateBuffer<T>(BufferRequest request)
        {
            _objectNullAssertion.AssertNotNull(request, nameof(request));

            IBuffer<T> createdBuffer = new Buffer<T>(_bufferAssertion, _bufferDispatcher, _objectNullAssertion, request);

            return createdBuffer;
        }
    }
}