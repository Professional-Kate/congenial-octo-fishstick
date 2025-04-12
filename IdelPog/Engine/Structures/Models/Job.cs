using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Levelable;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Structures
{
    /// <summary>
    /// The Job model
    /// </summary>
    public sealed class Job(ILevelable levelable, JobType jobType, Information information) : ICloneable
    {
        public readonly ILevelable Levelable = levelable;
        public readonly Information Information = information;
        public readonly JobType JobType = jobType;

        public object Clone()
        {
            return new Job(Levelable, JobType, Information);
        }
    }
}