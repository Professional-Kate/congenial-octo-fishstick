using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatantStateChange
    {
        public required byte CombatantID { get; init; }
        public required CombatantCard CombatantCard { get; init; }
        public required byte AttackerID { get; init; }
        public required bool IsFriendly { get; init; }
        public required bool IsAlive { get; init; }
    }
}