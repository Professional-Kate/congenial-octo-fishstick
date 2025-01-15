using System;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Orchestration
{
    public class JobMediator : IJobMediator
    {
        private readonly IExperienceService _experienceService;
        private readonly IRepository<JobType, Job> _repository;

        public JobMediator(IExperienceService experienceService, IRepository<JobType, Job> repository)
        {
            _experienceService = experienceService;
            _repository = repository;
        }

        /// <summary>
        /// Creates a <see cref="IJobMediator"/> with all required dependencies
        /// </summary>
        /// <returns>A new <see cref="IJobMediator"/> class with all dependencies resolved</returns>
        public static IJobMediator CreateDefault()
        {
            IExperienceService service = new ExperienceService();
            IRepository<JobType, Job> repository = new Repository<JobType, Job>();

            return new JobMediator(service, repository);
        }
        
        public ServiceResponse ProcessJobAction(JobType jobType)
        {
            try
            {
                Job job = _repository.Get(jobType);

                ServiceResponse serviceResponse = _experienceService.AddExperience(job, job.ExperiencePerAction);
                if (serviceResponse.IsSuccess == false)
                {
                    return serviceResponse;
                }
                
                _repository.Update(jobType, job);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}