using System;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using IdelPog.Validation;
using IdelPog.Validation.Handlers;
using IdelPog.Validation.Interfaces;

namespace IdelPog.Orchestration
{
    public class JobMediator : IJobMediator
    {
        private readonly IExperienceService _experienceService;
        private readonly ILevelService _levelService;
        private readonly IRepository<JobType, Job> _repository;

        public JobMediator()
        {
            _experienceService = new ExperienceService();
            _levelService = new LevelService();
            IAssertFound assertFound = new AssertFound(new ThrowHandler());
            _repository = new Repository<JobType, Job>(assertFound);
        }
        
        public JobMediator(IExperienceService experienceService, ILevelService levelService, IRepository<JobType, Job> repository)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _repository = repository;
        }
        
        public ServiceResponse ProcessJobAction(JobType jobType)
        {
            try
            {
                Job job = _repository.Get(jobType);
                
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