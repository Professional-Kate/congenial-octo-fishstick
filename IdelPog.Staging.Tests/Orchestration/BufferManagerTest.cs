using IdelPog.Staging.Collection;
using IdelPog.Staging.Factory;
using IdelPog.Staging.Orchestration;
using Moq;

namespace IdelPog.Staging.Tests.Orchestration
{
    [TestFixture]
    public class BufferManagerTest
    {
        private IBufferManager _bufferManager { get; set; }
        private BufferRequest<int> _bufferRequest { get; set; }
        private Mock<IBufferFactory> _bufferFactoryMock { get; set; }
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bufferFactoryMock = new Mock<IBufferFactory>();
            _bufferRequest = new BufferRequest<int>(3);
            _bufferManager = new BufferManager(_bufferFactoryMock.Object);

            _bufferFactoryMock.Setup(library => library.CreateBuffer<int>())
                .Returns(new Buffer<int>());
        }

        [Test]
        public void Positive_RequestBuffer_ReturnsBuffer()
        {
            Buffer<int> buffer = _bufferManager.RequestBuffer(_bufferRequest);
            
            Assert.That(buffer, Is.Not.Null);
            _bufferFactoryMock.Verify(library => library.CreateBuffer<int>());
        }
    }
}