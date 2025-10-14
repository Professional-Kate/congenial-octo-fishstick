using IdelPog.Loot.Assertion;
using IdelPog.Loot.Exceptions;

namespace Loot.Tests
{
    [TestFixture]
    public sealed class WeightAssertionTest
    {
        private WeightAssertion _weightAssertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _weightAssertion = new WeightAssertion();
        }

        [TestCase(0)]
        [TestCase(1)]
        public void Positive_AssertWeightIsPositive_PassedPositive_NoThrow(int weight)
        {
            Assert.DoesNotThrow(() => _weightAssertion.AssertWeightIsPositive(weight));
        }

        [Test]
        public void Negative_AssertWeightIsPositive_PassedNegative_Throws()
        {
            Assert.Throws<InvalidWeightException>(() => _weightAssertion.AssertWeightIsPositive(-1));
        }
    }
}