using IdelPog.Model;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Models.Levelable;

namespace IdelPog.Structures.Builders
{
    /// <summary>
    /// Builds a new <see cref="Job"/> 
    /// </summary>
    /// <seealso cref="Information"/>
    /// <seealso cref="Levelable"/>
    /// <seealso cref="JobType"/>
    /// <seealso cref="Build"/>
    public interface IJobBuilder
    {
        public IJobBuilder Information(Information information);

        public IJobBuilder Levelable(ILevelable levelable);

        public IJobBuilder JobType(JobType jobType);

        public Job Build();
    }
}