using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;

namespace IdelPog.Combat.Runtime.Event.Trigger
{
    public sealed class TriggerSubscriber : ITriggerSubscriber, ITriggerReader
    {
        private readonly Dictionary<TriggerEventType, List<AbilityEntity>> _subscribedAbilities = new();

        public void SubscribeAbility(AbilityEntity abilityEntity)
        { 
            TriggerComponent triggerComponent = abilityEntity.GetComponent<TriggerComponent>();
            
            _subscribedAbilities.TryAdd(triggerComponent.TriggerEventType, []);
            _subscribedAbilities[triggerComponent.TriggerEventType].Add(abilityEntity);
        }

        public ImmutableArray<AbilityEntity> GetAbilities(TriggerEventType triggerEventType)
        {
            if (_subscribedAbilities.TryGetValue(triggerEventType, out List<AbilityEntity>? abilities) == false)
            {
                return [];
            }

            return [..abilities];
        }
    }
}