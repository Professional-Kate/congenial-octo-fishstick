using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Tests.Assertions
{
    [TestFixture]
    public class FoundAssertionTest
    {
        private IFoundAssertion _foundAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _foundAssertion = new FoundAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertItemIsFound_PassedTrue_NoThrow()
        {
            Assert.DoesNotThrow(() => _foundAssertion.AssertFound(1, true));
        }

        [Test]
        public void Negative_AssertItemIsFound_PassedFalse_Throws()
        {
            NotFoundException<int> exception = Assert.Throws<NotFoundException<int>>(() => _foundAssertion.AssertFound(1, false));
            Assert.That(exception.Key, Is.EqualTo(1));
        }
    }
}