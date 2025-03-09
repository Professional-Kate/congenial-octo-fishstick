using System;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Pipelines;
using IdelPog.Validation.Pipelines.Interfaces;

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
            IAssertNotNull assertNotNull = new AssertNotNull(handler);
            IAssertPositive assertPositive = new AssertPositive(handler);
            ILevelableAsserter levelableAsserter = new LevelableAsserter(assertUnderMaxLevel, assertNotNull, assertPositive);
            
            _experienceService = new ExperienceService(levelableAsserter);
            _levelService = new LevelService(levelableAsserter);

            IAssertFound assertFound = new AssertFound(handler);
            IAssertNonDuplicate assertNonDuplicate = new AssertNonDuplicate(handler);
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(assertFound, assertNotNull, assertNonDuplicate);
            
            _repository = new Repository<JobType, Job>(repositoryAsserter);
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