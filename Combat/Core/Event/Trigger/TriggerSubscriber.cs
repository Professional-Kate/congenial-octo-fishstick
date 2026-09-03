using System.Collections.Immutable;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event.Trigger.Interface;

namespace IdelPog.Combat.Core.Event.Trigger
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