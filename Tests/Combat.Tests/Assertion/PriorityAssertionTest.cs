using IdelPog.Combat.Assertion;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;
using IdelPog.Combat.Runtime.Entities;
using IdelPog.Combat.Tests.TestFactory;

namespace IdelPog.Combat.Tests.Assertion
{
    [TestFixture]
    public sealed class PriorityAssertionTest
    {
        private PriorityAssertion _priorityAssertion;

        private readonly AbilityEntity _abilityEntity = TestAbilityEntityFactory.Create();
        
        private readonly StrategyCard _strategyCard = new()
        {
            CombatantStatType = CombatantStatType.HEALTH, 
            TargetingPreference = TargetingPreference.HIGHEST, 
            TargetingType = TargetingType.ENEMY, 
            Priority = 0
        };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _priorityAssertion = new PriorityAssertion();
        }

        [Test]
        public void Positive_AssertPriority_SamePriority_NoThrow()
        { 
            Assert.DoesNotThrow(() => _priorityAssertion.AssertPriority(_abilityEntity.AbilityStages, [_strategyCard]));
        }

        [Test]
        public void Negative_AssertPriority_DifferentPriority_Throws()
        {
            PriorityMismatchException exception = Assert.Throws<PriorityMismatchException>(() => _priorityAssertion.AssertPriority(_abilityEntity.AbilityStages, [_strategyCard with { Priority = 102 }]));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(exception.AbilityStagePriority, Is.Zero);
                Assert.That(exception.StrategyCardPriority, Is.EqualTo(102));
            }
        }

        [Test]
        public void Negative_AssertPriority_DifferentCollectionLength_Throws()
        { 
            Assert.Throws<PriorityMissingException>(() => _priorityAssertion.AssertPriority(_abilityEntity.AbilityStages, [_strategyCard, _strategyCard]));
        }
    }
}