using IdelPog.Common.Structures;
using IdelPog.SimulationEngine.Flows.Skill;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Models
{
    /// <summary>
    /// The Job model
    /// </summary>
    public sealed class Job(ILevelable levelable, SkillID skillID, Information information) : ICloneable<Job>
    {
        public readonly ILevelable Levelable = levelable;
        public readonly Information Information = information;
        public readonly SkillID SkillID = skillID;

        public Job DeepClone()
        {
            return new Job(Levelable, SkillID, Information);
        }
    }
}