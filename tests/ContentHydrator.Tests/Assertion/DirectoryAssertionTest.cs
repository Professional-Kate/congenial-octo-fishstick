using IdelPog.Content.Hydrator.Assertion;
using IdelPog.Content.Hydrator.Exceptions;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.ContentHydrator.Tests.Assertion
{
    [TestFixture]
    public class DirectoryAssertionTest
    {
        private IDirectoryAssertion _directoryAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _directoryAssertion = new DirectoryAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertDirectoryIsFound_DirectoryIsFound_NoThrow()
        {
            Assert.DoesNotThrow(() => _directoryAssertion.AssertDirectoryIsFound("Resources"));
        }

        [Test]
        public void Negative_AssertDirectoryIsFound_DirectoryIsNotFound_Throws()
        {
            Assert.Throws<DirectoryNotFoundException>(() => _directoryAssertion.AssertDirectoryIsFound("Error404"));
        }

        [Test]
        public void Positive_AssertDirectoryNotEmpty_PassesPositiveNumber_NoThrow()
        {
            Assert.DoesNotThrow(() => _directoryAssertion.AssertDirectoryNotEmpty(1, "Resources"));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Negative_AssertDirectoryNotEmpty_PassesBadNumber_Throws(int number)
        {
            EmptyDirectoryException exception = Assert.Throws<EmptyDirectoryException>(() => _directoryAssertion.AssertDirectoryNotEmpty(number, "Error404"));
            Assert.That(exception.Path, Is.EqualTo("Error404"));
        }
    }
}