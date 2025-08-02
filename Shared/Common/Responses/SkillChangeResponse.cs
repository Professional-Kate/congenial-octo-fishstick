using IdelPog.Common.Enums;

namespace IdelPog.Common.Responses
{
    public readonly record struct SkillChangeResponse
    {
        public required SkillID SkillID { get; init; }
    }
}