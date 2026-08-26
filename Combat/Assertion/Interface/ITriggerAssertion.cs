using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Assertion.Interface
{
    public interface ITriggerAssertion
    {
        public void AssertTrigger(TriggerCard triggerCard);
    }
}