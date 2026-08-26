using System.Collections.Immutable;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatantStateChange
    {
        public required double Tick { get; init; }
        public required ImmutableArray<ReadOnlyCombatant> TargetCombatants { get; init; }
        public required ReadOnlyAbilityStage ReadOnlyAbilityStage { get; init; }
    }
}