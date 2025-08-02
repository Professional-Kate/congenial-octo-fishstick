using IdelPog.Common.Enums;

namespace IdelPog.Common.Responses
{
    public readonly record struct SetSkillResponse
    {
        public required SkillID SkillID { get; init; }
    }
}