using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.Core.Contracts.Response
{
    public readonly record struct HarvestNodeUpdateResponse
    {
        public required ItemID ItemID { get; init; }
        public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
        public required bool HasLeveled { get; init; }
    }
}