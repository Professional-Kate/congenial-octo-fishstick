using IdelPog.Combat.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    public readonly record struct TargetingPreferenceComponent : IComponent
    {
        public required TargetingPreference TargetingPreference { get; init; }
        public required CombatantStatType CombatantStatType { get; init; }
    }
}