using System.Collections.Immutable;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
using IdelPog.Combat.Combatant.Runtime.Component;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Contracts.Enum;
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
        
        protected override IEnumerable<AbilityTrigger> Filter(ImmutableArray<AbilityEntity> combatantAbilityEntities, CombatantDamagedData triggerData, double tick)
        {
            CombatantEntity damagedCombatant = GetCombatantEntity(triggerData.DamagedCombatantID);
            if (damagedCombatant.TryGetComponent(out RetaliationComponent retaliationComponent))
            {
                if (damagedCombatant.InstanceID == triggerData.InitiatingCombatantID == false)
                {
                    retaliationComponent.Enqueue(new CombatantDamageComponent { CombatantID = triggerData.InitiatingCombatantID, DamageValue = triggerData.DamageValue });
                }
            }
            
            List<AbilityTrigger> abilityTriggers = [];
            foreach (AbilityEntity combatantAbilityEntity in combatantAbilityEntities)
            {
                TriggerComponent triggerComponent = combatantAbilityEntity.GetComponent<TriggerComponent>();
                if (IsValueInRange(triggerComponent.MinTriggerValue, triggerComponent.MaxTriggerValue, triggerData.DamageValue) == false)
                {
                    continue;
                }

                if (IsEligible(combatantAbilityEntity, triggerData.DamagedCombatantTargetingType, triggerData.DamagedCombatantID, triggerComponent, tick) == false)
                {
                    continue;
                }
                
                abilityTriggers.Add(new AbilityTrigger { Tick = tick, CombatantID = combatantAbilityEntity.InstanceID, AbilityID = combatantAbilityEntity.AbilityID });
            }

            return abilityTriggers;
        }
    }
}