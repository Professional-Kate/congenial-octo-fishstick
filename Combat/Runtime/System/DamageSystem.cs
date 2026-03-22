using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DamageSystem : IDamageSystem
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly IFoundAssertion _foundAssertion;
        private readonly INumberAssertion _numberAssertion;
        private readonly ITargetFinder _targetFinder;
        private readonly ICombatantStoreService _combatantStoreService;

        public DamageSystem(ICombatantRepository combatantRepository, IFoundAssertion foundAssertion, INumberAssertion numberAssertion, ITargetFinder targetFinder, ICombatantStoreService combatantStoreService)
        {
            _combatantRepository = combatantRepository;
            _foundAssertion = foundAssertion;
            _numberAssertion = numberAssertion;
            _targetFinder = targetFinder;
            _combatantStoreService = combatantStoreService;
        }

        public void ApplyDamage(byte combatantID)
        {
            _foundAssertion.AssertFound(combatantID, _combatantRepository.Contains(combatantID));
            
            CombatantEntity attackingEntity = _combatantRepository.Get(combatantID);
            StatCard attackerStats = attackingEntity.GetComponent<CombatantStatsComponent>().StatCard;
            
            _numberAssertion.AssertNumberNotZero(attackerStats.Attack, attackerStats.ToString());
            
            CombatantEntity targetEntity = _targetFinder.FindBestTarget(attackingEntity);
            StatCard targetStats = targetEntity.GetComponent<CombatantStatsComponent>().StatCard;
            targetEntity.UpdateCombatantStats(targetStats with { Health = targetStats.Health - attackerStats.Attack });
            
            // TODO: see if health is zero, if zero murder entity and inform ICombatantStoreService
            
            _combatantStoreService.RegisterCombatantChange(targetEntity);
        }
    }
}