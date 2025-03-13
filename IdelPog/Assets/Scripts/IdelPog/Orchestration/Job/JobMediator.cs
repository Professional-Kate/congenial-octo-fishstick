using System;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Service.Level;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Models.Levelable;

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
            _repository = new Repository<JobType, Job>();
        }
        
        public JobMediator(IExperienceService experienceService, ILevelService levelService, IRepository<JobType, Job> repository)
        {
            _experienceService = experienceService;
            _levelService = levelService;
            _repository = repository;
        }
        
        public ServiceResponse ProcessJobAction(JobType jobType)
        {
            if (_repository.Contains(jobType) == false)
            {
                return ServiceResponse.Failure($"Error! Passed JobType {jobType} was not found!");
            }
            
            Job job = _repository.Get(jobType);
            ILevelable levelable = job.Levelable;
            
            try
            {
                _experienceService.AddExperience(levelable);

                if (_levelService.CanJobLevel(levelable))
                {
                    _levelService.LevelUpJob(levelable);
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