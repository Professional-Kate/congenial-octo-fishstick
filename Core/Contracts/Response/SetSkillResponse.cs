using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct SetSkillResponse
    {
        public required SkillID SkillID { get; init; }
    }
}