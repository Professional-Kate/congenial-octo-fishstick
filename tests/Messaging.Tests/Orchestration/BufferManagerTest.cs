using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Factory;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using Moq;

namespace IdelPog.Messaging.Tests.Orchestration
{
    [TestFixture]
    public class BufferManagerTest
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
            _bufferManager = new BufferManager(_bufferFactoryMock.Object, new ObjectNullAssertion(new ThrowHandler()));
            
            _intBuffer = new Buffer<int>(new Mock<IBufferAssertion>().Object, new Mock<IBufferDispatcher>().Object, new ObjectNullAssertion(new  ThrowHandler()), _bufferRequest);
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