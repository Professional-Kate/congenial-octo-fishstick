namespace IdelPog.Combat.Contracts.Ability
{
    public readonly record struct AbilityCard
    {
        public required AbilityType AbilityType { get; init; }
        public required StrategyCard StrategyCard { get; init; }
    }
}