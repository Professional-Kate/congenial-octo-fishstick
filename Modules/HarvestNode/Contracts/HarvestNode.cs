using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Progression;

namespace IdelPog.HarvestNode.Contracts
{
    public sealed record HarvestNode : ICloneable<HarvestNode>
    {
        public required LocationID LocationID { get; init; }
        public required HarvestNodeID HarvestNodeID { get; init; }
        public required ItemID ItemID { get; init; }
        public required Levelable Levelable { get; init; }
        public required Information Information { get; init; }

        public HarvestNode DeepClone()
        {
            return this with { Levelable = Levelable.DeepClone() };
        }
    }
}