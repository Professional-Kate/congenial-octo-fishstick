using IdelPog.Content.Hydrator.Assertion;
using IdelPog.Content.Hydrator.Assertion.Pipeline;
using IdelPog.Content.Hydrator.Exceptions;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.Content.Tests.Assertion
{
    [TestFixture]
    public class DirectoryAssertionPipelineTest
    {
        private IDirectoryAssertionPipeline _directoryAssertionPipeline { get; set; }

        [SetUp]
        public void SetUp()
        {
            _directoryAssertionPipeline = new DirectoryAssertionPipeline(
                new DirectoryAssertion(new ThrowHandler()), new ObjectNullAssertion(new ThrowHandler()));
        }

        [Test]
        public void Positive_AssertDirectory_CorrectDirectory()
        {
            Assert.DoesNotThrow(() => _directoryAssertionPipeline.AssertDirectory("Resources"));
        }

        [Test]
        public void Negative_AssertDirectory_MissingDirectory()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _directoryAssertionPipeline.AssertDirectory("hidden"));
        }

        [Test]
        public void Negative_AssertDirectory_NullDirectory()
        {
            Assert.Throws<ArgumentNullException>(() => _directoryAssertionPipeline.AssertDirectory(null!));
        }

        [Test]
        public void Positive_AssertFiles_PositiveLength_NoThrow()
        {
            string[] goodStrings = ["testing", "hello"];

            Assert.DoesNotThrow(() => _directoryAssertionPipeline.AssertFiles(goodStrings.Length, "no"));
        }

        [Test]
        public void Negative_AssertFiles_ZeroLength_Throws()
        {
            EmptyDirectoryException exception = Assert.Throws<EmptyDirectoryException>(() => _directoryAssertionPipeline.AssertFiles(0, "no"));
            Assert.That(exception.Path, Is.EqualTo("no"));
        }

        [Test]
        public void Negative_AssertFiles_NullDirectoryPathContext_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _directoryAssertionPipeline.AssertFiles(1, null!));
        }
    }
}