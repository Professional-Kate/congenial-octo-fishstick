using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Core.Logging
{
    public readonly record struct ReadOnlyAbilityStage
    {
        public required AbilityEffectType AbilityEffectType { get; init; }
        public required AffinityType AffinityType { get; init; }
        public required uint Value { get; init; }
    }
}