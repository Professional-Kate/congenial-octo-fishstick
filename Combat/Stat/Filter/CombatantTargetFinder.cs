using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Combat.Stat.Filter.Interface;
using IdelPog.Combat.Stat.Provider.Interface;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Stat.Filter
{
    public sealed class CombatantTargetFinder : ICombatantTargetFinder
    {
        private readonly ICombatantFilters _combatantFilters;
        private readonly IAssetRepository<StatType, IStatProvider> _statProviderRepository;
        private readonly INumberAssertion _numberAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public CombatantTargetFinder(ICombatantFilters combatantFilters, IAssetRepository<StatType, IStatProvider> statProviderRepository, INumberAssertion numberAssertion, ICollectionAssertion collectionAssertion)
        {
            _combatantFilters = combatantFilters;
            _statProviderRepository = statProviderRepository;
            _numberAssertion = numberAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public IEnumerable<CombatantEntity> SelectPreferredTargets(TargetingPreference targetingPreference, StatType statType, TargetingType targetingType, TargetingType casterTargetingType, byte targetCount)
        {
            _numberAssertion.AssertNumberNotZero(targetCount, nameof(targetCount));
            
            IStatProvider statProvider = _statProviderRepository.Get(statType);
            IReadOnlyList<CombatantEntity> entities = _combatantFilters.GetCombatants(targetingType, casterTargetingType);
            _collectionAssertion.AssertHasElements(entities);
            
            return GetTopEntities(targetingPreference, targetCount, entities, statProvider, statType);
        }

        private static CombatantEntity[] GetTopEntities(TargetingPreference targetingPreference, byte targetCount, IReadOnlyList<CombatantEntity> combatantEntities, IStatProvider statProvider, StatType statType)
        {
            PriorityQueue<CombatantEntity, uint> combatantQueue = new();

            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                uint currentEntityStat = statProvider.GetStat(combatantEntity, statType);
                uint priority = GetPriority(currentEntityStat, targetingPreference);
                
                if (combatantQueue.Count < targetCount)
                {
                    combatantQueue.Enqueue(combatantEntity, priority);
                    continue;
                }

                combatantQueue.TryPeek(out _, out uint peekedPriority);
                if (priority <= peekedPriority)
                {
                    continue;
                }

                combatantQueue.Dequeue();
                combatantQueue.Enqueue(combatantEntity, priority);
            }

            CombatantEntity[] orderedEntities = new CombatantEntity[combatantQueue.Count];
            for (int i = combatantQueue.Count; i != 0;)
            { 
                // Dequeue is lowest first so we reverse the final collection order
                orderedEntities[--i] = combatantQueue.Dequeue();
            }
            
            return orderedEntities;
        }

        /// <summary>
        /// Calculates the priority used for combatant target selection.
        /// When <see cref="TargetingPreference.HIGHEST"/> is used, higher stat values have higher priority.
        /// When <see cref="TargetingPreference.LOWEST"/> is used, lower non-zero stat values have higher priority.
        /// A stat value of zero is treated as the lowest possible priority, while remaining a valid target.
        /// </summary>
        /// <param name="stat">The stat value of the combatant.</param>
        /// <param name="targetingPreference">The preference used to determine target priority.</param>
        /// <returns>The priority value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="targetingPreference"/> is not a supported targeting preference.</exception>
        private static uint GetPriority(uint stat, TargetingPreference targetingPreference)
        {
            return targetingPreference switch
            {
                TargetingPreference.HIGHEST => stat,
                TargetingPreference.LOWEST => stat == 0 ? 0 : uint.MaxValue - stat + 1,
                _ => throw new ArgumentOutOfRangeException(nameof(targetingPreference))
            };
        }
    }
}