using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class EntityDamageMediator : IEntityDamageMediator
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ITargetFinder _targetFinder;
        private readonly IDamageSystem _damageSystem;
        private readonly ICombatLog _combatLog;
        private readonly IDeathSystem _deathSystem;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly IFoundAssertion _foundAssertion;
        private readonly ICombatantAssertion _combatantAssertion;
        private readonly INumberAssertion _numberAssertion;

        public EntityDamageMediator(ICombatantRepository combatantRepository, ITargetFinder targetFinder, IDamageSystem damageSystem, ICombatLog combatLog,
            IDeathSystem deathSystem, ICombatantStoreService combatantStoreService, IFoundAssertion foundAssertion,
            ICombatantAssertion combatantAssertion, INumberAssertion numberAssertion)
        {
            _combatantRepository = combatantRepository;
            _targetFinder = targetFinder;
            _damageSystem = damageSystem;
            _combatLog = combatLog;
            _deathSystem = deathSystem;
            _combatantStoreService = combatantStoreService;
            _foundAssertion = foundAssertion;
            _combatantAssertion = combatantAssertion;
            _numberAssertion = numberAssertion;
        }

        public void ApplyDamage(byte attackingCombatantID)
        {
            _foundAssertion.AssertFound(attackingCombatantID, _combatantRepository.Contains(attackingCombatantID));
            
            CombatantEntity attackingCombatant = _combatantRepository.Get(attackingCombatantID);
            _combatantAssertion.AssertCombatantAlive(attackingCombatant);
            
            CombatantStatsComponent attackerStats = attackingCombatant.GetComponent<CombatantStatsComponent>();
            _numberAssertion.AssertNumberNotZero(attackerStats.Attack, attackerStats.ToString());
            
            CombatantEntity targetCombatant = _targetFinder.FindBestTarget(attackingCombatant);
            _combatantAssertion.AssertCombatantAlive(targetCombatant);

            uint newHealth = _damageSystem.DealDamage(targetCombatant, attackerStats.Attack);
            _combatLog.Append(targetCombatant, attackingCombatant);
            
            if (newHealth == 0)
            {
                _deathSystem.KillEntity(targetCombatant);
                return;
            }
            
            _combatantStoreService.RegisterCombatantChange(targetCombatant);
        }
    }
}