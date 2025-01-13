using System;
using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Model
{
    /// <summary>
    /// The Job model
    /// </summary>
    public class Job : ICloneable
    {
        public readonly Information Information;
        
        public readonly JobType JobType;
        public uint Experience { get; private set; }

        public Job(JobType jobType, Information information, uint experience = 0)
        {
            JobType = jobType;
            Information = information;
            Experience = experience;
        }

        public void SetExperience(uint experience)
        {
            Experience = experience;
        }

        public object Clone()
        {
            return new Job(JobType, Information, Experience);
        }
    }
}