using IdelPog.Combat.Contracts.Skill;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct CombatantSkillCreationResponse
    {
        public required Information Information { get; init; }
        public required SkillType SkillType { get; init; }
        public required uint Speed { get; init; }
        public required uint Damage { get; init; }
    }
}