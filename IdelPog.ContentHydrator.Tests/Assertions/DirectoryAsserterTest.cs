using ContentHydrator.Assertions;
using ContentHydrator.Assertions.Pipelines;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentHydratorTests.Assertions
{
    [TestFixture]
    public class DirectoryAsserterTest
    {
        private IDirectoryAsserter _directoryAsserter { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            IHandler throwHandler = new ThrowHandler();
            _directoryAsserter = new DirectoryAsserter(new AssertDirectoryFound(throwHandler), new AssertDirectoryNotEmpty(throwHandler), new AssertNotNull(throwHandler));
        }

        [Test]
        public void Positive_AssertDirectory_CorrectDirectory()
        {
            Assert.DoesNotThrow(() => _directoryAsserter.AssertDirectory("Resources"));
        }

        [Test]
        public void Negative_AssertDirectory_MissingDirectory()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _directoryAsserter.AssertDirectory("hidden"));
        }

        [Test]
        public void Negative_AssertDirectory_NullDirectory()
        {
            Assert.Throws<ArgumentNullException>(() => _directoryAsserter.AssertDirectory(null!));
        }

        [Test]
        public void Positive_AssertFiles_CorrectString()
        {
            string[] goodStrings = ["testing", "hello"];
            
            Assert.DoesNotThrow(() => _directoryAsserter.AssertFiles(goodStrings, "no"));
        }
    }
}