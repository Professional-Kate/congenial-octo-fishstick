namespace IdelPog.Combat.Core.Contracts.Card
{
    public readonly record struct AgilityCard
    {
        public required uint Speed { get; init; }
        public required uint Initiative { get; init; }
    }
}