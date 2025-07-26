using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Tests.Assertions
{
    [TestFixture]
    public class UniqueAssertionTest
    {
        private IUniqueAssertion _uniqueAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _uniqueAssertion = new UniqueAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertUnique_PassedFalse_NoThrow()
        {
            Assert.DoesNotThrow(() => _uniqueAssertion.AssertUnique(1, false));
        }

        [Test]
        public void Negative_AssertUnique_PassedTrue_Throws()
        {
            Assert.Throws<DuplicateItemException>(() => _uniqueAssertion.AssertUnique(1, true));
        }
    }
}