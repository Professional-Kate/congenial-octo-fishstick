using IdelPog.Combat.Runtime.Entities.Combatant;

namespace IdelPog.Combat.Runtime.Event.Trigger.Interface
{
    public interface ITriggerSubscriber
    { 
        public void SubscribeAbility(CombatantAbilityEntity combatantAbilityEntity);
    }
}