using IdelPog.Staging.Assertions;
using IdelPog.Staging.Assertions.Pipelines;
using IdelPog.Staging.Collection;
using IdelPog.Staging.Factory;
using IdelPog.Staging.Messaging;
using IdelPog.Staging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using Moq;

namespace IdelPog.Staging.Tests.Orchestration
{
    [TestFixture]
    public class BufferManagerTest
    {
        private IBufferManager _bufferManager { get; set; }
        private BufferRequest _bufferRequest { get; set; }
        private Mock<IBufferFactory> _bufferFactoryMock { get; set; }
        private Mock<IBufferMessenger> _bufferDispatcherMock { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bufferFactoryMock = new Mock<IBufferFactory>();
            _bufferDispatcherMock = new Mock<IBufferMessenger>();
            _handlerMock = new Mock<IHandler>();
            
            _bufferRequest = new BufferRequest(3);
            _bufferManager = new BufferManager(_bufferFactoryMock.Object, _bufferDispatcherMock.Object, new AssertNotNull(_handlerMock.Object));
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
                .Returns(new Buffer<int>(new BufferAsserter(new AssertNotNull(new ThrowHandler()), new AssertCollectionSize(new ThrowHandler()), new AssertValidCollectionSize(new ThrowHandler())), new AssertBufferState(new ThrowHandler()), new BufferRequest(3)));
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
            _handlerMock.Setup(library => library.Handle(It.IsAny<ArgumentNullException>()))
                .Throws<ArgumentNullException>();
            
            Assert.Throws<ArgumentNullException>(() => _bufferManager.RequestBuffer<int>(null!));
        }
    }
}