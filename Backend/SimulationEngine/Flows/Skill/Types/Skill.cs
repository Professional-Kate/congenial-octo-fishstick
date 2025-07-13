using IdelPog.Common.Structures;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Skill
{
    public sealed class Skill : ICloneable<Skill>
    {
        public readonly ILevelable Levelable;
        public readonly Information Information;
        public readonly SkillID SkillID;

        public Skill(ILevelable levelable, SkillID skillID, Information information)
        {
            Levelable = levelable;
            Information = information;
            SkillID = skillID;
        }

        public Skill DeepClone()
        {
            return new Skill(Levelable, SkillID, Information);
        }
    }
}