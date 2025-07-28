using IdelPog.Common.Enums;

namespace IdelPog.Common.DTO
{
    public readonly record struct SkillChangeDTO
    {
        public required SkillID SkillID { get; init; }
    }
}