using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatantStateChange
    {
        public required double Tick { get; init; }
        public required CombatantCreation CombatantCreation { get; init; }
        public required byte CombatantID { get; init; }
        public required bool IsFriendly { get; init; }
        public required bool IsAlive { get; init; }
        public required AttackingCombatant AttackingCombatant { get; init; }
    }
}