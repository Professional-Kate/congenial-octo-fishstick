using IdelPog.ContentHydrator.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentHydratorTests.Assertions
{
    [TestFixture]
    public class AssertDirectoryFoundTest
    {
        private IAssertDirectoryFound _assertDirectoryFound { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assertDirectoryFound = new AssertDirectoryFound(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertDirectoryIsFound_DirectoryIsFound()
        {
            Assert.DoesNotThrow(() => _assertDirectoryFound.AssertDirectoryIsFound("Resources"));
        }

        [Test]
        public void Negative_AssertDirectoryIsFound_DirectoryIsNotFound_Throws()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _assertDirectoryFound.AssertDirectoryIsFound("Error404"));
        }
    }
}