namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct AgilityCard
    {
        public required uint Speed { get; init; }
        public required uint Initiative { get; init; }
    }
}