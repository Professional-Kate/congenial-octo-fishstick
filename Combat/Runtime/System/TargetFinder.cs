using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
{
    public sealed class TargetFinder : ITargetFinder
    {
        private readonly ICombatantFilters _combatantFilters;
        private readonly ICombatantStore _combatantStore;
        private readonly Random _random;

        public TargetFinder(ICombatantFilters combatantFilters, Random random, ICombatantStore combatantStore)
        {
            _combatantFilters = combatantFilters;
            _random = random;
            _combatantStore = combatantStore;
        }

        public CombatantEntity FindBestTarget(CombatantEntity attackingEntity)
        {
            CombatantStatsComponent stats = attackingEntity.GetComponent<CombatantStatsComponent>();
            
            if (attackingEntity.IsFriendly)
            {
                return EnumerateCombatants(stats.StatCard, _combatantFilters.GetEnemies());
            }

            return EnumerateCombatants(stats.StatCard, _combatantFilters.GetFriendlies());
        }

        private CombatantEntity EnumerateCombatants(StatCard attackerStats, IEnumerable<CombatantEntity> combatants)
        {
            CombatantEntity[] combatantEntities = combatants.ToArray();
            foreach (CombatantEntity combatantEntity in combatantEntities)
            { 
                StatCard enemyStats = combatantEntity.GetComponent<CombatantStatsComponent>().StatCard;

                if (attackerStats.Attack >= enemyStats.Health)
                {
                    return combatantEntity;
                }
            }
            
            int index = _random.Next(0, combatantEntities.Length);
            return combatantEntities[index];
        }
    }
}