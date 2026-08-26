using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Event;

namespace IdelPog.Combat.Runtime.Component.Ability
{
    public readonly record struct AbilityStage
    {
        public required AbilityEffectType AbilityEffectType { get; init; }
        public required AffinityType AffinityType { get; init; }
        public required uint CastTime { get; init; }
        public required uint Value { get; init; }
        public required byte MaxTargets { get; init; }
        public required byte Priority { get; init; }
    }
}