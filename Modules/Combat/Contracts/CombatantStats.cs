namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatantStats
    {
        public required uint Health { get; init; }
        public required uint Attack { get; init; }
        public required uint Speed { get; init; }
    }
}