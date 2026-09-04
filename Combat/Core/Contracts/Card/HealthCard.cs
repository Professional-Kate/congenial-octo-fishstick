namespace IdelPog.Combat.Core.Contracts.Card
{
    public readonly record struct HealthCard
    {
        public required uint BaseHealth { get; init; }
        public required uint Health { get; init; }
    }
}