using IdelPog.Core.Messaging.Assertion;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Buffer.Factory;
using IdelPog.Core.Messaging.Messenger;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Handler;
using Moq;

namespace IdelPog.Core.Tests.Messaging.Factory
{
    [TestFixture]
    public class BufferFactoryTest
    {
        private IBufferFactory _bufferFactory { get; set; }
        private BufferRequest _bufferRequest { get; set; }

        private IBufferAssertion _bufferAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _bufferAssertion = new BufferAssertion(new ThrowHandler());

            Mock<IBufferDispatcher> bufferDispatcherMock = new();
            _bufferFactory = new BufferFactory(_bufferAssertion, new ObjectNullAssertion(new ThrowHandler()), bufferDispatcherMock.Object);
            _bufferRequest = new BufferRequest(5);
        }

        [Test]
        public void Positive_CreateBuffer_CreatesBuffer_CorrectLength()
        {
            IBuffer<int> buffer = _bufferFactory.CreateBuffer<int>(_bufferRequest);

            Assert.That(buffer, Is.Not.Null);
            Assert.That(buffer.Data, Has.Count.EqualTo(5));
        }

        [Test]
        public void Negative_CreateBuffer_NullRequest_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bufferFactory.CreateBuffer<int>(null!));
        }
    }
}