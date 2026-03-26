using IdelPog.Combat.Runtime.Component;
using IdelPog.Combat.Runtime.System.Interface;
using IdelPog.Combat.Runtime.System.Store.Interface;
using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Combat.Runtime.System.Store
{
    public sealed class CombatantStoreService : ICombatantStoreService
    {
        private readonly ICombatantStore _friendlyCombatantStore;
        private readonly ICombatantStore _enemyCombatantStore;
        private readonly ICombatantFilters _combatantFilters;
        private readonly ICollectionAssertion _collectionAssertion;

        public CombatantStoreService(ICombatantStore friendlyCombatantStore, ICombatantStore enemyCombatantStore, ICombatantFilters combatantFilters, ICollectionAssertion collectionAssertion)
        {
            _friendlyCombatantStore = friendlyCombatantStore;
            _enemyCombatantStore = enemyCombatantStore;
            _combatantFilters = combatantFilters;
            _collectionAssertion = collectionAssertion;
        }

        public void RegisterInitial()
        {
            _friendlyCombatantStore.RegisterInitial(GetCombatantEntities(true));
            _enemyCombatantStore.RegisterInitial(GetCombatantEntities(false));
        }

        public void RegisterCombatantChange(CombatantEntity combatantEntity)
        {
            if (combatantEntity.IsFriendly)
            {
                _friendlyCombatantStore.RegisterCombatantChange(combatantEntity.CombatantID, combatantEntity.GetComponent<CombatantStatsComponent>());
            }
            else
            {
                _enemyCombatantStore.RegisterCombatantChange(combatantEntity.CombatantID, combatantEntity.GetComponent<CombatantStatsComponent>());
            }
        }

        public void RegisterCombatantDeath(CombatantEntity deadCombatant)
        {
            if (DoesCombatantIDMatch(deadCombatant, deadCombatant.IsFriendly ? _friendlyCombatantStore : _enemyCombatantStore) == false)
            {
                return;
            }
            
            if (deadCombatant.IsFriendly)
            {
                _friendlyCombatantStore.RegisterCombatantDeath(deadCombatant.CombatantID, GetCombatantEntities(true));
            }
            else
            {
                _enemyCombatantStore.RegisterCombatantDeath(deadCombatant.CombatantID, GetCombatantEntities(false));
            }
        }

        private static bool DoesCombatantIDMatch(CombatantEntity combatantEntity, ICombatantStore combatantStore)
        {
            if (combatantStore.HighestAttackCombatant.CombatantID == combatantEntity.CombatantID)
            {
                return true;
            }

            return combatantStore.LowestHealthCombatant.CombatantID == combatantEntity.CombatantID;
        }

        private CombatantEntity[] GetCombatantEntities(bool isFriendly)
        {
            if (isFriendly)
            {
                CombatantEntity[] combatantEntities = _combatantFilters.GetFriendlies().ToArray();
                _collectionAssertion.AssertHasElements(combatantEntities);
                return combatantEntities;
            }
            else
            {
                CombatantEntity[] combatantEntities = _combatantFilters.GetEnemies().ToArray();
                _collectionAssertion.AssertHasElements(combatantEntities);
                return combatantEntities;
            }
        }
    }
}