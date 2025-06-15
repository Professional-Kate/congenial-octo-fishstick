using IdelPog.Infrastructure.Structures;
using IdelPog.SimulationEngine.Structures.Enums;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Models
{
    /// <summary>
    /// The Job model
    /// </summary>
    public sealed class Job(ILevelable levelable, JobType jobType, Information information) : ICloneable<Job>
    {
        public readonly ILevelable Levelable = levelable;
        public readonly Information Information = information;
        public readonly JobType JobType = jobType;

        public Job DeepClone()
        {
            return new Job(Levelable, JobType, Information);
        }
    }
}