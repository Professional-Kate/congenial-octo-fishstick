using System.Collections.Immutable;
using IdelPog.Combat.Combatant.Contracts;

namespace IdelPog.Combat.Combatant.Model
{
    public readonly record struct EquippedAbilityDefinition
    {
        public required byte CombatantID { get; init; }
        public required ImmutableArray<EquippedAbility> EquippedAbilities { get; init; }
    }
}