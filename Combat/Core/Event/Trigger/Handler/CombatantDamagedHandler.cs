using System.Collections.Immutable;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Contracts;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Interface;

namespace IdelPog.Combat.Core.Event.Trigger.Handler
{
    public sealed class CombatantDamagedHandler : TriggerAbilityHandler<CombatantDamagedData>
    {
        public CombatantDamagedHandler(ITriggerReader triggerReader, IAbilityEventScheduler abilityEventScheduler, ICombatantRepository combatantRepository) 
            : base(triggerReader, abilityEventScheduler,combatantRepository)
        {
        }

        protected override TriggerEventType TriggerEventType => TriggerEventType.COMBATANT_DAMAGED;
        
        protected override IEnumerable<AbilityTrigger> Filter(ImmutableArray<AbilityEntity> abilityEntities, CombatantDamagedData triggerData, double tick)
        {
            if (triggerData.DamagedCombatant.TryGetComponent(out RetaliationComponent retaliationComponent))
            {
                if (triggerData.DamagedCombatant.InstanceID == triggerData.InitiatingCombatant.InstanceID == false)
                {
                    retaliationComponent.Enqueue(new CombatantDamaged { InitiatingCombatant = triggerData.InitiatingCombatant, DamageValue = triggerData.DamageValue });
                }
            }
            
            List<AbilityTrigger> abilityTriggers = [];
            foreach (AbilityEntity abilityEntity in abilityEntities)
            {
                TriggerComponent triggerComponent = abilityEntity.GetComponent<TriggerComponent>();
                if (IsValueInRange(triggerComponent.MinTriggerValue, triggerComponent.MaxTriggerValue, triggerData.DamageValue) == false)
                {
                    continue;
                }

                CombatantEntity combatantEntity = GetCombatantEntity(abilityEntity.InstanceID);
                if (IsEligible(abilityEntity, triggerData.DamagedCombatant.TargetingType, combatantEntity, triggerComponent, tick) == false)
                {
                    continue;
                }
                
                abilityTriggers.Add(new AbilityTrigger { Tick = tick, CombatantEntity = combatantEntity, AbilityEntity = abilityEntity });
            }

            return abilityTriggers;
        }
    }
}