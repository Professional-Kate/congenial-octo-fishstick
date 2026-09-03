using System.Collections.Immutable;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Contracts;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Runtime.Event.Trigger.Handler
{
    public sealed class CombatantDeathHandler : TriggerAbilityHandler<CombatantDeathData>
    {
        public CombatantDeathHandler(ITriggerReader triggerReader, IAbilityEventScheduler abilityEventScheduler, ICombatantRepository combatantRepository) 
            : base(triggerReader, abilityEventScheduler, combatantRepository)
        {
        }

        protected override TriggerEventType TriggerEventType => TriggerEventType.COMBATANT_DEATH;
        
        protected override IEnumerable<AbilityTrigger> Filter(ImmutableArray<AbilityEntity> combatantAbilityEntities, CombatantDeathData triggerData, double tick)
        {
            List<AbilityTrigger> abilityTriggers = [];
            foreach (AbilityEntity combatantAbilityEntity in combatantAbilityEntities)
            {
                TriggerComponent triggerComponent = combatantAbilityEntity.GetComponent<TriggerComponent>();
                if (triggerComponent.TargetingType == TargetingType.SELF)
                {
                    continue;
                }
              
                if (IsEligible(combatantAbilityEntity, triggerData.CombatantTargetingType, triggerData.DeadCombatantID, triggerComponent, tick) == false)
                {
                    continue;
                }
                
                abilityTriggers.Add(new AbilityTrigger { Tick = tick, CombatantID = combatantAbilityEntity.InstanceID, AbilityID = combatantAbilityEntity.AbilityID });
            }

            return abilityTriggers;
        }
    }
}