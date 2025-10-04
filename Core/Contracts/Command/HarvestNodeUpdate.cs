using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct HarvestNodeUpdate
    {
        public required ItemID ItemID { get; init; }
        public required SkillID SkillID { get; init; }
    }
}