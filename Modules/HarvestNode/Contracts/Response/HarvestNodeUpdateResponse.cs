using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.HarvestNode.Contracts.Response
{
    public readonly record struct HarvestNodeUpdateResponse
    {
        public required ResourceID ResourceID { get; init; }
        public required LocationID LocationID { get; init; }
        public required SkillID SkillID { get; init; }
        public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
        public required bool HasLeveled { get; init; }
    }
}