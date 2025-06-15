using IdelPog.Staging.Assertions;
using IdelPog.Staging.Exceptions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using Moq;

namespace IdelPog.Staging.Tests.Assertions
{
    [TestFixture]
    public class AssertValidCollectionSizeTest
    {
        private IAssertValidCollectionSize _assertValidCollectionSize { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _handlerMock = new Mock<IHandler>();
            _assertValidCollectionSize = new AssertValidCollectionSize(_handlerMock.Object);

            _handlerMock.Setup(library => library.Handle(It.IsAny<BufferSizeInvalidException>()))
                .Throws(new BufferSizeInvalidException(0));
        }

        [TestCase(3)]
        [TestCase(100)]
        [TestCase(1)]
        public void Positive_AssertValidSize_ValidSize_NoThrow(int size)
        {
            Assert.DoesNotThrow(() => _assertValidCollectionSize.AssertValidSize(size));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(-100)]
        public void Negative_AssertValidSize_InvalidSize_Throws(int size)
        {
            Assert.Throws<BufferSizeInvalidException>(() => _assertValidCollectionSize.AssertValidSize(size));
        }
    }
}