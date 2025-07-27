using IdelPog.Common.Enums;
using IdelPog.Common.Structures;

namespace IdelPog.SimulationEngine.Models
{
    public readonly record struct SkillNode : ICloneable<SkillNode>
    {
        public required ResourceID ResourceID { get; init; }
        public required Levelable Levelable { get; init; }
        public required Information Information { get; init; }

        public SkillNode DeepClone()
        {
            return this with { Levelable = Levelable };
        }
    }
}