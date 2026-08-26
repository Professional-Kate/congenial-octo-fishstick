using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;

namespace IdelPog.Combat.Runtime.Event.Trigger
{
    public sealed class TriggerSubscriber : ITriggerSubscriber, ITriggerReader
    {
        private readonly IDictionary<TriggerEventType, IList<CombatantAbilityEntity>> _subscribedAbilities;

        public TriggerSubscriber(IDictionary<TriggerEventType, IList<CombatantAbilityEntity>> subscribedAbilities)
        {
            _subscribedAbilities = subscribedAbilities;
        }

        public void SubscribeAbility(CombatantAbilityEntity combatantAbilityEntity)
        { 
            TriggerComponent triggerComponent = combatantAbilityEntity.GetComponent<TriggerComponent>();
            
            _subscribedAbilities.TryAdd(triggerComponent.TriggerEventType, []);
            _subscribedAbilities[triggerComponent.TriggerEventType].Add(combatantAbilityEntity);
        }

        public ImmutableArray<CombatantAbilityEntity> GetAbilities(TriggerEventType triggerEventType)
        {
            if (_subscribedAbilities.TryGetValue(triggerEventType, out IList<CombatantAbilityEntity>? abilities) == false)
            {
                return [];
            }

            return [..abilities];
        }
    }
}