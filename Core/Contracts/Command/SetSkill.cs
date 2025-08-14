using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct SetSkill
    {
        public required SkillID SkillID { get; init; }
    }
}