using IdelPog.Combat.Runtime;

namespace IdelPog.Combat.Service.Interface
{
    public interface ICombatStateService
    {
        public bool IsCombatOver { get; }
        public bool FriendlyVictory { get; }

        public void Evaluate(CombatantEntity changedCombatant);
    }
}