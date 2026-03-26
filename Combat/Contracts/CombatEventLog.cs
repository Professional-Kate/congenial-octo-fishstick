using IdelPog.Combat.Runtime.Component;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatEventLog
    {
        public required byte DefenderID { get; init; }
        public required CombatantStatsComponent DefenderStats { get; init; }
        public required byte AttackerID { get; init; }
        public required CombatantStatsComponent AttackerStats { get; init; }
    }
}