using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Models
{
    public readonly record struct Skill
    {
        public required SkillID SkillID { get; init; }
        public required Levelable Levelable { get; init; }
        public required Information Information { get; init; }
    }
}