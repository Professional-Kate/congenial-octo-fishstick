using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Ability.Runtime.Component
{
    public readonly record struct TargetingPreferenceComponent : IComponent
    {
        public required TargetingPreference TargetingPreference { get; init; }
        public required StatType StatType { get; init; }
        public required TargetingType TargetingType { get; init; }
    }
}