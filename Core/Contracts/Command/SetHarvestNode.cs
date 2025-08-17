using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts.Command
{
    public readonly record struct SetHarvestNode
    {
        public required SkillID SkillID { get; init; }
        public required ItemID ItemID { get; init; }
    }
}