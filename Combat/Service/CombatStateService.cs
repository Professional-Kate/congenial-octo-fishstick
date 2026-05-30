using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Repository.Interface;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CombatStateService : ICombatStateService
    {
        private readonly ICombatantFilters _combatantFilters;

        public CombatStateService(ICombatantFilters combatantFilters)
        {
            _combatantFilters = combatantFilters;
        }

        public bool IsCombatOver { get; private set; }
        public bool FriendlyVictory { get; private set; }

        public void Evaluate(CombatantEntity changedCombatant)
        { 
            IsCombatOver = IsCombatResolved();
        }

        public void Reset()
        {
            IsCombatOver = false;
        }

        private bool IsCombatResolved()
        {
            if (DoesFilterContainEntities(_combatantFilters.GetEnemies()) == false)
            {
                FriendlyVictory = true;
                return true;
            }

            if (DoesFilterContainEntities(_combatantFilters.GetFriendlies()))
            {
                return false;
            }

            FriendlyVictory = false;
            return true;

        }
        
        private static bool DoesFilterContainEntities(IEnumerable<CombatantEntity> combatantEntities) => combatantEntities.Any(); 
    }
}