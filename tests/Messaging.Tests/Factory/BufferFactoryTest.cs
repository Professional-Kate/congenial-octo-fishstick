using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Factory;
using IdelPog.Messaging.Messenger;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using Moq;

namespace IdelPog.Messaging.Tests.Factory
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