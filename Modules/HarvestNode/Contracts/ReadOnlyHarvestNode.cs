using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.HarvestNode.Contracts
{
    public readonly record struct ReadOnlyHarvestNode
    {
        public required LocationID LocationID { get; init; }
        public required ResourceID ResourceID { get; init; }
        public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
        public required Information Information { get; init; }
    }
}