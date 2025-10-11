using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Progression;

namespace IdelPog.Skill.Contracts.Response
{
    public readonly record struct SkillCreationResponse
    {
        public required SkillID SkillID { get; init; }
        public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
        public required Information Information { get; init; }
    }
}