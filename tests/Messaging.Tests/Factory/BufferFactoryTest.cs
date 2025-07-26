using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Factory;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

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

            _bufferFactory = new BufferFactory(_bufferAssertion, new AssertNotNull(new ThrowHandler()));
            _bufferRequest = new BufferRequest(5);
        }

        [Test]
        public void Positive_CreateBuffer_CreatesBuffer_CorrectLength()
        {
            Buffer<int> buffer = _bufferFactory.CreateBuffer<int>(_bufferRequest);

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