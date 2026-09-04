using System.Collections.Immutable;
using IdelPog.Combat.Ability.Contracts;

namespace IdelPog.Combat.Ability.Model
{
    public readonly record struct EquippedAbilityDefinition
    {
        public required byte CombatantID { get; init; }
        public required ImmutableArray<EquippedAbility> EquippedAbilities { get; init; }
    }
}