using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatantStateChange
    {
        public required byte CombatantID { get; init; }
        public required CombatantCreation CombatantCreation { get; init; }
        public required byte AttackerID { get; init; }
        public required bool IsFriendly { get; init; }
        public required bool IsAlive { get; init; }
    }
}