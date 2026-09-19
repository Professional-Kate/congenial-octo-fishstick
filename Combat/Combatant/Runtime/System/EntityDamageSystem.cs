using IdelPog.Combat.Ability.Model;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Event.Trigger.Contracts;
using IdelPog.Combat.Core.Event.Trigger.Interface;

namespace IdelPog.Combat.Combatant.Runtime.System
{
    public sealed class EntityDamageSystem : IEntityDamageSystem
    {
        private readonly IDamageCalculator _damageCalculator;
        private readonly ITriggerAbilityHandler<CombatantDamagedData> _combatantDamagedTrigger;
        private readonly IDeathSystem _deathSystem;
        private readonly ITriggerAbilityHandler<CombatantDeathData> _combatantDeathTrigger;

        public EntityDamageSystem(IDamageCalculator damageCalculator, ITriggerAbilityHandler<CombatantDamagedData> combatantDamagedTrigger, IDeathSystem deathSystem,
            ITriggerAbilityHandler<CombatantDeathData> combatantDeathTrigger)
        {
            _damageCalculator = damageCalculator;
            _deathSystem = deathSystem;
            _combatantDamagedTrigger = combatantDamagedTrigger;
            _combatantDeathTrigger = combatantDeathTrigger;
        }

        public void ApplyDamage(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity initiatingCombatant, AbilityStage abilityStage, double tick)
        {
            foreach (CombatantEntity targetCombatant in targetCombatants)
            {
                uint newHealth = _damageCalculator.DealDamage(targetCombatant, abilityStage);
                _combatantDamagedTrigger.Handle(tick, CreateData(targetCombatant, _damageCalculator.GetCalculatedDamage(abilityStage), initiatingCombatant));

                if (newHealth != 0)
                {
                    continue;
                }

                _deathSystem.KillEntity(targetCombatant);
                _combatantDeathTrigger.Handle(tick, new CombatantDeathData { CombatantTargetingType = targetCombatant.TargetingType });
            }
        }

        private static CombatantDamagedData CreateData(CombatantEntity targetCombatant, uint calculatedDamage, CombatantEntity initiatingCombatant)
        {
            return new CombatantDamagedData
            {
                InitiatingCombatant = initiatingCombatant,
                DamagedCombatant = targetCombatant,
                DamageValue = calculatedDamage
            };
        }
    }
}