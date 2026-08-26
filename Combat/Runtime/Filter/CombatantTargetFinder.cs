using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.Filter.Interface;
using IdelPog.Combat.Runtime.Filter.Provider.Interface;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.Filter
{
    public sealed class CombatantTargetFinder : ICombatantTargetFinder
    {
        private readonly ICombatantFilters _combatantFilters;
        private readonly IAssetRepository<CombatantStatType, IStatProvider> _statProviderRepository;
        private readonly INumberAssertion _numberAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public CombatantTargetFinder(ICombatantFilters combatantFilters, IAssetRepository<CombatantStatType, IStatProvider> statProviderRepository, INumberAssertion numberAssertion, ICollectionAssertion collectionAssertion)
        {
            _combatantFilters = combatantFilters;
            _statProviderRepository = statProviderRepository;
            _numberAssertion = numberAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public IEnumerable<CombatantEntity> SelectPreferredTargets(TargetingPreference targetingPreference, CombatantStatType combatantStatType, TargetingType targetingType, TargetingType casterTargetingType, byte targetCount)
        {
            _numberAssertion.AssertNumberNotZero(targetCount, nameof(targetCount));
            
            IStatProvider statProvider = _statProviderRepository.Get(combatantStatType);
            IReadOnlyList<CombatantEntity> entities = _combatantFilters.GetCombatants(targetingType, casterTargetingType);
            _collectionAssertion.AssertHasElements(entities);
            
            return GetTopEntities(targetingPreference, targetCount, entities, statProvider);
        }

        private static CombatantEntity[] GetTopEntities(TargetingPreference targetingPreference, byte targetCount, IReadOnlyList<CombatantEntity> combatantEntities, IStatProvider statProvider)
        {
            PriorityQueue<CombatantEntity, uint> combatantQueue = new();

            foreach (CombatantEntity combatantEntity in combatantEntities)
            {
                uint currentEntityStat = statProvider.GetStat(combatantEntity);
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

        private static uint GetPriority(uint stat, TargetingPreference targetingPreference)
        {
            return targetingPreference switch
            {
                TargetingPreference.HIGHEST => stat,
                TargetingPreference.LOWEST => uint.MaxValue - stat,
                _ => throw new ArgumentOutOfRangeException(nameof(targetingPreference))
            };
        }
    }
}