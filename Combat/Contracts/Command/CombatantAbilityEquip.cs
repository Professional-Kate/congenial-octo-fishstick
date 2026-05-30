using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct CombatantAbilityEquip
    {
        public required byte CombatantID { get; init; }
        public required AbilityCard[] AbilityCards { get; init; }
    }
}