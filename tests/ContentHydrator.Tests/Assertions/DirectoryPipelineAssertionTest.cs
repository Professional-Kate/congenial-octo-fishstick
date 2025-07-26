using IdelPog.ContentHydrator.Assertions;
using IdelPog.ContentHydrator.Assertions.Pipelines;
using IdelPog.ContentHydrator.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentHydratorTests.Assertions
{
    [TestFixture]
    public class DirectoryPipelineAssertionTest
    {
        private IDirectoryPipelineAssertion _directoryPipelineAssertion { get; set; }

        [SetUp]
        public void SetUp()
        {
            _directoryPipelineAssertion = new DirectoryPipelineAssertion(
                new DirectoryAssertion(new ThrowHandler()), new AssertNotNull(new ThrowHandler()));
        }

        [Test]
        public void Positive_AssertDirectory_CorrectDirectory()
        {
            Assert.DoesNotThrow(() => _directoryPipelineAssertion.AssertDirectory("Resources"));
        }

        [Test]
        public void Negative_AssertDirectory_MissingDirectory()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _directoryPipelineAssertion.AssertDirectory("hidden"));
        }

        [Test]
        public void Negative_AssertDirectory_NullDirectory()
        {
            Assert.Throws<ArgumentNullException>(() => _directoryPipelineAssertion.AssertDirectory(null!));
        }

        [Test]
        public void Positive_AssertFiles_PositiveLength_NoThrow()
        {
            string[] goodStrings = ["testing", "hello"];

            Assert.DoesNotThrow(() => _directoryPipelineAssertion.AssertFiles(goodStrings.Length, "no"));
        }

        [Test]
        public void Negative_AssertFiles_ZeroLength_Throws()
        {
            Assert.Throws<EmptyDirectoryException>(() => _directoryPipelineAssertion.AssertFiles(0, "no"));
        }
        
        [Test]
        public void Negative_AssertFiles_NullDirectoryPathContext_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _directoryPipelineAssertion.AssertFiles(1, null!));
        }
    }
}