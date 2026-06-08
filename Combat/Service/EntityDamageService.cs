using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Combat.Service.Logging.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class EntityDamageService : IEntityDamageService
    {
        private readonly IDamageSystem _damageSystem;
        private readonly IDeathSystem _deathSystem;
        private readonly ICombatantLogger _combatantLogger;

        public EntityDamageService(IDamageSystem damageSystem, IDeathSystem deathSystem, ICombatantLogger combatantLogger)
        {
            _damageSystem = damageSystem;
            _deathSystem = deathSystem;
            _combatantLogger = combatantLogger;
        }

        public void ApplyDamage(IEnumerable<CombatantEntity> targetCombatants, CombatantEntity attackingCombatant, CombatantAbilityEntity attackingCombatantAbility, double tick)
        {
            foreach (CombatantEntity targetCombatant in targetCombatants)
            {
                uint newHealth = _damageSystem.DealDamage(targetCombatant, attackingCombatantAbility);
                if (newHealth == 0)
                { 
                    _deathSystem.KillEntity(targetCombatant);
                }
            
                _combatantLogger.LogCombatantChange(targetCombatant, attackingCombatant.CombatantID, attackingCombatantAbility.AbilityType, _damageSystem.GetCalculatedDamage(attackingCombatantAbility), tick);
            }
        }
    }
}