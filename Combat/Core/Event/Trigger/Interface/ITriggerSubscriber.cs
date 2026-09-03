using IdelPog.Combat.Ability.Runtime.Entities;

namespace IdelPog.Combat.Core.Event.Trigger.Interface
{
    public interface ITriggerSubscriber
    { 
        public void SubscribeAbility(AbilityEntity abilityEntity);
    }
}