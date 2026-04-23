using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Runtime.Component.Skills.Interface;

namespace IdelPog.Combat.Runtime.Component.Skills
{
    public readonly record struct BasicAttackComponent : ISkillComponent
    {
        public required uint Speed { get; init; }
        public required TargetingType TargetingType { get; init; }
    }
}