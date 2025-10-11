using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Skill.Contracts.Command
{
    public readonly record struct SkillUpdate
    {
        public required SkillID SkillID { get; init; }
    }
}