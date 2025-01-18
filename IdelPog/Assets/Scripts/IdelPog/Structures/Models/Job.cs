using System;
using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Model
{
    /// <summary>
    /// The Job model
    /// </summary>
    public sealed class Job : Levelable, ICloneable
    {
        public readonly Information Information;
        public readonly JobType JobType;
     
        public Job(JobType jobType, Information information)
        {
            JobType = jobType;
            Information = information;
        }

        public object Clone()
        {
            return new Job(JobType, Information);
        }
    }
}