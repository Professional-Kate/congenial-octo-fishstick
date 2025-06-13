using IdelPog.Engine.Models;
using IdelPog.Engine.Service;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;
using IdelPog.Infrastructure.Repository;

namespace IdelPog.Engine.Orchestration
{
    public class JobMediator(IExperienceService experienceService, ILevelService levelService, IStateRepository<JobType, Job> stateRepository)
        : IJobMediator
    {
        public ServiceResponse ProcessJobAction(JobType jobType)
        {
            try
            {
                Job job = stateRepository.Get(jobType);
                ILevelable levelable = job.Levelable;
                
                experienceService.AddExperience(levelable);

                if (levelService.CanJobLevel(levelable))
                {
                    levelService.LevelUpJob(levelable);
                }
                
                stateRepository.Update(jobType, job);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}