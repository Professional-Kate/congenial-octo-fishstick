using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.HarvestNode.Contracts
{
    public sealed record HarvestNode : ICloneable<HarvestNode>
    {
        public required LocationID LocationID { get; init; }
        public required ResourceID ResourceID { get; init; }
        public required Levelable Levelable { get; init; }
        public required Information Information { get; init; }

        public HarvestNode DeepClone()
        {
            return this with { Levelable = Levelable.DeepClone() };
        }
    }
}