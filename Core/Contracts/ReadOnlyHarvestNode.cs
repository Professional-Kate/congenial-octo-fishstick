using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.Core.Contracts
{
    public readonly record struct ReadOnlyHarvestNode
    {
        public required HarvestNodeID HarvestNodeID { get; init; }
        public required ItemID ItemID { get; init; }
        public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
        public required Information.Contracts.Information Information { get; init; }
    }
}