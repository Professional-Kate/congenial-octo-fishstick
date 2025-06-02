using IdelPog.Staging.Assertions;
using IdelPog.Staging.Exceptions;
using IdelPog.Validation.Assertions.Handlers;
using Moq;

namespace IdelPog.Staging.Tests.Assertions
{
    [TestFixture]
    public class AssertCollectionSizeTest
    {
        private IAssertCollectionSize _assertCollectionSize { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _handlerMock = new Mock<IHandler>();
            _assertCollectionSize = new AssertCollectionSize(_handlerMock.Object);

            _handlerMock.Setup(library => library.Handle(It.IsAny<BufferSizeMismatchException>()))
                .Throws(new BufferSizeMismatchException(0, 1));
        }

        [TestCase(3, 3)]
        [TestCase(100, 100)]
        public void Positive_AssertBufferSize_SameNumbers_NoThrow(int expected, int sourceSize)
        {
            Assert.DoesNotThrow(() => _assertCollectionSize.AssertSize(expected, sourceSize));
        }

        [TestCase(3, 2)]
        [TestCase(2, 3)]
        public void Negative_AssertBufferSize_DifferentNumbers_Throws(int expected, int sourceSize)
        {
            Assert.Throws<BufferSizeMismatchException>(() => _assertCollectionSize.AssertSize(expected, sourceSize));
        }
    }
}