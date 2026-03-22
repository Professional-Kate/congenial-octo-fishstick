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
        private readonly ITargetFinder _targetFinder;
        private readonly ICombatStateService _combatStateService;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly IFoundAssertion _foundAssertion;
        private readonly INumberAssertion _numberAssertion;

        public DamageSystem(ICombatantRepository combatantRepository, ITargetFinder targetFinder, ICombatStateService combatStateService, ICombatantStoreService combatantStoreService, IFoundAssertion foundAssertion, INumberAssertion numberAssertion)
        {
            _combatantRepository = combatantRepository;
            _targetFinder = targetFinder;
            _combatStateService = combatStateService;
            _combatantStoreService = combatantStoreService;
            _foundAssertion = foundAssertion;
            _numberAssertion = numberAssertion;
        }

        public void ApplyDamage(byte attackingCombatantID)
        {
            _foundAssertion.AssertFound(attackingCombatantID, _combatantRepository.Contains(attackingCombatantID));
            
            CombatantEntity attackingEntity = _combatantRepository.Get(attackingCombatantID);
            StatCard attackerStats = attackingEntity.GetComponent<CombatantStatsComponent>().StatCard;
            
            _numberAssertion.AssertNumberNotZero(attackerStats.Attack, attackerStats.ToString());
            
            CombatantEntity targetEntity = _targetFinder.FindBestTarget(attackingEntity);
            StatCard targetStats = targetEntity.GetComponent<CombatantStatsComponent>().StatCard;

            uint newHealth = CalculateNewHealth(targetStats.Health, attackerStats.Attack);
            targetEntity.UpdateCombatantStats(targetStats with { Health = newHealth });
            
            if (newHealth == 0)
            {
                targetEntity.UpdateLifeStatus(false);
                
                _combatStateService.Evaluate();
                if (_combatStateService.IsCombatOver)
                {
                    return;
                }
                
                _combatantStoreService.RegisterCombatantDeath(targetEntity);
                return;
            }
            
            _combatantStoreService.RegisterCombatantChange(targetEntity);
        }

        private static uint CalculateNewHealth(uint defenderHealth, uint attackerAttack)
        {
            if (defenderHealth <= attackerAttack)
            {
                return 0;
            }

            return defenderHealth - attackerAttack;
        }
    }
}