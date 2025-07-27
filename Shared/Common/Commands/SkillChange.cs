using IdelPog.Common.Enums;

namespace IdelPog.Common.Commands
{
    public readonly record struct SkillChange
    {
        public required SkillID SkillID { get; init; }
    }
}