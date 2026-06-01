using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct CombatantAbilityCard
    {
        public required AbilityType AbilityType { get; init; }
        public required StrategyCard StrategyCard { get; init; }
    }
}