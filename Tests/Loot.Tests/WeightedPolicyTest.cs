using IdelPog.Core.Validation.Handler;
using IdelPog.Loot.Assertion;
using IdelPog.Loot.Exceptions;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Random;
using Moq;

// ReSharper disable ObjectCreationAsStatement

namespace Loot.Tests
{
    [TestFixture]
    public class WeightedPolicyTest
    {
        private IGrantPolicy _grantPolicy;
        private Mock<ILootRoll> _lootRollMock;

        private const int GRANT_WEIGHT = 1;
        private const int SKIP_WEIGHT = 100;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _lootRollMock = new Mock<ILootRoll>();
            
            _grantPolicy = new WeightedPolicy(_lootRollMock.Object, GRANT_WEIGHT,  SKIP_WEIGHT, new WeightAssertion(new ThrowHandler()));
        }

        [Test]
        public void Positive_ShouldGrant_AllPossibleValues_MapsToExpectedWeights()
        {
            Dictionary<bool, int> counters = new()
            {
                { true, 0 },
                { false, 0 }
            };
            
            const int weight = GRANT_WEIGHT + SKIP_WEIGHT;
            for (int i = 0; i < weight; i++)
            {
                _lootRollMock.Setup(library => library.ExclusiveNextInt(0, weight)).Returns(i);
                
                bool shouldGrant = _grantPolicy.ShouldGrant();
                counters[shouldGrant]++;

                _lootRollMock.Reset();
            }
            
            Assert.Multiple(() =>
            {
                Assert.That(counters[true], Is.EqualTo(GRANT_WEIGHT));
                Assert.That(counters[false], Is.EqualTo(SKIP_WEIGHT));
            });
        }

        [TestCase(0)]
        [TestCase(1)]
        public void Negative_ConstructWithZeroAmounts_NoThrow(int weight)
        {
            Assert.DoesNotThrow(() => new WeightedPolicy(_lootRollMock.Object, weight, SKIP_WEIGHT, new WeightAssertion(new ThrowHandler())));
            Assert.DoesNotThrow(() => new WeightedPolicy(_lootRollMock.Object, GRANT_WEIGHT, weight, new WeightAssertion(new ThrowHandler())));
        }

        [Test]
        public void Negative_ConstructWithNegativeAmounts_Throws()
        {
            Assert.Throws<InvalidWeightException>(() => new WeightedPolicy(_lootRollMock.Object, -10, SKIP_WEIGHT, new WeightAssertion(new ThrowHandler())));
            Assert.Throws<InvalidWeightException>(() => new WeightedPolicy(_lootRollMock.Object, GRANT_WEIGHT, -10, new WeightAssertion(new ThrowHandler())));
        }
    }
}