using IdelPog.Common.Enums;
using IdelPog.Common.Structures;

namespace IdelPog.SimulationEngine.Models
{
    public readonly record struct Skill : ICloneable<Skill>
    {
        public required SkillID SkillID { get; init; }
        public required Levelable Levelable { get; init; }
        public required Information Information { get; init; }
        
        public Skill DeepClone()
        {
            return this with { Levelable = Levelable.DeepClone() };
        }
    }
}