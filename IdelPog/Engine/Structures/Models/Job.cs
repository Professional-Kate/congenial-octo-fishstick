using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Models.Levelable;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Structures.Models
{
    /// <summary>
    /// The Job model
    /// </summary>
    public sealed class Job : ICloneable
    {
        public readonly ILevelable Levelable;
        public readonly Information Information;
        public readonly JobType JobType;
     
        public Job(ILevelable levelable, JobType jobType, Information information)
        {
            Levelable = levelable;
            JobType = jobType;
            Information = information;
        }

        public object Clone()
        {
            return new Job(Levelable, JobType, Information);
        }
    }
}