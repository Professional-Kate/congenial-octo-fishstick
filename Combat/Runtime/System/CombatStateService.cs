using IdelPog.Combat.Runtime.System.Interface;

namespace IdelPog.Combat.Runtime.System
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

        public void Evaluate()
        { 
            IsCombatOver = IsCombatResolved();
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