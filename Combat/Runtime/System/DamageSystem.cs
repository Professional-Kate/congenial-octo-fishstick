using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class DamageSystem : IDamageSystem
    {
        private readonly IAssetRepository<byte, CombatantEntity> _combatantRepository;
        private readonly IFoundAssertion _foundAssertion;
        private readonly INumberAssertion _numberAssertion;

        public DamageSystem(IAssetRepository<byte, CombatantEntity> combatantRepository, IFoundAssertion foundAssertion, INumberAssertion numberAssertion)
        {
            _combatantRepository = combatantRepository;
            _foundAssertion = foundAssertion;
            _numberAssertion = numberAssertion;
        }

        public void ApplyDamage(byte targetInstanceID, StatCard attackerStats)
        {
            _numberAssertion.AssertNumberNotZero(attackerStats.Attack, attackerStats.ToString());
            _foundAssertion.AssertFound(targetInstanceID, _combatantRepository.Contains(targetInstanceID));
            
            CombatantEntity combatantEntity = _combatantRepository.Get(targetInstanceID);
            CombatantStatsComponent combatantStatsComponent = combatantEntity.GetComponent<CombatantStatsComponent>();
            StatCard targetStats = combatantStatsComponent.StatCard;
            
            StatCard statCard = targetStats with { Health = targetStats.Health - attackerStats.Attack };
            combatantEntity.UpdateCombatantStats(statCard);
        }
    }
}