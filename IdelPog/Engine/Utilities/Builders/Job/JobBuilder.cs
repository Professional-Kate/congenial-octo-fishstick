using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Models;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Utilities.Builders
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

        public Job Build()
        {
            // TODO: ensure _levelable and _information are not null
            Job job = new(_levelable, _jobType, _information);

            return job;
        }
    }
}