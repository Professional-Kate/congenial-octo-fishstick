using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Buffer.Factory;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Messenger;
using IdelPog.Core.Validation.Assertion;
using Moq;

namespace IdelPog.Core.Tests.Messaging.Orchestration
{
    [TestFixture]
    public sealed class BufferManagerTest
    {
        private IBufferManager _bufferManager { get; set; }
        private BufferRequest _bufferRequest { get; set; }
        private Mock<IBufferFactory> _bufferFactoryMock { get; set; }
        private IBuffer<int> _intBuffer;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bufferFactoryMock = new Mock<IBufferFactory>();
            _bufferRequest = new BufferRequest(3);
            _bufferManager = new BufferManager(_bufferFactoryMock.Object, new ObjectNullAssertion());
            
            _intBuffer = new Buffer<int>(new Mock<IBufferAssertion>().Object, new Mock<IBufferDispatcher>().Object, new ObjectNullAssertion(), _bufferRequest);
        }

        [SetUp]
        public void Setup()
        {
            _bufferFactoryMock.Reset();
        }

        [Test]
        public void Positive_RequestBuffer_ReturnsBuffer()
        {
            _bufferFactoryMock.Setup(library => library.CreateBuffer<int>(_bufferRequest))
                .Returns(_intBuffer);
            
            IBuffer<int> buffer = _bufferManager.RequestBuffer<int>(_bufferRequest);

            Assert.That(buffer, Is.Not.Null);
            _bufferFactoryMock.Verify(library => library.CreateBuffer<int>(_bufferRequest), Times.Once);
        }

        [Test]
        public void Positive_RequestBuffer_SetsOnReady()
        {
            _bufferFactoryMock.Setup(library => library.CreateBuffer<int>(_bufferRequest))
                .Returns(_intBuffer);
            
            IBuffer<int> buffer = _bufferManager.RequestBuffer<int>(_bufferRequest);

            buffer.Assign([1, 2, 3]);
            buffer.MarkReady();
            _bufferFactoryMock.Verify(library => library.CreateBuffer<int>(_bufferRequest), Times.Once);
        }

        [Test]
        public void Negative_RequestBuffer_NullRequest_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bufferManager.RequestBuffer<int>(null!));
            _bufferFactoryMock.Verify(library => library.CreateBuffer<int>(_bufferRequest), Times.Never);
            
        }
    }
}