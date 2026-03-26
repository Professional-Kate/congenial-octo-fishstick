using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Combat.Service.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class EntityDamageSystem : IEntityDamageSystem
    {
        private readonly ICombatantRepository _combatantRepository;
        private readonly ITargetFinder _targetFinder;
        private readonly ICombatStateService _combatStateService;
        private readonly ICombatantStoreService _combatantStoreService;
        private readonly ICombatLog _combatLog;
        private readonly IFoundAssertion _foundAssertion;
        private readonly INumberAssertion _numberAssertion;
        private readonly ICombatantAssertion _combatantAssertion;

        public EntityDamageSystem(ICombatantRepository combatantRepository, ITargetFinder targetFinder, ICombatStateService combatStateService, ICombatantStoreService combatantStoreService, IFoundAssertion foundAssertion, INumberAssertion numberAssertion, ICombatLog combatLog, ICombatantAssertion combatantAssertion)
        {
            _combatantRepository = combatantRepository;
            _targetFinder = targetFinder;
            _combatStateService = combatStateService;
            _combatantStoreService = combatantStoreService;
            _foundAssertion = foundAssertion;
            _numberAssertion = numberAssertion;
            _combatLog = combatLog;
            _combatantAssertion = combatantAssertion;
        }

        public void ApplyDamage(byte attackingCombatantID)
        {
            _foundAssertion.AssertFound(attackingCombatantID, _combatantRepository.Contains(attackingCombatantID));
            
            CombatantEntity attackingCombatant = _combatantRepository.Get(attackingCombatantID);
            _combatantAssertion.AssertCombatantAlive(attackingCombatant);
            
            StatCard attackerStats = attackingCombatant.GetComponent<CombatantStatsComponent>().StatCard;
            _numberAssertion.AssertNumberNotZero(attackerStats.Attack, attackerStats.ToString());
            
            CombatantEntity targetCombatant = _targetFinder.FindBestTarget(attackingCombatant);
            _combatantAssertion.AssertCombatantAlive(targetCombatant);
            
            StatCard targetStats = targetCombatant.GetComponent<CombatantStatsComponent>().StatCard;

            // TODO: DamageService.DealDamage(CombatantEntity targetEntity, StatCard attackerStats)
            uint newHealth = CalculateNewHealth(targetStats.Health, attackerStats.Attack);
            targetCombatant.UpdateCombatantStats(targetStats with { Health = newHealth });
            _combatLog.Append(targetCombatant, attackingCombatant);
            
            if (newHealth == 0)
            {
                targetCombatant.UpdateLifeStatus(false);    
                
                _combatStateService.Evaluate(targetCombatant);
                if (_combatStateService.IsCombatOver)
                {
                    return;
                }
                
                _combatantStoreService.RegisterCombatantDeath(targetCombatant);
                return;
            }
            
            _combatantStoreService.RegisterCombatantChange(targetCombatant);
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