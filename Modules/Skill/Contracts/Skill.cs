using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.Skill.Contracts
{
    public record class Skill : ICloneable<Skill>
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