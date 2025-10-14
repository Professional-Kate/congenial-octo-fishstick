using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Exceptions;

namespace IdelPog.Core.Tests.Validation.Assertion
{
    [TestFixture]
    public sealed class UniqueAssertionTest
    {
        private IUniqueAssertion _uniqueAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _uniqueAssertion = new UniqueAssertion();
        }

        [Test]
        public void Positive_AssertUnique_PassedFalse_NoThrow()
        {
            Assert.DoesNotThrow(() => _uniqueAssertion.AssertUnique(1, false));
        }

        [Test]
        public void Negative_AssertUnique_PassedTrue_Throws()
        {
            DuplicateEntityException exception = Assert.Throws<DuplicateEntityException>(() => _uniqueAssertion.AssertUnique(1, true));
            Assert.That(exception.ID, Is.EqualTo(1));
        }
    }
}