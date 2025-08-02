using IdelPog.Common.Enums;

namespace IdelPog.Common.Structures
{
    public readonly record struct HarvestNode : ICloneable<HarvestNode>
    {
        public required ResourceID ResourceID { get; init; }
        public required Levelable Levelable { get; init; }
        public required Information Information { get; init; }

        public HarvestNode DeepClone()
        {
            return this with { Levelable = Levelable };
        }
    }
}