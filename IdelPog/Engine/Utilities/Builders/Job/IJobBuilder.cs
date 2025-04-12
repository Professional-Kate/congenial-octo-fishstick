using IdelPog.Engine.Structures;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Models;
using IdelPog.Engine.Structures.Models.Levelable;

namespace IdelPog.Engine.Utilities.Builders.Job
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

        public Structures.Models.Job Build();
    }
}