using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Models
{
    /// <summary>
    /// The Job model
    /// </summary>
    public sealed class Job(ILevelable levelable, JobType jobType, Information information) : ICloneable<Job>
    {
        public readonly ILevelable Levelable = levelable;
        public readonly Information Information = information;
        public readonly JobType JobType = jobType;

        public Job Clone()
        {
            return new Job(Levelable, JobType, Information);
        }
    }
}