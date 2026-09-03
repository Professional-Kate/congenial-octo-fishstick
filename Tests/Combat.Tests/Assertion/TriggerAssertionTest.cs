using IdelPog.Combat.Assertion;
using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Tests.Assertion
{
    [TestFixture]
    public sealed class TriggerAssertionTest
    {
        private TriggerAssertion _triggerAssertion;

        private readonly TriggerCard _goodTriggerCard = new()
        {
            TriggerEventType = TriggerEventType.ABILITY_READY,
            TargetingType = TargetingType.SELF,
            MinTriggerValue = 0,
            MaxTriggerValue = 0
        };

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _triggerAssertion = new TriggerAssertion();
        }

        [Test]
        public void Positive_AssertTrigger_GoodTrigger_NoThrow()
        { 
            Assert.DoesNotThrow(() => _triggerAssertion.AssertTrigger(_goodTriggerCard));
        }

        [Test]
        public void Positive_AssertTrigger_BadTrigger_ButNotAbilityReady_NoThrow()
        { 
            TriggerCard badTriggerCard = _goodTriggerCard with { TargetingType = TargetingType.ENEMY };
            
            Assert.Throws<AbilityReadyException>(() => _triggerAssertion.AssertTrigger(badTriggerCard));
            Assert.DoesNotThrow(() => _triggerAssertion.AssertTrigger(badTriggerCard with { TriggerEventType = TriggerEventType.COMBATANT_CASTING_COMPLETE }));
        }

        [Test]
        public void Negative_AssertTrigger_BadTrigger_Throws()
        {
            Assert.Throws<AbilityReadyException>(() => _triggerAssertion.AssertTrigger(_goodTriggerCard with { TargetingType = TargetingType.ENEMY }));
            Assert.Throws<AbilityReadyException>(() => _triggerAssertion.AssertTrigger(_goodTriggerCard with { MinTriggerValue = 24 }));
            Assert.Throws<AbilityReadyException>(() => _triggerAssertion.AssertTrigger(_goodTriggerCard with { MaxTriggerValue = 120 }));
        }
    }
}