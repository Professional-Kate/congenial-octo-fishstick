using IdelPog.Core.Validation.Handler;
using IdelPog.Loot.Assertion;
using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Exceptions;

namespace Loot.Tests
{
    [TestFixture]
    public class WeightAssertionTest
    {
        private IWeightAssertion _weightAssertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _weightAssertion = new WeightAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertWeightIsNotZero_PassedNonZero_NoThrow()
        {
            Assert.DoesNotThrow(() => _weightAssertion.AssertWeightIsNotZero(1));
        }

        [Test]
        public void Negative_AssertWeightIsNotZero_PassedZero_Throw()
        {
            Assert.Throws<ZeroWeightException>(() => _weightAssertion.AssertWeightIsNotZero(0));
        }
    }
}