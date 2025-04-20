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
            _directoryAsserter.AssertDirectory("Resources");
        }
    }
}