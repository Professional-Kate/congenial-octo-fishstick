using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component.Abilities.Interface;

namespace IdelPog.Combat.Runtime.Component.Abilities
{
    public readonly record struct BasicAttackComponent : IAbilityComponent
    {
        public required uint Cooldown { get; init; }
        public required TargetingType TargetingType { get; init; }
    }
}