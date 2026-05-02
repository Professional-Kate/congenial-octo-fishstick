namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct DamageCard
    {
        public required uint PhysicalDamage { get; init; }
        public required uint LightningDamage { get; init; }
        public required uint ColdDamage { get; init; }
        public required uint FireDamage { get; init; }
    }
}