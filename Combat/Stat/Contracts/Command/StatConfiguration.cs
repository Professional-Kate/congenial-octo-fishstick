using IdelPog.Combat.Ability.Contracts;
using IdelPog.Combat.Combatant.Contracts;

namespace IdelPog.Combat.Stat.Contracts.Command
{
    public readonly record struct StatConfiguration
    {
        public required CombatantStatLinks CombatantStatLinks { get; init; }
        public required AbilityStatLinks AbilityStatLinks { get; init; }
    }
}