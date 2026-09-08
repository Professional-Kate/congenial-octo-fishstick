namespace IdelPog.Combat.Core.Contracts.Card
{
    public readonly record struct AbilityCard
    { 
        public required uint AbilitySlots { get; init; }
        public required uint Cooldown { get; init; }
    }
}