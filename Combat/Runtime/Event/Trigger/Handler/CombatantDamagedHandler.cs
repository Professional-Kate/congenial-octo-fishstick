using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Contracts;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.Event.Trigger.Handler
{
    public sealed class CombatantDamagedHandler : TriggerAbilityHandler<CombatantDamagedData>
    {
        public CombatantDamagedHandler(ITriggerReader triggerReader, IAbilityEventScheduler abilityEventScheduler, ICombatantRepository combatantRepository) 
            : base(triggerReader, abilityEventScheduler,combatantRepository)
        {
        }

        protected override TriggerEventType TriggerEventType => TriggerEventType.COMBATANT_DAMAGED;
        
        protected override IEnumerable<AbilityTrigger> Filter(ImmutableArray<CombatantAbilityEntity> combatantAbilityEntities, CombatantDamagedData triggerData, double tick)
        {
            CombatantEntity damagedCombatant = GetCombatantEntity(triggerData.DamagedCombatantID);
            if (damagedCombatant.TryGetComponent(out RetaliationComponent retaliationComponent))
            {
                if (damagedCombatant.CombatantID == triggerData.InitiatingCombatantID == false)
                {
                    retaliationComponent.Enqueue(new CombatantDamageComponent { CombatantID = triggerData.InitiatingCombatantID, DamageValue = triggerData.DamageValue });
                }
            }
            
            List<AbilityTrigger> abilityTriggers = [];
            foreach (CombatantAbilityEntity combatantAbilityEntity in combatantAbilityEntities)
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
                
                abilityTriggers.Add(new AbilityTrigger { Tick = tick, CombatantID = combatantAbilityEntity.CombatantID, AbilityID = combatantAbilityEntity.AbilityID });
            }

            return abilityTriggers;
        }
    }
}