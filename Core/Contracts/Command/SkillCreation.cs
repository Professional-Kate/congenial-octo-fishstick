using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct SkillCreation
    {
        public required SkillID SkillID { get; init; }
        public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
        public required Information.Contracts.Information Information { get; init; }
    }
}