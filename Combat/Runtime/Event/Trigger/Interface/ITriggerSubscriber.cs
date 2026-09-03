using IdelPog.Combat.Ability.Runtime.Entity;

namespace IdelPog.Combat.Runtime.Event.Trigger.Interface
{
    public interface ITriggerSubscriber
    { 
        public void SubscribeAbility(AbilityEntity abilityEntity);
    }
}