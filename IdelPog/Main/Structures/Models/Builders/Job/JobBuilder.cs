using IdelPog.Main.Structures.Enums;
using IdelPog.Main.Structures.Models.Levelable;

namespace IdelPog.Main.Structures.Models.Builders.Job
{
    /// <inheritdoc cref="IJobBuilder"/>
    public sealed class JobBuilder : IJobBuilder
    {
        private ILevelable _levelable { get; set; }
        private JobType _jobType { get; set; }
        private Information _information { get; set; }
        
        public static IJobBuilder Builder() => new JobBuilder();

        public IJobBuilder Levelable(ILevelable levelable)
        {
            // TODO: ensure ILevelable is valid
            _levelable = levelable;
            
            return this;
        }

        public IJobBuilder JobType(JobType jobType)
        {
            _jobType = jobType;
            
            return this;
        }
        
        public IJobBuilder Information(Information information)
        {
            // TODO: ensure Information is valid
            _information = information;
            
            return this;
        }

        public Models.Job Build()
        {
            // TODO: ensure _levelable and _information are not null
            Models.Job job = new(_levelable, _jobType, _information);

            return job;
        }
    }
}