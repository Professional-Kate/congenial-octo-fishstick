namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct PhysicalDamageCard
    {
        public required uint StrikeDamage { get; init; }
        public required uint SlashDamage { get; init; }
        public required uint ThrustDamage { get; init; }
    }
}