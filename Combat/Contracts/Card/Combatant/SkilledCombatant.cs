using IdelPog.Combat.Contracts.Ability;

namespace IdelPog.Combat.Contracts.Card.Combatant
{
    public readonly record struct SkilledCombatant
    {
        public required CombatantCard CombatantCard { get; init; }
        public required AbilityCard[] AbilityCards { get; init; }
    }
}