namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct StatCard
    {
        public required uint Health { get; init; }
        public required uint Attack { get; init; }
        public required uint Speed { get; init; }
    }
}