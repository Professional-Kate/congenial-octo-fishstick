using IdelPog.Combat.Combatant.Runtime.System.Interface;
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Service.Interface;

namespace IdelPog.Combat.Core.Service
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

        public void Reset()
        {
            IsCombatOver = false;
            FriendlyVictory = false;
        }

        private bool IsCombatResolved()
        {
            if (_combatantFilters.HasValidCombatants(TargetingType.ENEMY) == false)
            {
                FriendlyVictory = true;
                return true;
            }

            if (_combatantFilters.HasValidCombatants(TargetingType.FRIENDLY) == false)
            {
                FriendlyVictory = false;
                return true;
            }
            
            FriendlyVictory = false;
            return false;
        }
    }
}