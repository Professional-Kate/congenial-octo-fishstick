using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entity;
using IdelPog.Combat.Contracts.Enum;
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

        public void ApplyDamage(IEnumerable<CombatantEntity> targetCombatants, byte initiatingCombatantID, AbilityStage abilityStage, double tick)
        {
            foreach (CombatantEntity targetCombatant in targetCombatants)
            {
                uint newHealth = _damageSystem.DealDamage(targetCombatant, abilityStage);
                _combatantDamagedTrigger.Handle(tick, CreateData(targetCombatant, _damageSystem.GetCalculatedDamage(abilityStage), initiatingCombatantID));

                if (newHealth != 0)
                {
                    continue;
                }

                _deathSystem.KillEntity(targetCombatant);
                _combatantDeathTrigger.Handle(tick, new CombatantDeathData { CombatantTargetingType = GetCombatantTargetingType(targetCombatant), DeadCombatantID = targetCombatant.InstanceID });
            }
        }

        private static CombatantDamagedData CreateData(CombatantEntity targetCombatant, uint calculatedDamage, byte initiatingCombatantID)
        {
            return new CombatantDamagedData
            {
                InitiatingCombatantID = initiatingCombatantID,
                DamagedCombatantID = targetCombatant.InstanceID,
                DamagedCombatantTargetingType = GetCombatantTargetingType(targetCombatant),
                DamageValue = calculatedDamage
            };
        }
        
        private static TargetingType GetCombatantTargetingType(CombatantEntity combatantEntity) => combatantEntity.TargetingType;
    }
}