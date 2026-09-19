using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Core.Event;

namespace IdelPog.Combat.Core.Logging.Contracts
{
    public readonly record struct ReadOnlyAbilityStage
    {
        public required AbilityEffectType AbilityEffectType { get; init; }
        public required AffinityType AffinityType { get; init; }
        public required ReadOnlyStrategy ReadOnlyStrategy { get; init; }
        public required uint CastTime { get; init; }
        public required uint Value { get; init; }
        public required byte MaxTargets { get; init; }
    }
}