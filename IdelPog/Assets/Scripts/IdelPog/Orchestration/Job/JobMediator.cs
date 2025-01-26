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
        private readonly ILevelService _levelService;
        private readonly IRepository<JobType, Job> _repository;

        public JobMediator(IExperienceService experienceService, ILevelService levelService, IRepository<JobType, Job> repository)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _repository = repository;
        }

        /// <summary>
        /// Creates a <see cref="IJobMediator"/> with all required dependencies
        /// </summary>
        /// <returns>A new <see cref="IJobMediator"/> class with all dependencies resolved</returns>
        public static IJobMediator CreateDefault()
        {
            IExperienceService service = new ExperienceService();
            ILevelService levelService = new LevelService();
            IRepository<JobType, Job> repository = new Repository<JobType, Job>();

            return new JobMediator(service, levelService, repository);
        }
        
        public ServiceResponse ProcessJobAction(JobType jobType)
        {
            if (_repository.Contains(jobType) == false)
            {
                return ServiceResponse.Failure($"Error! Passed JobType {jobType} was not found!");
            }
            
            Job job = _repository.Get(jobType);
            
            try
            {
                _experienceService.AddExperience(job);

                if (_levelService.CanJobLevel(job))
                {
                    _levelService.LevelUpJob(job);
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