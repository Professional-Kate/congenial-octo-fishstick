using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Mediator.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Logging.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System.Mediator
{
    public sealed class EntityDamageMediator : IEntityDamageMediator
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ITargetFinder _targetFinder;
        private readonly IDamageSystem _damageSystem;
        private readonly IDeathSystem _deathSystem;
        private readonly ICombatantLogger _combatantLogger;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly IFoundAssertion _foundAssertion;
        private readonly ICombatantAssertion _combatantAssertion;
        private readonly INumberAssertion _numberAssertion;

        public EntityDamageMediator(ICombatantRepository combatantRepository, ITargetFinder targetFinder, IDamageSystem damageSystem,
            IDeathSystem deathSystem, ICombatantStoreService combatantStoreService, IFoundAssertion foundAssertion,
            ICombatantAssertion combatantAssertion, INumberAssertion numberAssertion, ICombatantLogger combatantLogger)
        {
            _combatantRepository = combatantRepository;
            _targetFinder = targetFinder;
            _damageSystem = damageSystem;
            _deathSystem = deathSystem;
            _combatantStoreService = combatantStoreService;
            _foundAssertion = foundAssertion;
            _combatantAssertion = combatantAssertion;
            _numberAssertion = numberAssertion;
            _combatantLogger = combatantLogger;
        }

        public void ApplyDamage(byte attackingCombatantID, AbilityType abilityType)
        {
            _foundAssertion.AssertFound(attackingCombatantID, _combatantRepository.Contains(attackingCombatantID));
            
            CombatantEntity attackingCombatant = _combatantRepository.Get(attackingCombatantID);
            _combatantAssertion.AssertCombatantAlive(attackingCombatant);
            
            CombatantStatsComponent attackerStats = attackingCombatant.GetComponent<CombatantStatsComponent>();
            _numberAssertion.AssertNumberNotZero(attackerStats.Attack, attackerStats.ToString());
            
            CombatantEntity targetCombatant = _targetFinder.FindBestTarget(attackingCombatant, abilityType);
            _combatantAssertion.AssertCombatantAlive(targetCombatant);

            uint newHealth = _damageSystem.DealDamage(targetCombatant, attackerStats.Attack);
            if (newHealth == 0)
            { 
                _deathSystem.KillEntity(targetCombatant);
            }
            else
            {
                _combatantStoreService.RegisterCombatantChange(targetCombatant);
            }
            
            _combatantLogger.LogCombatantChange(targetCombatant, attackingCombatant.CombatantID);
        }
    }
}