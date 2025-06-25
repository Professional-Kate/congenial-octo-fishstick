using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Exceptions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using Moq;

namespace IdelPog.Messaging.Tests.Assertions
{
    [TestFixture]
    public class AssertBufferStateTest
    {
        private IAssertBufferState _assertBufferState { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _handlerMock = new Mock<IHandler>();
            _assertBufferState = new AssertBufferState(_handlerMock.Object);
        }

        [SetUp]
        public void SetUp()
        {
            _handlerMock.Reset();
        }

        [Test]
        public void Positive_AssertBufferState_SameState()
        {
            Assert.DoesNotThrow(() => _assertBufferState.AssertState(BufferState.CREATED, BufferState.CREATED));
            _handlerMock.Verify(library => library.Handle(It.IsAny<InvalidBufferStateException>()), Times.Never);
        }

        [Test]
        public void Negative_AssertBufferState_DifferentState_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<InvalidBufferStateException>()))
                .Throws(new InvalidBufferStateException(BufferState.FILLED, BufferState.CREATED));
            
            Assert.Throws<InvalidBufferStateException>(() => _assertBufferState.AssertState(BufferState.FILLED, BufferState.CREATED));
            _handlerMock.Verify(library => library.Handle(It.IsAny<InvalidBufferStateException>()), Times.Once);
        }
    }
}