using System;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using IdelPog.Validation;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Handlers;
using IdelPog.Validation.Handlers.Interfaces;
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
            IHandler handler = new ThrowHandler();
            IAssertUnderMaxLevel assertUnderMaxLevel = new AssertUnderMaxLevel(handler);
            IAssertPositive assertPositive = new AssertPositive(handler);
            IAssertNotNull assertNotNull = new AssertNotNull(handler);

            ILevelableAsserter levelableAsserter = new LevelableAsserter(assertUnderMaxLevel, assertNotNull, assertPositive);
            
            _experienceService = new ExperienceService(levelableAsserter);
            _levelService = new LevelService(assertUnderMaxLevel);
            
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