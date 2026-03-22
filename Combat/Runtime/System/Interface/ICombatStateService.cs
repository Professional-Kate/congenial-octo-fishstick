namespace IdelPog.Combat.Runtime.System.Interface
{
    public interface ICombatStateService
    {
        public bool IsCombatOver { get; }
        public bool FriendlyVictory { get; }

        public void Evaluate();
    }
}