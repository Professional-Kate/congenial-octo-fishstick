using IdelPog.Engine.Structures;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Levelable;
using IdelPog.Engine.Structures.Types;

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

        public Structures.Job Build();
    }
}