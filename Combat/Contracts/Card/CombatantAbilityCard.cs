namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct CombatantAbilityCard
    {
        public required byte AbilityID { get; init; }
        public required StrategyCard[] StrategyCards { get; init; }
    }
}