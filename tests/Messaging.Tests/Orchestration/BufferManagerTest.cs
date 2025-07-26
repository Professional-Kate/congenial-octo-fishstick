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
        private Mock<IBufferDispatcher> _bufferDispatcherMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bufferFactoryMock = new Mock<IBufferFactory>();
            _bufferDispatcherMock = new Mock<IBufferDispatcher>();

            _bufferRequest = new BufferRequest(3);
            _bufferManager = new BufferManager(_bufferFactoryMock.Object, _bufferDispatcherMock.Object, new AssertNotNull(new ThrowHandler()));
        }

        [SetUp]
        public void SetUp()
        {
            _bufferDispatcherMock.Reset();
            _bufferFactoryMock.Reset();

            SetupMock();
        }

        private void SetupMock()
        {
            _bufferFactoryMock.Setup(library => library.CreateBuffer<int>(_bufferRequest))
                .Returns(new Buffer<int>(new BufferAssertion(new ThrowHandler()), new AssertNotNull(new ThrowHandler()), _bufferRequest));
        }

        [Test]
        public void Positive_RequestBuffer_ReturnsBuffer()
        {
            IBuffer<int> buffer = _bufferManager.RequestBuffer<int>(_bufferRequest);

            Assert.That(buffer, Is.Not.Null);
            _bufferFactoryMock.Verify(library => library.CreateBuffer<int>(_bufferRequest), Times.Once);
        }

        [Test]
        public void Positive_RequestBuffer_SetsOnReady()
        {
            IBuffer<int> buffer = _bufferManager.RequestBuffer<int>(_bufferRequest);

            buffer.Assign([1, 2, 3]);
            buffer.MarkReady();

            _bufferDispatcherMock.Verify(library => library.DispatchMessage(It.IsAny<IReadOnlyList<int>>()), Times.Once);
        }

        [Test]
        public void Negative_RequestBuffer_NullRequest_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bufferManager.RequestBuffer<int>(null!));
        }
    }
}