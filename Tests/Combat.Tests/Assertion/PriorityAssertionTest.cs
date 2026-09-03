using IdelPog.Combat.Assertion;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Tests.Assertion
{
    [TestFixture]
    public sealed class PriorityAssertionTest
    {
        private PriorityAssertion _priorityAssertion;

        private readonly AbilityStageCard _stageCard = new()
        {
            AbilityEffectType = AbilityEffectType.DIRECT_DAMAGE,
            AffinityType = AffinityType.HOLY,
            CastTime = 10,
            MaxTargets = 1, 
            Priority = 0, 
            Value = 4
        };
        
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
            Assert.DoesNotThrow(() => _priorityAssertion.AssertPriority([_stageCard], [_strategyCard]));
        }

        [Test]
        public void Negative_AssertPriority_DifferentPriority_Throws()
        {
            PriorityMismatchException exception = Assert.Throws<PriorityMismatchException>(() => _priorityAssertion.AssertPriority([_stageCard], [_strategyCard with { Priority = 102 }]));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(exception.AbilityStagePriority, Is.Zero);
                Assert.That(exception.StrategyCardPriority, Is.EqualTo(102));
            }
        }

        [Test]
        public void Negative_AssertPriority_DifferentCollectionLength_Throws()
        { 
            Assert.Throws<PriorityMissingException>(() => _priorityAssertion.AssertPriority([_stageCard], [_strategyCard, _strategyCard]));
        }
    }
}