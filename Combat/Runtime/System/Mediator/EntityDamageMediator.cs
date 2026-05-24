using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Logging.Interface;

namespace IdelPog.Combat.Runtime.System.Mediator
{
    public sealed class EntityDamageMediator : IEntityDamageMediator
    {
        private readonly IDamageSystem _damageSystem;
        private readonly IDeathSystem _deathSystem;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly ICombatantLogger _combatantLogger;

        public EntityDamageMediator(IDamageSystem damageSystem, IDeathSystem deathSystem, ICombatantStoreService combatantStoreService, ICombatantLogger combatantLogger)
        {
            _damageSystem = damageSystem;
            _deathSystem = deathSystem;
            _combatantStoreService = combatantStoreService;
            _combatantLogger = combatantLogger;
        }

        public void ApplyDamage(CombatantEntity targetCombatant, CombatantEntity attackingCombatant, CombatantAbilityEntity attackingCombatantAbility, double tick)
        {
            StatsComponent attackerStats = attackingCombatant.GetComponent<StatsComponent>();
            
            uint newHealth = _damageSystem.DealDamage(targetCombatant, attackerStats.Attack, attackingCombatantAbility);
            if (newHealth == 0)
            { 
                _deathSystem.KillEntity(targetCombatant);
            }
            else
            {
                _combatantStoreService.RegisterCombatantChange(targetCombatant);
            }
            
            _combatantLogger.LogCombatantChange(targetCombatant, attackingCombatant.CombatantID, attackingCombatantAbility.AbilityType, _damageSystem.GetCalculatedDamage(attackerStats.Attack, attackingCombatantAbility), tick);
        }
    }
}