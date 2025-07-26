using IdelPog.ContentHydrator.Assertions;
using IdelPog.ContentHydrator.Assertions.Pipelines;
using IdelPog.ContentHydrator.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using Moq;

namespace ContentHydratorTests.Assertions
{
    [TestFixture]
    public class DirectoryAsserterTest
    {
        private IDirectoryAsserter _directoryAsserter { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

        [SetUp]
        public void SetUp()
        {
            _handlerMock = new Mock<IHandler>();
            _directoryAsserter = new DirectoryAsserter(
                new AssertDirectoryFound(_handlerMock.Object), new AssertDirectoryNotEmpty(_handlerMock.Object), new AssertNotNull(_handlerMock.Object));

            _handlerMock.Setup(library => library.Handle(It.IsAny<Exception>()))
                .Callback<Exception>(ex => throw ex);
        }

        [Test]
        public void Positive_AssertDirectory_CorrectDirectory()
        {
            Assert.DoesNotThrow(() => _directoryAsserter.AssertDirectory("Resources"));

            _handlerMock.Verify(library => library.Handle(It.IsAny<Exception>()), Times.Never);
        }

        [Test]
        public void Negative_AssertDirectory_MissingDirectory()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _directoryAsserter.AssertDirectory("hidden"));

            _handlerMock.Verify(library => library.Handle(It.IsAny<DirectoryNotFoundException>()), Times.Once);
        }

        [Test]
        public void Negative_AssertDirectory_NullDirectory()
        {
            Assert.Throws<ArgumentNullException>(() => _directoryAsserter.AssertDirectory(null!));

            _handlerMock.Verify(library => library.Handle(It.IsAny<ArgumentNullException>()), Times.Once);
        }

        [Test]
        public void Positive_AssertFiles_CorrectString()
        {
            string[] goodStrings = ["testing", "hello"];

            Assert.DoesNotThrow(() => _directoryAsserter.AssertFiles(goodStrings, "no"));

            _handlerMock.Verify(library => library.Handle(It.IsAny<Exception>()), Times.Never);
        }

        [Test]
        public void Negative_AssertFiles_NullArray_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _directoryAsserter.AssertFiles(null!, "no"));

            _handlerMock.Verify(library => library.Handle(It.IsAny<ArgumentNullException>()), Times.Once);
        }

        [Test]
        public void Negative_AssertFiles_EmptyArray_Throws()
        {
            Assert.Throws<EmptyDirectoryException>(() => _directoryAsserter.AssertFiles([], "no"));

            _handlerMock.Verify(library => library.Handle(It.IsAny<EmptyDirectoryException>()), Times.Once);
        }
    }
}