using System.Collections.Immutable;
using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Ability.Runtime.System.Interface;
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