using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Combatant.Contracts
{
    public readonly record struct EquippedAbility
    {
        public required byte AbilityID { get; init; }
        public required StrategyCard[] StrategyCards { get; init; }
    }
}