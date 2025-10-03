using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct SkillCreationResponse
    {
        public required SkillID SkillID { get; init; }
        public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
        public required Information.Contracts.Information Information { get; init; }
    }
}