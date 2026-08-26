using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Component.Ability;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Event.Trigger.Contracts;
using IdelPog.Combat.Runtime.Event.Trigger.Interface;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class EntityDamageService : IEntityDamageService
    {
        private readonly IDamageSystem _damageSystem;
        private readonly ITriggerAbilityHandler<CombatantDamagedData> _combatantDamagedTrigger;
        private readonly IDeathSystem _deathSystem;
        private readonly ITriggerAbilityHandler<CombatantDeathData> _combatantDeathTrigger;

        public EntityDamageService(IDamageSystem damageSystem, ITriggerAbilityHandler<CombatantDamagedData> combatantDamagedTrigger, IDeathSystem deathSystem,
            ITriggerAbilityHandler<CombatantDeathData> combatantDeathTrigger)
        {
            _damageSystem = damageSystem;
            _deathSystem = deathSystem;
            _combatantDamagedTrigger = combatantDamagedTrigger;
            _combatantDeathTrigger = combatantDeathTrigger;
        }

        public void ApplyDamage(IEnumerable<CombatantEntity> targetCombatants, byte initiatingCombatantID, CombatantAbilityStage combatantAbilityStage, double tick)
        {
            foreach (CombatantEntity targetCombatant in targetCombatants)
            {
                uint newHealth = _damageSystem.DealDamage(targetCombatant, combatantAbilityStage);
                _combatantDamagedTrigger.Handle(tick, CreateData(targetCombatant, _damageSystem.GetCalculatedDamage(combatantAbilityStage), initiatingCombatantID));

                if (newHealth != 0)
                {
                    continue;
                }

                _deathSystem.KillEntity(targetCombatant);
                _combatantDeathTrigger.Handle(tick, new CombatantDeathData { CombatantTargetingType = GetCombatantTargetingType(targetCombatant), DeadCombatantID = targetCombatant.CombatantID });
            }
        }

        private static CombatantDamagedData CreateData(CombatantEntity targetCombatant, uint calculatedDamage, byte initiatingCombatantID)
        {
            return new CombatantDamagedData
            {
                InitiatingCombatantID = initiatingCombatantID,
                DamagedCombatantID = targetCombatant.CombatantID,
                DamagedCombatantTargetingType = GetCombatantTargetingType(targetCombatant),
                DamageValue = calculatedDamage
            };
        }
        
        private static TargetingType GetCombatantTargetingType(CombatantEntity combatantEntity) => combatantEntity.GetComponent<TargetingTypeComponent>().TargetingType;
    }
}