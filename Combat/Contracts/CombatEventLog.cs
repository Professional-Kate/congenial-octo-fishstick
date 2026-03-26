using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatEventLog
    {
        public required byte DefenderID { get; init; }
        public required StatCard DefenderStatCard { get; init; }
        public required byte AttackerID { get; init; }
        public required StatCard AttackerStatCard { get; init; }
    }
}