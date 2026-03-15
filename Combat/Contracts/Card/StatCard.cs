namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct StatCard : ICard
    {
        public required uint Health { get; init; }
        public required uint Attack { get; init; }
    }
}