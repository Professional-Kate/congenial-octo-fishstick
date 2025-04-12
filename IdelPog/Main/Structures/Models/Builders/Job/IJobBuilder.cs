using IdelPogTemp.Main.Structures.Enums;
using IdelPogTemp.Main.Structures.Models.Levelable;

namespace IdelPogTemp.Main.Structures.Models.Builders.Job
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

        public Models.Job Build();
    }
}