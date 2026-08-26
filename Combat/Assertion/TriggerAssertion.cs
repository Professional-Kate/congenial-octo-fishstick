using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Assertion
{
    public sealed class TriggerAssertion : ITriggerAssertion
    {
        public void AssertTrigger(TriggerCard triggerCard)
        {
            if (triggerCard.TriggerEventType != TriggerEventType.ABILITY_READY)
            {
                return;
            }

            if (triggerCard.TargetingType != TargetingType.SELF || triggerCard.MinTriggerValue != 0 || triggerCard.MaxTriggerValue != 0)
            {
                throw new AbilityReadyException();
            }
        }
    }
}