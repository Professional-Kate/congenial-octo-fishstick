using System.Collections.Immutable;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Interface;

namespace IdelPog.Combat.Core.Event.Trigger.Handler
{
    public sealed class CombatantDeathHandler : TriggerAbilityHandler<CombatantDeathData>
    {
        public CombatantDeathHandler(ITriggerReader triggerReader, IAbilityEventScheduler abilityEventScheduler, ICombatantRepository combatantRepository) 
            : base(triggerReader, abilityEventScheduler, combatantRepository)
        {
        }

        protected override TriggerEventType TriggerEventType => TriggerEventType.COMBATANT_DEATH;
        
        protected override IEnumerable<AbilityTrigger> Filter(ImmutableArray<AbilityEntity> abilityEntities, CombatantDeathData triggerData, double tick)
        {
            List<AbilityTrigger> abilityTriggers = [];
            foreach (AbilityEntity abilityEntity in abilityEntities)
            {
                TriggerComponent triggerComponent = abilityEntity.GetComponent<TriggerComponent>();
                if (triggerComponent.TargetingType == TargetingType.SELF)
                {
                    continue;
                }
              
                CombatantEntity combatantEntity = GetCombatantEntity(abilityEntity.InstanceID);
                if (IsEligible(abilityEntity, triggerData.CombatantTargetingType, combatantEntity, triggerComponent, tick) == false)
                {
                    continue;
                }
                
                abilityTriggers.Add(new AbilityTrigger { Tick = tick, CombatantEntity = combatantEntity, AbilityEntity = abilityEntity });
            }

            return abilityTriggers;
        }
    }
}