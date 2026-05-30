namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct ElementalDamageCard
    {
        public required uint LightningDamage { get; init; }
        public required uint ColdDamage { get; init; }
        public required uint FireDamage { get; init; }
    }
}