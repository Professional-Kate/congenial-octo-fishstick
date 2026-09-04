using IdelPog.Combat.Core.Contracts.Card;

namespace IdelPog.Combat.Ability.Contracts
{
    public readonly record struct EquippedAbility
    {
        public required byte AbilityID { get; init; }
        public required StrategyCard[] StrategyCards { get; init; }
    }
}