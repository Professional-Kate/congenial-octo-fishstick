using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Logging.Interface;

namespace IdelPog.Combat.Runtime.System.Mediator
{
    public sealed class EntityDamageMediator : IEntityDamageMediator
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ITargetFinder _targetFinder;
        private readonly ICombatantAbilityEntityRepository _combatantAbilityEntityRepository;
        private readonly IDamageSystem _damageSystem;
        private readonly IDeathSystem _deathSystem;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly ICombatantLogger _combatantLogger;
        private readonly ICombatantAssertion _combatantAssertion;

        public EntityDamageMediator(ICombatantRepository combatantRepository, ITargetFinder targetFinder, IDamageSystem damageSystem,
            ICombatantAbilityEntityRepository abilityEntityRepository, IDeathSystem deathSystem, ICombatantStoreService combatantStoreService, ICombatantLogger combatantLogger, ICombatantAssertion combatantAssertion)
        {
            _combatantRepository = combatantRepository;
            _targetFinder = targetFinder;
            _damageSystem = damageSystem;
            _combatantAbilityEntityRepository = abilityEntityRepository;
            _deathSystem = deathSystem;
            _combatantStoreService = combatantStoreService;
            _combatantAssertion = combatantAssertion;
            _combatantLogger = combatantLogger;
        }

        public void ApplyDamage(byte attackingCombatantID, AbilityType abilityType)
        {
            (CombatantEntity attackingCombatant, CombatantEntity targetCombatant) = GetCombatantEntities(attackingCombatantID, abilityType);

            CombatantStatsComponent attackerStats = attackingCombatant.GetComponent<CombatantStatsComponent>();
            CombatantAbilityEntity attackingAbility = _combatantAbilityEntityRepository.Get(attackingCombatantID, abilityType);
            
            uint newHealth = _damageSystem.DealDamage(targetCombatant, attackerStats.Attack, attackingAbility);
            if (newHealth == 0)
            { 
                _deathSystem.KillEntity(targetCombatant);
            }
            else
            {
                _combatantStoreService.RegisterCombatantChange(targetCombatant);
            }
            
            _combatantLogger.LogCombatantChange(targetCombatant, attackingCombatant.CombatantID, abilityType, _damageSystem.GetCalculatedDamage(attackerStats.Attack, attackingAbility));
        }

        /// <summary>
        /// Gets and validates both <see cref="CombatantEntity"/> needed for a damage event
        /// </summary>
        /// <param name="attackingCombatantID">The ID of the attacking combatant</param>
        /// <param name="abilityType">What Ability the attacking Combatant is using</param>
        /// <returns>a tuple of (CombatantEntity, CombatantEntity), the first is the attacker, the second is the target</returns>
        private (CombatantEntity attackingCombatant, CombatantEntity targetCombatant) GetCombatantEntities(byte attackingCombatantID, AbilityType abilityType)
        {
            CombatantEntity attackingCombatant = _combatantRepository.Get(attackingCombatantID);
            _combatantAssertion.AssertCombatantAlive(attackingCombatant);
            
            CombatantEntity targetCombatant = _targetFinder.FindBestTarget(attackingCombatant, abilityType);
            _combatantAssertion.AssertCombatantAlive(targetCombatant);
            
            return (attackingCombatant, targetCombatant);
        }
    }
}