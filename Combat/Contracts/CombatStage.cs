using System.Collections.Immutable;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct CombatStage
    {
        public required byte AbilityID { get; init; }
        public required ReadOnlyCombatant InitiatingCombatant { get; init; }
        public required ImmutableArray<CombatantStateChange> CombatantStateChanges { get; init; }
    }
}