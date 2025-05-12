using IdelPog.Staging.Assertions;
using IdelPog.Staging.Assertions.Pipelines;
using IdelPog.Staging.Collection;
using IdelPog.Staging.Factory;
using Moq;

namespace IdelPog.Staging.Tests.Factory
{
    [TestFixture]
    public class BufferFactoryTest
    {
        private IBufferFactory _bufferFactory { get; set; }
        private BufferRequest _bufferRequest { get; set; }
        
        private Mock<IAssertBufferState> _assertBufferStateMock { get; set; }
        private Mock<IBufferAsserter> _bufferAsserterMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertBufferStateMock = new Mock<IAssertBufferState>();
            _bufferAsserterMock = new Mock<IBufferAsserter>();
            
            _bufferFactory = new BufferFactory(_bufferAsserterMock.Object, _assertBufferStateMock.Object);
            _bufferRequest = new BufferRequest(5);
        }

        [Test]
        public void Positive_CreateBuffer_CreatesBuffer_CorrectLength()
        {
            Buffer<int> buffer = _bufferFactory.CreateBuffer<int>(_bufferRequest);
            
            Assert.That(buffer, Is.Not.Null);
            Assert.That(buffer.Data, Has.Count.EqualTo(5));
        }
    }
}